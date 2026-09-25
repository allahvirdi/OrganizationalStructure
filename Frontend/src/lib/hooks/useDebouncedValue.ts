"use client";

import * as React from "react";

/**
 * مقدار ورودی را با تأخیر برمی‌گرداند.
 *
 * برای جلوگیری از ارسال درخواست به ازای هر ضربهٔ کلید در جستجوهای سروری
 * (مثلاً انتخابگرهای سازمان/پست) استفاده می‌شود.
 *
 * @param value مقدار جاری
 * @param delayMs تأخیر به میلی‌ثانیه (پیش‌فرض ۳۰۰)
 * @returns مقدار تأخیرخورده
 */
export function useDebouncedValue<T>(value: T, delayMs = 300): T {
  const [debounced, setDebounced] = React.useState(value);

  React.useEffect(() => {
    const timer = window.setTimeout(() => setDebounced(value), delayMs);
    return () => window.clearTimeout(timer);
  }, [value, delayMs]);

  return debounced;
}
