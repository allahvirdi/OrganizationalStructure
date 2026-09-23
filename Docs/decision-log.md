# Decision Log — ثبت تصمیم‌های پروژه

> این فایل مرجع زنده تمام تصمیم‌های قطعی پروژه است.
> هر تصمیم مهم معماری باید به ADR تبدیل شود.
> تصمیم‌های گرفته‌نشده یا نیازمند بررسی انسانی در `Docs/open-questions.md` ثبت می‌شوند.

**آخرین به‌روزرسانی:** `2026-09-23`

---

## تصمیم‌های قطعی

| شناسه   | تاریخ       | تصمیم                                      | وضعیت    | ADR مرتبط | تصمیم‌گیرنده |
|---------|-------------|--------------------------------------------|----------|-----------|--------------|
| DEC-001 | 2026-09-14 | پذیرش Architecture Baseline v2.4 (Stack منجمد) | Accepted | ADR-001 | کارفرما |
| DEC-002 | 2026-09-14 | پذیرش AI Development Path v2.4 (تایم‌باکس + کامیت پس از تأیید) | Accepted | — | کارفرما |
| DEC-003 | 2026-09-14 | Frontend: Next.js (App Router) + TypeScript + TanStack Query + React Hook Form + Zod + MUI (RTL) + Tailwind طبق Baseline | Accepted | ADR-001 | کارفرما |
| DEC-004 | 2026-09-14 | Backend: ۴ پروژه لایه‌ای طبق Baseline §۳.۲ (Domain/Application/Infrastructure/API) | Accepted | ADR-001 | کارفرما |
| DEC-005 | 2026-09-14 | Multi-tenancy لازم است (`TenantId` روی جداول اصلی) | Accepted | ADR-005 | کارفرما |
| DEC-006 | 2026-09-14 | افزودن `Region=3` به `OrganizationType` در IAM (با هماهنگی مالک IAM) | Accepted | ADR-009 (pending) | کارفرما |
| DEC-007 | 2026-09-14 | هر Organization درخت Post مستقل دارد؛ جابجایی Post بین Organizationها ممنوع است | Accepted | ADR-004 | کارفرما |
| DEC-008 | 2026-09-14 | Permissionهای دامنه‌ای در OrgStructure تعریف و در IAM ثبت/تخصیص می‌شوند | Accepted | ADR-008 | کارفرما |
| DEC-009 | 2026-09-14 | حفاظت PII با الگوی Always Encrypted (الگوی ADR-004 پروژه خواهر) | Accepted | ADR-006 | کارفرما |
| DEC-010 | 2026-09-14 | Integration در MVP فقط REST؛ RabbitMQ/Outbox/Event-driven و LDAP/AD Deferred | Accepted | ADR-007 | کارفرما |
| DEC-011 | 2026-09-14 | Organization و User Identity، Master در IAM است؛ OrgStructure فقط Reference می‌کند | Accepted | ADR-002 | کارفرما |
| DEC-012 | 2026-09-14 | Employee موجودیت مستقل و مالک کامل داده پرسنلی در OrgStructure است؛ `UserId` فقط Reference (بدون FK) | Accepted | ADR-003 | کارفرما |
| DEC-013 | 2026-09-14 | IAM Organization/OrganizationType فعلی Source of Truth است و OrgStructure آن را تغییر نمی‌دهد | Accepted | ADR-002 | کارفرما |
| DEC-014 | 2026-09-14 | SampleAdminPanel فقط مرجع بصری/UX است، نه Domain/Architecture؛ آیتم‌های Product/Order حذف می‌شوند | Accepted | ADR-001 | کارفرما |
| DEC-015 | 2026-09-14 | تفکیک MVP Core / MVP Secondary / Deferred | Accepted | — | کارفرما |
| DEC-016 | 2026-09-14 | Schema جدول واسط Import ابتدا طراحی پیشنهادی می‌شود؛ پیاده‌سازی پس از تأیید Business | Accepted | — | کارفرما |
| DEC-017 | 2026-09-14 | تغییرات لازم در کد IAM فقط با تأیید صریح کارفرما مجاز است | Accepted | ADR-002 | کارفرما |
| DEC-018 | 2026-09-14 | Q-002: پروتکل اتصال IAM با الگوی BFF (HttpOnly session cookie + اعتبارسنجی سرورside، fail-closed) | Accepted | ADR-002 / `Docs/Architecture/iam-integration.md` | کارفرما |
| DEC-019 | 2026-09-14 | Q-001: مجوز افزودن `Region=3` به `OrganizationType` در IAM (مقادیر ۰–۲ بدون تغییر)؛ پیاده‌سازی در مخزن IAM به‌صورت تسک مستقل | Accepted | ADR-009 | کارفرما |
| DEC-020 | 2026-09-14 | Q-004: IAM مرجع Role و Policy/Permission است؛ OrgStructure هیچ Authorization Master موازی نمی‌سازد و فقط مصرف + enforce می‌کند | Accepted | ADR-008 | کارفرما |
| DEC-021 | 2026-09-14 | Q-006 (مالکیت): تمام Roleها و Permissionها در IAM تعریف می‌شوند؛ `Post.*`/`Employee.*`/`Authority.*` فعلاً الگوی دسته‌بندی‌اند نه نام نهایی | Accepted | ADR-008 | کارفرما |
| DEC-022 | 2026-09-14 | Q-005: اطلاعات پرسنلی هرگز حذف فیزیکی نمی‌شوند؛ پایان همکاری/بازنشستگی/فوت/استعفا = غیرفعال‌سازی Employee با حفظ سابقه؛ بدون Anonymization در MVP (Archive آینده حذف نیست) | Accepted | — | کارفرما |
| DEC-023 | 2026-09-14 | Q-006: کاتالوگ پیشنهادی ۲۰ Permission دامنه‌ای ثبت شد (وضعیت Proposed/Pending Approval)؛ ثبت در IAM فقط پس از تأیید کارفرما + مالک IAM؛ بدون Role/Policy موازی در OrgStructure | Proposed / Pending Approval | ADR-008 | کارفرما |
| DEC-024 | 2026-09-15 | Domain Decision تفکیک ۴ مفهوم (IAM Role ≠ Post ≠ Responsibility ≠ Authority)؛ Responsibility و Authority موجودیت مستقل با Assignment تاریخ‌دار و Scope سازمانی؛ حذف `Post.HasSigningAuthority` و Responsibility VO؛ Breaking داخلی API (بدون Consumer خارجی)؛ `FindResponsible` در MVP پیاده‌سازی نمی‌شود؛ بدون Seed تا تأیید Business Catalog | Accepted | ADR-011 | کارفرما |
| DEC-025 | 2026-09-15 | Q-006: نام نهایی ۲۷ Permission (۷ پست + ۸ پرسنل + ۶ اختیار + ۶ مسئولیت، قالب سه‌بخشی `OrganizationStructure.{Resource}.{Action}`) مصوب کارفرما شد؛ ثبت در IAM اقدام مالک IAM است | Accepted | ADR-008 | کارفرما |
| DEC-026 | 2026-09-15 | تمدید Deadline به دو ماه (MVP تا `2026-11-14`) + تأیید برنامه اجرایی (Phase 5 کوتاه، Phase 6 اولویت، Phase 7 پس از Q-003، Phase 8 پایانی) | Accepted | — | کارفرما |
| DEC-027 | 2026-09-15 | احراز هویت سیستمی مصرف‌کننده‌ها با Bearer JWT معتبر IAM در کنار کوکی BFF (PolicyScheme هوشمند) | Accepted | ADR-012 | کارفرما |
| DEC-028 | 2026-09-17 | Q-009: تأمین Master Data سازمان محیط توسعه با تأیید کارفرما فقط از طریق API خود IAM (بدون تغییر کد/الگوی داده IAM): ثبت سازمان ریشه توسعه (`herasat-dev`) و تخصیص `OrganizationId` به کاربران Seed؛ IAM مرجع Master باقی می‌ماند (سازگار با DEC-011/DEC-013/DEC-017) | Accepted | — | کارفرما |
| DEC-029 | 2026-09-20 | ADR-013 (پیشنهادی — نیازمند تأیید کارفرما): `PezhvakMobile` در **ویرایش تکمیلی** اجباری می‌شود و فیلد سه‌حالتیِ غیر PII `PezhvakIsActive` (`true`/`false`/`null`) به مدل پرسنل افزوده می‌شود؛ در **ثبت اولیه** هر دو اختیاری می‌مانند تا مسیر Import (Q-003) و رکوردهای موجود نشکنند. `null` یعنی «هنوز تعیین نشده» و با `false` (غیرفعال) تفاوت دارد تا داده تاریخی جعل نشود. | Proposed / Pending Approval | ADR-013 | کارفرما |
| DEC-030 | 2026-09-21 | Q-003: طراحی نهایی Import از جدول واسط در `Docs/Architecture/staging-import-proposal.md` — سه جدول `ImportBatches`/`EmployeeStagingRows`/`ImportErrors` (بدون `TenantId` و بدون `PezhvakMobile`)، فرآیند یکسان بازبینی/تأیید برای هر دو منشأ (فایل + درج بیرونی)، مجوز موجود `OrganizationStructure.Employee.Import`، **ثبت معتبرها + اعلام نامعتبرها** (نه همه-یا-هیچ)، پاک‌سازی ۱۰ روزه، حذف فیزیکی تأییدشده، فرمت تاریخ بیرونی میلادی. | Accepted | — | کارفرما |
| DEC-031 | 2026-09-23 | Q-007: تفکیک نهایی مسئولیت داده پرسنلی — (۱) IAM فقط احراز هویت و دسترسی؛ نام/کد ملی در IAM مدیریت نمی‌شود؛ (۲) OrgStructure مالک کامل داده پرسنلی؛ (۳) غیرفعال‌سازی پرسنل در IAM باید خودکار در OrgStructure منعکس شود؛ (۴) Import آینده (مثلاً از HR System) مستقیم به OrgStructure می‌آید، نه از طریق IAM. | Accepted | — | کارفرما |
| DEC-032 | 2026-09-23 | Q-008: فقط `Organization` تخت سازمانی مبنای Scope است؛ `GeographicUnit` (استان/منطقه) از IAM مصرف نمی‌شود و در آینده حذف خواهد شد. | Accepted | — | کارفرما |
| DEC-033 | 2026-09-23 | Q-010: قرارداد خطای احراز هویت فعلاً همان `401` با پیام گویا باقی می‌ماند؛ تفکیک خطای اعتبارنامه کاربر از خطای وابستگی IAM با کدهای `502`/`503` لازم نیست. | Accepted | — | کارفرما + Tech Lead |
| DEC-034 | 2026-09-23 | دیالوگ انتساب مسئولیت به پست: شناسه خام پست حذف و با دو دراپ‌داون جایگزین شد — (۱) انتخاب سازمان با دسترسی درختی (ستاد→استان‌ها، استان→مناطق، منطقه→فقط خودش)؛ (۲) جستجوی پست در سازمان انتخاب‌شده. اسکوپ سازمانی توسط `GET /api/v1/organizations` در بک‌اند اعمال می‌شود. | Accepted | — | کارفرما |
| DEC-035 | 2026-09-23 | ماژول Authorities در UI با نام **«حق امضا»** ارائه می‌شود («ثبت اختیار» → «ثبت حق امضا»)؛ فیلد `Code` آن به مانند مسئولیت از ورودی کاربر حذف و به‌صورت خودکار معادل `Id` (GUID) تولید می‌شود و در فرانت‌اند نمایش داده نمی‌شود. دیالوگ انتساب به پست دقیقاً مانند مسئولیت شد (دراپ‌داون جستجوپذیر سازمان + پست). در نتیجه کد ثابت `SIGNING_AUTHORITY` بی‌اثر شد و نشان «صاحب امضا» (`hasSigningAuthority`) به «وجود حداقل یک انتساب جاری حق امضا» تغییر یافت. شناسه‌های فنی (مسیر `/api/v1/authorities`، موجودیت `Authority` و Permissionهای `OrganizationStructure.Authority.*`) دست‌نخورده ماندند تا ثبت‌نام Permission در IAM نشکند. | Accepted | ADR-011 (به‌روزرسانی) | کارفرما |

---

## تصمیم‌های در انتظار (Open)

| شناسه   | موضوع | وابسته به |
|---------|-------|-----------|
| —       | (در حال حاضر موردی در انتظار نیست — همه سؤالات بسته شده‌اند) | — |

---

**قانون:** هیچ تصمیمی بدون ثبت در این فایل و در صورت لزوم ADR، مبنای پیاده‌سازی نیست.