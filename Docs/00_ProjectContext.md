# 00 — Project Context (زمینه پروژه)

> این فایل نقطه شروع درک پروژه برای هر AI یا توسعه‌دهنده جدید است.
> باید همیشه به‌روز و کامل باشد تا بدون نیاز به تاریخچه گفتگو بتوان کار را ادامه داد.

**آخرین به‌روزرسانی:** `2026-09-14`
**وضعیت:** `Active`

---

## ۱. نام پروژه
سامانه ساختار سازمانی (پوشه/نام فنی: `OrganizationalStructure`)

## ۲. یکخطی (Elevator Pitch)
هسته مستقل و مرجع معتبر مدیریت ساختار درختی سازمان‌ها، پست‌های سازمانی، انتساب کارکنان، مسئولیت‌ها، صاحبان امضا و کنترل دسترسی به ساختار که خدمات خود را در اختیار سایر سامانه‌ها قرار می‌دهد.

## ۳. مسئله کسب‌وکار (Problem Statement)
اطلاعات ساختار سازمانی و جایگاه پست‌ها در سامانه‌های مختلف پراکنده و ناهمگون نگهداری می‌شود و هر سامانه مجدداً منطق ساختار را پیاده‌سازی می‌کند. مرجع واحد و معتبری برای روابط Parent/Child پست‌ها، انتساب کارکنان به پست‌ها، مسئولیت‌ها، صاحبان امضا و کنترل دسترسی مبتنی بر محدوده سازمانی وجود ندارد.

## ۴. چشم‌انداز (Vision)
تبدیل شدن به منبع رسمی و واحد ساختار سازمانی در افق ۱۲–۲۴ ماهه: مرجع معتبر برای Organization، Post Hierarchy، Employee، Assignment، Responsibility، Signing Authority و Access که به‌جای تکرار منطق در سامانه‌های مصرف‌کننده (HR / BPMS / سایر)، از طریق API معتبر سرویس می‌دهد.

## ۵. Bounded Contextهای اصلی
- **BC-1 — Organization Structure (هسته):** پست سازمانی، روابط Parent/Child، مشاهده چندسطحی، فعال/غیرفعال.
- **BC-2 — Employee & Assignment:** اطلاعات پرسنلی، انتساب چند‌پستی کارمند به پست، وضعیت پرسنل.
- **BC-3 — Authority & Responsibility:** مسئولیت سازمانی و اختیار امضا روی پست و نمایش متمایز آن در چارت.
- **BC-4 — Access & Visibility:** Role/Permission/Organization Scope و قواعد مشاهده مبتنی بر محدوده.
- **BC-5 — Import:** ورود اطلاعات پرسنلی از CSV، Excel و جدول واسط (فقط نقش‌های ستادی).
- **BC-6 — Integration API:** ارائه اطلاعات معتبر ساختار به سامانه‌های مصرف‌کننده (REST).

> `Organization` و `User` Identity از `Enterprise-IAM-V2` به‌صورت Reference مصرف می‌شوند و Master موازی ساخته نمی‌شود.

## ۶. کاربران اصلی / Personas
| Persona | نیاز اصلی | سطح دسترسی |
|---------|-----------|------------|
| کارشناس/مدیر ستاد | مشاهده و مدیریت ساختار و پرسنل در محدوده تحت اختیار؛ Import فایل/جدول واسط | مبتنی بر Permission + Organization Scope |
| کارشناس/مدیر استان | مدیریت پرسنل استان و مناطق زیرمجموعه در محدوده مجاز | مبتنی بر Permission + Organization Scope |
| کارشناس/مدیر منطقه | مدیریت پرسنل منطقه در محدوده مجاز | مبتنی بر Permission + Organization Scope |
| سامانه مصرف‌کننده (BPMS/HR) | استعلام معتبر ساختار از طریق API | دسترسی سرویس‌به‌سرویس |

> داشتن رابطه سازمانی با یک Organization به‌تنهایی به معنی داشتن مجوز نیست.

## ۷. محدودیت‌ها و فرضیات کلیدی
- استک فناوری طبق `Architecture Baseline v2.4` منجمد است؛ انحراف فقط با ADR.
- Identity/User/Organization Master در `Enterprise-IAM-V2` است؛ فقط مصرف‌کننده هستیم.
- `OrganizationType` فعلی IAM فقط `Organization=0, Department=1, Unit=2` دارد؛ افزودن `Region=3` نیازمند هماهنگی مالک IAM است (ADR).
- Multi-tenancy لازم است (`TenantId`).
- هر Organization درخت Post مستقل دارد؛ جابجایی بین Organizationها ممنوع است.
- PII با الگوی Always Encrypted/ADR-004 پروژه خواهر حفاظت می‌شود.
- Event-driven Integration در MVP نیست؛ فقط REST.
- Deadline پروژه دو ماه (تا `2026-11-14` — DEC-026)؛ تیم شامل Backend/Frontend/Database/Security Lead.

## ۸. لینک به اسناد اصلی
- Architecture Baseline: `Docs/01-Architecture-Baseline-FA.md`
- AI Development Path v2.4: `Docs/02-AI-Development-Path-0-to-100-FA.md`
- PROJECT-BASELINE: `Docs/PROJECT-BASELINE-v0.1.md`
- Big Picture: `Docs/Organizational-Structure-Big-Picture-Scenario.md`
- معماری عملیاتی: `Docs/01_Architecture.md`
- نقشه راه: `Docs/03_Roadmap.md`
- قوانین توسعه: `Docs/06_DevelopmentRules.md`
- ADRها: `Docs/adr/`
- مدل دامنه: `Docs/domain/`
- سوالات باز: `Docs/open-questions.md`

## ۹. وضعیت فعلی پروژه (خلاصه)
- فاز جاری: `Phase 0 — Project Baseline`
- درصد پیشرفت کلی: ~۱۵٪ (مرجع زنده: `Docs/04_Progress.md`)
- آخرین Session Report: `Docs/SessionReports/` (اولین گزارش در همین Session)

---

**قانون:** هر تغییری در دامنه یا چشم‌انداز باید اینجا و در Decision Log ثبت شود.
