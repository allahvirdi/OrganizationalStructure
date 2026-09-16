# 03 — Roadmap (نقشه راه)

> نقشه راه کلی پروژه بر اساس فازها.
> جزئیات هر فاز در `Docs/Phases/PhaseXX.md` آمده است.

**آخرین به‌روزرسانی:** `2026-09-15`
**ددلاین:** `2026-11-14` (دو ماه — DEC-026)

---

## فازهای اصلی

| فاز | عنوان | وضعیت | درصد تقریبی | وابستگی |
|-----|-------|-------|-------------|---------|
| 0   | Project Baseline | Done | 100% | - |
| 1   | Architecture Foundation (Freeze) | Done | 100% | Phase 0 |
| 2   | Domain Modeling | Done | 100% | Phase 1 |
| 3   | Backend Core Vertical Slices | Done | 100% | Phase 2 |
| 4   | Access & Visibility | Done | 100% | Phase 3 |
| 5   | Integration REST API (کوتاه: احراز سیستمی + راهنما) | Not Started | 0% | Phase 3 |
| 6   | Frontend Development (اولویت اصلی) | Not Started | 0% | Phase 3 + Phase 4 |
| 7   | Import — MVP Secondary (پس از Q-003) | Blocked (Q-003) | 0% | Phase 3 + Q-003 |
| 8   | Production Readiness | Not Started | 0% | همه فازها |

## ترتیب اجرای مصوب (DEC-026)

1. **Phase 5 — کوتاه:** فقط احراز هویت سیستمی (Bearer + PolicyScheme هوشمند، ADR-012) + سند راهنمای Integration؛ ۲–۳ تسک.
2. **Phase 6 — اولویت اصلی:** اسکلت Next.js+MUI RTL → Auth (کوکی/Guard) → چارت سازمانی → صفحات Post/Employee/Responsibility/Authority.
3. **Phase 7 — پس از Q-003:** به ترتیب CSV → Excel → Staging.
4. **Phase 8 — پایانی:** NU1903، Redis نشست، CI/CD + Fitness، مستندات استقرار، Push نهایی.

## تفکیک اولویت MVP

### MVP Core
Organization Reference · Post Management · Post Hierarchy · Employee Management · Employee↔Post Assignment · Signing Authority · Responsibility · User Access · Role · Permission · Organization Scope · Visibility Rules · REST API

### MVP Secondary
CSV Import · Excel Import · Staging Table Import

### Deferred
RabbitMQ · Outbox · Event-driven Integration · LDAP/AD · GeographicUnit موازی

## قوانین مرتبط
- فقط روی فاز اعلام‌شده توسط کاربر کار شود.
- هر تسک داخل فاز حداکثر ۱۵ دقیقه (قانون تایم‌باکس).
- کامیت فقط بعد از تأیید صریح انسان.
- بعد از پایان هر فاز منتظر دستور کاربر بمان.
- همه تست‌ها باید سبز باشند؛ هیچ تسکی جا نماند.

## لینک به Progress و Session Reports
- `Docs/04_Progress.md`
- `Docs/SessionReports/`