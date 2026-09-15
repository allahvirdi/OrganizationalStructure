# API Contract — Responsibilities (مسئولیت‌های سازمانی)

**نسخه:** `v1`
**پیشوند:** `/api/v1/responsibilities` (+ `/api/v1/posts/{postId}/responsibilities`)
**وضعیت:** مصوب (ADR-011 — جایگزین مدل title-based قبلی)
**تصریح‌دهی:** فعلاً بدون احراز هویت؛ Policyها در Phase 4 — Permissionهای پیشنهادی `OrganizationStructure.Responsibility.*` (Proposed)

---

## مدل‌ها

### ResponsibilityDto
```json
{ "id": "...", "code": "SECRETARIAT", "title": "مسئول دبیرخانه",
  "description": null, "isActive": true }
```

### ResponsibilityAssignmentDto
```json
{ "id": "...", "responsibilityId": "...", "responsibilityCode": "SECRETARIAT",
  "responsibilityTitle": "مسئول دبیرخانه", "organizationId": "...",
  "postId": "...", "postCode": "MGR-001", "postTitle": "مدیر اداره",
  "startDate": null, "endDate": null, "isActive": true }
```

### Error Contract
| کد خطا | وضعیت HTTP |
|---|---|
| `Responsibility.NotFound` / `Responsibility.PostNotFound` | 404 |
| `Responsibility.DuplicateCode` / `Responsibility.Inactive` / `Responsibility.AssignmentConflict` / `Responsibility.AssignmentNotFound` / `Responsibility.HasActiveAssignments` | 409 |
| خطای اعتبارسنجی | 400 |

---

## Endpointها

### تعریف مسئولیت — `POST /api/v1/responsibilities`
Request: `{ "code": "SECRETARIAT", "title": "مسئول دبیرخانه", "description": null }`
- موفق: `201 Created` + شناسه — ناموفق: `400` / `409` (کد تکراری)

### ویرایش مسئولیت — `PUT /api/v1/responsibilities/{id}`
Request: `{ "title": "...", "description": "..." }`
- موفق: `204` — ناموفق: `400` / `404`

### غیرفعال‌سازی — `PATCH /api/v1/responsibilities/{id}/disable`
- موفق: `204` — ناموفق: `400` / `404` / `409` (انتساب جاری دارد)

### دریافت با کد — `GET /api/v1/responsibilities/{code}` → `200` / `404`
### جستجو — `GET /api/v1/responsibilities?searchTerm=&isActive=&page=1&pageSize=20` → `200` + PagedResult

### انتساب به پست — `POST /api/v1/responsibilities/assignments`
Request: `{ "responsibilityCode": "SECRETARIAT", "postId": "...", "startDate": null, "endDate": null }`
- موفق: `201 Created` + شناسه انتساب — ناموفق: `400` / `404` / `409` (تکراری جاری یا مسئولیت غیرفعال)

### پایان انتساب — `POST /api/v1/responsibilities/assignments/{assignmentId}/end`
Request: `{ "endDate": "2026-09-30" }` (بدون حذف فیزیکی)
- موفق: `204` — ناموفق: `400` / `404` / `409`

### مسئولیت‌های پست — `GET /api/v1/posts/{postId}/responsibilities?onlyActive=true` → `200` + آرایه / `404`

---

## Versioning
- نسخه‌بندی مسیر: `/api/v1/...`