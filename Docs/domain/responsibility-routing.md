# Organizational Responsibility & Routing

**آخرین به‌روزرسانی:** `2026-09-15`
**وضعیت:** Accepted (DEC-024 / ADR-011)

---

## ۱. تفکیک چهار مفهوم

| Concept | Meaning | پاسخ به سؤال |
|---|---|---|
| IAM Role | سطح نقش/دسترسی امنیتی (متعلق به IAM) | کاربر چه مجوزی دارد؟ |
| Post | جایگاه فرد در ساختار سازمانی | فرد کجای ساختار است؟ |
| Responsibility | مسئولیت عملیاتی برای Routing/Automation | چه جایگاهی مسئول این وظیفه است؟ |
| Authority | اختیار سازمانی/امضایی | چه جایگاهی این اختیار را دارد؟ |

**اصل:** `IAM Role ≠ Post ≠ Responsibility ≠ Authority`

- Responsibility یک Role امنیتی IAM نیست و جایگزین Post نمی‌شود.
- Authority از Responsibility جداست و به Signing محدود نیست.

## ۲. چرا Responsibility مستقل است؟

سیستم‌های مصرف‌کننده (مثل مکاتبات) نباید به `UserId` ثابت وابسته باشند:

```text
❌ Letter -> UserId = 125   (با جابجایی/بازنشستگی فرد می‌شکند)
```

بلکه بر اساس مسئولیت سازمانی Routing می‌کنند:

```text
✅ Letter
     ↓
   Organization
     ↓
   Responsibility = SECRETARIAT
     ↓
   PostResponsibilityAssignment (جاری)
     ↓
   Post
     ↓
   Active Employee
     ↓
   IAM User
```

مسئولیت به **Post** متصل می‌شود، نه به User؛ با تغییر شاغل، مسئولیت برای فرد جدید برقرار می‌ماند. بنابراین Responsibility یک **Business Routing Key** است.

## ۳. مدل ارتباطی

```text
IAM
 ├── User / Identity
 ├── Organization
 ├── Role
 ├── Policy
 └── Permission

OrganizationalStructure
 ├── Employee ── EmployeePostAssignment ──▶ Post
 │                                            │
 │              ┌── PostResponsibilityAssignment ── Responsibility (Code)
 │              │
 │              └── PostAuthorityAssignment ── Authority (Code)
 │
 └── (Employee ── UserId ──▶ IAM User)
```

- هر Assignment دارای `OrganizationId` (Scope)، `StartDate`/`EndDate` و `IsActive` است.
- پایان Assignment با تاریخ و غیرفعال‌سازی است، نه حذف فیزیکی.
- یک Code مسئولیت/اختیار می‌تواند در چند سازمان به پست‌های مختلف منتسب شود.

## ۴. Resolution آینده (خارج از MVP)

```text
FindResponsible(OrganizationId, ResponsibilityCode)
  → Responsibility → Assignment جاری → Post → Active Employee → IAM User
```

مدل فعلی از این Resolution پشتیبانی می‌کند ولی Endpoint آن در MVP پیاده‌سازی نمی‌شود.

## ۵. کدهای پیشنهادی (بدون Seed)

`SECRETARIAT_RESPONSIBLE`، `NEWSROOM_RESPONSIBLE`، `COLLECTION_RESPONSIBLE`، `REVIEW_RESPONSIBLE`، `RESPONSE_RESPONSIBLE`، `APPROVAL_RESPONSIBLE` و `SIGNING_AUTHORITY` صرفاً پیشنهادند؛ Seed پس از تأیید Business Catalog.

---

**منابع:** ADR-011، `Docs/decision-log.md` (DEC-024)، `Docs/domain/permission-catalog.md`