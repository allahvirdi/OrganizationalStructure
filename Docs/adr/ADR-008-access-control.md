# ADR-008: مدل Access Control — Permissionهای دامنه‌ای و Organization Scope

- **وضعیت:** Accepted (جزئیات ثبت/تخصیص در IAM وابسته به Q-004/Q-006)
- **تاریخ:** 2026-09-14
- **نویسنده:** Security Lead / Technical Lead (تأیید کارفرما)

## Context
مدل دسترسی باید ترکیبی از **User، Role، Permission و Organization Scope** باشد. IAM مالک Identity/User/Authentication/Organization Identity است. کارفرما تعیین کرد: OrgStructure مالک Access Control مربوط به قابلیت‌های خودش است (Role، Permission، Organization Scope، دسترسی کاربر به قابلیت‌های این سامانه) و Permissionهای دامنه‌ای در OrgStructure تعریف و **در IAM ثبت/تخصیص** میشوند.

اصل کلیدی: رابطه سازمانی کاربر با یک Organization به‌تنهایی به معنی داشتن Permission نیست.

## Decision
- **تعریف Permissionهای دامنه‌ای** (`Post.*`, `Employee.*`, `Authority.*`, `Assignment.*`, `Import.*`) متعلق به OrgStructure است.
- **ثبت/تخصیص** این Permissionها/Roleها در **IAM** انجام میشود (منبع صدور Claim). این سامانه Master موازی دسترسی نمی‌سازد.
- مدل تصریح‌دهی: **Deny by Default**؛ هر Endpoint باید Policy صریح داشته باشد.
- **Organization Scope** مبنای محدوده مشاهده/مدیریت است و از داده ساختار محاسبه میشود، نه از صرف رابطه سازمانی.
- قواعد Visibility:
  - ستاد: در محدوده تحت اختیار + تنها سطح مجاز Import فایل/جدول واسط.
  - استان: پرسنل استان و مناطق زیرمجموعه در محدوده مجاز.
  - منطقه: پرسنل منطقه در محدوده مجاز.
- Postهای دارای مسئولیت/امضا میتوانند در صورت Permission، زیرمجموعه ساختاری چندسطحی خود را ببینند.
- نقش‌ها و Permissionهای نهایی با نام و املای دقیق پس از حل Q-006 قطعی میشوند.

## Rationale
- حاکمیت دسترسی در یک نقطه (IAM) از واگرایی جلوگیری میکند.
- تفکیک «تعریف» از «تخصیص» مرز دامنه و Identity را روشن نگه میدارد.
- Deny by Default و Policy صریح مطابق الزام امنیتی Baseline.

## Considered and rejected alternatives
- مدیریت کامل و محلی Role/Permission در OrgStructure: رد شد — مغایر تصمیم صریح کارفرما (ثبت/تخصیص در IAM).
- اعتماد به رابطه سازمانی به‌عنوان مجوز: رد شد — نقض اصل کلیدی کسب‌وکار و ریسک امنیتی.

## Consequences
- مثبت: کنترل دسترسی شفاف، ممیزی‌پذیر و مبتنی بر Scope.
- منفی: نیاز به هماهنگی عملیاتی با مالک IAM برای تعریف/تخصیص Permission (Q-004) و نقش‌ها (Q-006).

**منابع:** `Docs/PROJECT-BASELINE-v0.1.md` (DEC-008)، `Docs/Organizational-Structure-Big-Picture-Scenario.md` (§۵، §۷–§۱۰)، `Docs/open-questions.md` (Q-004، Q-006)