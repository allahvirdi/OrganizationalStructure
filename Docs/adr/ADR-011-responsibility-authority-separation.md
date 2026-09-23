# ADR-011: تفکیک Responsibility و Authority به‌عنوان مفاهیم مستقل دامنه

- **وضعیت:** Accepted
- **تاریخ:** 2026-09-15
- **نویسنده:** Technical Lead / Domain (تصمیم کارفرما — Domain Decision)

> **به‌روزرسانی ۲۰۲۶-۰۹-۲۳ (DEC-035):** `Code` مسئولیت و حق امضا (Authority) دیگر ورودی کاربر
> نیست و به‌صورت خودکار معادل `Id` (GUID) تولید می‌شود؛ بنابراین «Business Routing Key خوانا»ی
> توصیف‌شده در این ADR فعلاً در API پیاده نشده است. همچنین نشان «صاحب امضا»
> (`hasSigningAuthority`) از «انتساب جاری به کد ثابت `SIGNING_AUTHORITY`» به «وجود حداقل یک
> انتساب جاری حق امضا» تغییر کرد. ماژول Authorities در UI با نام **«حق امضا»** نمایش داده می‌شود
> (شناسه‌های فنی — مسیر API، نام موجودیت و Permissionها — بدون تغییر ماندند تا ثبت‌نام Permission
> در IAM نشکند).

## Context
مدل Phase 2 مسئولیت را Value Object بدون Code (Owned در Post) و اختیار را پرچم bool (`Post.HasSigningAuthority`) مدل کرده بود. تصمیم جدید کارفرما تفکیک صریح چهار مفهوم را الزامی می‌کند: `IAM Role ≠ Post ≠ Responsibility ≠ Authority`. مسئولیت باید Business Routing Key باشد (ارجاع بر اساس مسئولیت سازمانی، نه UserId ثابت) و اختیار نیز جمعی و تاریخ‌دار باشد.

## Decision
- **Responsibility** موجودیت مستقل (Aggregate Root): `Code` یکتا در Tenant + Title/Description + IsActive.
- **PostResponsibilityAssignment** (عضو Aggregate): `OrganizationId` + `PostId` + `ResponsibilityId` + `StartDate`/`EndDate` + `IsActive`. پایان با `EndDate/IsActive=false`؛ بدون حذف فیزیکی.
- **Authority** موجودیت مستقل (Aggregate Root) با همان ساختار؛ **PostAuthorityAssignment** مشابه.
- Scope سازمانی از طریق `OrganizationId` روی Assignment (یک Code در چند سازمان قابل انتساب).
- حذف: `Post.HasSigningAuthority`، `Post.Responsibilities`، Responsibility VO/Owned، دستورات/Endpointهای title-based قبلی.
- `SIGNING_AUTHORITY` فقط Proposed Seed (بدون Seed عملیاتی)؛ `FindResponsible(OrganizationId, ResponsibilityCode)` در MVP پیاده‌سازی نمی‌شود ولی مدل از آن پشتیبانی می‌کند.
- این تغییر **Breaking داخلی** در API فعلی است؛ هیچ Consumer خارجی/فرانت‌اندی به Endpointهای قبلی وابسته نیست (ثبت‌شده در Decision Log).

## Rationale
- پایداری Routing در برابر جابجایی/بازنشستگی افراد (مسئولیت متعلق به Post می‌ماند).
- تفکیک امنیت (IAM Role) از جایگاه (Post)، وظیفه (Responsibility) و اختیار (Authority).
- چرخه حیات تاریخ‌دار به‌جای حذف.

## Considered and rejected alternatives
- حفظ VO + bool با افزودن Code: رد شد — چرخه حیات و Scope سازمانی بدون Assignment ممکن نیست.
- Assignment مستقیم به User: رد شد — مغایر تصمیم صریح (اتصال به Post).
- معماری موازی/بازنویسی کامل: رد شد — حداقل تغییر روی وضعیت فعلی.

## Consequences
- مثبت: مدل صادق با دامنه، آماده Resolution آینده، بدون ابهام امنیتی.
- منفی: Breaking داخلی (۳ Endpoint + DTOها)؛ Migration جدید (بدون داده تولیدی، بازسازی تمیز).
- نیازمند: تأیید Business Catalog برای Seed؛ تأیید نام ۱۲ Permission جدید (Q-006).

**منابع:** `Docs/decision-log.md` (DEC-024)، `Docs/domain/responsibility-routing.md`، ADR-004 (بخش Authority آن با این ADR جایگزین شد)، ADR-008 (Permissionها)