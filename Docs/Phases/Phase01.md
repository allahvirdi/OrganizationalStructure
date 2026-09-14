# Phase 1 — Architecture Foundation (Freeze)

**وضعیت:** Not Started
**درصد پیشرفت:** 0%
**وابستگی‌ها:** Phase 0

---

## ۱. اهداف فاز
- Freeze تصمیم‌های معماری و ساختار Solution.
- ایجاد ۴ پروژه لایه‌ای (Domain/Application/Infrastructure/API).
- راه‌اندازی Error Contract، Multi-tenancy، Audit/Soft Delete پایه.
- نهایی‌سازی پروتکل اتصال IAM (حل Q-002) و انجماد `Docs/Architecture/iam-integration.md`.

## ۲. Deliverables
- [ ] اسکلت `Backend/OrganizationalStructure.slnx` (۴ پروژه لایه‌ای)
- [ ] Common پایه (BaseEntity، Auditable، Result/ErrorContract، Clock)
- [ ] تنظیمات EF Core + SQL Server + Global Query Filter (TenantId)
- [ ] تنظیمات Serilog + OpenTelemetry + Health Checks
- [ ] نهایی‌سازی IAM Integration (BFF یا JWT Validate)
- [ ] Architectural Fitness Functions پایه

## ۳. Definition of Done
- [ ] Solution build می‌شود
- [ ] Unit/Integration Test پایه پاس شده
- [ ] XML Documentation فارسی کامل
- [ ] ADRها از Phase 0 الزامی است

## ۴. پیش‌نیازها
- Phase 0 تکمیل و کامیت شده.
- پاسخ Q-002 (پروتکل اتصال IAM).

## ۵. ریسک‌ها
| ریسک | احتمال | تأثیر | mitigation |
|------|--------|-------|------------|
| وابستگی پروتکل IAM به مالک IAM | بالا | بالا | حل Q-002 + fail-closed |

## ۶. لیست کلاس‌ها / پروژه‌ها
- `OrganizationalStructure.Domain`, `.Application`, `.Infrastructure`, `.API`

## ۷. APIها / DTOها / Commands / Queries / Validators
- (پایه در Phase 1؛ دامنه در Phase 2+)

## ۸. Eventها / Background Jobs (Hangfire)
- تنظیم پایه Hangfire

---

> پس از تأیید، جزئیات تسک‌های ≤ ۱۵ دقیقه‌ای پیش از شروع فاز ارائه می‌شود.