# 02 — Coding Standards

> این سند استانداردهای کدنویسی اجباری پروژه است.  
> تمام قوانین زیر در هر Session باید رعایت شوند.

**آخرین به‌روزرسانی:** `[YYYY-MM-DD]`

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
/// ایجاد کارمند جدید در سیستم.
/// </summary>
/// <param name="command">دستور ایجاد کارمند</param>
/// <returns>شناسه کارمند ایجادشده</returns>
```

## ۳. ممنوعیت‌ها (هرگز تولید نکن)
- `TODO`
- `Mock` / `Fake` (مگر داخل پروژه تست)
- Sample Code / Incomplete Code
- Comment اضافی و غیرمستند
- استفاده مستقیم از `DateTime.Now` / `DateTime.UtcNow` (باید از Clock Abstraction استفاده شود)

## ۴. نام‌گذاری
- مطابق Naming Convention تعریف‌شده در Architecture Baseline
- نام‌های معنادار و به زبان انگلیسی (کد) + توضیحات فارسی (XML)

## ۵. Validation و Mapping
- FluentValidation برای تمام Command/Queryها
- Mapster برای Mapping

## ۶. امنیت در کد
- Tenant Isolation
- Claim Validation
- عدم وجود Secret در کد
- جلوگیری از IDOR، Path Traversal، SQL Injection و ...

## ۷. تست
- هر قابلیت جدید: Unit Test + Integration Test
- Playwright در صورت نیاز

## ۸. ارجاع به قوانین Session
- قانون تایم‌باکس ۱۵ دقیقه‌ای و کامیت فقط بعد از تأیید انسان در `06_DevelopmentRules.md` و سند مسیر توسعه الزامی است.

---

**این سند اولویت بالایی در حل تناقض دارد.**
