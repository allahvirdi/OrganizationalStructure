"use client";

import * as React from "react";
import createCache, { type EmotionCache } from "@emotion/cache";
import { CacheProvider } from "@emotion/react";
import { useServerInsertedHTML } from "next/navigation";
import { ThemeProvider, type PaletteMode } from "@mui/material/styles";
import CssBaseline from "@mui/material/CssBaseline";
import rtlPlugin from "stylis-plugin-rtl";
import { createAppTheme } from "./theme";

const COLOR_MODE_KEY = "orgstructure-color-mode";

/** مشترکین تغییر حالت تم در همان صفحه */
const colorModeListeners = new Set<() => void>();

function subscribeColorMode(callback: () => void): () => void {
  colorModeListeners.add(callback);
  const onStorage = (e: StorageEvent) => {
    if (e.key === COLOR_MODE_KEY) {
      callback();
    }
  };
  window.addEventListener("storage", onStorage);
  return () => {
    colorModeListeners.delete(callback);
    window.removeEventListener("storage", onStorage);
  };
}

function getColorModeSnapshot(): PaletteMode {
  try {
    return window.localStorage.getItem(COLOR_MODE_KEY) === "dark" ? "dark" : "light";
  } catch {
    return "light";
  }
}

function getColorModeServerSnapshot(): PaletteMode {
  return "light";
}

/**
 * ساخت کش Emotion راست‌به‌چپ.
 */
function createRtlCache(): EmotionCache {
  const cache = createCache({ key: "muirtl", stylisPlugins: [rtlPlugin] });
  cache.compat = true;
  return cache;
}

interface ColorModeContextValue {
  mode: PaletteMode;
  toggleMode: () => void;
}

const ColorModeContext = React.createContext<ColorModeContextValue>({
  mode: "light",
  toggleMode: () => {},
});

/**
 * هوک تغییر حالت روشن/تیره.
 */
export function useColorMode(): ColorModeContextValue {
  return React.useContext(ColorModeContext);
}

/**
 * فراهم‌کننده تم MUI برای App Router با کش راست‌به‌چپ و تزریق SSR.
 */
export default function ThemeRegistry({ children }: { children: React.ReactNode }) {
  const [cache] = React.useState(createRtlCache);
  const seenRef = React.useRef<Set<string>>(new Set());
  // استفاده از useSyncExternalStore برای خواندن امن localStorage در کلاینت
  // با fallback به light در SSR بدون هیدریشن میس‌مچ و بدون هشدار useEffect
  const mode = React.useSyncExternalStore(
    subscribeColorMode,
    getColorModeSnapshot,
    getColorModeServerSnapshot,
  );

  const theme = React.useMemo(() => createAppTheme(mode), [mode]);

  const toggleMode = React.useCallback(() => {
    try {
      const current = getColorModeSnapshot();
      const next = current === "light" ? "dark" : "light";
      window.localStorage.setItem(COLOR_MODE_KEY, next);
      colorModeListeners.forEach((listener) => listener());
    } catch {
      // نادیده گرفتن در صورت محدودیت حافظه محلی
    }
  }, []);

  useServerInsertedHTML(() => {
    const inserted = cache.inserted;
    const fresh = Object.keys(inserted).filter((name) => !seenRef.current.has(name));
    for (const name of fresh) {
      seenRef.current.add(name);
    }

    if (fresh.length === 0) {
      return null;
    }

    let styles = "";
    for (const name of fresh) {
      const value = inserted[name];
      if (typeof value === "string") {
        styles += value;
      }
    }

    return (
      <style
        key={cache.key}
        data-emotion={`${cache.key} ${fresh.join(" ")}`}
        dangerouslySetInnerHTML={{ __html: styles }}
      />
    );
  });

  return (
    <CacheProvider value={cache}>
      <ColorModeContext.Provider value={{ mode, toggleMode }}>
        <ThemeProvider theme={theme}>
          <CssBaseline enableColorScheme />
          {children}
        </ThemeProvider>
      </ColorModeContext.Provider>
    </CacheProvider>
  );
}
