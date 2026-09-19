# Phase 7 — Import (MVP Secondary)

**وضعیت:** In Progress
**درصد پیشرفت:** 30%
**وابستگی‌ها:** Phase 3

---

## ۱. اهداف فاز
- پیاده‌سازی سه مسیر Import اطلاعات پرسنلی برای نقش‌های مجاز ستادی: CSV، Excel، Staging Table.

## ۲. Deliverables
- [x] Import ساختار پست‌ها از Excel (با قالب فارسی)
- [ ] Import از CSV (پرسنل)
- [ ] Import از Staging Table (Schema مصوب — Q-003)
- [ ] آماده‌سازی Batch/Validation/Error/Duplicate Handling
- [ ] Audit و پیگیری وضعیت Import
- [ ] (اختیاری) پردازش پس‌زمینه با Hangfire

## ۳. Definition of Done
- [ ] تست CSV و Excel
- [ ] تست Staging (پس از تأیید Schema)
- [ ] XML Documentation فارسی
- [ ] Security: فقط نقش‌های مجاز ستادی

## ۴. پیش‌نیازها
- پاسخ Q-003 (Schema نهایی جدول واسط)
- Phase 3

## ۵. ریسک‌ها
| ریسک | احتمال | تأثیر | mitigation |
|------|--------|-------|------------|
| Schema واسط تأیید نشده | بالا | بالا | طرح پیشنهادی + تأیید Business قبل از Implementation |

---

## تسک‌های ≤ ۱۵ دقیقه (پس از Q-003)
1. ✅ `Import ساختار پست‌ها از Excel + قالب فارسی + صفحه فرانت`
2. `طراحی پیشنهادی Schema واسط + تأیید Business`
3. `Import CSV پرسنل (فقط ستاد) + تست`
4. `Import جدول واسط + Batch/Error/Duplicate + تست`
5. `صفحه Import پرسنل در Frontend`
6. `Progress/ChangeLog/SessionReport + بستن فاز ۷`