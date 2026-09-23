# Phase 8 — Production Readiness

**وضعیت:** In Progress
**درصد پیشرفت:** 80%
**وابستگی‌ها:** همه فازها

---

## ۱. اهداف فاز
- آماده‌سازی پروژه برای Production: تست، امنیت، Observability، مستندسازی و استقرار.

## ۲. Deliverables
- [ ] تست کامل (Unit/Integration/Architecture/Contract + استراتژی Coverage)
- [ ] Security Hardening (OWASP/SAST/Secrets/Rate Limit)
- [ ] Observability (Serilog/OTel/Health Checks/Dashboard)
- [ ] Backup/DR و Policy‌های Retention
- [ ] CI/CD و Architectural Fitness Functions
- [ ] مستندات استقرار و Runbook
- [ ] بررسی قابلیت ادامه بدون تاریخچه (گزارش نهایی)

## ۳. Definition of Done
- [ ] تمام Acceptance Criteria Vertical Slice
- [ ] ریسک‌های باز ارزیابی و بسته شده
- [ ] SLO/Alerting تعریف شده

## ۴. پیش‌نیازها
- Phase 0..7

---

## تسک‌های ≤ ۱۵ دقیقه
1. ✅ `رفع NU1903 (ارتقای Microsoft.OpenApi/Swashbuckle)` — Microsoft.OpenApi 2.12.0؛ `dotnet list --vulnerable` سبز در هر ۴ پروژه
2. ✅ `نشست Redis (جایگزینی InMemory برای چندنمونه‌ای)` — `RedisBffSessionStore` با `StackExchange.Redis 3.3.0`؛ انتخاب خودکار بر اساس `ConnectionStrings:Redis` + تست DI
3. ✅ `CI/CD + Fitness Functions در CI` — GitHub Actions: backend (windows) = build + 5 test project + vulnerability scan؛ frontend (ubuntu) = npm ci + lint + build
4. ✅ `Rate Limiting + Hardening نهایی + SAST/Dependency Scan` — Fixed Window Rate Limiter (100 req/min/IP)؛ SecurityHeadersMiddleware (OWASP Secure Headers)؛ حذف هدر Kestrel؛ تبدیل اسکن آسیب‌پذیری به فیلتر قطعی در CI + افزودن `npm audit`
5. ✅ `مستندات استقرار + Runbook + Push نهایی` — `Docs/Deployment/deployment-runbook.md`: معماری استقرار، پیکربندی، پشتیبان‌گیری/بازیابی، عیب‌یابی، چک‌لیست و پیشنهادها
6. `گزارش نهایی قابلیت ادامه + بستن پروژه`