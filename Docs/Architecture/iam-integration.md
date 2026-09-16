# طراحی یکپارچگی IAM (پروتکل مصرف احراز هویت/تصریح‌دهی)

> مسیر: `Docs/Architecture/iam-integration.md`
> وضعیت: **مصوب و پیاده‌سازی‌شده در Phase 4** (الگوی BFF — DEC-018)
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

## ۳. تصمیم نهایی (Q-002 — بسته شد)
**الگوی BFF** انتخاب و پیاده‌سازی شد:
- مرورگر فقط کوکی نشست HttpOnly (`orgstructure_session`) دارد؛ توکن‌های IAM فقط سمت سرور (`IBffSessionStore`).
- اعتبارسنجی با کش کوتاه (`IamOptions.ValidationCacheSeconds`) و fail-closed؛ در دسترس‌نبودن IAM یعنی رد درخواست.
- در ورود: حل Scope از درخت IAM (`GET /api/organizations/tree` با Bearer کاربر)؛ Scope خالی یعنی شکست ورود.
- Claimهای نشست: `user_id`، `tenant_id`، `organization_id`، `role`، `permission`، `organization_scope` (چندمقداری).

## ۴. پیاده‌سازی (Phase 4)
| جزء | مسیر |
|---|---|
| قرارداد و نشست | `Application/Integration/Iam/{IIamClient,IBffSessionStore}` |
| کلاینت HTTP | `Infrastructure/Integration/{IamClient,InMemoryBffSessionStore}` + `Security/IamOptions` |
| هندلر احراز هویت | `API/Security/BffSessionAuthenticationHandler` (Scheme «Bff») |
| ورود/خروج/کاربر جاری | `API/Controllers/AuthController` |
| Scope | `Application/Authorization/OrganizationScope` (خالص) |
| Policyها (۲۷) + Fallback | `API/Security/AuthorizationPolicies` |

محدودیت ثبت‌شده: نشست درون‌حافظه‌ای (تک‌نمونه)؛ Redis برای چندنمونه‌ای در Phase 8.

## ۴. قواعد امنیتی مقدماتی (پیش‌نویس)
- fail-closed: در نبود IAM، درخواست رد می‌شود.
- هیچ اعتبارنامه/توکنی در مرورگر/لاگ/کد ذخیره نشود.
- فراخوانی به IAM فقط سرور→سرور با HTTPS.
- نگاشت `role` با املای دقیق (Q-006) به Policy داخلی.

## ۵. پیگیری
- Q-002 (پروتکل)، Q-004 و Q-006 (نقش/Permission) از `Docs/open-questions.md`.

> **این سند پیش‌نویس است و تا انجماد در Phase 1 مبنای پیاده‌سازی نیست.**