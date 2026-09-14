# سند معماری توسعه (Enterprise Baseline)
## نسخه ۲.۴ — قالب عمومی و قابل استفاده مجدد

> این سند **ثابت** و مستقل از دامنه کسب‌وکار است و برای تمام پروژه‌ها قابل استفاده می‌باشد.  
> بخش‌های مشخص‌شده با `[PROJECT-SPECIFIC]` باید برای هر پروژه تکمیل شوند.  
> هدف: ایجاد پایه معماری یکسان، کنترل‌شده و انترپرایز برای توسعه با هوش مصنوعی و تیم انسانی.

---

## ۱. ساختار ریشه پروژه (اجباری و غیرقابل تغییر)

```
/
├── Docs/                       # تمام اسناد پروژه (معماری، ADR، قراردادها، راهنماها)
├── Backend/                    # کد سمت سرور
├── Frontend/                   # کد سمت کلاینت (شامل قالب SampleAdminPanel)
├── .gitignore
├── README.md
├── docker-compose.yml          # (اختیاری در شروع، اجباری قبل از Production)
└── .editorconfig / .dockerignore / دیگر فایل‌های ریشه
```

**قواعد سخت:**
- هیچ کدی خارج از پوشه‌های `Backend` و `Frontend` قرار نمی‌گیرد.
- تمام اسناد (از جمله ADR، قرارداد API، Domain Model، Decision Log) فقط داخل `Docs` نگهداری می‌شوند.
- نام پوشه‌های ریشه دقیقاً به همین شکل حفظ شود تا یکنواختی بین پروژه‌ها برقرار بماند.

---

## ۲. Domain Vision و مسئله کسب‌وکار `[PROJECT-SPECIFIC]`

قبل از هر تصمیم فنی، این بخش باید تکمیل شود:

- **Problem Statement**: مشکل اصلی کسب‌وکار چیست؟
- **Vision**: سیستم در نهایت چه ارزشی ایجاد می‌کند؟
- **Primary Users / Personas**
- **Core Business Capabilities**
- **Bounded Contexts اولیه**
- **Non-Goals** (چه چیزهایی عمداً خارج از محدوده هستند)

> بدون تکمیل این بخش، Phase 2 (Domain Modeling) شروع نمی‌شود.

---

## ۳. Backend

### ۳.۱ تکنولوژی پایه (Freeze)

| حوزه                | تکنولوژی                              | توضیح / دلیل انتخاب |
|---------------------|---------------------------------------|---------------------|
| Runtime             | .NET 10 (LTS)                         | عملکرد بالا، پشتیبانی بلندمدت، اکوسیستم بالغ |
| Framework           | ASP.NET Core Web API                  | Minimal APIs + Controllers ترکیبی |
| Architecture        | Clean Architecture                    | جداسازی سخت لایه‌ها و قابلیت تست‌پذیری |
| CQRS                | MediatR                               | جداسازی Command و Query |
| Validation          | FluentValidation                      | Validation در لایه Application |
| Mapping             | Mapster                               | سرعت بالا و کد کمتر |
| ORM                 | EF Core                               | Code-First و Migration قوی |
| Database            | SQL Server                            | Primary Relational Store |
| Cache               | Redis                                 | Distributed Cache + Locking |
| Messaging           | RabbitMQ + Outbox Pattern             | Event-Driven و قابلیت اطمینان |
| Search              | Elasticsearch                         | Full-text Search و Analytics |
| File / Object Storage | MinIO (S3-compatible)               | ذخیره‌سازی اسناد و فایل‌ها |
| Logging             | Serilog                               | Structured Logging |
| Observability       | OpenTelemetry + Prometheus + Grafana  | Metrics, Traces, Logs |
| Authentication      | OIDC + LDAP / Active Directory + SSO  | پشتیبانی محیط‌های سازمانی |
| Authorization       | RBAC + Policy-Based + Organization Scope | کنترل دسترسی دقیق |
| API Documentation   | OpenAPI 3.1 + Swagger UI              | |
| Document Schema     | پشتیبانی تا سطح Property (JSON/XML Schema) | |

**جایگزین‌های ردشده** باید در ADR مربوطه ثبت شوند.

### ۳.۲ ساختار پوشه‌های Backend

```
Backend/
└── src/
    ├── Domain/                 # Entities, Aggregates, Value Objects, Domain Events, Enums, Interfaces
    ├── Application/            # Commands, Queries, Handlers, DTOs, Validators, Interfaces, Behaviors
    ├── Infrastructure/         # EF Core, Redis, RabbitMQ, MinIO, External Services, Outbox
    └── API/                    # Endpoints, Middleware, Filters, DI Composition Root
```

**جریان وابستگی (اجباری و غیرقابل نقض):**

```
Domain
  ↓
Application
  ↓
Infrastructure
  ↓
API
```

- Domain هیچ وابستگی به لایه‌های بیرونی ندارد.
- Application فقط به Domain وابسته است.
- Infrastructure پیاده‌سازی اینترفیس‌های Application و Domain را انجام می‌دهد.
- API نقطه ورود است و منطق کسب‌وکار ندارد.

### ۳.۳ استانداردهای اجباری Backend

- هر قابلیت به صورت **Vertical Slice** پیاده‌سازی می‌شود.
- استفاده از **Outbox Pattern** برای تمام پیام‌های خروجی اجباری است.
- Soft Delete + ستون‌های Audit روی تمام جداول اصلی.
- Multi-tenancy از طریق `TenantId` (یا معادل) پشتیبانی شود.
- تمام Exceptionها به **Error Contract** استاندارد تبدیل شوند.
- Idempotency برای عملیات حساس (پرداخت، ایجاد پرونده و ...) پشتیبانی شود.
- استفاده از `DateTime.Now` یا `DateTime.UtcNow` مستقیم ممنوع است (باید از Abstraction ساعت استفاده شود).
- Concurrency Token (RowVersion) برای موجودیت‌های مهم توصیه می‌شود.

---

## ۴. Frontend

| تکنولوژی                    | کاربرد                          |
|-----------------------------|---------------------------------|
| Next.js (App Router)        | Framework اصلی                  |
| TypeScript                  | Type Safety کامل                |
| TanStack Query (React Query)| مدیریت Server State             |
| React Hook Form             | مدیریت فرم‌ها                   |
| Zod                         | Validation سمت کلاینت (همسو با Backend) |
| MUI (با پشتیبانی RTL)       | Component Library               |
| Tailwind CSS                | Utility-first Styling           |

**قالب ادمین پروژه (اجباری):**
- قالب آماده ادمین در پوشه `SampleAdminPanel` قرار دارد.
- **Frontend باید الزاماً بر اساس این قالب توسعه داده شود.**
- تغییر ساختار اصلی، Layout، Theme و کامپوننت‌های پایه قالب فقط با ADR مجاز است.
- هر Feature جدید باید با سبک، ساختار پوشه و الگوهای موجود در `SampleAdminPanel` هم‌خوان باشد.

### ساختار پیشنهادی Frontend

```
Frontend/
├── app/                        # App Router (صفحات و Layoutها)
├── components/                 # کامپوننت‌های عمومی
├── features/                   # ماژول‌های مبتنی بر Feature
├── hooks/                      # Custom Hooks
├── lib/                        # API Client، Utils، Config
├── stores/                     # Client State (در صورت نیاز)
├── types/                      # Typeهای مشترک
└── styles/
```

**قواعد:**
- هر Feature Frontend باید با قرارداد API مربوطه هم‌تراز باشد.
- Permission Guard برای تمام صفحات و اکشن‌های حساس اجباری است.
- حالت‌های Loading / Error / Empty باید به صورت یکنواخت پیاده‌سازی شوند.

---

## ۵. Authentication & Authorization

| Authentication                        | Authorization                                      |
|---------------------------------------|----------------------------------------------------|
| AD / LDAP + OIDC + SSO                | RBAC + Policy-Based Authorization + Organization Scope |

- پشتیبانی از Multi-Organization / Multi-Tenant
- Deny by Default
- تمام Endpointها باید دارای Policy مشخص باشند.
- Claim-based و Permission-based به صورت ترکیبی پشتیبانی شود.

---

## ۶. API First Contract (اجباری)

قبل از شروع هر فیچر Frontend، قرارداد API باید تعریف و در مسیر زیر ثبت شود:

```
Docs/api-contracts/
```

**حداقل موارد برای هر Resource:**

- Request DTO
- Response DTO
- Error Contract (یکسان برای کل سیستم)
- Pagination (ترجیحاً Cursor-based یا Offset + Limit استاندارد)
- Filtering و Sorting
- Versioning (`/api/v1/...`)
- مثال‌های موفق و ناموفق (مثال‌های واقعی)

**مسیرهای پایه نمونه (قابل گسترش بر اساس دامنه):**

- `/api/v1/[resource]`
- `/api/v1/workflows`
- `/api/v1/tasks`
- `/api/v1/dashboard`
- `/api/v1/documents`

---

## ۷. Database Baseline

### ۷.۱ الزامات اولیه

- ERD (Conceptual + Logical)
- Naming Convention کامل (جداول، ستون‌ها، ایندکس‌ها، FK، Constraints)
- Migration Strategy (EF Core Migrations + نسخه‌گذاری)
- سیاست Audit و Soft Delete
- استراتژی Multi-tenancy

### ۷.۲ ستون‌های استاندارد هر جدول اصلی

| ستون         | نوع پیشنهادی             | توضیح                              |
|--------------|--------------------------|------------------------------------|
| Id           | `uniqueidentifier` / `bigint` | Primary Key                     |
| CreatedAt    | `datetimeoffset`         | زمان ایجاد                         |
| CreatedBy    | `nvarchar` / `uniqueidentifier` | کاربر ایجادکننده             |
| ModifiedAt   | `datetimeoffset` NULL    | زمان آخرین تغییر                   |
| ModifiedBy   | `nvarchar` / `uniqueidentifier` NULL | کاربر تغییردهنده         |
| IsDeleted    | `bit`                    | Soft Delete                        |
| TenantId     | `uniqueidentifier` / `nvarchar` | Multi-tenancy                 |
| RowVersion   | `rowversion`             | Optimistic Concurrency (توصیه می‌شود) |

---

## ۸. Non-Functional Requirements (NFR)

| حوزه                | حداقل استاندارد پیشنهادی                          |
|---------------------|---------------------------------------------------|
| Performance         | p95 Response Time < 300ms برای عملیات معمولی      |
| Scalability         | پشتیبانی از Horizontal Scaling                    |
| Availability        | حداقل 99.9٪ (بسته به زیرساخت)                     |
| Security            | پوشش OWASP Top 10 + Secure Coding Checklist       |
| Observability       | Logs + Metrics + Distributed Traces کامل          |
| Data Retention      | سیاست نگهداری، آرشیو و حذف مشخص                   |
| Backup & Disaster Recovery | RPO و RTO تعریف و تست‌شده باشد               |
| Resilience          | Retry، Circuit Breaker، Timeout برای وابستگی‌های خارجی |

---

## ۹. استراتژی تست (اجباری)

| نوع تست                  | محدوده                              | ابزار پیشنهادی                  |
|--------------------------|-------------------------------------|---------------------------------|
| Unit Test                | Domain + Application Handlers       | xUnit + FluentAssertions        |
| Integration Test         | Infrastructure + API                | WebApplicationFactory + Testcontainers |
| Architecture Test        | رعایت لایه‌ها و وابستگی‌ها          | NetArchTest یا ArchUnitNET      |
| Contract Test            | هماهنگی Backend و Frontend          | Pact یا معادل                   |
| End-to-End (انتخابی)     | جریان‌های اصلی کاربر                | Playwright                      |

**حداقل Coverage هدف:**  
- Domain و Application: ≥ 80٪  
- تمام Handlerهای Command باید حداقل یک تست مثبت و یک تست منفی داشته باشند.

---

## ۱۰. Architecture Decision Records (ADR)

هر تصمیم معماری مهم باید در قالب ADR ثبت شود:

```
Docs/adr/ADR-XXX-short-title.md
```

**قالب استاندارد ADR:**

- عنوان
- وضعیت (Proposed / Accepted / Deprecated / Superseded)
- زمینه (Context)
- تصمیم (Decision)
- دلایل (Why this option)
- جایگزین‌های بررسی‌شده و ردشده
- پیامدها (Positive & Negative Consequences)
- تاریخ و نویسنده

---

## ۱۱. Architecture Fitness Functions

قواعدی که باید به صورت خودکار در CI بررسی شوند:

- لایه API / Controller نباید مستقیماً به Database یا DbContext دسترسی داشته باشد.
- Domain نباید به Infrastructure یا Application وابسته باشد.
- هیچ وابستگی معکوس بین لایه‌ها مجاز نیست.
- تمام Command/Queryهای عمومی باید Validator داشته باشند.
- استفاده مستقیم از `DateTime.Now` / `DateTime.UtcNow` ممنوع است.
- هیچ Secret یا Connection String در کد یا Repository وجود نداشته باشد.

---

## ۱۲. امنیت (Security Baseline)

- Authorization + Authentication (Deny by Default)
- Tenant Isolation
- Claim Validation
- پوشش OWASP Top 10
- XSS / CSRF / SQL Injection / IDOR / Path Traversal
- Secret Management (هیچ Secret در کد یا Repository)
- HTTPS و Data Protection
- Rate Limiting برای APIهای عمومی
- Audit Log برای عملیات حساس


- احراز هویت و مجوزدهی Deny by Default
- Input Validation در تمام لایه‌ها
- Output Encoding
- محافظت در برابر OWASP Top 10
- مدیریت Secret با ابزار مناسب (نه در کد)
- Dependency Scanning و SAST در Pipeline
- Rate Limiting و Throttling برای APIهای عمومی
- Audit Log برای عملیات حساس

---

## ۱۳. Observability و عملیات

- Structured Logging با Correlation Id
- Distributed Tracing
- Metrics کسب‌وکار و فنی
- Alerting بر اساس SLO
- Health Checks (Liveness / Readiness)
- Runbook برای عملیات رایج و خطاهای شناخته‌شده

---

**پایان سند معماری توسعه — نسخه ۲.۴**
```
---

## ۱۴. کنترل توسعه با هوش مصنوعی (مرجع)

قوانین اجباری کار با هوش مصنوعی (تایم‌باکس ۱۵ دقیقه‌ای + کامیت فقط بعد از تأیید صریح انسان) در سند **مسیر توسعه از ۰ تا ۱۰۰ با هوش مصنوعی (نسخه ۲.۴)** به تفصیل آمده است و برای تمام پروژه‌ها الزامی است.
