# Decision Log — ثبت تصمیم‌های پروژه

> این فایل مرجع زنده تمام تصمیم‌های قطعی پروژه است.
> هر تصمیم مهم معماری باید به ADR تبدیل شود.
> تصمیم‌های گرفته‌نشده یا نیازمند بررسی انسانی در `Docs/open-questions.md` ثبت می‌شوند.

**آخرین به‌روزرسانی:** `2026-09-14`

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

---

## تصمیم‌های در انتظار (Open)

| شناسه   | موضوع | وابسته به |
|---------|-------|-----------|
| —       | (در حال حاضر موردی در انتظار نیست) | — |
| —       | Schema نهایی جدول واسط | `open-questions.md` Q-003 |

---

**قانون:** هیچ تصمیمی بدون ثبت در این فایل و در صورت لزوم ADR، مبنای پیاده‌سازی نیست.