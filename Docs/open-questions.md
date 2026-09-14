# Open Questions — ابهامات باز پروژه

> این فایل مرجع زنده ابهاماتی است که برای حل آن‌ها نیاز به تصمیم انسان یا هماهنگی خارجی وجود دارد.
>
> **قانون مهم:** Open Question به معنی مجوز برای حدس زدن نیست.
> تا زمانی که یک سؤال باز است، AI Coder نباید بر اساس فرض شخصی آن را به تصمیم معماری، مدل داده یا Implementation تبدیل کند.

**آخرین به‌روزرسانی:** `2026-09-14`

---

## سؤالات باز

| شناسه  | سؤال | منبع/زمینه | اولویت | مسئول پاسخ | وضعیت |
|--------|------|-----------|--------|------------|-------|
| Q-001 | افزودن `Region=3` به `OrganizationType` در IAM چه زمانی و توسط چه کسی انجام می‌شود؟ (فعلاً فقط `Organization=0, Department=1, Unit=2` وجود دارد) | تعارض C-1 در Phase 0 | بالا (فوری) | مالک IAM | Open |
| Q-002 | پروتکل اتصال احراز هویت به IAM چیست؟ الگوی BFF (مثل پروژه خواهر) یا اعتبارسنجی مستقیم JWT در همین سامانه؟ | متن Baseline: OIDC/OpenIddict؛ واقعیت IAM: REST + JWT | بالا | Security Lead + مالک IAM | Open |
| Q-003 | Schema نهایی جدول واسط (Staging) Import پرسنل چیست؟ | `Organizational-Structure-Big-Picture-Scenario` + درخواست Import | متوسط | Business / کارفرما | Open |
| Q-004 | Permissionهای دامنه‌ای (`Post.*`, `Employee.*`, `Authority.*`) چگونه در IAM ثبت/تخصیص می‌شوند؟ آیا IAM باید گسترش یابد؟ | DEC-008 | بالا | مالک IAM | Open |
| Q-005 | سیاست نگهداری، آرشیو و حذف PII پرسنلی چیست؟ (مدت نگهداری، حق حذف، Anonymization) | Security Baseline | متوسط | Security Lead | Open |
| Q-006 | نقش‌ها و Permissionهای نهایی این سامانه با نام و املای دقیق چه هستند؟ آیا در IAM ساخته می‌شوند؟ | DEC-008، الگوی ADR-003 پروژه خواهر | بالا | کارفرما + مالک IAM | Open |
| Q-007 | تفکیک دقیق مسئولیت داده پرسنلی بین IAM (`EmployeeIdentifier`, Lifecycle, Bulk Import) و OrgStructure | همپوشانی Employee با IAM | بالا | کارفرما + مالک IAM | Open |
| Q-008 | آیا `GeographicUnit` (استان/منطقه) از IAM مصرف می‌شود یا فقط `Organization` تخت سازمانی مبنای Scope است؟ | تعارض نام‌گذاری استان/منطقه | متوسط | کارفرما | Open |

---

## سؤالات بسته‌شده (پاسخ‌داده‌شده)

| شناسه | پاسخ | تصمیم مرجع | تاریخ |
|-------|------|-----------|-------|
| — | Frontend: Next.js + MUI طبق Baseline | DEC-003 / ADR-001 | 2026-09-14 |
| — | Multi-tenancy لازم است | DEC-005 / ADR-005 | 2026-09-14 |
| — | Permission دامنه‌ای در OrgStructure، ثبت/تخصیص در IAM | DEC-008 / ADR-008 | 2026-09-14 |
| — | PII با Always Encrypted | DEC-009 / ADR-006 | 2026-09-14 |

---

**قانون:** هر Open Question که بسته می‌شود، باید به Decision Log منتقل و در صورت اهمیت معماری به ADR تبدیل شود.