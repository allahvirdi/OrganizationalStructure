# Bounded Contexts

**آخرین به‌روزرسانی:** `2026-09-15`

---

## BC-1 — Organization Structure (هسته)

مدیریت پست‌های سازمانی و روابط سلسله‌مراتبی آن‌ها.

- **Aggregate Root:** `Post`
- **مسئولیت:** تعریف Post، Parent/Child، مشاهده چندسطحی (Subtree/Descendants)، جابجایی درون درخت، فعال/غیرفعال کردن.
- **وابستگی خارجی:** `OrganizationId` (Reference به IAM) — فقط ظرف، بدون Master موازی.

## BC-2 — Employee & Assignment

اطلاعات کامل پرسنلی و انتساب کارکنان به پست‌ها.

- **Aggregate Root:** `Employee`
- **مسئولیت:** ثبت/ویرایش/غیرفعال‌سازی پرسنل، انتساب یک یا چند پست (`EmployeePostAssignment`)، وضعیت پرسنل.
- **رابطه:** `UserId` به IAM Reference (اختیاری در صورت وجود حساب).
- **پشتیبانی از Import** (CSV/Excel/Staging) در سطح سرویس.

## BC-3 — Responsibility (مسئولیت عملیاتی)

مسئولیت‌های عملیاتی/وظیفه‌ای برای Business Routing و Automation.

- **Aggregate Root:** `Responsibility` (با `PostResponsibilityAssignment` به‌عنوان عضو)
- **مسئولیت:** تعریف مسئولیت با Code، انتساب Code-based به Post در Scope سازمانی، چرخه حیات تاریخ‌دار.
- **اصل:** Responsibility یک IAM Role نیست و جایگزین Post نمی‌شود (ADR-011).

## BC-4 — Authority (اختیار سازمانی)

اختیارهای سازمانی/امضایی پست‌ها.

- **Aggregate Root:** `Authority` (با `PostAuthorityAssignment` به‌عنوان عضو)
- **مسئولیت:** تعریف اختیار با Code، انتساب به Post، نمایش متمایز صاحب‌امضا در چارت.
- **اصل:** از Responsibility جداست و به Signing محدود نیست (ADR-011).

## BC-5 — Access & Visibility

کنترل دسترسی مبتنی بر Role/Permission/Organization Scope.

- **مسئولیت:** Map کردن Claimهای IAM به Policy داخلی، اعمال Organization Scope، قواعد Visibility (ستاد/استان/منطقه + مشاهده سلسله‌مراتبی).
- **اصل کلیدی:** رابطه سازمانی کاربر به‌تنهایی مجوز نیست.

## BC-6 — Import (MVP Secondary)

ورود اطلاعات پرسنلی از CSV، Excel و جدول واسط.

- **مسئولیت:** Batch بندی، اعتبارسنجی، مدیریت خطا و Duplicate، Audit.
- **دسترسی:** فقط نقش‌های مجاز ستادی.

## BC-7 — Integration API (سرویس‌دهی)

ارائه اطلاعات معتبر به سامانه‌های مصرف‌کننده.

- **مسئولیت:** Endpointهای REST نسخه‌بندی‌شده (`/api/v1/...`) برای Organization، Post، Parent/Child/Subtree، Employee، Assignment، Responsibility، Signing Authority، وضعیت فعال/غیرفعال.

---

## Context Map (خلاصه)

```
Business: HR → (consumes) ← OrganizationalStructure Core → (consumes Organization/Identity) ← IAM
Business: BPMS → (consumes) ← OrganizationalStructure Core
```

- OrgStructure از IAM: Organization/User Identity (Reference).
- OrgStructure به سامانه‌های مصرف‌کننده: فقط REST (MVP).