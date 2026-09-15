# 01 — Architecture (معماری)

> این فایل خلاصه و لینک به سند اصلی معماری است.
> جزئیات کامل در `Architecture Baseline` (نسخه ۲.۴) نگهداری می‌شود.

**آخرین به‌روزرسانی:** `2026-09-14`

---

## لینک سند اصلی
- `Docs/01-Architecture-Baseline-FA.md` (هسته منجمد — نسخه ۲.۴)
- `Docs/02-AI-Development-Path-0-to-100-FA.md` (هسته منجمد — نسخه ۲.۴)
- `Docs/PROJECT-BASELINE-v0.1.md` (سند پایه پروژه)
- `Docs/Organizational-Structure-Big-Picture-Scenario.md` (تصویر بزرگ)
- `Docs/Architecture/iam-integration.md` (پروتکل یکپارچگی IAM)
- `Docs/Architecture/context-map.md`, `Docs/Architecture/container-diagram.md`

## تصمیم‌های Freeze فعلی
- Backend: .NET 10 (LTS) + ASP.NET Core Web API + Clean Architecture + Modular Monolith
- CQRS + MediatR؛ FluentValidation؛ Mapster
- EF Core + SQL Server؛ Redis؛ RabbitMQ + Outbox (Deferred در MVP)؛ Hangfire
- MinIO؛ Elasticsearch/OpenSearch؛ Serilog + OpenTelemetry
- نسخه‌بندی API: `/api/v1/...`؛ OpenAPI 3.1
- Frontend: **Next.js (App Router) + TypeScript + TanStack Query + React Hook Form + Zod + MUI (RTL) + Tailwind** (طبق Baseline، ADR-001)
- احراز هویت: مصرف IAM خارجی (OIDC/OpenIddict در متن Baseline؛ واقعیت IAM: REST + JWT) — فقط مصرف‌کننده، هرگز سرور هویت (ADR-002)
- **Multi-tenancy:** لازم است (`TenantId`) — ADR-005
- **Organization/User Master:** در IAM؛ فقط Reference — ADR-002
- **Employee:** مالک داده پرسنلی در OrgStructure — ADR-003
- **Post Tree:** درخت مستقل به‌ازای هر Organization؛ جابجایی بین Organization ممنوع — ADR-004
- **PII:** Always Encrypted — ADR-006

## ساختار برنامه‌ریزی‌شده Backend (۴ پروژه لایه‌ای طبق Baseline §۳.۲)
```
Backend/
└── src/
    ├── OrganizationalStructure.Domain
    ├── OrganizationalStructure.Application
    ├── OrganizationalStructure.Infrastructure
    └── OrganizationalStructure.API
```

## دیاگرام‌های کلیدی
(در پوشه `Docs/Architecture/` قرار گیرند)
- Context Map
- Container Diagram
- Sequenceهای اصلی (Post/Employee/Visibility/Integration)

## ADRهای فعال
| شناسه | عنوان | وضعیت |
|-------|-------|-------|
| ADR-001 | انتخاب Frontend: Next.js + MUI طبق Baseline | Accepted |
| ADR-002 | مرز Master Data با Enterprise-IAM-V2 | Accepted |
| ADR-003 | Employee به‌عنوان مالک داده پرسنلی + Reference به IAM User | Accepted |
| ADR-004 | مدل درخت Post و رابطه با Organization | Accepted |
| ADR-005 | Multi-tenancy و Organization Scope | Accepted |
| ADR-006 | حفاظت PII (Always Encrypted) | Accepted |
| ADR-007 | تعلیق RabbitMQ/Outbox و REST-only در MVP | Accepted |
| ADR-008 | Access Control: IAM مرجع Role/Policy؛ بدون Authorization موازی (+ متمم ۱) | Accepted |
| ADR-009 | افزودن Region=3 به OrganizationType در IAM | Accepted |
| ADR-010 | فیلدهای تکمیلی پرسنل (تاریخ تولد، سابقه حراست، موبایل پژواک) | Accepted |

---

**توجه:** هر تغییر معماری فقط از طریق ADR مجاز است.
