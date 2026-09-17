# 05 — Change Log

> ثبت تمام تغییرات مهم پروژه به ترتیب زمانی معکوس (جدیدترین بالا).

**فرمت هر ورودی:**

```
## [YYYY-MM-DD] - Session-XXXX / Phase XX
### Added
- 

### Changed
- 

### Fixed
- 

### Removed
- 

### Decisions / ADRs
- 
```

---

## [2026-09-17] - Session-20260917-Phase6-IamFix / Phase 6 (اشکال‌زدایی ورود)
### Added
- `IamConfigurationValidator` — بررسی خالص و قابل‌تست کامل بودن پیکربندی IAM (`BaseAddress`، `ClientId`، `ClientSecret`)
- هشدار صریح در راه‌اندازی `Program.cs` برای هر تنظیم بدون مقدار IAM (جلوگیری از شکست خاموش ورود)
- ۱۵ تست جدید: `IamConfigurationValidatorTests` (۷ تست) + `IamClientTests` (۸ تست) — مجموع ۱۲۰ تست سبز
- بخش ۶ «پیش‌نیازهای عملیاتی اتصال به IAM» در `Docs/Architecture/iam-integration.md`

### Changed
- `Iam:ClientSecret` در `appsettings.Development.json` (skip-worktree — محلی و بدون کامیت) با کلاینت `personnel-bff` سمت IAM هم‌تراز شد
- `IamClient.PostEnvelopeAsync` بدنه پاسخ IAM را در پاسخ‌های غیرموفق نیز می‌خواند تا پیام دقیق IAM حفظ شود
- `IBffSessionStore` از `AddScoped` به `AddSingleton` تغییر کرد — استور درون‌حافظه‌ای نشست BFF باید تک‌نمونه باشد
- Q-009 بسته شد (DEC-028): Master Data سازمان محیط توسعه با تأیید کارفرما از طریق API خود IAM ثبت و به کاربران Seed تخصیص یافت

### Fixed
- شکست ۴۰۱ ورود با اعتبارنامه صحیح در گام `POST /api/token/validate` با خطای `invalid_client` (علت: `ClientSecret` نادرست)
- ۴۰۱ نشست روی `/api/v1/auth/me` («نشست معتبر نیست») — علت: ثبت Scoped استور نشست؛ هر درخواست استور خالی میگرفت
- پیام رمز اشتباه از «خطا در ارتباط با سامانه هویت.» به «نام کاربری یا رمز عبور نامعتبر است.» اصلاح شد
- ۴۰۱ `Auth.NoScope` — با ثبت Organization و تخصیص `OrganizationId` در IAM (DEC-028) برطرف شد
- اعتبارسنجی سرتاسری سبز: login و `/me` روی `:5297` و پروکسی `:6300` هر دو ۲۰۰

### Removed
-

### Decisions / ADRs
- DEC-028 (Q-009: تأمین Master Data سازمان محیط توسعه از طریق API خود IAM) — Accepted؛ Q-009 بسته شد
- بدون ADR جدید؛ اتکا به DEC-018/ADR-002 و DEC-027/ADR-012
- Q-010 (قرارداد خطای Auth) باز ماند — نیازمند تصمیم کارفرما/Tech Lead

## [2026-09-16] - Frontend Shell Remediation / Phase 6
### Added
- `AppShell` همسو با قالب مرجع (سایدبار 272px، هدر، حالت تیره/روشن، دراور موبایل) + سیم‌کشی در `(protected)/layout`
- منوی فیلترشده با Permission + ریدایرکت خانه به داشبورد + `RequirePermission` (اعمال در ۵ صفحه ورودی)
- تم MUI همسو با توکن‌های قالب (ink/brand/secondary)

### Changed
- حذف RequireAuth تکراری صفحات (متمرکز در layout)

## [2026-09-16] - Session-20260916-Phase6 / Phase 6
### Added
- فرانت‌اند Next.js 16 + MUI RTL + Vazirmatn (پورت 6300) + TanStack/RHF/Zod
- API Client (rewrite + کوکی) + Auth (login/logout/me) + RequireAuth
- چارت سازمانی + صفحات Post/Employee/Responsibility/Authority + ماسک PII
- `CurrentUserDto.Permissions` در بک‌اند برای تصمیم ماسک UI

### Decisions / ADRs
- بدون تصمیم جدید؛ اتکا به DEC-003 (Next.js+MUI) و ADR-001

## [2026-09-15] - Session-20260915-Phase5 / Phase 5
### Added
- احراز Bearer مصرف‌کننده‌ها + PolicyScheme هوشمند + Resolver مشترک Scope
- سند راهنمای Integration + ۴ تست جدید (۱۰۵ سبز)

### Decisions / ADRs
- بدون تصمیم جدید؛ اتکا به DEC-027 و ADR-012

## [2026-09-15] - Session-20260915-Phase4 / Phase 4
### Added
- BFF کامل: IIamClient/IamClient، نشست سمت‌سرور، هندلر کوکی، AuthController (login/MFA/logout/me)
- OrganizationScope (خالص + Claim) با حل در ورود و تازه‌سازی دوره‌ای
- ۲۷ Policy + Fallback Deny + `[Authorize]` روی ۳۴ Endpoint
- Scope در ۱۶ هندلر + قانون مشاهده پرسنل + تست‌های 403/BFF (۱۰۱ تست سبز)

### Changed
-

### Decisions / ADRs
- بدون تصمیم جدید؛ اتکا به DEC-001..025 و ADR-001..011

## [2026-09-15] - Q-006 Approval / Permission Catalog
### Added
-

### Changed
- وضعیت Permission Catalog: `Proposed` به `Approved` (۲۷ Permission، قالب سه‌بخشی)

### Decisions / ADRs
- DEC-025 (Q-006: نام نهایی ۲۷ Permission مصوب کارفرما)؛ Q-006 بسته شد؛ ثبت در IAM اقدام مالک IAM

## [2026-09-15] - Session-20260915-Phase3-Close / Phase 3 (بستن فاز)
### Added
- بستن Phase 3: همه Sliceها کامل (پست/پرسنل/مسئولیت/اختیار) + ۸۹ تست سبز + قراردادهای API
- `Docs/SessionReports/Session-20260915-Phase3.md`

### Changed
-

### Decisions / ADRs
- بدون تصمیم جدید؛ اتکا به DEC-001..024 و ADR-001..011

## [2026-09-15] - Responsibility/Authority Separation / Phase 3 (توسعه مدل)
### Added
- Aggregateهای `Responsibility` و `Authority` + Assignmentهای تاریخ‌دار (`PostResponsibilityAssignment`/`PostAuthorityAssignment`)
- ۱۰ رویداد دامنه جدید + `AuthorityCodes.SigningAuthority` (Proposed)
- Controllers جدید `Responsibilities`/`Authorities` (CRUD + Assign/End + Queries)
- قراردادهای `responsibilities.md` و `authorities.md`
- سند `Docs/domain/responsibility-routing.md` + بخش ۱۹ Big Picture
- ۱۲ Permission پیشنهادی جدید در Permission Catalog

### Changed
- حذف `Post.HasSigningAuthority`، `Post.Responsibilities`، Responsibility VO و Endpointهای title-based قبلی
- بازنویسی `PostDto` (تفصیلی با انتساب‌ها) + `PostSummaryDto` (فهرست) + محاسبه نشان امضا از Assignment
- اسناد دامنه (BC، Aggregates، UL، Events، ERD) همگام‌سازی شد

### Decisions / ADRs
- DEC-024 + ADR-011 (تفکیک ۴ مفهوم)؛ Breaking داخلی API ثبت شد (بدون Consumer خارجی)

## [2026-09-14] - Session-20260914-Phase2-Extension / Phase 2 (توسعه مدل)
### Added
- فیلدهای تکمیلی Employee: `BirthDate` (PII Randomized)، `PezhvakMobile` (PII Deterministic)، `HerasatServiceRecord` (VO: سال+ماه، غیر PII) + متد `UpdateSupplementaryInfo`
- `Domain/ValueObjects/HerasatServiceRecord.cs` + ۳ تست جدید (۲۲→۲۵ تست سبز: ۲۱ دامنه + ۴ معماری)
- `Docs/domain/permission-catalog.md` — کاتالوگ پیشنهادی ۲۰ Permission (وضعیت Proposed/Pending Approval)
- `Docs/adr/ADR-010-employee-supplementary-fields.md`

### Changed
- `Docs/Architecture/erd.md` — ستون‌های جدید Employees

### Decisions / ADRs
- DEC-022 (Q-005: عدم حذف فیزیکی پرسنل) — Closed
- DEC-023 (Q-006: کاتالوگ پیشنهادی Permission) — Proposed / Pending Approval
- ADR-010 (Accepted)

## [2026-09-14] - Session-20260914-Phase2 / Phase 2
### Added
- Aggregate `Post` (Factory، تغییر والد، صاحب‌امضا، مسئولیت‌ها، فعال/غیرفعال) + ۷ رویداد دامنه + Value Object `Responsibility`
- Aggregate `Employee` (ثبت، ویرایش، اتصال/قطع کاربر، اصلاح کد پرسنلی، فعال/غیرفعال) + `EmployeePostAssignment` (چندپستی + اصلی/تاریخی) + ۶ رویداد دامنه
- `PiiEncryptedAttribute`/`EncryptionType` برای اعلام سیاست رمزنگاری در دامنه
- `OrganizationalStructure.Domain.UnitTests` — ۱۸ تست واحد سبز
- `Docs/Architecture/erd.md` — ERD مفهومی + منطقی

### Changed
-

### Fixed
-

### Removed
-

### Decisions / ADRs
- بدون تصمیم جدید در فاز ۲؛ اتکا به DEC-001..021 و ADR-001..009

## [2026-09-14] - Session-20260914-Phase1 / Phase 1
### Added
- اسکلت Solution با ۴ پروژه لایهای (net10.0) + وابستگیهای Clean Architecture
- Domain Common: BaseEntity/AuditableEntity/TenantEntity/FullAuditableEntity + IAuditable/ITenantScoped/IDomainEvent + EntityStatus
- Domain Abstractions: IClock/ICurrentUser/ITenantContext
- Application Common: Error/ErrorType + Result/Result{T}
- Infrastructure: DbContext با Global Filters (TenantId/IsDeleted)، AuditSaveChangesInterceptor، DbContextFactory، SystemClock، CurrentUserTenantContext، DI
- API: Program.cs (Serilog/DI/Swagger/Health)، ClaimNames، HttpContextCurrentUser (BFF)، ExceptionHandlingMiddleware (RFC 7807)، appsettings
- `Backend/tests/OrganizationalStructure.ArchitectureTests` — ۴ Fitness Function (همه سبز)

### Changed
- بهبود Global Filter مستأجر در DbContext: ارزیابی داینامیک `CurrentTenantId` در هر کوئری (بهجای قفل در ساخت مدل)

### Fixed
- خطای Build ناشی از `Microsoft.Extensions.DependencyInjection.Abstractions` ناقص در Application (با افزودن پکیج)
- خطای HealthChecks در Infrastructure (با افزودن `Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore`)

### Removed
- فایلهای نمونه `Class1.cs` از سه پروژه لایهای

### Decisions / ADRs
- Q-002 (پروتکل IAM) با الگوی BFF قطعی شد
- ⚠️ آسیب‌پذیری شناختهشده `Microsoft.OpenApi 2.3.0` (NU1903) ثبت شد — حل در Phase 8

## [2026-09-14] - Session-20260914-1040 / Phase 0
### Added
- ایجاد ساختار ریشه پروژه: `Backend/`, `Frontend/`, `.gitignore`, `.editorconfig`, `README.md`
- ایجاد پوشه‌های اسناد: `Docs/adr/`, `Docs/domain/`, `Docs/Architecture/`, `Docs/Phases/`, `Docs/SessionReports/`, `Docs/api-contracts/`
- اسناد عملیاتی `00_ProjectContext.md`, `01_Architecture.md`, `02_CodingStandards.md`, `03_Roadmap.md`, `04_Progress.md`, `05_ChangeLog.md`, `06_DevelopmentRules.md`
- `Docs/PROJECT-BASELINE-v0.1.md` (شارتر، مخصوص، Gap Analysis، Decision Log، Risk Register)
- `Docs/decision-log.md` (DEC-001..DEC-017) و `Docs/open-questions.md` (Q-001..Q-008)
- `Docs/adr/` — ADR-001 تا ADR-009
- `Docs/domain/` — vision-and-problem, bounded-contexts, ubiquitous-language, aggregates-and-entities, domain-events
- `Docs/Phases/Phase00.md` + اسکلت `Phase01..08.md`
- `Docs/Architecture/` — context-map, container-diagram, iam-integration (پیش‌نویس)
- `Docs/api-contracts/README.md`
- `Docs/SessionReports/Session-20260914-1040.md`

### Changed
- (اسناد پایه موجود از نظر سازگاری با تصمیمات بررسی شدند)

### Fixed
- 

### Removed
- 

### Decisions / ADRs
- DEC-001 تا DEC-017 — مرز IAM، Frontend/Backend، Multi-tenancy، Region، Post Tree، Employee، PII، REST-only، Import
- ADR-001 تا ADR-009 (معماری/دامنه)