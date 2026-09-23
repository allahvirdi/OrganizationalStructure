# 04 — Progress

> این فایل باید در **پایان هر Session** به‌روز شود.
> هدف: امکان ادامه کار توسط AI یا توسعه‌دهنده جدید بدون نیاز به تاریخچه گفتگو.

**آخرین به‌روزرسانی:** `2026-09-23`
**Session مربوطه:** `Session-20260923-Phase8-ProductionReadiness`

## ادامه — خودکارسازی کد مسئولیت + بهبود فرم انتساب (2026-09-23)
- فیلد `Code` مسئولیت از ورودی کاربر حذف شد؛ بک‌اند به‌صورت خودکار `Code = Guid` تولید می‌کند (کامیت `59cfcb0`).
- دیالوگ انتساب مسئولیت به پست بازطراحی شد: شناسه خام پست حذف و با دو `Autocomplete` جایگزین شد — انتخاب سازمان (دسترسی درختی) + جستجوی پست در سازمان انتخاب‌شده.
- مستند `Docs/api-contracts/responsibilities.md` برای حذف فیلد `code` از بدنه `POST` به‌روز شد.
- اعتبارسنجی: `tsc --noEmit` ✅ · `eslint` ✅ · `next build` ✅ (۱۵ روت).

## Phase 8 — Production Readiness (2026-09-23) — ✅ Done
- **تسک ۱:** رفع آسیب‌پذیری NU1903 (Microsoft.OpenApi 2.12.0).
- **تسک ۲:** `RedisBffSessionStore` با `StackExchange.Redis` — انتخاب خودکار بر اساس `ConnectionStrings:Redis`؛ در نبود آن، `InMemoryBffSessionStore` برای توسعه.
- **تسک ۳:** پایپ‌لاین CI/CD با GitHub Actions (`.github/workflows/ci.yml`): بیلد .NET 10 + ۵ پروژه تست + اسکن آسیب‌پذیری (ویندوز، به‌دلیل LocalDB)؛ فرانت‌اند: `npm ci` + `lint` + `build` + `npm audit` (اوبونتو).
- **تسک ۴:** Rate Limiting سراسری (Fixed Window, 100 req/min/IP)؛ `SecurityHeadersMiddleware` (OWASP: X-Content-Type-Options, X-Frame-Options, CSP, Referrer-Policy, Permissions-Policy)؛ حذف هدر `Server` Kestrel؛ اسکن وابستگی‌ها به‌صورت فیلتر قطعی در CI.
- **تسک ۵:** مستندات استقرار و Runbook (`Docs/Deployment/deployment-runbook.md`).
- **تسک ۶:** گزارش نهایی + بستن فاز (`Docs/SessionReports/Session-20260923-Phase8-ProductionReadiness.md`).
- اعتبارسنجی: ۲۴۷ تست سبز (Domain ۳۷ + Application ۹۸ + Infrastructure ۲۷ + Architecture ۴ + Integration ۸۱)؛ بیلد بدون هشدار/خطا.
- کامیت‌ها: `26dbdcc`, `b749191`, `b2350c5`, `ad38ef2`, `a621b48` — همه پوش‌شده به `origin/main`.

## Phase 7 — ورود دسته‌جمعی پرسنل از اکسل/CSV (2026-09-20)
- نیاز: مسیر فایل (نه جدول واسط) برای ثبت دسته‌جمعی پرسنل یک سازمان، مستقل از Q-003 که هنوز باز است.
- Endpoint: `POST /api/v1/import/employees` (multipart: `organizationId` + `file` با پسوند `.xlsx` یا `.csv`) → `201` با `{organizationId, importedCount}`؛ `GET /api/v1/import/employees/template?format=xlsx|csv` برای قالب فارسی.
- مجوز: `OrganizationStructure.Employee.Import` (مصوب DEC-025). تا ثبت در IAM همه درخواست‌ها ۴۰۳ می‌گیرند — Deny by Default حفظ شده. محدودیت سازمانی (`VisibleOrganizationIds`) در هندلر دوباره بررسی می‌شود.
- اتمیک بودن: هر خطای فایل/ردیف ⇒ هیچ پرسنلی ثبت نمی‌شود (هم‌راستا با مسیر پست‌ها). تکراری در فایل → ۴۰۰؛ تکراری در دیتابیس → ۴۰۹ `Employee.DuplicatePersonnelCode`.
- حفاظت PII: پیام‌های خطای کد ملی/شماره فقط «شماره ردیف + علت» دارند و مقدار حساس افشا نمی‌شود.
- بدون افزودن پکیج تازه: اکسل با ClosedXML موجود؛ CSV با پارسر دستی RFC 4180؛ تاریخ شمسی با `PersianCalendar` استاندارد دات‌نت (نه `jalaali-js` و نه کتابخانه جدید).
- نرمال‌سازی ورودی: `PersianDigitNormalizer` (ارقام فارسی/عربی، نیم‌فاصله، nbsp، جداکننده هزارگان) و `JalaliConverter` (شمسی → `DateOnly` میلادی).
- `PezhvakIsActive` در این مسیر ورودی ندارد و `null` می‌ماند (تعیین‌نشده) — تعیین آن فقط از ویرایش تکمیلی؛ سازگار با ADR-013 Pending.
- Frontend: صفحه `/import` دو تب شد؛ `MenuItem.anyOfPermissions` اضافه شد تا آیتم منو با OR دو مجوز دیده شود.
- اعتبارسنجی: Release سبز — Domain ۳۷ · Application ۷۱ · Infrastructure ۲۴ · Architecture ۴ · API Integration ۷۶ (۲۱۲ تست)؛ `tsc` + `eslint --max-warnings 0` + `next build` (۱۷ روت) تمیز؛ `git diff --check` بدون خطا.
- انجام‌نشده: اجرای Migration (در این Session Migration جدید لازم نبود)، تست HTTP زنده با IAM واقعی، آزمون تعاملی مرورگر، و مسیر Staging Table (مسدود به Q-003). هیچ کامیتی انجام نشد؛ درخت کاری شامل تغییرات معوق Sessionهای قبل نیز هست و هنگام کامیت باید تفکیک شود. گزارش: `Session-20260920-Phase7-EmployeeImport.md`.

## ادامه — وضعیت شبکه پژواک + اجباری‌بودن شماره پژواک (2026-09-20)
- نیاز: شماره ثبت‌شده در پژواک در **ویرایش تکمیلی** اجباری شود و وضعیت فعال بودن آن در شبکه پژواک ثبت/نمایش شود.
- مدل: ستون غیر PII `PezhvakIsActive` (`bool?`) با معنای سه‌حالتی (`true`/`false`/`null` = تعیین نشده)؛ رکوردهای موجود `null` می‌مانند تا داده تاریخی جعل نشود. در `POST` ثبت اولیه هیچ‌کدام اجباری نشد (سازگاری با Import/Q-003 و Seedها).
- قرارداد API: `PATCH /api/v1/employees/{id}/supplementary` حالا `pezhvakMobile` (فرمت `^09[0-9]{9}$`) و `pezhvakIsActive` (بولی) را اجباری می‌خواهد؛ `EmployeeDto` فیلد جدید را برمی‌گرداند.
- UI: تاریخ تولد فقط با `JalaliDatePicker` (خروجی ISO میلادی، ورودی متنی آزاد حذف) و وضعیت پژواک با `Select` اجباری پر می‌شود؛ هر دو از طریق `Controller` به React Hook Form متصل‌اند (نه `setValue` دستی).
- معماری/اسناد: `ADR-013` به‌صورت **Proposed — Pending Approval** نوشته شد و `DEC-029` در انتظار تأیید کارفرماست؛ `erd.md`، `01_Architecture.md`، `api-contracts/employees.md` و ChangeLog به‌روز شدند.
- Migration `20260920045347_AddEmployeePezhvakActiveFlag` ساخته شد (ستون + ایندکس `(TenantId, OrganizationId)` که از Session قبل در درخت کاری معوق بود).
- اعتبارسنجی: `tsc` و `eslint --max-warnings 0` (کل پروژه) و `next build` (۱۶ روت) تمیز؛ `dotnet build/test` در Release سبز (Domain ۳۷ / Infrastructure ۲۴ / Architecture ۴ / API Integration ۷۰ / Application سبز)؛ `git diff --check` تمیز.
- انجام‌نشده: آزمون تعاملی مرورگر با نشست واقعی IAM، اجرای Migration روی SQL Server، و تست HTTP زندهٔ `PATCH` با بدنهٔ ناقص (فقط شواهد تست واحد/یکپارچگی با `TestAuthHandler`). هیچ کامیتی انجام نشد؛ درخت کاری شامل تغییرات معوق Sessionهای قبل (محدوده سازمانی/تم/مجوز) نیز هست و هنگام کامیت باید تفکیک شود. گزارش: `Session-20260920-Phase6-PezhvakStatus.md`.

## ادامه — نام سازمان در UI و محدوده سازمانی (2026-09-18)
- ادعای «سازمان فقط با شناسه خام نمایش داده می‌شود» باطل شد: Endpoint جدید `GET /api/v1/organizations` نام/کد/والد/عمق سازمان‌های مجاز را برمی‌گرداند و فرم ایجاد پست سازمان را با **نام** (جستجوپذیر روی نام/کد و تودرتو با عمق) نشان می‌دهد؛ پیش‌فرض «سازمان خود کاربر».
- این Endpoint فقط **احراز هویت** می‌خواهد (Permission دامنه‌ای ندارد) چون پاسخ به Scope کاربر محدود است؛ همه نقش‌های احرازشده پاسخ یکسان می‌گیرند و Fallback Deny حفظ شده است.
- Scope مشاهده به‌صورت یکسان «خود سازمان + تمام زیرمجموعه‌ها» محاسبه می‌شود (`OrganizationScope.ComputeVisibleOrganizations`) و در نشست هم به‌صورت شناسه (`organization_scope`) و هم مرجع کامل با نام/کد (`organization_scope_node` — JSON) نگهداری می‌شود تا UI نام داشته باشد.
- حل Scope در `OrganizationScopeResolver` مشترک هندلر BFF و Bearer شده است؛ هر دو مسیر احراز هویت یک Scope تولید می‌کنند و رفتار fail-closed (نبود سازمان کاربر در درخت IAM ⇒ Scope خالی و رد عملیات) حفظ شده است.
- اعتبارسنجی: `tsc --noEmit`، `npm run lint`، `npm run build` موفق؛ Release: Domain ۳۵، Application ۳۹، Infrastructure ۲۴، Architecture ۴، API Integration ۶۶ — همه سبز (۱۶۸ تست).
- تست تعاملی مرورگر و آزمون با IAM واقعی انجام نشده است؛ تغییرات این ادامه کامیت نشده‌اند. گزارش: `Session-20260918-Phase6-OrganizationScope.md`.
- قرارداد API ثبت شد: `Docs/api-contracts/organizations.md` + به‌روزرسانی `posts.md` (کد ۴۰۳ برای سازمان خارج از محدوده) و `Docs/Architecture/iam-integration.md`.


## ادامه — تعریف پست و انتخاب والد (2026-09-17)
- سازمان نشست به‌صورت شناسهٔ فقط‌خواندنی در فرم ایجاد نمایش داده می‌شود؛ نبود سازمان مانع ثبت است. نام سازمان در قرارداد فعلی نشست موجود نیست.
- انتخابگر جستجوی کد/عنوان والد با صفحه‌بندی، محدود به سازمان نشست، به POST موجود متصل شد؛ بدون والد یعنی ریشه.
- SystemAdmin و OrganizationStructureAdmin می‌توانند ایجاد و مشاهدهٔ پست داشته باشند؛ مجوزهای صریح و کنترل محدودهٔ سازمان حفظ شدند.
- TypeScript، lint، build و diff --check موفق؛ ۵۸ تست API و ۳۲ تست Application در Release موفق. تست تعاملی فرم و انتشار/راه‌اندازی مجدد API انجام نشده است.
- تغییرات این درخواست کامیت نشده‌اند؛ گزارش: Session-20260917-Phase6-PostCreation.md.


## بازبینی قالب و لندینگ
- صفحه `/` فرم ورود است؛ نشست معتبر به داشبورد هدایت می‌شود. `/login` به `/` هدایت می‌شود.
- تم با پالت و سطوح SampleAdminPanel همسو شد؛ منوی صفحات و دسترسی سریع داشبورد متصل است.
- TypeScript، lint و production build موفق. رگرسیون CDP روی ۱۰ مسیر × ۴ عرض اجرا شد؛ عرض سند در هیچ حالت از viewport بیشتر نبود. جدول پرسنل و span برچسب مسئولیت هنوز در بررسی عنصرمحور علامت می‌خورند و نیاز به بررسی clipping دارند.
- سرریز داشبورد با ستون‌های minmax(0, …) و دسترسی سریع تک‌ستونی در موبایل رفع شد؛ در ۳۲۰px مقدار clientWidth و scrollWidth هر دو ۳۱۴ است.
- منوی موبایل سمت راست، عرض ۲۷۲px؛ چهار API خواندن با نشست فعلی پاسخ ۲۰۰ دادند. نبود permission claim در این اجرای زنده مستقلاً اثبات نشده است.
- تطابق کامل قالب تأیید نیست: ساختار hero، کارت‌های شاخص، نمودار و ابزارهای هدر با مرجع متفاوت است. تم تیره نیز هنوز نیاز به اصلاح/اعتبارسنجی دارد.
- هیچ کامیتی در این ادامه انجام نشد؛ رفع ایرادهای باقی‌مانده و تأیید بصری هنوز لازم است. فایل داشبورد شامل بازنویسی قبلیِ کامیت‌نشده نیز هست؛ تغییر آن نسبت به HEAD فقط اصلاح سه ستون نیست.

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
