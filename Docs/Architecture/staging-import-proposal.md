# طراحی پیشنهادی Import از جدول واسط (پاسخ Q-003 — DEC-030)

**تاریخ:** `2026-09-21`
**وضعیت:** تأییدشده با تغییرات (کارفرما ۲۰۲۶-۰۹-۲۱) — آماده پیاده‌سازی
**مرتبط با:** فاز ۷، `open-questions.md` (Q-003)، `Docs/domain/aggregates-and-entities.md` بخش ۳، `Docs/Architecture/erd.md`

---

## ۱. نیاز و تصمیم مبدأ

تصویب جلسه ۲۰۲۶-۰۹-۲۱: **هر دو منشأ با یک فرآیند یکسان بررسی/تأیید**:

1. **مسیر فایل:** کاربر فایل Excel/CSV را در سامانه بارگذاری می‌کند؛ سامانه ردیف‌ها را پس از پارس و اعتبارسنجی اولیه وارد جدول واسط می‌کند؛ کاربر بازبینی و تأیید/رد می‌کند.
2. **مسیر بیرونی:** سامانه بیرونی (مثلاً منابع انسانی) ردیف‌ها را با کاربر اختصاصی دیتابیس (فقط `INSERT`) مستقیم در جدول واسط درج می‌کند؛ سپس اپراتور در سامانه بازبینی و تأیید/رد می‌کند.

مسیر فعلی «ورود مستقیم از فایل» (`POST /api/v1/import/employees`) بدون تغییر باقی می‌ماند؛ جدول واسط یک مسیر **جداگانه با بازبینی انسانی** است.

## ۲. چرخه حیات (یکسان برای هر دو منشأ)

وضعیت‌های Batch: `AwaitingRows` (فقط مسیر بیرونی) → `Ready` → `Committed` / `Rejected`.

## ۳. جداول پیشنهادی

> قراردادها مطابق `erd.md`: PII مطابق ADR-006، بدون FK به IAM، Audit استاندارد.
> **تغییر مصوب کارفرما:** `TenantId` در جداول واسط حذف شده (جدول واسط موقتی و وابسته به Batch است).

### ۳.۱. ImportBatches (شناسنامه هر بارگذاری)

| ستون | نوع | توضیح |
|------|-----|-------|
| Id | uniqueidentifier PK | |
| OrganizationId | uniqueidentifier NOT NULL | سازمان مالک (مرجع IAM؛ بدون FK) |
| Source | tinyint NOT NULL | `1=File`، `2=External` |
| FileName | nvarchar(255) NULL | نام فایل اصلی (فقط مسیر فایل) |
| Status | tinyint NOT NULL | `1=AwaitingRows`، `2=Ready`، `3=Committed`، `4=Rejected` |
| TotalRows | int NOT NULL DEFAULT 0 | تعداد ردیف‌های بارگذاری‌شده |
| ValidRows | int NOT NULL DEFAULT 0 | نتیجه اعتبارسنجی |
| InvalidRows | int NOT NULL DEFAULT 0 | نتیجه اعتبارسنجی |
| CommittedCount | int NULL | تعداد ثبت‌شده نهایی در لحظه Commit |
| Notes | nvarchar(500) NULL | یادداشت بازبین (به‌ویژه هنگام رد) |
| CreatedById / CreatedAt | audit NOT NULL | ایجادکننده بارگذاری |
| ReviewedById / ReviewedAt | audit NULL | تأیید/ردکننده |
| CommittedAt | datetimeoffset NULL | لحظه ثبت نهایی |
| Version | rowversion | همروندی |

ایندکس‌ها: `(OrganizationId, Status)` و `(CreatedAt DESC)` برای فهرست بارگذاری‌ها.

### ۳.۲. EmployeeStagingRows (جدول واسط — همان فیلدهای ورودی پرسنل)

| ستون | نوع | توضیح |
|------|-----|-------|
| Id | uniqueidentifier PK | |
| BatchId | uniqueidentifier NOT NULL, FK→ImportBatches.Id (Cascade) | ایندکس |
| RowNumber | int NOT NULL | شماره ردیف در فایل / ترتیب درج |
| PersonnelCode | char(8) NOT NULL | خام (همان قواعد پرسنل: ۸ رقم) |
| FirstName | PII (Randomized) NOT NULL | هم‌راستا با Employees |
| LastName | PII (Randomized) NOT NULL | |
| NationalCode | PII (Deterministic) NOT NULL | |
| Mobile | PII (Deterministic) NULL | |
| BirthDate | date NULL — PII (Randomized) | شمسی در لحظه پارس به میلادی تبدیل می‌شود |
| ServiceYears | int NULL | |
| ServiceMonths | int NULL | ۰..۱۱ |
| ValidationStatus | tinyint NOT NULL | `1=Pending`، `2=Valid`، `3=Invalid` |
| EmployeeId | uniqueidentifier NULL | Employee ایجادشده پس از Commit |
| CommitStatus | tinyint NOT NULL DEFAULT 1 | `1=Pending`، `2=Committed`، `3=Skipped` |
| CreatedAt | datetimeoffset NOT NULL | |

ایندکس: `(BatchId)`. بررسی تکراری (کد پرسنلی درون بارگذاری و در دیتابیس) در لایه کاربرد انجام می‌شود (هم‌راستا با مسیر فایل).

> **نکته:** `PezhvakMobile` در جدول واسط حضور ندارد (تعیین آن فقط از ویرایش تکمیلی — ADR-013).
> **نکته PII:** ستون‌های حساس دقیقاً مانند `Employees` رمزنگاری می‌شوند؛ جدول واسط **موقتی** است و سیاست پاک‌سازی دارد (بخش ۵).

### ۳.۳. ImportErrors (خطاهای ردیف/ستون — مطابق سند دامنه)

| ستون | نوع | توضیح |
|------|-----|-------|
| Id | uniqueidentifier PK | |
| BatchId | uniqueidentifier NOT NULL, FK→ImportBatches.Id (Cascade) | ایندکس |
| StagingRowId | uniqueidentifier NULL | NULL برای خطاهای سطح فایل/بارگذاری |
| RowNumber | int NULL | |
| ColumnName | nvarchar(100) NULL | |
| ErrorCode | nvarchar(100) NOT NULL | مانند `Import.EmployeeRowInvalid` |
| Message | nvarchar(500) NOT NULL | **بدون افشای مقدار حساس** (ADR-006) — فقط «ردیف + علت» |
| CreatedAt | datetimeoffset NOT NULL | |

## ۴. قرارداد API پیشنهادی

همه با مجوز موجود `OrganizationStructure.Employee.Import` (کاتالوگ، سطر ۴۲: «جدول واسط» پوشش داده شده — **بدون مجوز جدید**) + بررسی `VisibleOrganizationIds` در هندلر.

| متد | مسیر | شرح | پاسخ |
|-----|------|------|------|
| POST | `/api/v1/import/employees/staging/upload` | multipart (`organizationId`+`file`) → پارس و درج در واسط | `201 {batchId, totalRows, validRows, invalidRows}` |
| POST | `/api/v1/import/employees/staging/batches` | ایجاد بارگذاری مسیر بیرونی (`organizationId`) | `201 {batchId}` با وضعیت `AwaitingRows` |
| POST | `/api/v1/import/employees/staging/{batchId}/ready` | اعلام آماده بودن پس از درج بیرونی + اعتبارسنجی | `200` + آمار |
| GET | `/api/v1/import/employees/staging/batches` | فهرست بارگذاری‌ها (صفحه‌بندی + فیلتر وضعیت/سازمان) | صفحه‌بندی‌شده |
| GET | `/api/v1/import/employees/staging/{batchId}/rows` | ردیف‌ها + وضعیت اعتبارسنجی + خطاها (صفحه‌بندی) | صفحه‌بندی‌شده |
| POST | `/api/v1/import/employees/staging/{batchId}/commit` | تأیید و ثبت نهایی (ثبت معتبرها + اعلام نامعتبرها) | `200 {committedCount, skippedCount, errors}` |
| POST | `/api/v1/import/employees/staging/{batchId}/reject` | رد بارگذاری با `notes` | `200` |

### قواعد اعتبارسنجی ردیف (مشترک با مسیر فایل)

کد پرسنلی ۸ رقمی · نام/نام‌خانوادگی الزامی · کد ملی با checksum · موبایل `^09[0-9]{9}$` در صورت حضور · تاریخ شمسی معتبر · سابقه ۰..۵۰ / ۰..۱۱ · تکراری کد پرسنلی/ملی **درون بارگذاری** → خطای ردیف.

### قواعد لحظه Commit (مصوب کارفرما: ثبت معتبرها + اعلام نامعتبرها)

- بازاعتبارسنجی کامل + بررسی تکراری در دیتابیس.
- **ثبت معتبرها:** ردیف‌های معتبر به `Employee` تبدیل می‌شوند (`PezhvakIsActive = null`، بدون انتساب پست).
- **اعلام نامعتبرها:** ردیف‌های نامعتبر/تکراری در پاسخ به کاربر گزارش می‌شوند (شماره ردیف + علت) ولی مانع ثبت بقیه نمی‌شوند.
- پاسخ شامل `committedCount` + `skippedCount` + فهرست خطاها.

## ۵. امنیت، دسترسی بیرونی و نگهداری

- **کاربر دیتابیس بیرونی:** فقط `INSERT`/`SELECT` روی دو جدول واسط؛ بدون `UPDATE`/`DELETE`. (پیکربندی DBA — خارج از کد سامانه.)
- **پاک‌سازی:** ردیف‌های بارگذاری‌های `Committed`/`Rejected` پس از **۱۰ روز** حذف فیزیکی می‌شوند (تأییدشده کارفرما؛ داده اصلی در `Employees` ثبت شده).
- پیام‌های خطا فاقد مقدار حساس هستند (هم‌راستا با مسیر فایل و ADR-006).

## ۶. تصمیمات کسب‌وکار (مصوب کارفرما ۲۰۲۶-۰۹-۲۱)

| # | سؤال | تصمیم نهایی |
|---|------|-------------|
| ۱ | هنگام وجود ردیف نامعتبر در لحظه ثبت | **ثبت معتبرها + اعلام نامعتبرها به کاربر** |
| ۲ | بازه پاک‌سازی ردیف‌های واسط | **۱۰ روز** |
| ۳ | اعلام آماده بودن بارگذاری بیرونی | **دستی (فعلاً)؛ خودکار در آینده** |
| ۴ | فرمت تاریخ در درج بیرونی | **میلادی (ISO)** |
| ۵ | حذف فیزیکی ردیف واسط | **بله — حذف فیزیکی تأیید می‌شود** |

## ۷. طرح اجرا (تأییدشده) — تسک‌های ≤ ۱۵ دقیقه

1. موجودیت‌ها + پیکربندی EF + Migration سه جدول (بدون `TenantId` و بدون `PezhvakMobile`)
2. پارس فایل → واسط + `upload` endpoint + خطاهای ردیف
3. مسیر بیرونی: ایجاد بارگذاری + `ready`
4. فهرست/جزئیات بارگذاری و ردیف‌ها (API + صفحه سوم `/import`)
5. `commit` (ثبت معتبرها + اعلام نامعتبرها) + `reject` + تست‌های واحد/یکپارچگی
6. پاک‌سازی ۱۰ روزه + مستندات و بستن فاز ۷