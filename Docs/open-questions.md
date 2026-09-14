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
| Q-001 | ✅ افزودن `Region=3` به IAM مجاز شد — پیاده‌سازی در مخزن IAM به‌صورت تسک مستقل (بسته شد) | تعارض C-1 در Phase 0 | بالا (فوری) | کارفرما | Closed — DEC-019 |
| Q-002 | ✅ پروتکل اتصال IAM: الگوی BFF ✅ (بسته شد) | متن Baseline: OIDC/OpenIddict؛ واقعیت IAM: REST + JWT | بالا | کارفرما | Closed — DEC-018 |
| Q-003 | Schema نهایی جدول واسط (Staging) Import پرسنل چیست؟ | `Organizational-Structure-Big-Picture-Scenario` + درخواست Import | متوسط | Business / کارفرما | Open |
| Q-004 | ✅ IAM مرجع Role/Policy/Permission شد؛ OrgStructure بدون Authorization موازی، فقط مصرف + enforce می‌کند (بسته شد) | DEC-008 / DEC-020 | بالا | کارفرما | Closed — DEC-020 |
| Q-005 | سیاست نگهداری، آرشیو و حذف PII پرسنلی چیست؟ (مدت نگهداری، حق حذف، Anonymization) | Security Baseline | متوسط | Security Lead | Open |
| Q-006 | فهرست دقیق و نام‌گذاری نهایی Permissionهای اختصاصی سامانه (`Post.*`/`Employee.*`/`Authority.*` فعلاً الگوی دسته‌بندی‌اند) — مالکیت Role/Permission در IAM حل شد، فقط نام‌گذاری نهایی باز است | DEC-008، DEC-021 | بالا | کارفرما + مالک IAM | Open (فقط نام‌گذاری) |
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
| Q-002 | پروتکل اتصال IAM: الگوی BFF | DEC-018 / ADR-002 | 2026-09-14 |
| Q-001 | مجوز افزودن Region=3 به IAM (پیاده‌سازی در مخزن IAM) | DEC-019 / ADR-009 | 2026-09-14 |
| Q-004 | IAM مرجع Role/Policy/Permission؛ بدون Authorization موازی | DEC-020 / ADR-008 | 2026-09-14 |
| Q-006 (مالکیت) | تمام Role/Permissionها در IAM تعریف می‌شوند؛ `Post.*`/`Employee.*` فعلاً الگوست | DEC-021 / ADR-008 | 2026-09-14 |

---

**قانون:** هر Open Question که بسته می‌شود، باید به Decision Log منتقل و در صورت اهمیت معماری به ADR تبدیل شود.