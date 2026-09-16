"use client";

import * as React from "react";
import createCache, { type EmotionCache } from "@emotion/cache";
import { CacheProvider } from "@emotion/react";
import { useServerInsertedHTML } from "next/navigation";
import { ThemeProvider } from "@mui/material/styles";
import CssBaseline from "@mui/material/CssBaseline";
import rtlPlugin from "stylis-plugin-rtl";
import theme from "./theme";

/**
 * ساخت کش Emotion راست‌به‌چپ.
 */
function createRtlCache(): EmotionCache {
  const cache = createCache({ key: "muirtl", stylisPlugins: [rtlPlugin] });
  cache.compat = true;
  return cache;
}

/**
 * فراهم‌کننده تم MUI برای App Router با کش راست‌به‌چپ و تزریق SSR.
 * (بدون تزریق سمت سرور، HTML سرور/کلاینت ناهماهنگ و Hydration خطا می‌دهد)
 */
export default function ThemeRegistry({ children }: { children: React.ReactNode }) {
  const [cache] = React.useState(createRtlCache);
  const seenRef = React.useRef<Set<string>>(new Set());

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
      <ThemeProvider theme={theme}>
        <CssBaseline />
        {children}
      </ThemeProvider>
    </CacheProvider>
  );
}
