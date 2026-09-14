# Domain Vision & Problem Statement

**آخرین به‌روزرسانی:** `2026-09-14`
**وضعیت:** Accepted (تصویب در Phase 0)

---

## ۱. Vision (چشم‌انداز)

تبدیل شدن به **منبع رسمی و واحد ساختار سازمانی** در افق ۱۲–۲۴ ماهه؛ مرجع معتبر و یکپارچه برای:

- Organization (مصرف از IAM)
- Post و روابط سلسله‌مراتبی Parent/Child
- Employee و انتساب چند‌پستی به Postها
- Responsibility و Signing Authority
- کنترل دسترسی مبتنی بر Organisation Scope

به‌گونه‌ای که سایر سامانه‌ها (HR، BPMS و ...) **منطق ساختار را مجدداً پیاده‌سازی نکنند** و از طریق REST API معتبر از این سامانه مصرف کنند.

## ۲. Problem Statement (مسئله کسب‌وکار)

اطلاعات ساختار سازمانی و جایگاه پست‌ها در سامانه‌های مختلف پراکنده و ناهمگون نگهداری می‌شود و هر سامانه به‌صورت جداگانه منطق ساختار را پیاده‌سازی می‌کند. در نتیجه:

- روابط Parent/Child و سلسله‌مراتب چندسطحی در جای متمرکز و معتبری وجود ندارد.
- انتساب کارکنان به پست‌ها و مسئولیت/صاحب‌امضا بودن پست‌ها قابل استعلام واحد نیست.
- کنترل دسترسی مبتنی بر محدوده سازمانی (ستاد/استان/منطقه) شفاف و اجباری نیست.
- اصل «رابطه سازمانی ≠ مجوز» در هیچ‌کدام تضمین نمی‌شود.

## ۳. Goal (هدف)

ایجاد هسته مستقل `OrganizationalStructure` به‌عنوان **مرجع معتبر ساختار سازمانی** که:

1. داده‌های دامنه خود (Post، Employee، Assignment، Authority، Access) را با کیفیت Enterprise مدیریت کند.
2. Organization و Identity را از IAM **Reference** کند (بدون Master موازی).
3. از طریق REST API اطلاعات معتبر را در اختیار سامانه‌های مصرف‌کننده قرار دهد.
4. دسترسی را بر مبنای Permission + Organization Scope با Deny by Default اعمال کند.

## ۴. Non-Goals (عمداً خارج از محدوده)

- ساخت Master موازی Organization/User.
- پیاده‌سازی Identity Server / Authentication.
- Event-driven Integration، RabbitMQ، Outbox در MVP.
- LDAP/AD.
- Payroll، Recruitment، Workflow/BPMS.