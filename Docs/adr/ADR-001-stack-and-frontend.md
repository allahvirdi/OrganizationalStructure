# ADR-001: پذیرش استک معماری و تفکیک Frontend/Backend

- **وضعیت:** Accepted
- **تاریخ:** 2026-09-14
- **نویسنده:** Technical Lead / Architecture (تأیید کارفرما)

## Context
سندهای هسته (`Architecture Baseline` و `AI Development Path` نسخه ۲.۴) یک استک فناوری سازمانی مشترک را الزامی کرده‌اند. برای این پروژه باید استک پایه رسماً پذیرفته و منجمد شود. همچنین انتخاب Frontend/Backend باید قطعی شود و رابطه با قالب مرجع `SampleAdminPanel` روشن باشد.

## Decision
- **Backend (منجمد):** .NET 10 (LTS) + ASP.NET Core Web API + Clean Architecture + **Modular Monolith** با **۴ پروژه لایه‌ای** طبق Baseline §۳.۲:
  `Domain ← Application ← Infrastructure ← API`
  + CQRS/MediatR، FluentValidation، Mapster، EF Core + SQL Server، Redis، Hangfire، MinIO، Elasticsearch/OpenSearch، Serilog + OpenTelemetry، نسخه‌بندی `/api/v1/...` و OpenAPI 3.1.
- **Frontend (منجمد):** **Next.js (App Router) + TypeScript + TanStack Query + React Hook Form + Zod + MUI (RTL) + Tailwind CSS** طبق Baseline §۴.
- **SampleAdminPanel:** فقط مرجع بصری/UX است، نه مرجع Domain/Architecture. آیتم‌های نمونه‌ی خارج از دامنه (Product/Order) حذف می‌شوند.
- هر انحراف از موارد فوق فقط با ADR جدید مجاز است.

## Rationale
- یکنواختی با سایر پروژه‌های سازمان، دانش مشترک و تست‌پذیری.
- MUI برای پشتیبانی RTL و کامپوننت‌های سازمانی در Baseline توصیه شده است.
- Modular Monolith سادگی استقرار را با حفظ مرز ماژول فراهم می‌کند.

## Considered and rejected alternatives
- Frontend بر پایه Vite/React (SampleAdminPanel): رد شد — طبق Baseline و تصمیم صریح کارفرما، Next.js + MUI انتخاب شده است؛ SampleAdminPanel فقط مرجع بصری است.
- Microservices: رد شد — پیچیدگی عملیاتی بیش از نیاز فعلی.
- ۵ پروژه لایه‌ای (+Worker): رد شد — Baseline §۳.۲ چهار پروژه تعریف کرده و پروژه خواهر نیز بر همین اساس است؛ Hangfire در API/Infrastructure میزبانی می‌شود.

## Consequences
- مثبت: یکنواختی، قابلیت تست، استقرار ساده.
- منفی: نیاز به انطباق بصری SampleAdminPanel با MUI؛ تفاوت واقعی Stack قالب با استک مصوب باید در طراحی UI مدیریت شود.

**منابع:** `Docs/01-Architecture-Baseline-FA.md` (§۳.۱، §۴)، `Docs/PROJECT-BASELINE-v0.1.md` (DEC-003، DEC-004، DEC-014)