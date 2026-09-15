# ERD — مدل داده سامانه ساختار سازمانی

**آخرین به‌روزرسانی:** `2026-09-14`
**وضعیت:** طراحی Phase 2 — پیاده‌سازی EF در Phase 3

> قراردادها: Multi-tenancy با `TenantId` (ADR-005)، Audit/Soft Delete روی جداول اصلی، PII با Always Encrypted (ADR-006)، بدون FK فیزیکی به IAM (ADR-002).

---

## ۱. ERD مفهومی

```
┌──────────────────┐         ┌──────────────────┐
│   Organization   │         │     IAM User     │
│   (مرجع — IAM)   │         │  (مرجع — IAM)    │
└────────┬─────────┘         └────────┬─────────┘
         │ OrganizationId             │ UserId (اختیاری)
         │ (Reference)                │ (Reference)
         ▼                            ▼
┌──────────────────┐         ┌──────────────────┐
│       Post       │         │     Employee     │
│  ──────────────  │         │  ──────────────  │
│  Code (UQ/Org)   │         │  PersonnelCode   │
│  Title           │         │  FirstName (PII) │
│  ParentId (FK)   │         │  LastName (PII)  │
│  SigningAuth     │         │  NationalCode    │
│  IsActive        │         │  Mobile (PII)    │
└────────┬─────────┘         └────────┬─────────┘
         │                            │
         │  PostId                    │  EmployeeId
         │                            │
         └────────────┬───────────────┘
                      ▼
         ┌──────────────────────────┐
         │ EmployeePostAssignment   │
         │  ──────────────────────  │
         │  EmployeeId (FK)         │
         │  PostId (FK, بدون ناوبری)│
         │  FromDate / ToDate       │
         │  IsPrimary               │
         └──────────────────────────┘
```

## ۲. ERD منطقی (جدول‌ها)

### Posts
| ستون | نوع | توضیح |
|------|-----|-------|
| Id | uniqueidentifier PK | |
| TenantId | uniqueidentifier NOT NULL | ایندکس ترکیبی |
| OrganizationId | uniqueidentifier NOT NULL | مرجع IAM؛ ایندکس |
| Code | nvarchar(50) NOT NULL | یکتا درون (TenantId, OrganizationId) |
| Title | nvarchar(200) NOT NULL | |
| Description | nvarchar(1000) NULL | |
| ParentId | uniqueidentifier NULL, FK→Posts.Id | ایندکس |
| HasSigningAuthority | bit NOT NULL DEFAULT 0 | |
| IsActive | bit NOT NULL DEFAULT 1 | |
| CreatedAt/CreatedById/UpdatedAt/UpdatedById | audit | |
| IsDeleted/DeletedAt/DeletedById | soft delete | |
| Version | rowversion | همروندی |

- ایندکس یکتا: `(TenantId, OrganizationId, Code)` — فیلتر `IsDeleted = 0` در صورت پشتیبانی
- ایندکس: `(TenantId, ParentId)` برای پیمایش درخت
- Responsibilities به‌صورت Owned Collection (جدول `PostResponsibilities`: PostId FK + Title + Description)

### Employees
| ستون | نوع | توضیح |
|------|-----|-------|
| Id | uniqueidentifier PK | |
| TenantId | uniqueidentifier NOT NULL | ایندکس ترکیبی |
| UserId | uniqueidentifier NULL | مرجع IAM؛ بدون FK؛ ایندکس |
| PersonnelCode | char(8) NOT NULL | عدد ۸ رقمی؛ یکتا درون TenantId |
| FirstName | PII (Randomized) NOT NULL | |
| LastName | PII (Randomized) NOT NULL | |
| NationalCode | PII (Deterministic) NOT NULL | ایندکس جستجو روی هش/رمز |
| Mobile | PII (Deterministic) NULL | |
| BirthDate | date NULL — PII (Randomized, ADR-010) | تاریخ تولد |
| PezhvakMobile | PII (Deterministic) NULL — ADR-010 | موبایل پژواک |
| ServiceYears | int NULL — ADR-010 | سال سابقه حراست (Owned VO) |
| ServiceMonths | int NULL — ADR-010 | ماه سابقه حراست ۰..۱۱ (Owned VO) |
| IsActive | bit NOT NULL DEFAULT 1 | پایان فعالیت = غیرفعال؛ هرگز حذف فیزیکی (DEC-022) |
| Audit/SoftDelete/Version | استاندارد | |

### EmployeePostAssignments
| ستون | نوع | توضیح |
|------|-----|-------|
| Id | uniqueidentifier PK | |
| TenantId | uniqueidentifier NOT NULL | |
| EmployeeId | uniqueidentifier NOT NULL, FK→Employees.Id | ایندکس |
| PostId | uniqueidentifier NOT NULL, FK→Posts.Id | ایندکس (بدون ناوبری در دامنه) |
| FromDate/ToDate | date NULL | |
| IsPrimary | bit NOT NULL DEFAULT 0 | |
| Audit/SoftDelete | استاندارد | |

- ایندکس یکتا: `(TenantId, EmployeeId, PostId)` برای انتساب‌های فعال (با فیلتر IsDeleted)
- قانون «حداکثر یک اصلی فعال» در دامنه + کنترل در لایه کاربرد

## ۳. یادداشت‌های پیاده‌سازی (Phase 3)

1. پیکربندی EF: `IEntityTypeConfiguration` به‌ازای هر Aggregate + Owned برای Responsibilities.
2. Global Query Filters از Phase 1 اعمال است (`TenantId` + `IsDeleted`).
3. رمزنگاری PII در Phase 3 (ستون + هش جستجو برای Deterministic).
4. Migration اولیه پس از تأیید ERD در همین فاز/فاز ۳.
5. هیچ FK به جداول IAM ساخته نمی‌شود (فقط ستون Reference + ایندکس).