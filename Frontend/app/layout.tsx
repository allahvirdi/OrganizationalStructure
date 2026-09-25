import type { Metadata } from "next";
import "@fontsource/vazirmatn/400.css";
import "@fontsource/vazirmatn/500.css";
import "@fontsource/vazirmatn/600.css";
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
      <head>
        {/*
         * اسکریپت مسدودکنندهٔ جلوگیری از فلش تم (FOUC):
         * پیش از اولین رندر، پس‌زمینه و color-scheme ریشه را بر اساس انتخاب
         * ذخیره‌شدهٔ کاربر تنظیم می‌کند تا در تم تاریک، صفحه روشن چشمک نزند.
         */}
        <script
          dangerouslySetInnerHTML={{
            __html: `(function(){try{var m=window.localStorage.getItem('orgstructure-color-mode');var d=document.documentElement;if(m==='dark'){d.style.backgroundColor='#111b22';d.style.colorScheme='dark';}else{d.style.backgroundColor='#f7f9fb';d.style.colorScheme='light';}}catch(e){}})();`,
          }}
        />
      </head>
      <body>
        <ThemeRegistry>
          <Providers>{children}</Providers>
        </ThemeRegistry>
      </body>
    </html>
  );
}
