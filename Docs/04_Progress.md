# 04 — Progress

> این فایل باید در **پایان هر Session** به‌روز شود.
> هدف: امکان ادامه کار توسط AI یا توسعه‌دهنده جدید بدون نیاز به تاریخچه گفتگو.

**آخرین به‌روزرسانی:** `2026-09-17`
**Session مربوطه:** `Session-20260917-Phase6-IamFix`

---

## فاز جاری
`Phase 6 — Frontend Development` ✅ تکمیل شد

## درصد پیشرفت فاز جاری
`100%`

## درصد پیشرفت کلی پروژه
`85%`

## فایل‌های ایجاد شده در این Session
- ساختار ریشه: `Backend/`, `Frontend/`, `.gitignore`, `.editorconfig`, `README.md`
- `Docs/00_ProjectContext.md`, `Docs/01_Architecture.md`, `Docs/02_CodingStandards.md`, `Docs/03_Roadmap.md`, `Docs/04_Progress.md`, `Docs/05_ChangeLog.md`, `Docs/06_DevelopmentRules.md`
- `Docs/PROJECT-BASELINE-v0.1.md`, `Docs/decision-log.md`, `Docs/open-questions.md`
- `Docs/adr/ADR-001` تا `ADR-009`
- `Docs/domain/` (vision-and-problem, bounded-contexts, ubiquitous-language, aggregates-and-entities, domain-events)
- `Docs/Phases/Phase00.md` + اسکلت `Phase01..08.md`
- `Docs/Architecture/` (context-map, container-diagram, iam-integration)
- `Docs/api-contracts/README.md`
- `Docs/SessionReports/Session-20260914-1040.md`

## فایل‌های ایجاد شده در فاز ۱ (Backend)
- اسکلت Solution: `Backend/src/OrganizationalStructure.slnx` با ۴ پروژه لایهای (net10.0)
- Domain Common: `BaseEntity`, `AuditableEntity`, `TenantEntity`, `FullAuditableEntity`, `IAuditable`, `ITenantScoped`, `IDomainEvent`, `EntityStatus`
- Domain Abstractions: `IClock`, `ICurrentUser`, `ITenantContext`
- Application Common: `Error`/`ErrorType`, `Result`/`Result<T>`
- Infrastructure: `OrganizationalStructureDbContext` (Global Filters), `AuditSaveChangesInterceptor`, `OrganizationalStructureDbContextFactory`, `SystemClock`, `CurrentUserTenantContext`, `DependencyInjection`
- API: `Program.cs` (Serilog/DI/Swagger/Health), `ClaimNames`, `HttpContextCurrentUser`, `ExceptionHandlingMiddleware`, `appsettings`
- Tests: `Backend/tests/OrganizationalStructure.ArchitectureTests` (۴ Fitness Function، همه سبز)

## فایل‌های ایجاد شده در فاز ۴ (Access & Visibility)
- BFF: `IIamClient` + `IamClient` (قراردادهای واقعی IAM) + `IamOptions` + نشست سمت‌سرور + هندلر کوکی + `AuthController`
- Scope: `OrganizationScope` (خالص) + Claim چندمقداری + حل در ورود (fail-closed)
- ۲۷ Policy + Fallback Deny + `[Authorize]` روی ۳۴ Endpoint
- Scope در ۱۶ هندلر (403 در تخلف) + قانون مشاهده پرسنل + فیلتر جستجو
- تست‌ها: ۱۰۱ سبز (۳۵ دامنه + ۳۲ کاربرد + ۷ زیرساخت + ۴ معماری + ۲۳ یکپارچگی)

## فایل‌های ایجاد شده در فاز ۵ (Integration کوتاه)
- `IamBearerAuthenticationHandler` + `AuthenticationSchemes` (Smart) + `OrganizationScopeResolver` مشترک
- `Docs/Architecture/external-integration.md` (راهنمای مصرف‌کننده‌ها)
- تست‌ها: ۱۰۵ سبز (۴ تست Bearer/Selector جدید)

## فایل‌های ایجاد شده در فاز ۶ (Frontend)
- اسکلت Next.js 16 + MUI RTL + Vazirmatn + TanStack/RHF/Zod (پورت 6300)
- API Client + Auth (login/logout/me) + RequireAuth + rewrite به بک‌اند
- چارت سازمانی (Tree + نشان امضا) + صفحات Post/Employee/Responsibility/Authority
- ماسک PII با `ViewSensitiveData`
- tsc/lint/build سبز

## فایل‌های ایجاد شده در Session اشکال‌زدایی ورود (۲۰۲۶-۰۹-۱۷)
- `Backend/src/OrganizationalStructure.Infrastructure/Security/IamConfigurationValidator.cs`
- `Backend/tests/OrganizationalStructure.Infrastructure.UnitTests/IamConfigurationValidatorTests.cs` (۷ تست)
- `Backend/tests/OrganizationalStructure.Infrastructure.UnitTests/IamClientTests.cs` (۸ تست)
- `Docs/SessionReports/Session-20260917-Phase6-IamFix.md`

## فایل‌های تغییر یافته در Session اشکال‌زدایی ورود (۲۰۲۶-۰۹-۱۷)
- `Backend/src/OrganizationalStructure.API/Program.cs` (هشدار راه‌اندازی برای تنظیمات بدون مقدار IAM)
- `Backend/src/OrganizationalStructure.Infrastructure/Integration/IamClient.cs` (خواندن بدنه خطای IAM در پاسخ‌های غیرموفق)
- `Backend/src/OrganizationalStructure.Infrastructure/DependencyInjection.cs` (**IBffSessionStore از Scoped به Singleton** — ریشه دوم ۴۰۱ نشست)
- `Backend/src/OrganizationalStructure.API/appsettings.Development.json` (محلی و skip-worktree — `ClientSecret` هم‌تراز با IAM؛ بدون کامیت)
- `Docs/Architecture/iam-integration.md` (بخش ۶: پیش‌نیازهای عملیاتی اتصال به IAM)
- `Docs/open-questions.md` (Q-009 بسته شد — DEC-028، Q-010)
- تست‌ها: ۱۲۰ سبز (۳۵ دامنه + ۳۲ کاربرد + ۲۲ زیرساخت + ۴ معماری + ۲۷ یکپارچگی)
- Frontend: `tsc` تمیز + ESLint تمیز (۰ خطا/هشدار) + `next build` موفق (۱۷ روت)

## ریشه‌های سه‌گانه ۴۰۱ و رفع نهایی (۲۰۲۶-۰۹-۱۷)
1. `Iam:BaseAddress` خالی → تنظیم شد (skip-worktree)
2. `Iam:ClientSecret` ناهم‌تراز با کلاینت `personnel-bff` سمت IAM → هم‌تراز شد (۴۰۱ `invalid_client` رفع)
3. **باگ ثبت‌نشست BFF:** `IBffSessionStore` با `AddScoped` رجیستر شده بود درحالیکه `InMemoryBffSessionStore` حافظه داخلی دارد ⇒ هر درخواست استور خالی میگرفت و `/auth/me` همیشه «نشست معتبر نیست» (۴۰۱) → `AddSingleton` (سازگار با کامنت خود کلاس و طراحی تک‌نمونه؛ Redis در Phase 8)
- Q-009 (Master Data سازمان در IAM) با تأیید کارفرما از طریق API خود IAM بسته شد (DEC-028): سازمان `herasat-dev` ثبت و به ۵ کاربر Seed تخصیص یافت
- **اعتبارسنجی سرتاسری سبز:** login (مستقیم `:5297` و از طریق پروکسی `:6300`) = ۲۰۰ + کوکی `orgstructure_session`؛ `/api/v1/auth/me` = ۲۰۰ با `organizationId`، `roles` و `permissions`


## فایل‌های باقیمانده (برای فاز جاری)
- هیچ — فاز ۶ تکمیل شد.

## قدم بعدی دقیق
`Phase 7 — Import (مسدود: Q-003). سپس Phase 8 — Production (NU1903، Redis، CI/CD، Push).`
۱) ~~رفع مسدودکننده Q-009 توسط مالک IAM~~ — ✅ بسته شد (DEC-028)؛ ورود سرتاسری سبز است.
۲) ~~اعتبارسنجی سرتاسری ورود~~ — ✅ انجام شد (login → `/me` → ۲۰۰ روی `:5297` و `:6300`).
۳) کامیت تأییدشده تغییرات معوق Frontend و بک‌اند این Session (شامل اصلاح Singleton نشست BFF).


## مشکلات / Blockers
- ✅ **Q-009 بسته شد (DEC-028):** سازمان توسعه `herasat-dev` در IAM ثبت و `OrganizationId` به ۵ کاربر Seed تخصیص یافت؛ ورود سرتاسری سبز است.
- ✅ **رفع‌شده در این Session:** ۴۰۱ ناشی از `Iam:BaseAddress` خالی، `Iam:ClientSecret` نادرست (۴۰۱ `invalid_client`) و **ثبت Scoped نشست BFF** (`IBffSessionStore` → Singleton).

- ثبت ۲۷ Permission در IAM (اقدام مالک IAM) — تا آن زمان همه درخواست‌ها 403 (صحیح).
- Schema جدول واسط (Q-003) برای Phase 7 باز است.
- Seed کدهای Responsibility/Authority پس از تأیید Business Catalog.
- نشست درون‌حافظه‌ای (تک‌نمونه)؛ Redis در Phase 8.
- ⚠️ آسیب‌پذیری `Microsoft.OpenApi 2.3.0` (NU1903) — ثبت‌شده، حل در Phase 8.
- ⚠️ تغییرات Frontend (ساختار protected + RequirePermission) و تغییرات بک‌اند این Session هنوز **کامیت نشده** و در انتظار تأیید انسان است.


## تصمیمات گرفته‌شده در این Session
- DEC-001 تا DEC-025 (مرجع: `Docs/decision-log.md`)
- ADR-001 تا ADR-011 (مرجع: `Docs/adr/`)
- Q-002/Q-004/Q-005/Q-006/Q-009 بسته شدند؛ ثبت IAM و Q-003/Q-007/Q-008/Q-010 باز است.

## وضعیت کامیت‌ها
- تعداد کامیت‌های Phase 6 (همه پس از تأیید انسان): ۸
- آخرین کامیت: «fix(phase6): singleton BFF session store + IAM master data alignment (login 200)» (شامل بک‌اند + تستها + فرانت protected + Docs؛ تأیید کارفرما در 2026-09-17)
- کامیت‌نشده: هیچ — درخت کاری تمیز است.


## یادآوری قوانین اجباری
- تایم‌باکس ۱۵ دقیقه‌ای رعایت شد؟ `بله`
- XML Documentation فارسی اضافه شد؟ `بله — همه کلاس/متد/پراپرتی جدید`
- هیچ TODO یا Incomplete Code وجود ندارد؟ `بله`
- Session Report ایجاد شد؟ `بله - Session-20260917-Phase6-IamFix`

---

**قانون:** قبل از خاتمه Session این فایل باید کامل باشد.
