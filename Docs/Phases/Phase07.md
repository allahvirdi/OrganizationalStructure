# Phase 7 — Import (MVP Secondary)

**وضعیت:** Not Started
**درصد پیشرفت:** 0%
**وابستگی‌ها:** Phase 3

---

## ۱. اهداف فاز
- پیاده‌سازی سه مسیر Import اطلاعات پرسنلی برای نقش‌های مجاز ستادی: CSV، Excel، Staging Table.

## ۲. Deliverables
- [ ] Import از CSV
- [ ] Import از Excel
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

> پس از تأیید، جزئیات تسک‌های ≤ ۱۵ دقیقه‌ای پیش از شروع فاز ارائه می‌شود.