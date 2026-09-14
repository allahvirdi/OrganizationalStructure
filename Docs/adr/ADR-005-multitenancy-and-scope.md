# ADR-005: Multi-tenancy و Organization Scope

- **وضعیت:** Accepted
- **تاریخ:** 2026-09-14
- **نویسنده:** Technical Lead / Database (تأیید کارفرما)

## Context
Architecture Baseline چندمستأجری و ستون `TenantId` را در جدولهای اصلی الزامی می‌کند. کارفرما تأیید کرد **Multi-tenancy لازم است**. همچنین مدل دسترسی باید ترکیبی از User، Role، Permission و **Organization Scope** باشد.

## Decision
- **Multi-tenancy فعال است:** ستون `TenantId` روی تمام جداول اصلی و EF Core Global Query Filter برای جداسازی داده.
- الگوی ستون‌های پایه: `Id`, `TenantId`, `CreatedAt`, `CreatedById`, `UpdatedAt`, `UpdatedById`, `IsDeleted`, `DeletedAt`, `DeletedById`, `RowVersion/Version`.
- `Organization Scope` مبنای محدوده دسترسی کاربر است و از داده ساختار سازمانی (نه صرفاً رابطه سازمانی) محاسبه میشود.
- داشتن رابطه سازمانی با یک Organization به‌تنهایی به معنی داشتن Permission نیست.
- ستون/سطح دسترسی مبتنی بر محدوده سازمانی در فاز ۴ (Access & Visibility) پیادهسازی میشود.

## Rationale
- الزام صریح کارفرما و Baseline.
- جداسازی داده و جلوگیری از دسترسی غیرمجاز در سطح مستأجر.
- تفکیک Scope از رابطه سازمانی، مطابق اصل کلیدی Big Picture (§۷).

## Considered and rejected alternatives
- بدون Multi-tenancy: رد شد — مغایر تصمیم صریح کارفرما.
- جداسازی با چند پایگاه‌داده فیزیکی: رد شد — هزینه عملیاتی بدون نیاز.

## Consequences
- مثبت: جداسازی داده و مقیاس‌پذیری سازمانی؛ انطباق با Baseline.
- منفی: پیچیدگی Query Filter و ایندکس‌گذاری؛ نیاز به انضباط در تمام Queryها.

**منابع:** `Docs/PROJECT-BASELINE-v0.1.md` (DEC-005)، `Docs/01-Architecture-Baseline-FA.md` (§۷)، `Docs/Organizational-Structure-Big-Picture-Scenario.md` (§۵، §۷)