# API Contract — Employees & Assignments (پرسنل و انتساب)

**نسخه:** `v1`
**پیشوند:** `/api/v1/employees` (+ `/api/v1/posts/{postId}/employees`)
**وضعیت:** مصوب Phase 3 (Slice پرسنل)
**تصریح‌دهی:** فعلاً بدون احراز هویت؛ Policyها در Phase 4 (DEC-008/ADR-008) — مشاهده PII با `OrganizationStructure.Employee.ViewSensitiveData`

---

## مدل‌ها

### EmployeeDto (Response — شامل PII)
```json
{
  "id": "...",
  "personnelCode": "00000001",
  "firstName": "علی",
  "lastName": "رضایی",
  "nationalCode": "0013542419",
  "mobile": "09120000000",
  "birthDate": "1981-08-03",
  "serviceYears": 12,
  "serviceMonths": 3,
  "pezhvakMobile": "09190000000",
  "userId": null,
  "isActive": true
}
```

### EmployeePostDto / PostEmployeeDto (Response — انتساب)
```json
{ "postId": "...", "postCode": "MGR-001", "postTitle": "مدیر اداره",
  "fromDate": null, "toDate": null, "isPrimary": true }
```

### Error Contract
| کد خطا | وضعیت HTTP |
|---|---|
| `Employee.NotFound` / `Employee.PostNotFound` | 404 |
| `Employee.DuplicatePersonnelCode` / `Employee.AssignmentConflict` / `Employee.NoActiveAssignment` | 409 |
| خطای اعتبارسنجی (`Validation.Failed`) | 400 |

---

## Endpointها

### ثبت پرسنل — `POST /api/v1/employees`
Request:
```json
{
  "personnelCode": "00000001",
  "firstName": "علی", "lastName": "رضایی",
  "nationalCode": "0013542419", "mobile": "09120000000",
  "birthDate": "1981-08-03",
  "serviceYears": 12, "serviceMonths": 3,
  "pezhvakMobile": "09190000000",
  "userId": null
}
```
- موفق: `201 Created` + شناسه
- ناموفق: `400` / `409` (کد تکراری)

**قواعد اعتبارسنجی:**
| فیلد | قاعده |
|---|---|
| `personnelCode` | اجباری، عدد ۸ رقمی (`^[0-9]{8}$`)، یکتا در مستأجر |
| `nationalCode` | اجباری، ۱۰ رقم، الگوریتم چک‌سام کد ملی ایران (ضریب ۱۰..۲، مد ۱۱) |
| `mobile` | اجباری، شماره موبایل ایرانی (`^09[0-9]{9}$`) |
| `serviceYears`/`serviceMonths` | اختیاری ولی فقط با هم (هر دو یا هیچ‌کدام) |

### ویرایش پرسنل — `PUT /api/v1/employees/{id}`
Request: `{ "firstName": "...", "lastName": "...", "nationalCode": "...", "mobile": "..." }`
- موفق: `204` — ناموفق: `400` / `404`

> قواعد اعتبارسنجی مشابه ثبت پرسنل (چک‌سام کد ملی + فرمت موبایل ایرانی اجباری).

### ویرایش تکمیلی — `PATCH /api/v1/employees/{id}/supplementary`
Request: `{ "birthDate": "...", "serviceYears": 12, "serviceMonths": 3, "pezhvakMobile": "..." }`
- موفق: `204` — ناموفق: `400` / `404`

### انتساب به پست — `POST /api/v1/employees/{id}/posts`
Request: `{ "postId": "...", "fromDate": null, "toDate": null, "isPrimary": true }`
- موفق: `204` — ناموفق: `400` / `404` (پرسنل/پست) / `409` (تکراری یا اصلی دوم)

### پایان انتساب — `DELETE /api/v1/employees/{id}/posts/{postId}`
Request body: `{ "endDate": "2026-09-30" }`
- موفق: `204` — ناموفق: `400` / `404` / `409` (انتساب فعال نیست)

### تعیین وضعیت — `PATCH /api/v1/employees/{id}/status`
Request: `{ "isActive": false }` (غیرفعال ≠ حذف اطلاعات — DEC-022)
- موفق: `204` — ناموفق: `400` / `404`

### اتصال به کاربر — `POST /api/v1/employees/{id}/link-user`
Request: `{ "userId": "..." }` (مرجع IAM، بدون FK)
- موفق: `204` — ناموفق: `400` / `404`

### دریافت پرسنل — `GET /api/v1/employees/{id}` → `200` + EmployeeDto / `404`
### پست‌های پرسنل — `GET /api/v1/employees/{id}/posts?onlyActive=true` → `200` + آرایه / `404`
### پرسنل پست — `GET /api/v1/posts/{postId}/employees?onlyActive=true` → `200` + آرایه / `404`
### جستجو — `GET /api/v1/employees?searchTerm=&isActive=&page=1&pageSize=20` → `200` + PagedResult

> محدودیت جستجو (ADR-006): عبارت روی کد پرسنلی (شامل) و کد ملی (تساوی دقیق) اعمال می‌شود؛ جستجوی نام در DB ممکن نیست (Randomized).

---

## Versioning و Pagination
- نسخه‌بندی مسیر: `/api/v1/...`
- صفحه‌بندی: `page` (از ۱) + `pageSize` (۱..۱۰۰) + `totalCount`/`totalPages`