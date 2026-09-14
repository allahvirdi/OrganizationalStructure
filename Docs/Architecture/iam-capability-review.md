# بررسی قابلیت دسترسی IAM (Q-004)

**تاریخ:** `2026-09-14`
**منبع بررسی:** `EnterpriseIAM.Domain` (کد واقعی — Entities، Enums، Constants)
**مرجع:** `Docs/adr/ADR-008-access-control.md` (متمم ۱)، `Docs/open-questions.md` (Q-004 — بسته)

> دستور کارفرما: «ابتدا قابلیت موجود IAM را بررسی کن و فقط در صورت وجود Gap، آن را به‌عنوان یک Change/ADR مستقل گزارش کن. بدون تأیید، IAM را تغییر نده.»

---

## یافته‌ها

| نیاز OrgStructure | قابلیت موجود در IAM | وضعیت |
|---|---|---|
| تعریف Role | `Role` (+ `RoleType`: System/Custom/**Organization/Geographic** + `ScopeId`) | ✅ موجود |
| تعریف Permission دامنه‌ای | `Permission` (Module → SubModule → Action، نام یکتا در مستأجر) | ✅ موجود |
| اتصال Role↔Permission | `RolePermission` (junction) | ✅ موجود |
| تخصیص Role به کاربر | `UserRole` (via Identity) | ✅ موجود |
| Policy با قواعد | `Policy` (`PolicyType`: Authentication/**Authorization**/Security + `Rules` JSON) | ✅ موجود |
| تخصیص Policy به کاربر | `UserPolicy` | ✅ موجود |
| Scope تفویضی (سازمان/جغرافیا) | `DelegatedAdminScope` (ScopeType: **Organization/GeographicUnit** + ScopeId + انقضا) | ✅ موجود |
| انتقال Role/Permission به مصرف‌کننده | Claimهای توکن: `organization_id`, `geographic_unit_id`, `role`, **`permission`**, `client_id` | ✅ موجود |

## نتیجه

**Gap یافت نشد.** تعریف Permissionهای دامنه‌ای (`Post.*`/`Employee.*`/`Authority.*`) و Scope سازمانی با مدل موجود IAM ممکن است — نیازی به Change مستقل در IAM نیست.

تنها استثنا: افزودن `Region=3` به `OrganizationType` که مجوز آن صادر شد (DEC-019 / ADR-009) و پیاده‌سازی آن در مخزن IAM به‌صورت تسک مستقل انجام می‌شود.

---

**قانون:** هر Gap آینده در IAM فقط به‌صورت Change/ADR مستقل و پس از تأیید صریح گزارش و اجرا می‌شود.