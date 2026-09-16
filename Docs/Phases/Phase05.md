# Phase 5 — Integration REST API (کوتاه: احراز سیستمی + راهنما)

**وضعیت:** Done
**درصد پیشرفت:** 100%
**وابستگی‌ها:** Phase 3 (قراردادها و Endpointها از قبل موجودند)

---

## ۱. اهداف فاز (محدود و مشخص — DEC-026)
APIهای مصرف‌کننده از Phase 3 آماده‌اند؛ این فاز فقط دو شکاف را می‌بندد:
1. احراز هویت سیستمی مصرف‌کننده‌ها (Bearer JWT معتبر IAM در کنار کوکی BFF — ADR-012).
2. سند راهنمای Integration برای BPMS/HR.

## ۲. Deliverables
- [ ] `IamBearerAuthenticationHandler` (introspection + ساخت Claims یکسان)
- [ ] `PolicyScheme` هوشمند (Bearer در صورت هدر، وگرنه کوکی BFF)
- [ ] تست‌های Bearer (معتبر/نامعتبر/بدون توکن)
- [ ] `Docs/Architecture/external-integration.md` (راهنمای مصرف‌کننده‌ها)

## ۳. Definition of Done
- [ ] هر دو الگو (کوکی/Bearer) با Policyهای یکسان کار می‌کنند
- [ ] تست‌های جدید سبز؛ همه ۱۰۱+ تست سبز
- [ ] XML فارسی؛ بدون TODO؛ مستندات به‌روز

## ۴. پیش‌نیازها
- Phase 3 + Phase 4 (Done)

## ۵. نکات خاص
- بدون API Key اختصاصی (حاکمیت IAM — ADR-012)؛ بدون تغییر Policyها.

---

## تسک‌های ≤ ۱۵ دقیقه
1. `Bearer handler + PolicyScheme + تست‌ها`
2. `سند external-integration.md`
3. `Progress/ChangeLog/SessionReport + بستن فاز ۵`