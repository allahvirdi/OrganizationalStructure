# مسیر توسعه از ۰ تا ۱۰۰ با هوش مصنوعی
## (Enterprise AI-Assisted Development Path)
### نسخه ۲.۴ — قالب عمومی و قابل استفاده مجدد

> این سند **ثابت** و مشترک برای تمام پروژه‌ها است.  
> هدف: توسعه کنترل‌شده، قابل تکرار، قابل ممیزی و کم‌ریسک با کمک هوش مصنوعی کدنویس و تیم انسانی.  
> این نسخه با تقویت‌های برگرفته از AI Master Prompt عملیاتی تکمیل شده است.

---

## ۱. نقش هوش مصنوعی

شما در نقش **معمار ارشد نرم‌افزار، توسعه‌دهنده ارشد .NET، کارشناس امنیت و Technical Lead** پروژه عمل می‌کنید.

- هیچ تصمیمی خارج از مستندات پروژه گرفته نشود.
- هرگز بر اساس حدس تصمیم نگیرید. اگر اطلاعات کافی نیست، سؤال بپرسید.
- هدف: توسعه کامل سامانه مطابق مستندات پروژه.

---

## ۲. ساختار اسناد عملیاتی اجباری (Docs)

در ریشه پروژه پوشه `Docs/` باید حداقل شامل موارد زیر باشد:

```
Docs/
├── 00_ProjectContext.md
├── 01_Architecture.md
├── 02_CodingStandards.md
├── 03_Roadmap.md
├── 04_Progress.md
├── 05_ChangeLog.md
├── 06_DevelopmentRules.md
├── PROJECT-BASELINE-v0.1.md
├── open-questions.md
├── decision-log.md
├── adr/                    # Architecture Decision Records
├── Architecture/           # دیاگرام‌ها و جزئیات معماری
├── Phases/                 # Phase00.md, Phase01.md, ...
├── api-contracts/
├── domain/
└── SessionReports/         # گزارش هر Session
```

---

## ۳. اولویت حل تناقض اسناد

اگر بین مستندات تناقض وجود داشت، اولویت به ترتیب زیر است:

1. `06_DevelopmentRules.md`
2. `03_Roadmap.md`
3. سند فاز مربوطه (`Phases/PhaseXX.md`)
4. Architecture Baseline / `01_Architecture.md`
5. ADRها
6. Session Reports

هرگز بر اساس حدس تصمیم نگیرید.

---

## Phase 0 — Project Baseline

**خروجی‌های اجباری:**

- [ ] شناخت کامل اسناد موجود
- [ ] تکمیل بخش Domain Vision و Problem Statement
- [ ] استخراج Bounded Contextهای اولیه
- [ ] ثبت ابهامات در `Docs/open-questions.md`
- [ ] ایجاد Decision Log
- [ ] ایجاد ساختار ریشه پروژه (`Docs` / `Backend` / `Frontend`)
- [ ] ایجاد فایل `Docs/PROJECT-BASELINE-v0.1.md`
- [ ] ایجاد اسکلت فایل‌های عملیاتی (`00` تا `06`، `Phases/`، `SessionReports/`)

---

## Phase 1 — Architecture Foundation (Freeze)

تصمیم‌های این فاز **Freeze** می‌شوند. هرگونه تغییر بعداً فقط از طریق ADR جدید مجاز است.

### Backend (Freeze)
- .NET 10 (LTS)
- Clean Architecture + Modular Monolith
- CQRS + MediatR
- FluentValidation
- Mapster
- EF Core + SQL Server
- Redis
- RabbitMQ + Outbox Pattern
- Hangfire (Background Jobs)
- Elasticsearch یا OpenSearch
- MinIO
- Serilog + OpenTelemetry

### Frontend (Freeze)
- Next.js (App Router) یا React (بر اساس تصمیم پروژه)
- TypeScript
- TanStack Query
- React Hook Form + Zod
- MUI (RTL) + Tailwind CSS
- **قالب ادمین: `SampleAdminPanel` (اجباری)** — Frontend باید بر اساس این قالب توسعه داده شود

### Authentication
- Identity Provider خارجی (از طریق OIDC / OpenIddict)
- این پروژه Identity Server را پیاده‌سازی نمی‌کند؛ فقط مصرف‌کننده است.

### سایر تصمیم‌های Freeze
- ساختار پوشه‌های ریشه و داخلی
- Naming Conventions
- Error Contract استاندارد
- استراتژی Audit و Soft Delete
- استراتژی Multi-tenancy
- استاندارد ستون‌های دیتابیس
- سیاست Versioning API

---

## Phase 2 تا Phase 7

(همان ساختار قبلی حفظ می‌شود: Domain Modeling → Backend Vertical Slice → Frontend → Workflow → Enterprise Features → Production Readiness)

**توجه:** هر فاز باید فایل مستقل `Docs/Phases/PhaseXX.md` داشته باشد که شامل اهداف، Deliverables، Definition of Done، وابستگی‌ها، ریسک‌ها و لیست کلاس‌ها/APIها باشد.

---

## ۴. پروتکل کار با هوش مصنوعی (AI Development Protocol) — نسخه ۲.۴

### ۴.۱ شروع هر Session (اجباری قبل از هر خط کد)

1. مطالعه فایل‌های زیر (در صورت وجود):
   - `Docs/00_ProjectContext.md`
   - `Docs/01_Architecture.md`
   - `Docs/02_CodingStandards.md`
   - `Docs/03_Roadmap.md`
   - `Docs/04_Progress.md`
   - `Docs/05_ChangeLog.md`
   - `Docs/06_DevelopmentRules.md`
   - `Docs/ADR/`
   - `Docs/Architecture/`
   - `Docs/SessionReports/` (آخرین گزارش‌ها)

2. شماره فازی که کاربر اعلام کرده را پیدا کن (فقط همان فاز).
3. فایل `Docs/Phases/PhaseXX.md` مربوطه را بخوان.
4. اگر `04_Progress.md` نشان می‌دهد بخشی از فاز قبلاً انجام شده، ابتدا ادامه همان قسمت را بده. هیچ فایل تکراری ایجاد نکن.

### ۴.۲ قبل از کدنویسی — استخراج اجباری

ابتدا این موارد را استخراج و اعلام کن:

- اهداف فاز
- Deliverables
- Definition of Done
- وابستگی‌ها و پیش‌نیازها
- ریسک‌ها
- کلاس‌ها / پروژه‌های موردنیاز
- APIها / DTOها
- Commands / Queries / Validators
- Repositoryها / Eventها / Background Jobs
- تست‌های موردنیاز

اگر هرکدام قبلاً پیاده‌سازی شده‌اند، دوباره تولید نکن.

### ۴.۳ تحلیل وابستگی

اگر فاز جاری به فاز قبلی وابسته است:
- بررسی کن زیرساخت آن وجود دارد یا خیر.
- اگر وجود ندارد، ابتدا همان زیرساخت را ایجاد کن، سپس ادامه بده.

### ۴.۴ قوانین تایم‌باکس و کامیت (اجباری)

#### قانون تایم‌باکس ۱۵ دقیقه‌ای
- هر تسک و هر زیر‌فاز حداکثر **۱۵ دقیقه**.
- تسک طولانی‌تر باید شکسته شود.
- قبل از شروع، لیست واحدهای ≤ ۱۵ دقیقه‌ای را ارائه بده.
- بعد از هر واحد، وضعیت را گزارش کن و منتظر دستور بمان.

#### قانون کامیت فقط بعد از تأیید صریح انسان
- هیچ کامیتی بدون تأیید صریح شما زده نمی‌شود.
- بعد از هر تسک: خلاصه تغییرات + لیست فایل‌ها + درخواست تأیید.
- فقط پس از تأیید صریح («تأیید شد» / «کامیت بزن») کامیت انجام شود.
- پیام کامیت باید واضح و شامل شناسه تسک باشد.

### ۴.۵ قوانین کیفیت کد (ممنوعیت‌ها)

هرگز تولید نکن:
- `TODO`
- `Mock` / `Fake` (مگر در تست)
- Sample Code
- Incomplete Code
- Comment اضافی و غیرمستند

### ۴.۶ مستندسازی کد

تمام Classها، Propertyها، Methodها و Interfaceها باید **XML Documentation به زبان فارسی** داشته باشند.

### ۴.۷ امنیت (اجباری)

همیشه رعایت شود:
- Authorization + Authentication
- Tenant Isolation
- Claim Validation
- OWASP Top 10
- XSS / CSRF / SQL Injection / IDOR / Path Traversal
- Secret Management
- HTTPS
- Data Protection

### ۴.۸ تست

هر قابلیت جدید باید دارای:
- Unit Test
- Integration Test

در صورت نیاز: Playwright (E2E).

### ۴.۹ قوانین توسعه دائمی

- Production Ready
- Clean Architecture + Modular Monolith
- SOLID / DRY / KISS
- CQRS + MediatR
- FluentValidation + Mapster
- Dependency Injection
- Provider Pattern / Strategy Pattern در صورت نیاز

---

## ۵. پایان Session (اجباری)

قبل از خاتمه هر Session حتماً:

1. `Docs/04_Progress.md` را به‌روز کن.
2. `Docs/05_ChangeLog.md` را به‌روز کن.
3. یک فایل جدید در `Docs/SessionReports/` ایجاد کن.

### ساختار اجباری Progress

- فاز جاری
- درصد پیشرفت
- فایل‌های ایجاد شده
- فایل‌های تغییر یافته
- فایل‌های باقی‌مانده
- قدم بعدی
- مشکلات
- تصمیمات

### ساختار Session Report

- خلاصه
- Deliverables
- کلاس‌های جدید
- APIها
- Migrationها
- تست‌ها
- مشکلات
- ریسک‌ها
- تصمیمات
- قدم بعد

### اگر Context یا Token تمام شد

قبل از توقف:
- Progress را ذخیره کن.
- Session Report را کامل کن.
- فایل‌های ناقص را مشخص کن.
- قدم بعدی را بنویس.
- هیچ اطلاعاتی فقط داخل حافظه مدل باقی نماند.

### قانون نهایی خاتمه Session

قبل از خاتمه بررسی کن:

> آیا ساختار پروژه، مستندات، Progress و Session Report به اندازه‌ای کامل هستند که یک AI دیگر یا یک توسعه‌دهنده جدید بتواند **بدون نیاز به تاریخچه گفتگو** دقیقاً از همان نقطه ادامه دهد؟

اگر پاسخ منفی است، ابتدا مستندات را کامل کن و سپس Session را خاتمه بده.

---

## ۶. قوانین توقف و Compact Session

- هیچ فاز جدیدی را خودسرانه شروع نکن.
- بعد از پایان فاز، منتظر دستور کاربر بمان.
- اگر حجم کار از ظرفیت Session بیشتر است، خودت آن را به چند Session تقسیم کن (Compact Session).
- در پایان هر Session: Progress ذخیره + Report تولید + قدم بعدی مشخص + منتظر ادامه.

---

## ۷. دستورهای مجاز کاربر

کاربر فقط یکی از دستورهای زیر را می‌دهد. فقط همان را اجرا کن:

- شروع فاز X
- ادامه فاز X
- ادامه آخرین Session
- Resume
- Continue

---

## ۸. Vertical Slice Acceptance Criteria (نهایی)

یک Feature فقط زمانی **Done** است که:

- [ ] کد کامل، تمیز و Production Ready باشد
- [ ] Unit + Integration Test نوشته و پاس شده باشد
- [ ] XML Documentation فارسی کامل باشد
- [ ] مستندات فاز و Progress به‌روز باشد
- [ ] Migration (در صورت نیاز) آماده باشد
- [ ] API Contract به‌روز باشد
- [ ] Security Checklist پاس شده باشد
- [ ] Fitness Functions نقض نشده باشند
- [ ] Review انسانی انجام شده باشد
- [ ] تمام تسک‌ها با قانون ۱۵ دقیقه‌ای اجرا و کامیت‌ها پس از تأیید شما ثبت شده باشند
- [ ] هیچ TODO / Incomplete / Sample Code وجود نداشته باشد

---

## ۹. مکانیزم‌های دائمی کیفیت

1. ADR واقعی برای هر تصمیم مهم
2. Architecture Fitness Functions در CI
3. AI Development Protocol کامل (تایم‌باکس + کامیت پس از تأیید + Session Report)
4. Contract Testing
5. Definition of Done سخت‌گیرانه
6. Open Questions Log زنده
7. Risk Register زنده
8. اولویت‌بندی اسناد در زمان تناقض
9. قابلیت ادامه توسط AI/توسعه‌دهنده جدید بدون تاریخچه گفتگو

---

## ۱۰. قدم‌های پیشنهادی بلافاصله

1. ایجاد ساختار ریشه و پوشه `Docs/` کامل
2. نوشتن `PROJECT-BASELINE-v0.1.md`
3. ایجاد اسکلت `00` تا `06` و `Phases/` و `SessionReports/`
4. تکمیل Domain Vision
5. ثبت ADRهای اولیه
6. شروع استخراج Domain Model

---

**پایان سند مسیر توسعه از ۰ تا ۱۰۰ با هوش مصنوعی — نسخه ۲.۴**
