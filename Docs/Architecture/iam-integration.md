# طراحی یکپارچگی IAM (پروتکل مصرف احراز هویت/تصریح‌دهی)

> مسیر: `Docs/Architecture/iam-integration.md`
> وضعیت: **Pre-Draft** — نهایی و Freeze در Phase 1 پس از حل Q-002
> سامانه خارجی: `C:\Users\hp_zbook\Documents\GitHub\Herasat\Enterprise-IAM-V2` (فقط‌خواندنی از دید OrgStructure)

**آخرین به‌روزرسانی:** `2026-09-14`

---

## ۱. واقعیت‌های تأییدشده از بررسی IAM (Phase 0)
- IAM یک **سرویس REST با JWT سفارشی** است؛ **OIDC/OpenIddict نیست** (بدون `/.well-known` و `/connect/*`).
- لاگین تعاملی `POST /api/auth/login` → JWT + Refresh + Session (+ MFA در login/verify-mfa).
- اندپوینت‌های توکن: `POST /api/token` (تولید)، `/refresh`، `/revoke`، `/validate`.
- دو صادرکننده کلید (HS256 لاگین؛ RSA-2048 سرویس توکن) — ریسک ثبت‌شده در Phase 0.
- Claimها: `user_id, sub, username, first_name, last_name, national_identifier, mobile, email, role, organization_id, organization_name, organization_code, geographic_unit_id, ...`.
- `OrganizationType` فعلی: فقط `Organization=0, Department=1, Unit=2` (افزودن `Region=3` — Q-001/ADR-009).

## ۲. وابستگی محصول
- پروژه خواهر `NewPersonalManagmentPhysical` همین سناریو را با الگوی **BFF** حل کرده و برای اتصال امن، تغییراتی در IAM انجام داده است (ثبت RefreshToken، اعتبارسنجی ابطال‌آگاه، کلاینت سروربه‌سرور، کلید پایدار). این یک الگوی مرجع است.

## ۳. تصمیم باز (Q-002)
دو گزینه:
- **A) الگوی BFF** (همسو با پروژه خواهر و امن‌ترین): مرورگر کوکی نشست HttpOnly دارد؛ Backend توکن IAM را اعتبارسنجی (validate با کش کوتاه، fail-closed) می‌کند.
- **B) اعتبارسنجی مستقیم JWT** در OrgStructure با کلید/کیفیت IAM.

> تصمیم نهایی در **Phase 1** با ارجاع به Risk Register و هماهنگی مالک IAM گرفته می‌شود. تا آن زمان هیچ پیاده‌سازی نمی‌شود.

## ۴. قواعد امنیتی مقدماتی (پیش‌نویس)
- fail-closed: در نبود IAM، درخواست رد می‌شود.
- هیچ اعتبارنامه/توکنی در مرورگر/لاگ/کد ذخیره نشود.
- فراخوانی به IAM فقط سرور→سرور با HTTPS.
- نگاشت `role` با املای دقیق (Q-006) به Policy داخلی.

## ۵. پیگیری
- Q-002 (پروتکل)، Q-004 و Q-006 (نقش/Permission) از `Docs/open-questions.md`.

> **این سند پیش‌نویس است و تا انجماد در Phase 1 مبنای پیاده‌سازی نیست.**