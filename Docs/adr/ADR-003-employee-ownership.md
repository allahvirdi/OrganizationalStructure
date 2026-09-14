# ADR-003: Employee به‌عنوان مالک داده پرسنلی + Reference به IAM User

- **وضعیت:** Accepted
- **تاریخ:** 2026-09-14
- **نویسنده:** Technical Lead / Domain (تأیید کارفرما)

## Context
پرسش کلیدی: `Employee` و `IAM User` چه رابطه‌ای دارند؟ IAM دارای `User` (هویت/حساب)، `EmployeeIdentifier` و چرخه‌های Join/Move/Leave و Bulk Import است. کارفرما صراحتاً تعیین کرد که اطلاعات کامل پرسنلی در `OrganizationalStructure` نگهداری میشود.

## Decision
- `Employee` **موجودیت مستقل و مالک کامل اطلاعات پرسنلی در این دامنه** است. `Employee ≠ IAM User`.
- `Employee` اطلاعات کامل پرسنلی را نگهداری می‌کند.
- در صورت وجود حساب کاربری برای یک Employee، ارتباط از طریق `UserId` به IAM برقرار می‌شود؛ این Reference است و **FK فیزیکی ندارد**.
- `Employee` صرفاً Projection یا View از IAM نیست؛ یک موجودیت اصلی با چرخه حیات و قواعد دامنه‌ای خودش است.
- یک کارمند می‌تواند به **یک یا چند Post** منتسب شود (رابطه یک‌به‌یک نیست).

## Rationale
- داده پرسنلی (سوابق، وضعیت استخدامی، اطلاعات تکمیلی) متعلق به دامنه ساختار سازمانی است.
- تفکیک صریح از هویت کاربری، از ابهام مدل داده و همپوشانی مسئولیت جلوگیری میکند.
- Reference بدون FK، استقلال پایگاه‌دادهای دو سامانه را حفظ میکند.

## Considered and rejected alternatives
- تعریف Employee به‌عنوان Projection از IAM User: رد شد — ناقض تصمیم کارفرما و ناتوان در نگهداری داده پرسنلی کامل.
- FK فیزیکی به جدول User در IAM: رد شد — وابستگی پایگاه‌دادهای نامطلوب.
- یکسان‌دانستن Employee و User: رد شد — دو مفهوم متفاوت هستند.

## Consequences
- مثبت: مدل داده روشن، مالکیت مشخص PII پرسنلی، استقلال از IAM.
- منفی: همپوشانی احتمالی با داده پرسنلی موجود در IAM نیازمند هماهنگی مرزی است (Q-007)؛ همگام‌سازی `UserId` باید مدیریت شود.

**منابع:** `Docs/PROJECT-BASELINE-v0.1.md` (DEC-012)، `Docs/Organizational-Structure-Big-Picture-Scenario.md` (§۳، §۱۳)، `Docs/open-questions.md` (Q-007)