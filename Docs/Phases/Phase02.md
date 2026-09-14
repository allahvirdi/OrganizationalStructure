# Phase 2 — Domain Modeling

**وضعیت:** Not Started
**درصد پیشرفت:** 0%
**وابستگی‌ها:** Phase 1

---

## ۱. اهداف فاز
- نهایی‌سازی و انجماد مدل دامنه بر اساس `Docs/domain/` و Big Picture.
- تعریف Enums، Aggregateها (Post، Employee)، روابط، اعتبارسنجی‌های دامنه.
- Event Storming و تعریف رویدادهای دامنه نهایی.

## ۲. Deliverables
- [ ] نهایی‌سازی `Post` + روابط Parent/Child + قواعد درخت
- [ ] نهایی‌سازی `Employee` + PII + State
- [ ] `EmployeePostAssignment` (چند‌پستی)
- [ ] `Responsibility` + `SigningAuthority` روی Post
- [ ] Enums و Value Objects نهایی
- [ ] رویدادهای دامنه نهایی
- [ ] طراحی شمای دیتابیس (ERD Conceptual + Logical)

## ۳. Definition of Done
- [ ] مدل دامنه با تخصص دامنه تأیید شده
- [ ] تست‌های دامنه (Unit)
- [ ] سند ERD به‌روز شده

## ۴. پیش‌نیازها
- Phase 1 (مبنای فنی)
- پاسخ Q-007 (مرز داده پرسنلی با IAM)

## ۵. ریسک‌ها
| ریسک | احتمال | تأثیر | mitigation |
|------|--------|-------|------------|
| پیچیدگی درخت/Visibility | متوسط | بالا | مدل‌سازی ساده + تست دامنه |

---

> پس از تأیید، جزئیات تسک‌های ≤ ۱۵ دقیقه‌ای پیش از شروع فاز ارائه می‌شود.