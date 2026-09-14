# ADR-002: مرز Master Data با Enterprise-IAM-V2 (Organization و User Identity)

- **وضعیت:** Accepted (جزئیات پروتکل اتصال در `open-questions.md` Q-002)
- **تاریخ:** 2026-09-14
- **نویسنده:** Technical Lead / Architecture (تأیید کارفرما)

## Context
اطلاعات اصلی سازمان‌ها و هویت کاربران در `Enterprise-IAM-V2` نگهداری می‌شود. Entity `Organization` در IAM مالک اطلاعات سازمان است. درخواست کارفرما: در `OrganizationalStructure` نباید Master Data موازی و مستقل برای Organization یا هویت User ایجاد شود.

بررسی IAM نشان داد این سامانه یک سرویس REST با JWT سفارشی است (نه سرور OIDC/OpenIddict استاندارد) و Claimهایی مانند `organization_id`, `organization_name`, `organization_code`, `user_id` را صادر می‌کند. `OrganizationType` فعلی IAM فقط سه مقدار `Organization=0, Department=1, Unit=2` دارد.

## Decision
- **Organization و User Identity، Master در IAM هستند.** این سامانه هیچ Master موازی برای آن‌ها نمی‌سازد و در دیتابیس/سرویس IAM نمی‌نویسد.
- `OrganizationId` و `UserId` صرفاً **Reference** هستند؛ بین دو سیستم **FK فیزیکی** ایجاد نمی‌شود.
- کلید سازمان از طریق Claim در اختیار سیستم است.
- احراز هویت/تصریح‌دهی فقط از IAM مصرف میشود؛ این سامانه هرگز سرور هویت پیاده‌سازی نمیکند.
- `OrganizationType` فعلی IAM **Source of Truth** است و OrgStructure آن را تغییر نمی‌دهد.
- جزئیات پروتکل اتصال (BFF یا اعتبارسنجی مستقیم JWT، توزیع کلید، Refresh/Revoke) در فاز ۱ و با حل Q-002 نهایی می‌شود.
- تغییرات لازم در کد IAM فقط با تأیید صریح کارفرما مجاز است (DEC-017).

## Rationale
- حاکمیت داده سازمانی و هویتی در یک نقطه، از واگرایی و ناسازگاری جلوگیری می‌کند.
- عدم نگهداری اعتبارنامه کاربر در این سامانه سطح حمله را کاهش میدهد.
- ثبت انحراف از متن Baseline (OIDC/OpenIddict) به‌جای پنهان‌کردن آن.

## Considered and rejected alternatives
- ساخت Master موازی Organization/User در OrgStructure: رد شد — مغایر تصمیم صریح کارفرما.
- پیاده‌سازی OIDC/OpenIddict مستقل: رد شد — مغایر قانون «مصرف‌کننده بودن» و هزینه نگهداری.
- FK فیزیکی متقابل بین پایگاه‌داده OrgStructure و IAM: رد شد — وابستگی پایگاه‌داده‌ای نامطلوب.

## Consequences
- مثبت: یکپارچگی با زیرساخت هویتی موجود، عدم تکرار مدیریت هویت.
- منفی: وابستگی در دسترس‌پذیری به IAM؛ نیاز به هماهنگی مالک IAM برای Region و Permission.
- ریسکهای شناسایی‌شده در Risk Register ثبت و در فاز ۱ ارزیابی می‌شوند.

**منابع:** `Docs/PROJECT-BASELINE-v0.1.md` (DEC-011، DEC-013)، `Docs/open-questions.md` (Q-001، Q-002، Q-004)