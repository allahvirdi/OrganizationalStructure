# ADR-004: مدل درخت Post و رابطه با Organization

- **وضعیت:** Accepted
- **تاریخ:** 2026-09-14
- **نویسنده:** Technical Lead / Domain (تأیید کارفرما)

## Context
ساختار سازمانی درختی است و هر جایگاه یک Post سازمانی است. پرسش: رابطه درخت Post با درخت Organization که Master آن در IAM است، چگونه است؟ در Big Picture، هر Post می‌تواند Parent مستقیم و چند Child مستقیم داشته باشد.

## Decision
- **هر Organization درخت Post مستقل خود دارد.** ریشه درخت در سطح هر Organization تعریف می‌شود.
- `OrganizationId` روی Post یک **Reference به IAM** است و کلید ظرف سازمانی محسوب می‌شود.
- Post دارای `ParentId` خودارجاع است (صفر یا یک Parent مستقیم، صفر یا چند Child مستقیم).
- **جابجایی Post بین Organizationهای مختلف ممنوع است.** جابجایی فقط درون همان درخت/Organization مجاز است.
- درخت Organization موازی در این سامانه ساخته نمیشود.
- قواعد کسب‌وکار کلیدی: هر Post درخت خود را دارد؛ فعال/غیرفعالسازی به معنی حذف فیزیکی نیست؛ مشاهده سلسله‌مراتبی (Descendants/Subtree) با هماهنگی Visibility پشتیبانی میشود.

## Rationale
- تثبیت مرز Master Data (Organization در IAM) و تمرکز OrgStructure بر Post.
- جلوگیری از مدل داده متناقض و پیچیدگی غیرضروری در جابجایی بین سازمانی.
- همسویی با قواعد کسب‌وکار Big Picture.

## Considered and rejected alternatives
- یک درخت Post سراسری با امکان جابجایی بین Organizationها: رد شد — تصمیم صریح کارفرما و افزایش پیچیدگی Visibility/Accuracy.
- ساخت درخت Organization موازی: رد شد — مغایر مرز Master Data.

## Consequences
- مثبت: مدل ساده و قابل تست، مطابق قواعد کسب‌وکار.
- منفی: در صورت نیاز آینده به جابجایی بین‌سازمانی، نیازمند ADR و بازنگری مدل است.
- وابستگی: اعتبارسنجی `OrganizationId` نیازمند دسترسی به IAM است (Reference بدون FK).

**منابع:** `Docs/PROJECT-BASELINE-v0.1.md` (DEC-007)، `Docs/Organizational-Structure-Big-Picture-Scenario.md` (§۲، §۴، §۶، §۱۴)