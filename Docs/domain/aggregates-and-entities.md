# Aggregates & Entities

**آخرین به‌روزرسانی:** `2026-09-14`
**وضعیت:** طراحی اولیه Phase 0 — نهایی شدن در Phase 2 (Domain Modeling)

> اسامی/ساختار این سند موقعیت طراحی اولیه را نشان می‌دهد و در Phase 2 با اعتبارسنجی تیم، نهایی و انجماد می‌شود.

---

## ۱. موجودیت‌های اصلی

### Post (Aggregate Root — BC-1)

- `Id` (Guid) ـ `TenantId` ـ `OrganizationId` (Reference به IAM)
- `Code` (یکتا درون Organization) ـ `Title` ـ `Description?`
- `ParentId?` (خودارجاع؛ یکتا و مستقیم)
- `HasSigningAuthority` (bool) ـ `Responsibilities` (مجموعه)
- `IsActive`
- ستون‌های Audit/Soft Delete/RowVersion
- قواعد: یک Parent مستقیم، صفر یا چند Child؛ جابجایی فقط درون Organization.

### Employee (Aggregate Root — BC-2)

- `Id` (Guid) ـ `TenantId`
- `UserId?` (Reference به IAM؛ اختیاری)
- اطلاعات پرسنلی (PII رمزنگاری‌شده): نام، نام خانوادگی، کد ملی، موبایل، پرونده‌های مرتبط
- `EmployeePostAssignmentCollection` (یک یا چند Post)
- `IsActive`
- ستون‌های Audit/Soft Delete/RowVersion

### EmployeePostAssignment (رابطه)

- `Id` ـ `EmployeeId` ـ `PostId` ـ `FromDate?` ـ `ToDate?` ـ `IsPrimary?`
- یکتا درون `{EmployeeId, PostId}`
- امکان چند انتساب برای یک Employee وجود دارد.

### SigningAuthority / Responsibility (مؤلفه‌های Post — BC-3)

- به‌صورت فیلد/مجموعه روی `Post` مدل می‌شود (`HasSigningAuthority`, `Responsibilities`).
- هر دو اطلاعات معتبر Domain هستند و در API/Integration قابل استعلام‌اند.

### OrganizationReference (مصرفی — BC-1/BC-6)

- موجودیت محلی نیست؛ `OrganizationId` به‌صورت Reference است.
- برای اعتبارسنجی/نمایش ممکن است یک Read Model فقط‌خواندنی از سرویس IAM کش شود (طراحی Phase 2/۴).

## ۲. موجودیت‌های Access (BC-4)

- `Permission` (تعریف دامنه‌ای، ثبت/تخصیص در IAM)
- `Role` (تخصیص در IAM)
- `OrganizationScopeRule` / Visibility Policy (منطق دامنه در OrgStructure)

## ۳. موجودیت‌های Import (BC-5 — MVP Secondary)

- `StagingTable` (Schema نهایی: Q-003)
- `ImportBatch` (شناسه، وضعیت، زمان، کنشگر، آمار موفق/خطا)
- `ImportError` (سطر/ستون/پیام/کد خطا)

## ۴. جدول‌های مشترک

- `AuditLog` (پیوستگی، hashing)
- `OutboxMessage` (برای Future Phase — Deferred در MVP، ADR-007)

## ۵. یادداشت‌های Phase 2

- ارزش‌های نهایی، Enums (وضعیت پرسنل، انواع مسئولیت، نوع انتساب) و رویدادهای دامنه در Phase 2 با تایید تیم تعریف و انجماد می‌شوند.
- ستون‌های استاندارد و ایندکس‌ها طبق Database Baseline فاز ۱/۲ طراحی می‌شوند.
- هیچ تصمیم نهایی مدل‌داده در این سند منعقد نمی‌شود؛ فقط موقعیت اولیه است.