# 03 — Roadmap (نقشه راه)

> نقشه راه کلی پروژه بر اساس فازها.
> جزئیات هر فاز در `Docs/Phases/PhaseXX.md` آمده است.

**آخرین به‌روزرسانی:** `2026-09-23`
**ددلاین:** `2026-11-14` (دو ماه — DEC-026) — ✅ تحویل زودتر از ددلاین

---

## فازهای اصلی

| فاز | عنوان | وضعیت | درصد تقریبی | وابستگی |
|-----|-------|-------|-------------|---------|
| 0   | Project Baseline | Done | 100% | - |
| 1   | Architecture Foundation (Freeze) | Done | 100% | Phase 0 |
| 2   | Domain Modeling | Done | 100% | Phase 1 |
| 3   | Backend Core Vertical Slices | Done | 100% | Phase 2 |
| 4   | Access & Visibility | Done | 100% | Phase 3 |
| 5   | Integration REST API (احراز سیستمی + راهنما) | Done | 100% | Phase 3 |
| 6   | Frontend Development | Done | 100% | Phase 3 + Phase 4 |
| 7   | Import — MVP Secondary (CSV/Excel/Staging) | Done | 100% | Phase 3 + Q-003 (بسته‌شده — DEC-030) |
| 8   | Production Readiness | Done | 100% | همه فازها |

> ✅ **تمام ۸ فاز پروژه تکمیل شده‌اند.** جزئیات هر فاز در `Docs/Phases/PhaseXX.md` و خلاصه پیشرفت در `Docs/04_Progress.md`.

## ترتیب اجرای مصوب (DEC-026) — تکمیل‌شده ✅

1. ✅ **Phase 5 — کوتاه:** احراز هویت سیستمی (Bearer + PolicyScheme هوشمند، ADR-012) + سند راهنمای Integration.
2. ✅ **Phase 6 — اولویت اصلی:** اسکلت Next.js+MUI RTL → Auth (کوکی/Guard) → چارت سازمانی → صفحات Post/Employee/Responsibility/Authority.
3. ✅ **Phase 7 — پس از Q-003:** به ترتیب CSV → Excel → Staging (DEC-030).
4. ✅ **Phase 8 — پایانی:** رفع NU1903، Redis نشست، CI/CD + Fitness Functions، Rate Limiting + Security Headers، مستندات استقرار، گزارش نهایی.

## سؤالات باز (نیازمند تصمیم کارفرما)

| شناسه | موضوع | اولویت |
|-------|-------|--------|
| Q-007 | تفکیک دقیق مسئولیت داده پرسنلی بین IAM و OrgStructure | بالا |
| Q-008 | مصرف `GeographicUnit` از IAM یا فقط `Organization` تخت | متوسط |
| Q-010 | قرارداد خطای ۴۰۱ در برابر خطای وابستگی IAM | پایین |

## آیتم‌های Deferred (خارج از MVP)

- RabbitMQ · Outbox · Event-driven Integration
- LDAP/AD
- GeographicUnit موازی
- (اختیاری) پردازش پس‌زمینه Import با Hangfire

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