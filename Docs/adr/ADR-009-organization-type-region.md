# ADR-009: افزودن Region=3 به OrganizationType در IAM

- **وضعیت:** Accepted (مجوز کارفرما صادر شد — DEC-019؛ پیاده‌سازی در مخزن IAM به‌صورت تسک مستقل)
- **تاریخ:** 2026-09-14
- **نویسنده:** Technical Lead / Architecture (تصمیم کارفرما؛ اجرا توسط مالک IAM)

## Context
تعریف کسب‌وکاری تأییدشده چهار سطح سازمانی است:

```text
0 = وزارت
1 = حوزه‌های ستادی وزارت
2 = استان
3 = منطقه
```

اما `OrganizationType` فعلی در IAM فقط سه مقدار دارد:

```csharp
public enum OrganizationType
{
    Organization = 0,
    Department = 1,
    Unit = 2
}
```

کارفرما ترجیح داد سطح «منطقه» به‌عنوان `Region = 3` افزوده شود تا نگاشت چهار سطح ممکن گردد. `Organization` در IAM Source of Truth است و OrgStructure آن را تغییر نمیدهد؛ بنابراین افزودن باید توسط مالک IAM انجام شود.

## Decision
- `Region = 3` به `OrganizationType` در `Enterprise-IAM-V2` **افزوده میشود** (با حفظ مقادیر ۰ تا ۲ بدون تغییر برای سازگاری).
- نام‌گذاری موجود Enum بدون بررسی و هماهنگی مالک IAM تغییر نمیکند.
- OrgStructure چهار سطح را از طریق مقادیر عددی نگاشت میکند و به نام‌های فنی Enum وابسته نیست.
- تا زمان اجرای این تغییر، OrgStructure فقط با مقادیر موجود کار میکند و «منطقه» نگاشت نمیشود.

## Rationale
- تأمین نگاشت چهار سطح کسب‌وکاری با حداقل تغییر و بدون شکستن سازگاری.
- حفظ مرز مالکیت IAM.

## Considered and rejected alternatives
- تغییر نام‌های Enum فعلی: رد شد — ریسک شکستن سازگاری و خارج از مالکیت OrgStructure.
- عدم افزودن Region و نگاشت «منطقه» به `Unit`: رد شد — نادرست و مبهم برای کسب‌وکار.
- ساخت Enum موازی در OrgStructure: رد شد — مغایر مرز Master Data (ADR-002).

## Consequences
- مثبت: نگاشت شفاف چهار سطح سازمانی.
- منفی: وابستگی به زمان‌بندی تغییر در IAM؛ تا آن زمان یک محدودیت موقت باقی می‌ماند.
- نیازمند: هماهنگی و تأیید مالک IAM (Q-001).

**منابع:** `Docs/PROJECT-BASELINE-v0.1.md` (DEC-006)، `EnterpriseIAM.Domain/Enums/OrganizationType.cs`، `Docs/open-questions.md` (Q-001)