# Permission Catalog — سامانه ساختار سازمانی

**آخرین به‌روزرسانی:** `2026-09-15`
**وضعیت:** `Approved` (مصوب کارفرما — DEC-025)؛ ثبت در IAM اقدام مالک IAM است
**مرجع تصمیم:** DEC-023 / DEC-025 / ADR-008 (متمم ۱)

> **قوانین:**
> - مالکیت Role/Policy/Permission با IAM است؛ این سامانه هیچ Role/Policy موازی نمی‌سازد.
> - این فهرست ۲۷ Permission با قالب سه‌بخشی `OrganizationStructure.{Resource}.{Action}` مصوب شد.
> - ثبت در IAM فقط توسط مالک IAM انجام می‌شود.
> - OrgStructure فقط این Permissionها را مصرف و در سطح Domain enforce می‌کند (Phase 4).

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

## Authority (۵ مورد — بازنگری‌شده با ADR-011)

| Permission | قابلیت / Use Case | API | UI |
|---|---|---|---|
| `OrganizationStructure.Authority.View` | مشاهده اختیارها و انتساب‌ها | `GET /api/v1/authorities*`، `GET /api/v1/posts/{id}/authorities` | نشان چارت / جزئیات پست |
| `OrganizationStructure.Authority.Create` | تعریف اختیار | `POST /api/v1/authorities` | فرم اختیار |
| `OrganizationStructure.Authority.Update` | ویرایش اختیار | `PUT /api/v1/authorities/{id}` | فرم ویرایش |
| `OrganizationStructure.Authority.Disable` | غیرفعال اختیار | `PATCH /api/v1/authorities/{id}/disable` | عملیات ردیف |
| `OrganizationStructure.Authority.Assign` | انتساب اختیار به پست | `POST /api/v1/authorities/assignments` | دیالوگ انتساب |
| `OrganizationStructure.Authority.EndAssignment` | پایان انتساب اختیار | `POST /api/v1/authorities/assignments/{id}/end` | عملیات پایان |

> `Authority.SigningAuthority` قبلی با مدل Assignment جایگزین شد (انتساب اختیار `SIGNING_AUTHORITY`).

## Responsibility (۶ مورد — جدید، ADR-011)

| Permission | قابلیت / Use Case | API | UI |
|---|---|---|---|
| `OrganizationStructure.Responsibility.View` | مشاهده مسئولیت‌ها و انتساب‌ها | `GET /api/v1/responsibilities*`، `GET /api/v1/posts/{id}/responsibilities` | فهرست / جزئیات پست |
| `OrganizationStructure.Responsibility.Create` | تعریف مسئولیت | `POST /api/v1/responsibilities` | فرم مسئولیت |
| `OrganizationStructure.Responsibility.Update` | ویرایش مسئولیت | `PUT /api/v1/responsibilities/{id}` | فرم ویرایش |
| `OrganizationStructure.Responsibility.Disable` | غیرفعال مسئولیت | `PATCH /api/v1/responsibilities/{id}/disable` | عملیات ردیف |
| `OrganizationStructure.Responsibility.Assign` | انتساب مسئولیت به پست | `POST /api/v1/responsibilities/assignments` | دیالوگ انتساب |
| `OrganizationStructure.Responsibility.EndAssignment` | پایان انتساب مسئولیت | `POST /api/v1/responsibilities/assignments/{id}/end` | عملیات پایان |

---

## مراحل بعدی (پس از تأیید)

1. تأیید نهایی نام‌ها توسط کارفرما + مالک IAM.
2. ثبت Permissionها در IAM (Module/Action مربوطه).
3. تخصیص به Roleها در IAM و انتشار در Claim توکن.
4. Enforce در OrgStructure از طریق Policyهای داخلی (Phase 4).
5. در صورت مشاهده محدودیت در IAM برای مصرف، گزارش Gap/ADR مستقل (بدون تغییر IAM بدون تأیید).