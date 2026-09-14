# PROJECT BASELINE v0.1
## سامانه ساختار سازمانی (Organizational Structure)

> این سند نقطه شروع رسمی پروژه است.
> پس از تکمیل، مبنای تمام تصمیم‌ها، ADRها و توسعه قرار می‌گیرد.

**تاریخ ایجاد:** `2026-09-14`
**نسخه:** `0.1`
**وضعیت:** `Accepted`
**نویسنده / مالک سند:** `Technical Lead / Architecture`
**آخرین به‌روزرسانی:** `2026-09-14`

---

## ۱. README پروژه (خلاصه اجرایی)

### ۱.۱ نام پروژه
سامانه ساختار سازمانی (نام فنی: `OrganizationalStructure`)

### ۱.۲ یکخطی (Elevator Pitch)
هسته مستقل و مرجع معتبر مدیریت ساختار درختی سازمان‌ها، پست‌های سازمانی، انتساب کارکنان، مسئولیت‌ها، صاحبان امضا و کنترل دسترسی به ساختار که خدمات خود را در اختیار سایر سامانه‌ها قرار می‌دهد.

### ۱.۳ هدف اصلی
ایجاد مرجع واحد و معتبر ساختار سازمانی به‌گونه‌ای که سامانه‌های مصرف‌کننده (HR / BPMS / سایر) دیگر منطق ساختار را پیاده‌سازی نکنند.

### ۱.۴ وضعیت فعلی
- فاز فعلی: `Phase 0 — Project Baseline`
- درصد پیشرفت تقریبی: `15٪`
- تاریخ شروع برنامه‌ریزی‌شده: `2026-09-14`
- تاریخ هدف اولین نسخه قابل استفاده (MVP): `2026-10-14` (Deadline یک ماه — نیازمند تأیید نهایی)

### ۱.۵ لینکهای مهم
| مورد                    | مسیر / لینک                          |
|-------------------------|--------------------------------------|
| مخزن کد                 | `https://github.com/allahvirdi/OrganizationalStructure.git` |
| اسناد                   | `Docs/`                              |
| قراردادهای API          | `Docs/api-contracts/`                |
| ADRها                   | `Docs/adr/`                          |
| Domain Model            | `Docs/domain/`                       |
| Decision Log            | `Docs/decision-log.md`               |
| Open Questions          | `Docs/open-questions.md`             |

---

## ۲. Project Charter

### ۲.۱ مسئله کسب‌وکار (Problem Statement)
اطلاعات ساختار سازمانی و جایگاه پست‌ها در سامانه‌های مختلف پراکنده و ناهمگون نگهداری می‌شود و هر سامانه مجدداً منطق ساختار را پیاده‌سازی میکند. مرجع واحد و معتبری برای روابط Parent/Child پست‌ها، انتساب کارکنان به پست‌ها، مسئولیت‌ها، صاحبان امضا و کنترل دسترسی مبتنی بر محدوده سازمانی وجود ندارد.

### ۲.۲ چشم‌انداز (Vision)
تبدیل شدن به منبع رسمی و واحد ساختار سازمانی در افق ۱۲–۲۴ ماهه؛ مرجع معتبر برای Organization، Post Hierarchy، Employee، Assignment، Responsibility، Signing Authority و Access.

### ۲.۳ اهداف کلیدی (Objectives)
1. ارائه مرجع معتبر ساختار سازمانی (Post + Hierarchy) برای تمام سامانه‌های مصرف‌کننده.
2. مدیریت یکپارچه اطلاعات پرسنلی و انتساب چند‌پستی کارمند به پست‌ها.
3. کنترل دسترسی دقیق مبتنی بر Permission + Organization Scope (رابطه سازمانی ≠ مجوز).

### ۲.۴ محدوده (Scope)

**در محدوده (In Scope) — MVP Core:**
- Organization Reference (مصرف از IAM)
- Post Management + Post Hierarchy
- Employee Management + Employee↔Post Assignment
- Signing Authority + Responsibility
- User Access + Role + Permission + Organization Scope
- Visibility Rules
- REST API برای مصرف سایر سامانه‌ها

**در محدوده (MVP Secondary):**
- Import از CSV
- Import از Excel
- Import از جدول واسط (Staging) برای نقش‌های ستادی

**خارج از محدوده (Out of Scope / Non-Goals):**
- ساخت Master موازی برای Organization یا Identity/User (متعلق به IAM)
- LDAP / Active Directory
- Event-driven Integration، RabbitMQ، Outbox (Deferred)
- Payroll، Recruitment، Workflow/BPMS
- GeographicUnit موازی

### ۲.۵ ذینفعان اصلی (Stakeholders)

| نقش                    | نام / تیم              | مسئولیت اصلی                  | سطح درگیری |
|------------------------|------------------------|-------------------------------|------------|
| Product Owner          | کارفرما                | اولویت‌بندی و پذیرش           | بالا       |
| Technical Lead         | تیم فنی                | معماری و کیفیت فنی            | بالا       |
| Backend Lead           | تیم فنی                | توسعه و نظارت Backend         | بالا       |
| Frontend Lead          | تیم فنی                | توسعه و نظارت Frontend        | بالا       |
| Database Lead          | تیم فنی                | مدل داده و Migration          | بالا       |
| Security Lead          | تیم فنی                | الزامات امنیتی و PII          | بالا       |
| IAM Owner              | مالک Enterprise-IAM-V2 | تأمین Identity/Organization   | متوسط      |

### ۲.۶ موفقیت چگونه اندازه‌گیری می‌شود؟ (Success Metrics)
| شاخص                              | هدف                          | نحوه اندازه‌گیری          |
|-----------------------------------|------------------------------|---------------------------|
| زمان پاسخ عملیات اصلی (p95)       | < 300ms                      | APM / OpenTelemetry       |
| پوشش تست لایه‌های Domain/Application | ≥ 80٪                      | Coverage Report           |
| صحت روابط Parent/Child/Subtree    | 100٪ مطابق قواعد کسب‌وکار    | Integration Test          |
| Defect بحرانی در Production      | ۰ در ماه اول               | Issue Tracker             |

---

## ۳. Current State (وضعیت فعلی)

### ۳.۱ وضعیت کد و زیرساخت
| مورد                        | وضعیت فعلی                          |
|-----------------------------|-------------------------------------|
| ساختار ریشه پروژه           | ایجاد شد (ساختار خالی)              |
| Backend                     | خالی (اسکلت ایجاد نشده)             |
| Frontend                    | خالی (اسکلت ایجاد نشده)             |
| پایگاه داده                 | وجود ندارد                          |
| CI/CD                       | وجود ندارد                          |
| محیط‌های Deployment         | Local فقط                           |

### ۳.۲ وضعیت اسناد
| سند                              | وضعیت          | مسیر                          |
|----------------------------------|----------------|-------------------------------|
| Architecture Baseline            | موجود (فریز)   | `Docs/01-Architecture-Baseline-FA.md` |
| AI Development Path 0–100        | موجود (فریز)   | `Docs/02-AI-Development-Path-0-to-100-FA.md` |
| Big Picture Scenario             | موجود          | `Docs/Organizational-Structure-Big-Picture-Scenario.md` |
| Operational Docs 00–06           | تکمیل شد       | `Docs/`                       |
| Decision Log                     | تکمیل شد       | `Docs/decision-log.md`        |
| Open Questions                   | تکمیل شد       | `Docs/open-questions.md`      |
| ADRها                            | در حال ایجاد   | `Docs/adr/`                   |
| API Contracts                    | Planned        | `Docs/api-contracts/`         |

### ۳.۳ فرضیات فعلی (Assumptions)
1. IAM توکن JWT صادر می‌کند و این سامانه فقط مصرف‌کننده/اعتبارسنج است.
2. `OrganizationId` و `UserId` از Claimهای IAM تأمین میشوند.
3. PII پرسنلی با Always Encrypted حفاظت می‌شود.
4. ساختار Post درون Organization ریشه می‌گیرد و Organization از IAM Reference است.

### ۳.۴ محدودیت‌های شناخته‌شده (Constraints)
- تکنولوژی: Stack منجمد Baseline؛ انحراف فقط با ADR.
- زمان: Deadline یک ماه (MVP Core اول، Import دوم).
- منابع انسانی: ۴ نقش تخصصی (Backend/Frontend/Database/Security) با نظارت و توسعه به کمک AI.
- زیرساخت: SQL Server، Redis، MinIO، Elasticsearch، Hangfire.
- امنیتی: PII / OWASP / Deny by Default / Tenant Isolation.

---

## ۴. Document Inventory (فهرست اسناد)

| شناسه     | نام سند                              | نسخه   | وضعیت     | مالک          | مسیر                              |
|-----------|--------------------------------------|--------|-----------|---------------|-----------------------------------|
| DOC-001   | Architecture Baseline                | 2.4    | Accepted  | Architecture  | `Docs/01-Architecture-Baseline-FA.md` |
| DOC-002   | AI Development Path 0–100            | 2.4    | Accepted  | Architecture  | `Docs/02-AI-Development-Path-0-to-100-FA.md` |
| DOC-003   | PROJECT-BASELINE                     | 0.1    | Accepted  | Technical Lead| `Docs/PROJECT-BASELINE-v0.1.md`   |
| DOC-004   | Big Picture Scenario                 | —      | Accepted  | Domain Expert | `Docs/Organizational-Structure-Big-Picture-Scenario.md` |
| DOC-005   | Decision Log                         | —      | Active    | Tech Lead     | `Docs/decision-log.md`            |
| DOC-006   | Open Questions Log                   | —      | Active    | Tech Lead     | `Docs/open-questions.md`          |
| DOC-007   | Risk Register                        | —      | Active    | Tech Lead     | همین سند                          |
| DOC-008   | API Contracts                        | —      | Planned   | Backend Lead  | `Docs/api-contracts/`             |
| DOC-009   | ADR Series                           | —      | Active    | Architecture  | `Docs/adr/`                       |

---

## ۵. Architecture Gap Analysis

| حوزه                        | وضعیت مطلوب (Baseline)              | وضعیت فعلی          | شکاف (Gap)                  | اولویت |
|-----------------------------|-------------------------------------|---------------------|-----------------------------|--------|
| ساختار ریشه پروژه           | Docs / Backend / Frontend           | ایجاد شد (خالی)     | نیاز به اسکلت Solution      | بالا   |
| Clean Architecture          | رعایت کامل لایهها                  | ندارد               | ایجاد ۴ پروژه لایه‌ای        | بالا   |
| CQRS + MediatR              | پیاده‌سازی شده                      | ندارد               | Phase 1                      | بالا   |
| Outbox Pattern              | اجباری                              | Deferred در MVP     | ADR-007 + پیاده‌سازی آینده   | متوسط  |
| Multi-tenancy               | TenantId روی جداول اصلی             | تصمیم گرفته شد      | Phase 2                      | بالا   |
| Audit + Soft Delete         | ستون‌های استاندارد                  | ندارد               | Phase 1/2                    | بالا   |
| API First Contract          | قبل از Frontend                     | Planned             | Phase 5 قبل از Phase 6       | بالا   |
| Testing Strategy            | Unit + Integration + Architecture   | ندارد               | از Phase 1                   | بالا   |
| Observability               | OpenTelemetry کامل                  | ندارد               | Phase 1/8                    | متوسط  |
| Security Baseline           | Deny by Default + OWASP + PII       | تصمیم PII گرفته شد  | Phase 1/4                    | بالا   |
| CI/CD + Fitness Functions   | وجود دارد                           | ندارد               | Phase 8                      | متوسط  |

### ۵.۲ اقدامات اولویت‌دار برای بستن شکاف‌ها
1. تکمیل Phase 0 (اسناد، ADR، domain) — Tech Lead — `2026-09-14`
2. Phase 1: اسکلت Solution + Foundation — Backend Lead — هفته اول
3. Phase 2: Domain Modeling — Domain Expert / Tech Lead — هفته اول/دوم

---

## ۶. Decision Log (ثبت تصمیمها)

| شناسه   | تاریخ       | تصمیم                                      | وضعیت     | ADR مرتبط     | تصمیم‌گیرنده     |
|---------|-------------|--------------------------------------------|-----------|---------------|------------------|
| DEC-001 | 2026-09-14 | پذیرش Architecture Baseline v2.4        | Accepted  | —             | کارفرما          |
| DEC-002 | 2026-09-14 | پذیرش AI Development Path v2.4          | Accepted  | —             | کارفرما          |
| DEC-003 | 2026-09-14 | Frontend: Next.js + MUI طبق Baseline    | Accepted  | ADR-001       | کارفرما          |
| DEC-004 | 2026-09-14 | Backend: ۴ پروژه لایه‌ای طبق Baseline   | Accepted  | ADR-001       | کارفرما          |
| DEC-005 | 2026-09-14 | Multi-tenancy لازم است                  | Accepted  | ADR-005       | کارفرما          |
| DEC-006 | 2026-09-14 | افزودن Region=3 به IAM (با هماهنگی)     | Accepted  | ADR (pending) | کارفرما          |
| DEC-007 | 2026-09-14 | Post Tree مستقل به‌ازای هر Organization | Accepted  | ADR-004       | کارفرما          |
| DEC-008 | 2026-09-14 | Permission دامنه‌ای در OrgStructure، ثبت/تخصیص در IAM | Accepted | ADR-008 | کارفرما |
| DEC-009 | 2026-09-14 | PII: Always Encrypted (ADR-004 خواهر)   | Accepted  | ADR-006       | کارفرما          |
| DEC-010 | 2026-09-14 | REST-only در MVP؛ Event/RabbitMQ/Outbox و LDAP/AD Deferred | Accepted | ADR-007 | کارفرما |

> جزئیات کامل: `Docs/decision-log.md`

---

## ۷. Risk Register (ثبت ریسک‌ها)

| شناسه  | ریسک                                      | احتمال | تأثیر | امتیاز | استراتژی کاهش                          | مسئول     | وضعیت   |
|--------|-------------------------------------------|--------|-------|--------|----------------------------------------|-----------|---------|
| R-001  | نگاشت ۴ سطح سازمانی در نبود Region=3     | بالا   | بالا  | بالا   | مجوز افزودن Region=3 صادر شد (DEC-019)؛ پیاده‌سازی در مخزن IAM | Tech Lead | Accepted — پیاده‌سازی در انتظار |
| R-002  | همپوشانی داده پرسنلی با IAM              | متوسط  | بالا  | بالا   | ADR مرز Employee + عدم نوشتن در IAM    | Tech Lead | Open    |
| R-003  | نقض لایه‌ها توسط AI/توسعه‌دهنده          | متوسط  | بالا  | بالا   | Fitness Functions + Review اجباری      | Tech Lead | Open    |
| R-004  | Deadline یک ماه برای MVP گسترده           | بالا   | بالا  | بالا   | تفکیک MVP Core/Secondary و Deferred    | PO/Tech   | Open    |
| R-005  | پیچیدگی Visibility سلسله‌مراتبی          | متوسط  | بالا  | بالا   | مدل‌سازی زودهنگام + تست‌های دامنه       | Tech Lead | Open    |
| R-006  | Schema جدول واسط نهایی نشده              | متوسط  | متوسط | متوسط  | طراحی پیشنهادی Phase 0 + تأیید Business| Backend   | Open    |

**راهنمای امتیاز:** احتمال × تأثیر (بالا/متوسط/پایین)

---

## ۸. Non-Functional Requirements (خلاصه)

| حوزه                | حداقل قابل قبول                          | منبع                          |
|---------------------|------------------------------------------|-------------------------------|
| Performance         | p95 < 300ms برای عملیات معمولی           | Architecture Baseline         |
| Availability        | 99.9٪                                    | Architecture Baseline         |
| Security            | OWASP Top 10 + Deny by Default + PII     | Architecture Baseline         |
| Observability       | Logs + Metrics + Traces                  | Architecture Baseline         |
| Test Coverage       | Domain + Application ≥ 80               | Testing Strategy              |
| Multi-tenancy       | پشتیبانی کامل از TenantId                | ADR-005                       |
| Auditability        | تمام عملیات حساس لاگ شوند                | Security + Audit              |

---

## ۹. Open Questions (ابهامات باز)

| شناسه  | سؤال                                      | اولویت | مسئول پاسخ     | مهلت        | وضعیت   |
|--------|-------------------------------------------|--------|----------------|-------------|---------|
| Q-001  | افزودن Region=3 به IAM (مجوز صادر شد — پیاده‌سازی در مخزن IAM) | بالا | کارفرما | فوری | Closed — DEC-019 |
| Q-002  | پروتکل اتصال IAM: الگوی BFF | بالا | کارفرما | فاز 1 | Closed — DEC-018 |
| Q-003  | Schema نهایی جدول واسط Import | متوسط | Business | Phase 7 | Open |
| Q-004  | IAM مرجع Role/Policy/Permission شد؛ بدون Authorization موازی | بالا | کارفرما | فاز 1 | Closed — DEC-020 |
| Q-006  | نام‌گذاری نهایی Permissionهای اختصاصی (مالکیت در IAM حل شد) | بالا | کارفرما + مالک IAM | فاز 4 | Open (فقط نام‌گذاری) |

> فایل کامل و زنده: `Docs/open-questions.md`

---

## ۱۰. Next Immediate Actions (اقدامات فوری بعدی)

1. [ ] ثبت ADR-001 تا ADR-008 (Phase 0)
2. [ ] ایجاد اسناد `Docs/domain/` (Agent قراردادها)
3. [ ] ایجاد `Docs/Phases/Phase00.md` و اسکلت فازهای بعدی
4. [ ] هماهنگی با مالک IAM برای Region=3 و Permission (Q-001, Q-004)
5. [ ] Phase 1: ایجاد اسکلت Solution و Foundation
6. [ ] تعریف اولین Vertical Slice (Post)

---

## ۱۱. تأیید و پذیرش

| نقش                  | نام               | تاریخ       | امضا / تأیید |
|----------------------|-------------------|-------------|--------------|
| Product Owner        | کارفرما           | 2026-09-14  | تأیید شد (گفتگو) |
| Technical Lead       | تیم فنی           | 2026-09-14  | تأیید شد     |
| Architecture Owner   | تیم فنی           | 2026-09-14  | تأیید شد     |

**پس از پذیرش این سند، هرگونه انحراف از Architecture Baseline یا AI Development Path فقط از طریق ADR مجاز است.**

---

**پایان PROJECT BASELINE v0.1**