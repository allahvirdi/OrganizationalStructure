# Permission Catalog (پیشنهادی) — سامانه ساختار سازمانی

**آخرین به‌روزرسانی:** `2026-09-14`
**وضعیت:** `Proposed / Pending Approval` (تأیید کارفرما + مالک IAM)
**مرجع تصمیم:** DEC-023 / ADR-008 (متمم ۱)

> **قوانین:**
> - مالکیت Role/Policy/Permission با IAM است؛ این سامانه هیچ Role/Policy موازی نمی‌سازد.
> - این فهرست **پیشنهاد نهایی** است و بدون تأیید کارفرما + مالک IAM نباید در IAM ثبت شود.
> - OrgStructure فقط این Permissionها را مصرف و در سطح Domain enforce می‌کند.

---

## قرارداد نام‌گذاری

`OrganizationStructure.{Resource}.{Action}` — منبع: `Module`، عمل: `Action` در مدل Permission IAM.

## Post (۷ مورد)

| Permission | قابلیت / Use Case | API (برنامه‌ریزی‌شده) | UI |
|---|---|---|---|
| `OrganizationStructure.Post.View` | مشاهده پست و جزئیات | `GET /api/v1/posts/{id}`، `GET /api/v1/posts` | صفحه مدیریت پست / کارت جزئیات |
| `OrganizationStructure.Post.Create` | ایجاد پست جدید | `POST /api/v1/posts` | فرم ایجاد پست |
| `OrganizationStructure.Post.Update` | ویرایش پست | `PUT /api/v1/posts/{id}` | فرم ویرایش پست |
| `OrganizationStructure.Post.Disable` | فعال/غیرفعال کردن پست | `PATCH /api/v1/posts/{id}/status` | عملیات ردیف/کارت |
| `OrganizationStructure.Post.AssignEmployee` | انتساب پرسنل به پست | `POST /api/v1/posts/{id}/assignments` | دیالوگ انتساب |
| `OrganizationStructure.Post.RemoveEmployee` | قطع انتساب پرسنل از پست | `DELETE /api/v1/posts/{id}/assignments/{employeeId}` | عملیات حذف انتساب |
| `OrganizationStructure.Post.ViewHierarchy` | مشاهده والد/فرزندان/زیرشاخه | `GET /api/v1/posts/{id}/parent`، `/children`، `/subtree` | چارت سازمانی |

## Employee (۸ مورد)

| Permission | قابلیت / Use Case | API (برنامه‌ریزی‌شده) | UI |
|---|---|---|---|
| `OrganizationStructure.Employee.View` | مشاهده پرسنل (غیرحساس) | `GET /api/v1/employees`، `GET /api/v1/employees/{id}` | فهرست / پروفایل پرسنل |
| `OrganizationStructure.Employee.Create` | ثبت پرسنل | `POST /api/v1/employees` | فرم ثبت پرسنل |
| `OrganizationStructure.Employee.Update` | ویرایش پرسنل | `PUT /api/v1/employees/{id}` | فرم ویرایش پرسنل |
| `OrganizationStructure.Employee.Disable` | فعال/غیرفعال کردن پرسنل | `PATCH /api/v1/employees/{id}/status` | عملیات ردیف |
| `OrganizationStructure.Employee.AssignPost` | انتساب پست به پرسنل | `POST /api/v1/employees/{id}/posts` | دیالوگ انتساب |
| `OrganizationStructure.Employee.RemovePost` | قطع انتساب پست از پرسنل | `DELETE /api/v1/employees/{id}/posts/{postId}` | عملیات حذف انتساب |
| `OrganizationStructure.Employee.Import` | ورود از CSV/Excel/جدول واسط (فقط ستاد) | `POST /api/v1/import/*` | صفحه Import |
| `OrganizationStructure.Employee.ViewSensitiveData` | مشاهده فیلدهای PII (نام، کد ملی، موبایل‌ها، تاریخ تولد) | پاسخ‌های شامل PII | نمایش/ماسک فیلدهای حساس در UI |

## Authority (۵ مورد)

| Permission | قابلیت / Use Case | API (برنامه‌ریزی‌شده) | UI |
|---|---|---|---|
| `OrganizationStructure.Authority.View` | مشاهده مسئولیت/صاحب‌امضا بودن | `GET /api/v1/posts/{id}/authority` | نشان چارت / جزئیات پست |
| `OrganizationStructure.Authority.Create` | تعریف مسئولیت برای پست | `POST /api/v1/posts/{id}/responsibilities` | فرم مسئولیت |
| `OrganizationStructure.Authority.Update` | ویرایش مسئولیت | `PUT /api/v1/posts/{id}/responsibilities` | فرم ویرایش |
| `OrganizationStructure.Authority.Disable` | حذف/غیرفعال مسئولیت | `DELETE /api/v1/posts/{id}/responsibilities` | عملیات حذف |
| `OrganizationStructure.Authority.SigningAuthority` | تعیین/لغو صاحب‌امضا بودن | `PATCH /api/v1/posts/{id}/signing-authority` | عملیات صاحب‌امضا |

---

## مراحل بعدی (پس از تأیید)

1. تأیید نهایی نام‌ها توسط کارفرما + مالک IAM.
2. ثبت Permissionها در IAM (Module/Action مربوطه).
3. تخصیص به Roleها در IAM و انتشار در Claim توکن.
4. Enforce در OrgStructure از طریق Policyهای داخلی (Phase 4).
5. در صورت مشاهده محدودیت در IAM برای مصرف، گزارش Gap/ADR مستقل (بدون تغییر IAM بدون تأیید).