# Phase 3 — Backend Core Vertical Slices

**وضعیت:** Not Started
**درصد پیشرفت:** 0%
**وابستگی‌ها:** Phase 2

---

## ۱. اهداف فاز
- پیاده‌سازی Vertical Slices اصلی backend بر اساس مدل دامنه فاز ۲.
- پوشش: Post CRUD + Hierarchy، Employee CRUD + Assignment، Authority/Responsibility، Search، Pagination.

## ۲. Deliverables
- [ ] Post: Create/Update/Activate/Deactivate + Parent/Child/Subtree
- [ ] Employee: CRUD + PII Encryption + Active/Inactive
- [ ] EmployeePostAssignment (چند‌پستی)
- [ ] Responsibility + SigningAuthority (مدیریت + استعلام)
- [ ] Search + Pagination استاندارد
- [ ] API Contracts و DTO/VAlidators مربوطه
- [ ] Unit + Integration Tests

## ۳. Definition of Done
- [ ] Unit + Integration Test پاس شده
- [ ] XML Documentation فارسی کامل
- [ ] هیچ TODO / Incomplete / Sample
- [ ] API Contract به‌روز
- [ ] Fitness Functions پاس شده

## ۴. پیش‌نیازها
- Phase 2 (دومین مدل دامنه)

## ۵. ریسک‌ها
| ریسک | احتمال | تأثیر | mitigation |
|------|--------|-------|------------|
| پیچیدگی Subtree Query | متوسط | متوسط | پارامتر بازگشتی/CTE استاندارد |

---

> پس از تأیید، جزئیات تسک‌های ≤ ۱۵ دقیقه‌ای پیش از شروع فاز ارائه می‌شود.