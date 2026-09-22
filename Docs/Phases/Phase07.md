# Phase 7 — Import (MVP Secondary)

**وضعیت:** Done
**درصد پیشرفت:** 100%
**وابستگی‌ها:** Phase 3

---

## ۱. اهداف فاز
- پیاده‌سازی سه مسیر Import اطلاعات پرسنلی برای نقش‌های مجاز ستادی: CSV، Excel، Staging Table.

## ۲. Deliverables
- [x] Import ساختار پست‌ها از Excel (با قالب فارسی)
- [x] Import از CSV و Excel (پرسنل) — `POST /api/v1/import/employees` + قالب دوفرمتی
- [x] Import از Staging Table (Schema تأییدشده — DEC-030) — موجودیت‌ها، بارگذاری فایل/بیرونی، فهرست/ردیف‌ها، `commit`/`reject`، Cleanup Job ۱۰ روزه + تست‌ها
- [x] آماده‌سازی Batch/Validation/Error/Duplicate Handling (اتمیک + خطای ردیف‌محور؛ مسیر واسط: ثبت معتبرها + اعلام نامعتبرها)
- [x] Audit و پیگیری وضعیت Import — فهرست بارگذاری‌ها و ردیف‌ها + `commit`/`reject` + پاک‌سازی خودکار
- [ ] (اختیاری) پردازش پس‌زمینه با Hangfire

## ۳. Definition of Done
- [x] تست CSV و Excel (واحد + یکپارچگی)
- [x] تست Staging (واحد + یکپارچگی — `ImportStagingTests` و `ImportStagingApiTests`)
- [x] XML Documentation فارسی
- [x] Security: فقط نقش‌های مجاز ستادی (`OrganizationStructure.Employee.Import`)

## ۴. پیش‌نیازها
- ✅ پاسخ Q-003 (Schema نهایی جدول واسط — تأییدشده با تغییرات، DEC-030)
- Phase 3

## ۵. ریسک‌ها
| ریسک | احتمال | تأثیر | mitigation |
|------|--------|-------|------------|
| Schema واسط تأیید نشده | بالا | بالا | طرح پیشنهادی + تأیید Business قبل از Implementation |

---

## تسک‌های ≤ ۱۵ دقیقه (پس از Q-003)
1. ✅ `Import ساختار پست‌ها از Excel + قالب فارسی + صفحه فرانت`
2. ✅ `طراحی پیشنهادی Schema واسط` — ارائه‌شده در `Docs/Architecture/staging-import-proposal.md` (DEC-030؛ در انتظار تأیید کارفرما)
3. ✅ `Import CSV پرسنل (فقط ستاد) + تست`
4. ✅ `Import جدول واسط + Batch/Error/Duplicate + تست` (کلیه تسک‌های ۱-۶ تکمیل شد)
5. ✅ `صفحه Import پرسنل در Frontend` (تب دوم صفحه `/import`)
6. ✅ `Progress/ChangeLog/SessionReport` (این Session)
7. ✅ `Audit و پیگیری وضعیت Import` + `بستن فاز ۷`