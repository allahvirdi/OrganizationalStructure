# Session Report — Session-20260920-Phase6-PezhvakStatus

**فاز:** Phase 6 — Frontend Development (ادامه: داده پژواک)  
**تاریخ:** `2026-09-20`  
**دستور کاربر:** `ادامه فاز 6 — اجباری‌کردن شماره پژواک در ویرایش تکمیلی + ثبت وضعیت فعال بودن شبکه پژواک + اصلاح ورودی تاریخ تولد`

---

## ۱. خلاصه Session
فیلد `PezhvakIsActive` (سه‌حالتی و غیر PII) به مدل پرسنل، DTO، قرارداد API و صفحهٔ جزئیات اضافه شد و شماره پژواک در مسیر ویرایش تکمیلی اجباری گردید. ورودی متنی آزاد تاریخ تولد با `JalaliDatePicker` جایگزین شد تا خروجی همیشه ISO میلادی باشد. ADR-013 و DEC-029 به‌صورت پیشنهادی/در انتظار تأیید ثبت شدند.

## ۲. Deliverables تکمیل‌شده
- [x] `PezhvakIsActive` در دامنه، EF Configuration و Migration
- [x] عبور از Command → Handler → Validator → Request → DTO → Response
- [x] الزام `pezhvakMobile` و `pezhvakIsActive` در ویرایش تکمیلی (FluentValidation + Zod)
- [x] اتصال `JalaliDatePicker` و `Select` به RHF با `Controller`
- [x] نمایش وضعیت پژواک و تاریخ جلالی با ماسک PII
- [x] تست دامنه/اصلاح تست Application + اسناد ADR/Decision/ERD/API/ChangeLog/Progress

## ۳. فایل‌های جدید
| فایل | توضیح |
|---|---|
| `Backend/.../20260920045347_AddEmployeePezhvakActiveFlag.cs` (+ Designer) | ستون `bit NULL` و ایندکس سازمان |
| `Frontend/src/lib/date/jalali.ts` | تبدیل جلالی↔ISO بدون کتابخانه تازه |
| `Frontend/src/components/JalaliDatePicker.tsx` | انتخابگر تاریخ شمسی |
| `Docs/adr/ADR-013-pezhvak-network-status.md` | ADR پیشنهادی |
| همین گزارش | مستند Session |

## ۴. APIها / Endpointها
- `PATCH /api/v1/employees/{id}/supplementary`: بدنه حالا `pezhvakMobile` و `pezhvakIsActive` اجباری دارد.
- `GET /api/v1/employees/{id}` و جستجو: `EmployeeDto` شامل `pezhvakIsActive`.
- `PUT /api/v1/employees/{id}`: `mobile` در رکورد درخواست غیراختیاری شد (رفتار اعتبارسنجی از قبل اجباری بود).
- Endpoint/Policy جدید اضافه نشد.

## ۵. Migrationها
- `20260920045347_AddEmployeePezhvakActiveFlag` ساخته شد؛ اجرای آن روی SQL Server در این Session انجام نشد.

## ۶. تست‌ها
- Domain: ۳۷ سبز؛ شامل تغییر فقط پرچم پژواک و خالی بودن مقدار پیش‌فرض.
- Application: سبز؛ تست MissingEmployee با موبایل معتبر اصلاح شد.
- Infrastructure: ۲۴ سبز؛ Architecture: ۴ سبز؛ API Integration: ۷۰ سبز.
- Frontend: `tsc --noEmit`، `eslint . --max-warnings 0` و `next build` موفق (۱۶ روت).
- `git diff --check` تمیز.

## ۷. قانون ۱۵ دقیقه
- تسک‌ها: دامنه+EF، Application/API، فرانت schema/api، UI+دیت‌پیکر، اسناد+اعتبارسنجی.
- همه ≤ ۱۵ دقیقه: بله.

## ۸. کامیت‌ها
- تعداد کامیت: ۰.
- درخت کاری شامل تغییرات معوق Sessionهای قبل است؛ هنگام کامیت باید تفکیک شود.

## ۹. XML Documentation
- بله؛ فیلد جدید، پارامترها، Command/Request، EF Configuration، JSDoc کامپوننت و schema مستند شده‌اند.

## ۱۰. مشکلات و Blockers
- ADR-013/DEC-029 Pending Approval.
- Migration و تست HTTP زنده با IAM واقعی انجام نشد.
- رکوردهای موجود `null` می‌مانند تا ویرایش بعدی؛ Backfill در صورت نیاز تسک جداست.
- تغییرات معوق چند Session ریسک تفکیک کامیت دارد.

## ۱۱. ریسک‌ها
- تنگ‌ترشدن بدنه `PATCH` ممکن است Consumer خارجی را `400` دهد؛ راهنمای Integration باید به‌روز شود.
- تمایز `null` و `false` باید در گزارش/UI حفظ شود.

## ۱۲. تصمیمات
- ADR-013 (Proposed) و DEC-029 (Pending Approval): پژواک موبایل در تکمیلی اجباری، وضعیت سه‌حالتی غیر PII، ثبت اولیه بدون تغییر الزام.
- اتکا به ADR-001/006/010.

## ۱۳. قدم بعدی
۱) تأیید کارفرما برای ADR-013/DEC-029.  
۲) اجرای Migration در محیط توسعه.  
۳) تست HTTP زنده `PATCH` ناقص/صحیح.  
۴) به‌روزرسانی `external-integration.md` و `api-contracts/README.md`.  
۵) کامیت تفکیک‌شده.

## ۱۴. قابلیت ادامه
بله؛ `04_Progress.md`، `05_ChangeLog.md`، ADR-013 و همین گزارش کافی‌اند؛ تنها نکته تفکیک کامیت تغییرات معوق است.