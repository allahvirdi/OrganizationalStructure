# Aggregates & Entities

**آخرین به‌روزرسانی:** `2026-09-15`
**وضعیت:** منجمد Phase 2 + اصلاحیه ADR-011 (تفکیک Responsibility/Authority)

---

## ۱. موجودیت‌های اصلی

### Post (Aggregate Root — BC-1)

- `Id` (Guid) ـ `TenantId` ـ `OrganizationId` (Reference به IAM)
- `Code` (یکتا درون Organization) ـ `Title` ـ `Description?`
- `ParentId?` (خودارجاع؛ یکتا و مستقیم)
- `IsActive`
- ستون‌های Audit/Soft Delete/RowVersion
- قواعد: یک Parent مستقیم، صفر یا چند Child؛ جابجایی فقط درون Organization.
- مسئولیت/اختیار روی Post **فیلد نیست**؛ از طریق Assignment متصل می‌شود (ADR-011).

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

### Responsibility (Aggregate Root — BC-3)

- `Id` ـ `TenantId` ـ `Code` (یکتا درون Tenant؛ Business Routing Key) ـ `Title` ـ `Description?` ـ `IsActive`
- `PostResponsibilityAssignment` (عضو): `OrganizationId` ـ `PostId` ـ `ResponsibilityId` ـ `StartDate?` ـ `EndDate?` ـ `IsActive`
- ستون‌های Audit/Soft Delete/RowVersion
- قواعد: انتساب Code-based به Post؛ Scope از طریق OrganizationId؛ پایان با تاریخ (بدون حذف).

### Authority (Aggregate Root — BC-4)

- `Id` ـ `TenantId` ـ `Code` (یکتا درون Tenant) ـ `Title` ـ `Description?` ـ `IsActive`
- `PostAuthorityAssignment` (عضو): `OrganizationId` ـ `PostId` ـ `AuthorityId` ـ `StartDate?` ـ `EndDate?` ـ `IsActive`
- از Responsibility جداست و به Signing محدود نیست (ADR-011).

### OrganizationReference (مصرفی — BC-1/BC-6)

- موجودیت محلی نیست؛ `OrganizationId` به‌صورت Reference است.
- برای اعتبارسنجی/نمایش ممکن است یک Read Model فقط‌خواندنی از سرویس IAM کش شود (طراحی Phase 2/۴).

## ۲. موجودیت‌های Access (BC-5)

- `Permission` (تعریف دامنه‌ای، ثبت/تخصیص در IAM)
- `Role` (تخصیص در IAM)
- `OrganizationScopeRule` / Visibility Policy (منطق دامنه در OrgStructure)

## ۳. موجودیت‌های Import (BC-6 — MVP Secondary)

- `StagingTable` (Schema نهایی: Q-003)
- `ImportBatch` (شناسه، وضعیت، زمان، کنشگر، آمار موفق/خطا)
- `ImportError` (سطر/ستون/پیام/کد خطا)

## ۴. جدول‌های مشترک

- `AuditLog` (پیوستگی، hashing)
- `OutboxMessage` (برای Future Phase — Deferred در MVP، ADR-007)

## ۵. یادداشت‌ها

- مدل Phase 2 منجمد شد؛ اصلاحیه ADR-011 (DEC-024) اعمال شد.
- جزئیات Routing در `Docs/domain/responsibility-routing.md`.