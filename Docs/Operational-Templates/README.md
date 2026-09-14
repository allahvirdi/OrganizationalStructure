# Operational Templates — قالب‌های عملیاتی

این پوشه شامل قالب‌های خالی و آماده برای استفاده در هر پروژه است.

## فایل‌ها

| فایل | توضیح |
|------|-------|
| `00_ProjectContext.md` | زمینه و چشم‌انداز پروژه |
| `01_Architecture.md` | خلاصه معماری + لینک به Baseline |
| `02_CodingStandards.md` | استانداردهای کدنویسی + XML فارسی + ممنوعیت‌ها |
| `03_Roadmap.md` | نقشه راه فازها |
| `04_Progress.md` | پیشرفت زنده (اجباری در پایان هر Session) |
| `05_ChangeLog.md` | تاریخچه تغییرات |
| `06_DevelopmentRules.md` | **قوانین اجباری** (تایم‌باکس ۱۵ دقیقه، کامیت پس از تأیید، XML، امنیت و ...) — بالاترین اولویت |
| `PhaseXX_Template.md` | قالب استاندارد هر فاز |
| `SessionReport_Template.md` | قالب گزارش پایان هر Session |

## قوانین کلیدی که در این قالب‌ها لحاظ شده‌اند

- تایم‌باکس ۱۵ دقیقه‌ای برای هر تسک
- کامیت فقط بعد از تأیید صریح انسان
- XML Documentation فارسی اجباری
- ممنوعیت TODO / Incomplete / Sample
- ساختار Progress و Session Report
- قابلیت ادامه بدون تاریخچه گفتگو
- اولویت اسناد
- SampleAdminPanel
- امنیت و تست اجباری

## نحوه استفاده

1. محتویات این پوشه را به `Docs/` پروژه کپی کنید.
2. نام `PhaseXX_Template.md` را به `Phase00.md`، `Phase01.md` و ... تغییر دهید و پر کنید.
3. برای هر Session از `SessionReport_Template.md` یک کپی با نام مناسب بسازید.
4. `06_DevelopmentRules.md` را به عنوان مرجع اصلی قوانین نگه دارید.
