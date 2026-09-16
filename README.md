# سامانه ساختار سازمانی (Organizational Structure)

> هسته مستقل و مرجع معتبر مدیریت ساختار درختی سازمان‌ها، پست‌های سازمانی، انتساب کارکنان، مسئولیت‌ها، صاحبان امضا و کنترل دسترسی به ساختار.

## نام پروژه

- **نام فارسی:** سامانه ساختار سازمانی
- **نام انگلیسی:** Organizational Structure
- **نام فنی:** `OrganizationalStructure`

## هدف

این سامانه مرجع اطلاعات مربوط به سازمان، ساختار درختی سازمان، پست‌های سازمانی، روابط سلسله‌مراتبی پست‌ها، کارکنان، انتساب کارکنان به پست‌ها، مسئولیت‌ها، صاحبان امضا و دسترسی‌های مرتبط با ساختار است و خدمات ساختار سازمانی را به سایر سامانه‌ها ارائه می‌کند.

> سایر سامانه‌ها نباید منطق ساختار سازمانی را مجدداً پیاده‌سازی و نگهداری کنند؛ `OrganizationalStructure` باید مرجع معتبر ساختار سازمانی باشد.

## ساختار ریشه

```
/
├── Docs/          # تمام اسناد پروژه (معماری، ADR، قراردادها، فازها، گزارش‌ها)
├── Backend/       # کد سمت سرور (.NET 10 / Clean Architecture)
├── Frontend/      # کد سمت کلاینت (Next.js + MUI طبق Baseline)
├── SampleAdminPanel/  # قالب مرجع بصری/UX (نه مرجع Domain/Architecture)
├── .gitignore
├── .editorconfig
└── README.md
```

## وضعیت فعلی

- فاز جاری: `Phase 0 — Project Baseline`
- مرجع زنده پیشرفت: `Docs/04_Progress.md`
- تصمیم‌ها: `Docs/decision-log.md`
- ابهامات باز: `Docs/open-questions.md`

## اسناد پایه

- `Docs/01-Architecture-Baseline-FA.md` — معماری توسعه (Enterprise Baseline v2.4)
- `Docs/02-AI-Development-Path-0-to-100-FA.md` — مسیر توسعه با هوش مصنوعی v2.4
- `Docs/PROJECT-BASELINE-v0.1.md` — سند پایه پروژه
- `Docs/Organizational-Structure-Big-Picture-Scenario.md` — سناریو و تصویر بزرگ سامانه

## قوانین کلیدی توسعه

- تایم‌باکس هر تسک حداکثر ۱۵ دقیقه.
- کامیت فقط پس از تأیید صریح انسان.
- XML Documentation فارسی برای تمام Class/Property/Method/Interface.
- ممنوعیت `TODO` / `Mock` / `Sample` / `Incomplete`.
- اولویت اسناد در تناقض: `06_DevelopmentRules` → `03_Roadmap` → Phase → Architecture → ADR → SessionReports.

## وابستگی خارجی

- **Identity/User/Organization Master:** در `Enterprise-IAM-V2` نگهداری می‌شود.
- این سامانه Organization و User را فقط Reference می‌کند و Master موازی نمی‌سازد.
- جزئیات مرز در `Docs/adr/` ثبت شده است.

## پیش‌نیازهای توسعه محلی

- SQL Server محلی (یا LocalDB) برای اجرای API و تست‌های یکپارچگی.
- کلید رمزنگاری PII در `Backend/src/OrganizationalStructure.API/appsettings.Development.json`
  بخش `PiiEncryption:Key` (مقدار Base64 کلید ۲۵۶ بیتی) — این فایل با
  `git update-index --skip-worktree` از کامیت مستثنا شده است تا کلید محلی
  هر توسعه‌دهنده در مخزن ثبت نشود. برای تولید از Secret Management استفاده کنید.
