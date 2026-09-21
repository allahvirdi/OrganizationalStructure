# Session Report — Session-20260920-Phase7-EmployeeImport

**فاز:** Phase 7 — Import (MVP Secondary)  
**تاریخ:** `2026-09-20`  
**دستور کاربر:** `ادامه فاز ۷ — ورود دسته‌جمعی پرسنل از فایل اکسل و CSV (بدون انتظار Q-003)`

---

## ۱. خلاصه Session
مسیر فایل برای ثبت دسته‌جمعی پرسنل یک سازمان پیاده‌سازی شد: دو Endpoint جدید (بارگذاری + قالب)، پارسر اکسل و CSV، Command/Handler/Validator با عملیات اتمیک، و تب دوم در صفحه `/import`. این کار مستقل از Q-003 (Schema جدول واسط) است و مسیر Staging همچنان مسدود باقی می‌ماند.

## ۲. Deliverables تکمیل‌شده
- [x] `POST /api/v1/import/employees` با `multipart/form-data` (`.xlsx` یا `.csv`)
- [x] `GET /api/v1/import/employees/template?format=xlsx|csv`
- [x] پارسر اکسل (ClosedXML موجود) و پارسر CSV (RFC 4180، بدون پکیج تازه)
- [x] Batch/Validation/Error/Duplicate Handling به‌صورت اتمیک
- [x] نرمال‌سازی ارقام فارسی/عربی + تبدیل تاریخ شمسی
- [x] Frontend: تب «پرسنل» + هوک/لایه API + منو با `anyOfPermissions`
- [x] تست واحد (۳۵) و یکپارچگی (۶) + اسناد API/Phase/ChangeLog/Progress

## ۳. فایل‌های جدید
| فایل | توضیح |
|---|---|
| `Backend/src/OrganizationalStructure.Application/Common/PersianDigitNormalizer.cs` | نرمال‌سازی ارقام/جداکننده‌ها |
| `Backend/src/OrganizationalStructure.Application/Common/JalaliConverter.cs` | شمسی → `DateOnly` با `PersianCalendar` |
| `Backend/src/OrganizationalStructure.Application/Import/DTOs/ImportEmployeeRowDto.cs` | ردیف پارس‌شده + DTO نتیجه |
| `Backend/src/OrganizationalStructure.Application/Import/EmployeesImportHeaders.cs` | مرجع واحد سرستون‌ها |
| `Backend/src/OrganizationalStructure.Application/Import/EmployeesExcelParser.cs` (+`.Helpers`, `.Template`) | پارسر/قالب اکسل |
| `Backend/src/OrganizationalStructure.Application/Import/EmployeesCsvParser.cs` (+`.Reader`, `.Template`) | پارسر/قالب CSV |
| `Backend/src/OrganizationalStructure.Application/Import/ImportEmployeesCommand*.cs` | Command + Validator + Handler (partial) |
| `Backend/tests/OrganizationalStructure.Application.UnitTests/ImportEmployeesTests*.cs` | ۵ فایل تست واحد |
| `Backend/tests/OrganizationalStructure.Api.IntegrationTests/ImportEmployeesApiTests*.cs` | ۳ فایل تست یکپارچگی |
| `Frontend/src/features/import/useImportEmployees.ts` | هوک Mutation |
| `Frontend/src/features/import/EmployeesImportPanel.tsx` | پنل UI پرسنل |
| `Docs/SessionReports/Session-20260920-Phase7-EmployeeImport.md` | همین گزارش |

## ۴. APIها / Endpointها
- `POST /api/v1/import/employees` → `201 {organizationId, importedCount}`؛ خطاها: `Import.InvalidFile`، `Import.EmployeeRowInvalid`، `Import.DuplicatePersonnelCodeInFile`، `Import.DuplicateNationalCodeInFile`، `Import.RowLimitExceeded` (۴۰۰)، `Employee.DuplicatePersonnelCode` (۴۰۹)، `Access.Forbidden` (۴۰۳).
- `GET /api/v1/import/employees/template?format=xlsx|csv` → فایل قالب با دو ردیف نمونه.
- مجوز هر دو: `OrganizationStructure.Employee.Import` (Policy موجود در `AuthorizationPolicies`؛ بدون Policy جدید).

## ۵. Migrationها
- بدون Migration جدید (ستون‌های مورد نیاز از فازهای قبل موجود است).

## ۶. تست‌ها
- Domain ۳۷ · Application ۷۱ (از این Session ۳۵ تست جدید) · Infrastructure ۲۴ · Architecture ۴ · API Integration ۷۶ (۶ تست جدید) — مجموع ۲۱۲ سبز.
- Frontend: `tsc --noEmit` تمیز · `eslint . --max-warnings 0` تمیز · `next build` موفق (۱۷ روت).
- `git diff --check`: بدون خطای whitespace.

## ۷. قانون ۱۵ دقیقه
- واحدها: ابزارهای مشترک → DTO/Command/Validator → پارسر اکسل → پارسر CSV → هندلر/اعتبارسنجی → Controller → Frontend (api/hook/panel/tabs/menu) → تست‌ها → اسناد.
- همه ≤ ۱۵ دقیقه: بله.

## ۸. کامیت‌ها
- تعداد کامیت: ۰ (قانون ۲ — منتظر تأیید صریح انسان).
- درخت کاری شامل تغییرات معوق Sessionهای قبل است؛ هنگام کامیت باید تفکیک شود.

## ۹. XML Documentation
- بله؛ همه کلاس/متد/پراپرتی/فیلد جدید (شامل partial کلاس‌ها و تست‌ها) به فارسی مستند شده‌اند.

## ۱۰. مشکلات و Blockers
- Q-003 (Schema جدول واسط) همچنان Open → مسیر Staging و «بستن فاز ۷» انجام‌نشده ماند.
- Audit/تاریخچه وضعیت هر بارگذاری (Deliverable بعدی فاز) پیاده‌سازی نشد.
- ثبت ۲۷ Permission در IAM (اقدام مالک IAM) → تا آن زمان همه درخواست‌های این مسیر ۴۰۳ است (رفتار صحیح Deny by Default).
- تست HTTP زنده با IAM واقعی و آزمون تعاملی مرورگر انجام نشد.

## ۱۱. ریسک‌ها
- حجم ۵MB/۵۰۰ ردیف در حافظه پارس می‌شود؛ برای Staging/Background (Hangfire) باید Stream-based و صفحه‌بندی‌شده بازطراحی شود.
- `PersianCalendar` دات‌نت برای سال‌های بسیار دور از محدوده پشتیبانی خطا می‌دهد؛ هندلر آن را به «تاریخ نامعتبر» نگاشت می‌کند (fail-closed).
- تفکیک کامیت تغییرات معوق چند Session ریسک خطای انسانی در مرحله کامیت دارد.

## ۱۲. تصمیمات
- بدون ADR/DEC جدید؛ اتکا به DEC-025 (نام مجوز)، ADR-008 (مجوز در IAM)، ADR-006 (PII)، ADR-001 (استک منجمد → بدون پکیج تازه).
- تصمیم فنی: پارسر CSV دستی به‌جای افزودن وابستگی تازه (مطابق قانون «فقط کتابخانه‌های موجود»).

## ۱۳. قدم بعدی
۱) تأیید کارفرما برای کامیت تفکیک‌شده این Session.  
۲) طراحی پیشنهادی Schema واسط + تأیید Business (رفع Q-003).  
۳) Import جدول واسط + Batch/Error/Duplicate + تست.  
۴) Audit و پیگیری وضعیت Import (جدول/پروانه بارگذاری).  
۵) تست HTTP زنده با IAM واقعی و آزمون مرورگر.

## ۱۴. قابلیت ادامه
بله؛ `04_Progress.md`، `05_ChangeLog.md`، `Docs/api-contracts/import.md`، `Docs/Phases/Phase07.md` و همین گزارش برای ادامه بدون تاریخچه گفتگو کافی‌اند.