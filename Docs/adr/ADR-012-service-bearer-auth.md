# ADR-012: احراز هویت سیستمی مصرف‌کننده‌ها (Bearer JWT در کنار کوکی BFF)

- **وضعیت:** Accepted
- **تاریخ:** 2026-09-15
- **نویسنده:** Technical Lead / Architecture (تأیید کارفرما — DEC-027)

## Context
BFF با کوکی HttpOnly برای مرورگر طراحی شده است؛ سامانه‌های مصرف‌کننده (BPMS/HR) نمی‌توانند از کوکی استفاده کنند. آن‌ها توکن JWT معتبر IAM را در اختیار دارند (صادرشده توسط IAM برای کاربر/سرویس خودشان).

## Decision
- Endpointها هر دو الگو را می‌پذیرند: کوکی نشست BFF **یا** هدر `Authorization: Bearer <IAM-JWT>`.
- اعتبارسنجی Bearer از طریق introspection سمت IAM (`/api/token/validate`، fail-closed) انجام می‌شود، نه اعتبارسنجی محلی امضا (کلید عمومی IAM در دسترس نیست).
- پس از اعتبارسنجی موفق، همان Claimها (user/tenant/org/role/permission/scope) ساخته می‌شود؛ Policyها و Scope یکسان اعمال می‌گردد.
- پیاده‌سازی با `PolicyScheme` هوشمند: در صورت وجود هدر Bearer به Scheme توکن، وگرنه به Scheme کوکی BFF.
- هیچ Secret/clients جدید در این سامانه تعریف نمی‌شود؛ اعتماد فقط به IAM است.

## Rationale
- یکسان‌سازی کامل Authorization برای انسان و سیستم (۲۷ Policy بدون تغییر).
- عدم نگهداری کلید/اعتبارنامه مصرف‌کننده‌ها در این سامانه.
- Fail-closed در دسترس‌نبودن IAM.

## Considered and rejected alternatives
- API Key اختصاصی در OrgStructure: رد شد — مدیریت اعتبار موازی و مغایر حاکمیت IAM.
- اعتبارسنجی محلی امضا: رد شد — کلید امضای IAM در دسترس/پایدار نیست.
- فقط کوکی BFF: رد شد — مصرف‌کننده‌های ماشینی را پوشش نمی‌دهد.

## Consequences
- مثبت: یک API واحد برای همه مصرف‌کننده‌ها؛ بدون تغییر Policyها.
- منفی: هر درخواست سرویسی یک introspection (با کش کوتاه قابل بهینه‌سازی در آینده).

**منابع:** `Docs/decision-log.md` (DEC-027)، ADR-002، `Docs/Architecture/iam-integration.md`