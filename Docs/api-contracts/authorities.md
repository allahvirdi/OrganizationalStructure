# API Contract — Authorities (اختیارهای سازمانی)

**نسخه:** `v1`
**پیشوند:** `/api/v1/authorities` (+ `/api/v1/posts/{postId}/authorities`)
**وضعیت:** مصوب (ADR-011)
**تصریح‌دهی:** فعلاً بدون احراز هویت؛ Policyها در Phase 4 — Permissionهای پیشنهادی `OrganizationStructure.Authority.*` (Proposed)

---

## مدل‌ها

### AuthorityDto
```json
{ "id": "...", "code": "SIGNING_AUTHORITY", "title": "اختیار امضا",
  "description": null, "isActive": true }
```

### AuthorityAssignmentDto
```json
{ "id": "...", "authorityId": "...", "authorityCode": "SIGNING_AUTHORITY",
  "authorityTitle": "اختیار امضا", "organizationId": "...",
  "postId": "...", "postCode": "MGR-001", "postTitle": "مدیر اداره",
  "startDate": null, "endDate": null, "isActive": true }
```

### Error Contract
| کد خطا | وضعیت HTTP |
|---|---|
| `Authority.NotFound` / `Authority.PostNotFound` | 404 |
| `Authority.DuplicateCode` / `Authority.Inactive` / `Authority.AssignmentConflict` / `Authority.AssignmentNotFound` / `Authority.HasActiveAssignments` | 409 |
| خطای اعتبارسنجی | 400 |

---

## Endpointها

### تعریف اختیار — `POST /api/v1/authorities`
Request: `{ "code": "SIGNING_AUTHORITY", "title": "اختیار امضا", "description": null }`
- موفق: `201 Created` + شناسه — ناموفق: `400` / `409`

### ویرایش اختیار — `PUT /api/v1/authorities/{id}`
Request: `{ "title": "...", "description": "..." }`
- موفق: `204` — ناموفق: `400` / `404`

### غیرفعال‌سازی — `PATCH /api/v1/authorities/{id}/disable`
- موفق: `204` — ناموفق: `400` / `404` / `409` (انتساب جاری دارد)

### دریافت با کد — `GET /api/v1/authorities/{code}` → `200` / `404`
### جستجو — `GET /api/v1/authorities?searchTerm=&isActive=&page=1&pageSize=20` → `200` + PagedResult

### انتساب به پست — `POST /api/v1/authorities/assignments`
Request: `{ "authorityCode": "SIGNING_AUTHORITY", "postId": "...", "startDate": null, "endDate": null }`
- موفق: `201 Created` + شناسه انتساب — ناموفق: `400` / `404` / `409`

### پایان انتساب — `POST /api/v1/authorities/assignments/{assignmentId}/end`
Request: `{ "endDate": "2026-09-30" }` (بدون حذف فیزیکی)
- موفق: `204` — ناموفق: `400` / `404` / `409`

### اختیارهای پست — `GET /api/v1/posts/{postId}/authorities?onlyActive=true` → `200` + آرایه / `404`

---

## Versioning
- نسخه‌بندی مسیر: `/api/v1/...`