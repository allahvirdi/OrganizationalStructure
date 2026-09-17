// Run with Node 22+ and an authenticated dashboard tab in a local Chrome
// launched with --remote-debugging-port=9222. No credentials are stored here.
import assert from 'node:assert/strict';

const origin = 'http://localhost:6300';
const tabs = await (await fetch('http://127.0.0.1:9222/json/list', {
  signal: AbortSignal.timeout(5000),
})).json();
const tab = tabs.find(t => t.type === 'page' && t.url.startsWith(`${origin}/dashboard`));
assert.ok(tab, 'Open an authenticated localhost:6300/dashboard tab first');
const ws = new WebSocket(tab.webSocketDebuggerUrl);
await new Promise((resolve, reject) => {
  const timer = setTimeout(() => reject(new Error('CDP connection timeout')), 5000);
  ws.addEventListener('open', () => { clearTimeout(timer); resolve(); }, { once: true });
  ws.addEventListener('error', () => { clearTimeout(timer); reject(new Error('CDP connection failed')); }, { once: true });
});
let id = 0;
const pending = new Map();
ws.addEventListener('message', event => {
  const message = JSON.parse(event.data);
  const request = pending.get(message.id);
  if (!request) return;
  clearTimeout(request.timer);
  pending.delete(message.id);
  if (message.error) request.reject(new Error(message.error.message));
  else request.resolve(message.result);
});
function send(method, params = {}) {
  return new Promise((resolve, reject) => {
    const requestId = ++id;
    const timer = setTimeout(() => {
      pending.delete(requestId);
      reject(new Error(`CDP timeout: ${method}`));
    }, 5000);
    pending.set(requestId, { resolve, reject, timer });
    ws.send(JSON.stringify({ id: requestId, method, params }));
  });
}
async function evaluate(expression) {
  const response = await send('Runtime.evaluate', { expression, returnByValue: true, awaitPromise: true });
  assert.ok(!response.exceptionDetails, 'Browser evaluation failed');
  return response.result.value;
}
const pause = () => new Promise(resolve => setTimeout(resolve, 500));
const toggle = `document.querySelector('button[aria-label="تغییر حالت نمایش"]').click()`;
const isDark = `!!document.querySelector('[data-testid="LightModeIcon"]')`;
let originalDark;
try {
  assert.equal(await evaluate('location.origin'), origin);
  await send('Page.reload', { ignoreCache: true });
  await new Promise(resolve => setTimeout(resolve, 5000));
  assert.ok(await evaluate('!!document.querySelector("main")'), 'Dashboard must be loaded');
  originalDark = await evaluate(isDark);
  await send('Emulation.setDeviceMetricsOverride', { width: 320, height: 1000, deviceScaleFactor: 1, mobile: false });
  for (const dark of [false, true]) {
    if (await evaluate(isDark) !== dark) await evaluate(toggle);
    await pause();
    assert.equal(await evaluate(isDark), dark, 'Theme icon must update');
    const result = await evaluate(`(() => {
      const root = document.documentElement;
      const main = document.querySelector('main');
      const grids = [...main.querySelectorAll('*')].filter(e => getComputedStyle(e).display === 'grid');
      return {
        client: root.clientWidth, scroll: root.scrollWidth,
        background: getComputedStyle(document.body).backgroundColor,
        grids: grids.map(grid => {
          const r = grid.getBoundingClientRect();
          return { width: r.width, left: r.left, right: r.right,
            childrenFit: [...grid.children].every(child => {
              const c = child.getBoundingClientRect();
              return c.left >= r.left - 1 && c.right <= r.right + 1;
            }) };
        })
      };
    })()`);
    assert.ok(result.scroll <= result.client, `Document overflow: ${JSON.stringify(result)}`);
    assert.equal(result.grids.length, 3, 'Expected dashboard, session and quick-action grids');
    assert.ok(result.grids.every(g => g.width > 0 && g.left >= 0 && g.right <= result.client + 1 && g.childrenFit), 'Grid children must fit');
    assert.equal(result.background, dark ? 'rgb(17, 27, 34)' : 'rgb(247, 249, 251)', 'Body must follow the active theme');
    console.log(`PASS ${dark ? 'dark' : 'light'} 320px: client=${result.client}, scroll=${result.scroll}`);
  }
} finally {
  try {
    if (originalDark !== undefined && await evaluate(isDark) !== originalDark) await evaluate(toggle);
    await send('Emulation.clearDeviceMetricsOverride');
  } finally {
    ws.close();
  }
}
