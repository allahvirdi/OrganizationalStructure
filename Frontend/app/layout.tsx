import type { Metadata } from "next";
import "@fontsource/vazirmatn/400.css";
import "@fontsource/vazirmatn/500.css";
import "@fontsource/vazirmatn/700.css";
import "@fontsource/vazirmatn/800.css";
import "./globals.css";
import Providers from "../src/app/providers";
import ThemeRegistry from "../src/theme/ThemeRegistry";

export const metadata: Metadata = {
  title: "سامانه ساختار سازمانی",
  description: "هسته مرجع مدیریت ساختار سازمانی",
};

export default function RootLayout({ children }: { children: React.ReactNode }) {
  return (
    <html lang="fa" dir="rtl">
      <body>
        <ThemeRegistry>
          <Providers>{children}</Providers>
        </ThemeRegistry>
      </body>
    </html>
  );
}
