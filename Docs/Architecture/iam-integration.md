# طراحی یکپارچگی IAM (پروتکل مصرف احراز هویت/تصریح‌دهی)

> مسیر: `Docs/Architecture/iam-integration.md`
> وضعیت: **مصوب و پیاده‌سازی‌شده در Phase 4** (الگوی BFF — DEC-018)
> سامانه خارجی: `C:\Users\hp_zbook\Documents\GitHub\Herasat\Enterprise-IAM-V2` (فقط‌خواندنی از دید OrgStructure)

**آخرین به‌روزرسانی:** `2026-09-17`

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

## ۵. قواعد امنیتی مقدماتی
- fail-closed: در نبود IAM، درخواست رد می‌شود.
- هیچ اعتبارنامه/توکنی در مرورگر/لاگ/کد ذخیره نشود.
- فراخوانی به IAM فقط سرور→سرور (HTTPS در Production).
- نگاشت `role` با املای دقیق (Q-006) به Policy داخلی.

## ۶. پیش‌نیازهای عملیاتی اتصال به IAM
> یافته‌های اشکال‌زدایی ۴۰۱ ورود در `Session-20260917-Phase6-IamFix`.

اتصال BFF به IAM در محیط توسعه به سه شرط وابسته است؛ نقض هر شرط، ورود را fail-closed و با کد ۴۰۱ رد می‌کند:

| # | پیش‌نیاز | محل تنظیم | نشانه نقض |
|---|----------|-----------|-----------|
| ۱ | مقداردهی `Iam:BaseAddress` | `appsettings.Development.json` (skip-worktree) یا متغیر `Iam__BaseAddress` | هشدار راه‌اندازی برای «Iam:BaseAddress» و خطای «خطا در ارتباط با سامانه هویت.» |
| ۲ | هم‌ترازی `Iam:ClientSecret` با کلاینت `personnel-bff` در IAM | همان‌جا؛ مرجع سمت IAM: `Authentication:BffClient:Secret` | پاسخ ۴۰۱ `invalid_client` روی `POST /api/token/validate` |
| ۳ | وجود Claim `organization_id` در JWT و درخت سازمانی غیرخالی | سمت IAM (Master Data — Q-009، بسته‌شده با DEC-028) | ۴۰۱ با `title = Auth.NoScope` |
| ۴ | ثبت `IBffSessionStore` به‌صورت **Singleton** | `Infrastructure/DependencyInjection.cs` | `/api/v1/auth/me` با پیام «نشست معتبر نیست» (۴۰۱) علی‌رغم لاگین موفق — استور Scoped یعنی هر درخواست استور خالی |

مسیر کامل یک ورود موفق:

```text
POST /api/v1/auth/login
  ├─ IAM  POST /api/auth/login                 (نام کاربری/رمز)
  ├─ IAM  POST /api/token/validate             (هدرهای X-Client-Id / X-Client-Secret)
  ├─ IAM  GET  /api/organizations/tree          (Bearer توکن کاربر)
  └─ ساخت نشست سمت‌سرور + کوکی HttpOnly «orgstructure_session»
```

نکات قطعی:
- کلاینت `personnel-bff` در IAM باید **Confidential** و فعال باشد تا `ClientAuthenticationFilter` آن را بپذیرد.
- تنظیمات ناقص IAM در راه‌اندازی با هشدار صریح (`IamConfigurationValidator`) گزارش می‌شود تا شکست خاموش رخ ندهد.
- `IamClient` بدنه خطای IAM را حتی در پاسخ‌های ناموفق می‌خواند تا پیام دقیق (مثلاً «نام کاربری یا رمز عبور نامعتبر است.») از دست نرود.
- Master Data سازمان محیط توسعه (Q-009 — DEC-028): سازمان `شرکت هرسات (محیط توسعه)` با کد `herasat-dev` (شناسه `fd0e79eb-27b9-4e19-b348-070fa091d5dc`) از طریق API خودِ IAM (`POST /api/organizations` + `POST /api/users/{id}/assign-organization`) ثبت و به کاربران Seed تخصیص یافت؛ بدون تغییر کد/الگوی داده IAM.
- مسیر صحیح API تخصیص سازمان در IAM: `POST api/users/{userId}/assign-organization` (کنترلر `AssignmentsController` با Route `api/users`) — نه `api/assignments/...`.

## ۷. پیگیری
- Q-002 (پروتکل)، Q-004 و Q-006 (نقش/Permission) از `Docs/open-questions.md`.
- Q-009 بسته شد (DEC-028)؛ Q-010 (قرارداد خطای Auth) باز است.