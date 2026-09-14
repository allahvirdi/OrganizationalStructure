# 01 — Architecture

> این فایل خلاصه و لینک به سند اصلی معماری است.  
> جزئیات کامل در `Architecture Baseline` (نسخه ۲.۴) نگهداری می‌شود.

**آخرین به‌روزرسانی:** `[YYYY-MM-DD]`

---

## لینک سند اصلی
- `Docs/../01-Architecture-Baseline-FA.md` (یا مسیر معادل)

## تصمیم‌های Freeze فعلی
- .NET 10 + Clean Architecture + Modular Monolith
- CQRS + MediatR + FluentValidation + Mapster
- EF Core + SQL Server
- Redis + RabbitMQ (Outbox) + Hangfire
- MinIO + Elasticsearch/OpenSearch
- Serilog + OpenTelemetry
- Frontend بر اساس قالب `SampleAdminPanel`
- Identity Provider خارجی (OIDC / OpenIddict)

## دیاگرام‌های کلیدی
(در پوشه `Docs/Architecture/` قرار گیرند)
- Context Map
- Container Diagram
- Sequenceهای اصلی

## ADRهای فعال
| شناسه | عنوان | وضعیت |
|-------|-------|-------|
|       |       |       |

---

**توجه:** هر تغییر معماری فقط از طریق ADR مجاز است.
