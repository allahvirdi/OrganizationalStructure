# 03 — Roadmap (نقشه راه)

> نقشه راه کلی پروژه بر اساس فازها.
> جزئیات هر فاز در `Docs/Phases/PhaseXX.md` آمده است.

**آخرین به‌روزرسانی:** `2026-09-14`

---

## فازهای اصلی

| فاز | عنوان | وضعیت | درصد تقریبی | وابستگی |
|-----|-------|-------|-------------|---------|
| 0   | Project Baseline | در حال اجرا | ~۱۵٪ | - |
| 1   | Architecture Foundation (Freeze) | Not Started | 0% | Phase 0 |
| 2   | Domain Modeling | Not Started | 0% | Phase 1 |
| 3   | Backend Core Vertical Slices | Not Started | 0% | Phase 2 |
| 4   | Access & Visibility | Not Started | 0% | Phase 3 |
| 5   | Integration REST API | Not Started | 0% | Phase 3 |
| 6   | Frontend Development | Not Started | 0% | Phase 3 + Phase 4 |
| 7   | Import (MVP Secondary) | Not Started | 0% | Phase 3 |
| 8   | Production Readiness | Not Started | 0% | همه فازها |

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

## لینک به Progress و Session Reports
- `Docs/04_Progress.md`
- `Docs/SessionReports/`