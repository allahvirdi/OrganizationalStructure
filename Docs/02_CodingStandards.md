# 02 — Coding Standards (استانداردهای کدنویسی)

> این سند استانداردهای کدنویسی اجباری پروژه است.
> تمام قوانین زیر در هر Session باید رعایت شوند.

**آخرین به‌روزرسانی:** `2026-09-14`

---

## ۱. اصول کلی
- Production Ready
- Clean Architecture + Modular Monolith
- SOLID / DRY / KISS
- CQRS + MediatR
- Dependency Injection

## ۲. مستندسازی کد (اجباری)
- تمام `Class`، `Property`، `Method` و `Interface` باید **XML Documentation به زبان فارسی** داشته باشند.
- مثال:
```csharp
/// <summary>
/// ایجاد پست سازمانی جدید در سیستم.
/// </summary>
/// <param name="command">دستور ایجاد پست سازمانی</param>
/// <returns>شناسه پست سازمانی ایجادشده</returns>
```

## ۳. ممنوعیت‌ها (هرگز تولید نکن)
- `TODO`
- `Mock` / `Fake` (مگر داخل پروژه تست)
- Sample Code / Incomplete Code
- Comment اضافی و غیرمستند
- استفاده مستقیم از `DateTime.Now` / `DateTime.UtcNow` (باید از Clock Abstraction استفاده شود)

## ۴. نام‌گذاری
- مطابق Naming Convention تعریف‌شده در Architecture Baseline
- نامهای معنادار و به زبان انگلیسی (کد) + توضیحات فارسی (XML)

## ۵. Validation و Mapping
- FluentValidation برای تمام Command/Queryها
- Mapster برای Mapping

## ۶. امنیت در کد
- Tenant Isolation
- Claim Validation
- عدم وجود Secret در کد
- جلوگیری از IDOR، Path Traversal، SQL Injection و ...
- عدم بازگشت/لاگ PII غیرضروری

## ۷. تست
- هر قابلیت جدید: Unit Test + Integration Test
- Playwright در صورت نیاز (E2E)

## ۸. ارجاع به قوانین Session
- قانون تایم‌باکس ۱۵ دقیقه‌ای و کامیت فقط بعد از تأیید انسان در `06_DevelopmentRules.md` و سند مسیر توسعه الزامی است.

## ۹. Frontend
- Next.js (App Router) + TypeScript
- TanStack Query برای Server State
- React Hook Form + Zod برای فرم/اعتبارسنجی
- MUI (RTL) + Tailwind برای UI
- Permission Guard برای صفحات و اکشن‌های حساس

---

**این سند اولویت بالایی در حل تناقض دارد.**