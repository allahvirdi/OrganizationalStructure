# PROJECT BASELINE v0.1
## قالب پایه پروژه (Enterprise)

> این سند نقطه شروع رسمی هر پروژه است.  
> باید در ابتدای کار تکمیل و در مسیر `Docs/PROJECT-BASELINE-v0.1.md` نگهداری شود.  
> بخش‌های مشخص‌شده با `[PROJECT-SPECIFIC]` الزاماً باید پر شوند.  
> پس از تکمیل، این سند مبنای تمام تصمیم‌ها، ADRها و توسعه قرار می‌گیرد.

**تاریخ ایجاد:** `[YYYY-MM-DD]`  
**نسخه:** `0.1`  
**وضعیت:** `Draft / In Review / Accepted`  
**نویسنده / مالک سند:** `[نام]`  
**آخرین به‌روزرسانی:** `[YYYY-MM-DD]`

---

## ۱. README پروژه (خلاصه اجرایی)

### ۱.۱ نام پروژه
`[PROJECT-SPECIFIC: نام رسمی پروژه]`

### ۱.۲ یک‌خطی (Elevator Pitch)
`[PROJECT-SPECIFIC: در یک جمله توضیح دهید سیستم چه کاری انجام می‌دهد و برای چه کسی]`

### ۱.۳ هدف اصلی
`[PROJECT-SPECIFIC: هدف کسب‌وکاری اصلی پروژه]`

### ۱.۴ وضعیت فعلی
- فاز فعلی: `Phase 0 — Project Baseline`
- درصد پیشرفت تقریبی: `0٪`
- تاریخ شروع برنامه‌ریزی‌شده: `[YYYY-MM-DD]`
- تاریخ هدف اولین نسخه قابل استفاده (MVP): `[YYYY-MM-DD]`

### ۱.۵ لینک‌های مهم
| مورد                    | مسیر / لینک                          |
|-------------------------|--------------------------------------|
| مخزن کد                 | `[URL]`                              |
| اسناد                   | `Docs/`                              |
| قراردادهای API          | `Docs/api-contracts/`                |
| ADRها                   | `Docs/adr/`                          |
| Domain Model            | `Docs/domain/`                       |
| Decision Log            | همین سند + `Docs/decision-log.md`    |
| Open Questions          | `Docs/open-questions.md`             |

---

## ۲. Project Charter

### ۲.۱ مسئله کسب‌وکار (Problem Statement)
`[PROJECT-SPECIFIC: مشکل اصلی که این سیستم حل می‌کند را به‌صورت واضح بنویسید. از زبان کسب‌وکار استفاده کنید، نه زبان فنی.]`

### ۲.۲ چشم‌انداز (Vision)
`[PROJECT-SPECIFIC: سیستم در افق ۱۲–۲۴ ماهه چه ارزشی ایجاد می‌کند؟]`

### ۲.۳ اهداف کلیدی (Objectives)
1. `[هدف ۱ — قابل اندازه‌گیری]`
2. `[هدف ۲]`
3. `[هدف ۳]`

### ۲.۴ محدوده (Scope)

**در محدوده (In Scope):**
- `[قابلیت / حوزه ۱]`
- `[قابلیت / حوزه ۲]`
- `[قابلیت / حوزه ۳]`

**خارج از محدوده (Out of Scope / Non-Goals):**
- `[مورد ۱]`
- `[مورد ۲]`
- `[مورد ۳]`

### ۲.۵ ذی‌نفعان اصلی (Stakeholders)

| نقش                    | نام / تیم              | مسئولیت اصلی                  | سطح درگیری |
|------------------------|------------------------|-------------------------------|------------|
| Product Owner          | `[نام]`                | اولویت‌بندی و پذیرش           | بالا       |
| Technical Lead         | `[نام]`                | معماری و کیفیت فنی            | بالا       |
| Domain Expert          | `[نام]`                | صحت مدل دامنه                 | بالا       |
| Security Officer       | `[نام]`                | الزامات امنیتی                | متوسط      |
| Operations             | `[نام]`                | استقرار و نگهداری             | متوسط      |

### ۲.۶ موفقیت چگونه اندازه‌گیری می‌شود؟ (Success Metrics)
| شاخص                              | هدف                          | نحوه اندازه‌گیری          |
|-----------------------------------|------------------------------|---------------------------|
| زمان پاسخ عملیات اصلی (p95)       | < 300ms                      | APM / OpenTelemetry       |
| پوشش تست لایه‌های Domain/Application | ≥ 80٪                      | Coverage Report           |
| تعداد Defectهای Critical در Production | ۰ در ماه اول               | Issue Tracker             |
| رضایت ذی‌نفعان کلیدی              | ≥ 4 از 5                     | نظرسنجی دوره‌ای           |

---

## ۳. Current State (وضعیت فعلی)

### ۳.۱ وضعیت کد و زیرساخت
| مورد                        | وضعیت فعلی                          |
|-----------------------------|-------------------------------------|
| ساختار ریشه پروژه           | `[وجود دارد / ایجاد نشده]`          |
| Backend                     | `[خالی / اسکلت اولیه / در حال توسعه]` |
| Frontend                    | `[خالی / اسکلت اولیه / در حال توسعه]` |
| پایگاه داده                 | `[وجود ندارد / اسکریپت اولیه]`      |
| CI/CD                       | `[وجود ندارد / Pipeline اولیه]`     |
| محیط‌های Deployment         | `[Local فقط / Dev / Staging]`       |

### ۳.۲ وضعیت اسناد
| سند                              | وضعیت          | مسیر                          |
|----------------------------------|----------------|-------------------------------|
| Architecture Baseline            | موجود (قالب)   | `Docs/`                       |
| AI Development Path 0–100        | موجود (قالب)   | `Docs/`                       |
| Domain Vision & Problem          | `[تکمیل نشده]` | `Docs/domain/`                |
| ADRها                            | `[۰ عدد]`      | `Docs/adr/`                   |
| API Contracts                    | `[۰ عدد]`      | `Docs/api-contracts/`         |
| Open Questions                   | `[در حال جمع‌آوری]` | `Docs/open-questions.md` |

### ۳.۳ فرضیات فعلی (Assumptions)
1. `[فرض ۱]`
2. `[فرض ۲]`
3. `[فرض ۳]`

### ۳.۴ محدودیت‌های شناخته‌شده (Constraints)
- تکنولوژی: استفاده از Stack تعریف‌شده در Architecture Baseline الزامی است.
- زمان: `[محدودیت زمانی]`
- منابع انسانی: `[تعداد نفرات / تخصص‌ها]`
- زیرساخت: `[محدودیت‌های زیرساختی]`
- امنیتی / Compliance: `[الزامات خاص]`

---

## ۴. Document Inventory (فهرست اسناد)

| شناسه     | نام سند                              | نسخه   | وضعیت     | مالک          | مسیر                              |
|-----------|--------------------------------------|--------|-----------|---------------|-----------------------------------|
| DOC-001   | Architecture Baseline                | 2.2    | Accepted  | Architecture  | `Docs/01-Architecture-Baseline-*.md` |
| DOC-002   | AI Development Path 0–100            | 2.2    | Accepted  | Architecture  | `Docs/02-AI-Development-Path-*.md` |
| DOC-003   | PROJECT-BASELINE                     | 0.1    | Draft     | `[نام]`       | `Docs/PROJECT-BASELINE-v0.1.md`   |
| DOC-004   | Domain Vision & Problem Statement    | —      | Planned   | Domain Expert | `Docs/domain/vision-and-problem.md` |
| DOC-005   | Decision Log                         | —      | Active    | Tech Lead     | همین سند + فایل جداگانه           |
| DOC-006   | Open Questions Log                   | —      | Active    | Tech Lead     | `Docs/open-questions.md`          |
| DOC-007   | Risk Register                        | —      | Active    | Tech Lead     | همین سند                          |
| DOC-008   | API Contracts                        | —      | Planned   | Backend Lead  | `Docs/api-contracts/`             |
| DOC-009   | ADR Series                           | —      | Planned   | Architecture  | `Docs/adr/`                       |

> این فهرست باید به‌صورت مداوم به‌روز شود.

---

## ۵. Architecture Gap Analysis

### ۵.۱ مقایسه وضعیت فعلی با Architecture Baseline

| حوزه                        | وضعیت مطلوب (Baseline)              | وضعیت فعلی          | شکاف (Gap)                  | اولویت |
|-----------------------------|-------------------------------------|---------------------|-----------------------------|--------|
| ساختار ریشه پروژه           | Docs / Backend / Frontend           | `[...]`             | `[توضیح]`                   | بالا   |
| Clean Architecture          | رعایت کامل لایه‌ها                  | `[...]`             | `[توضیح]`                   | بالا   |
| CQRS + MediatR              | پیاده‌سازی شده                      | `[...]`             | `[توضیح]`                   | بالا   |
| Outbox Pattern              | اجباری                              | `[...]`             | `[توضیح]`                   | متوسط  |
| Multi-tenancy               | TenantId روی جداول اصلی             | `[...]`             | `[توضیح]`                   | بالا   |
| Audit + Soft Delete         | ستون‌های استاندارد                  | `[...]`             | `[توضیح]`                   | بالا   |
| API First Contract          | قبل از Frontend                     | `[...]`             | `[توضیح]`                   | بالا   |
| Testing Strategy            | Unit + Integration + Architecture   | `[...]`             | `[توضیح]`                   | بالا   |
| Observability               | OpenTelemetry کامل                  | `[...]`             | `[توضیح]`                   | متوسط  |
| Security Baseline           | Deny by Default + OWASP             | `[...]`             | `[توضیح]`                   | بالا   |
| CI/CD + Fitness Functions   | وجود دارد                           | `[...]`             | `[توضیح]`                   | متوسط  |

### ۵.۲ اقدامات اولویت‌دار برای بستن شکاف‌ها
1. `[اقدام ۱ — مسئول — تاریخ هدف]`
2. `[اقدام ۲]`
3. `[اقدام ۳]`

---

## ۶. Decision Log (ثبت تصمیم‌ها)

| شناسه   | تاریخ       | تصمیم                                      | وضعیت     | ADR مرتبط     | تصمیم‌گیرنده     |
|---------|-------------|--------------------------------------------|-----------|---------------|------------------|
| DEC-001 | `[YYYY-MM-DD]` | پذیرش Architecture Baseline v2.2        | Accepted  | —             | `[نام]`          |
| DEC-002 | `[YYYY-MM-DD]` | پذیرش AI Development Path v2.2          | Accepted  | —             | `[نام]`          |
| DEC-003 | `[YYYY-MM-DD]` | انتخاب Stack تکنولوژی (Freeze)          | Accepted  | ADR-001       | `[نام]`          |
| DEC-004 |             | `[تصمیم بعدی]`                             | Proposed  |               |                  |

> هر تصمیم مهم معماری باید به ADR تبدیل شود.

---

## ۷. Risk Register (ثبت ریسک‌ها)

| شناسه  | ریسک                                      | احتمال | تأثیر | امتیاز | استراتژی کاهش                          | مسئول     | وضعیت   |
|--------|-------------------------------------------|--------|-------|--------|----------------------------------------|-----------|---------|
| R-001  | ابهام در Domain Model                     | بالا   | بالا  | بالا   | تکمیل Domain Vision + Event Storming   | Domain Expert | Open |
| R-002  | نقض لایه‌ها توسط AI یا توسعه‌دهنده       | متوسط  | بالا  | بالا   | Fitness Functions + Review اجباری      | Tech Lead | Open |
| R-003  | تأخیر در تعریف قرارداد API                | متوسط  | متوسط | متوسط  | اجبار API-First قبل از Frontend        | Backend Lead | Open |
| R-004  | پیچیدگی Workflow Engine                  | متوسط  | بالا  | بالا   | شروع زودهنگام مدل‌سازی فرآیند          | Tech Lead | Open |
| R-005  | کمبود پوشش تست                            | متوسط  | بالا  | بالا   | Definition of Done سخت‌گیرانه          | Tech Lead | Open |
| R-006  | وابستگی به سرویس‌های خارجی (LDAP, ...)   | پایین  | متوسط | پایین  | Abstraction + Fallback                 | Infrastructure | Open |

**راهنمای امتیاز:** احتمال × تأثیر (بالا/متوسط/پایین)

---

## ۸. Non-Functional Requirements (خلاصه)

| حوزه                | حداقل قابل قبول                          | منبع                          |
|---------------------|------------------------------------------|-------------------------------|
| Performance         | p95 < 300ms برای عملیات معمولی           | Architecture Baseline         |
| Availability        | 99.9٪                                    | Architecture Baseline         |
| Security            | OWASP Top 10 + Deny by Default           | Architecture Baseline         |
| Observability       | Logs + Metrics + Traces                  | Architecture Baseline         |
| Test Coverage       | Domain + Application ≥ 80٪               | Testing Strategy              |
| Multi-tenancy       | پشتیبانی کامل از TenantId                | Database Baseline             |
| Auditability        | تمام عملیات حساس لاگ شوند                | Security + Audit              |

> جزئیات کامل در سند Architecture Baseline آمده است. هرگونه تغییر باید از طریق ADR انجام شود.

---

## ۹. Open Questions (ابهامات باز)

| شناسه  | سؤال                                      | اولویت | مسئول پاسخ     | مهلت        | وضعیت   |
|--------|-------------------------------------------|--------|----------------|-------------|---------|
| Q-001  | `[سؤال دامنه یا فنی]`                     | بالا   | `[نام]`        | `[تاریخ]`   | Open    |
| Q-002  |                                           |        |                |             |         |

> فایل کامل و زنده‌ی ابهامات در `Docs/open-questions.md` نگهداری می‌شود. این بخش فقط خلاصه مهم‌ترین‌هاست.

---

## ۱۰. Next Immediate Actions (اقدامات فوری بعدی)

1. [ ] تکمیل بخش Domain Vision و Problem Statement
2. [ ] ایجاد ساختار ریشه پروژه (`Docs` / `Backend` / `Frontend`)
3. [ ] ثبت ADR-001 برای تأیید Stack و ساختار لایه‌ها
4. [ ] ایجاد فایل `Docs/open-questions.md` و انتقال ابهامات
5. [ ] برگزاری جلسه کوتاه Domain Modeling kickoff
6. [ ] تعریف اولین Vertical Slice (پیشنهاد: ساده‌ترین Aggregate اصلی)

---

## ۱۱. تأیید و پذیرش

| نقش                  | نام               | تاریخ       | امضا / تأیید |
|----------------------|-------------------|-------------|--------------|
| Product Owner        |                   |             |              |
| Technical Lead       |                   |             |              |
| Architecture Owner   |                   |             |              |

**پس از پذیرش این سند، هرگونه انحراف از Architecture Baseline یا AI Development Path فقط از طریق ADR مجاز است.**

---

**پایان PROJECT BASELINE v0.1**
```