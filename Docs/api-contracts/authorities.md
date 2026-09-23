# API Contract — Authorities (حق امضاها / اختیارهای سازمانی)

**نسخه:** `v1`
**پیشوند:** `/api/v1/authorities` (+ `/api/v1/posts/{postId}/authorities`)
**وضعیت:** مصوب (ADR-011)
**تصریح‌دهی:** فعلاً بدون احراز هویت؛ Policyها در Phase 4 — Permissionهای پیشنهادی `OrganizationStructure.Authority.*` (Proposed)
**تغییر ۲۰۲۶-۰۹-۲۳ (DEC-035):** این ماژول در UI با نام **«حق امضا»** نمایش داده می‌شود؛ فیلد `code` از ورودی `POST` حذف شد (کد خودکار GUID)؛ نشان «صاحب امضا» بر پایه وجود انتساب جاری محاسبه می‌شود.

---

## مدل‌ها

### AuthorityDto
```json
{ "id": "…", "code": "8f14e45f-ceea-467f-a1d1-0d0d1c0d0d0d", "title": "حق امضای مدیر",
  "description": null, "isActive": true }
```
> `code` به‌صورت خودکار (GUID، معادل شناسه) در بک‌اند تولید می‌شود و ورودی کاربر نیست.

### AuthorityAssignmentDto
```json
{ "id": "…", "authorityId": "…", "authorityCode": "8f14e45f-ceea-467f-a1d1-0d0d1c0d0d0d",
  "authorityTitle": "حق امضای مدیر", "organizationId": "…",
  "postId": "…", "postCode": "MGR-001", "postTitle": "مدیر اداره",
  "startDate": null, "endDate": null, "isActive": true }
```

### Error Contract
| کد خطا | وضعیت HTTP |
|---|---|
| `Authority.NotFound` / `Authority.PostNotFound` | 404 |
| `Authority.Inactive` / `Authority.AssignmentConflict` / `Authority.AssignmentNotFound` / `Authority.HasActiveAssignments` | 409 |
| خطای اعتبارسنجی | 400 |

---

## Endpointها

### تعریف حق امضا — `POST /api/v1/authorities`
Request: `{ "title": "حق امضای مدیر", "description": null }`
> فیلد `code` حذف شد (DEC-035)؛ کد به‌صورت خودکار (GUID) تولید می‌شود.
- موفق: `201 Created` + شناسه — ناموفق: `400`

### ویرایش حق امضا — `PUT /api/v1/authorities/{id}`
Request: `{ "title": "...", "description": "..." }`
- موفق: `204` — ناموفق: `400` / `404`

### غیرفعال‌سازی — `PATCH /api/v1/authorities/{id}/disable`
- موفق: `204` — ناموفق: `400` / `404` / `409` (انتساب جاری دارد)

### دریافت با کد — `GET /api/v1/authorities/{code}` → `200` / `404`
> `code` همان GUID خودکار است؛ جستجوی انسانی روی آن معنا ندارد.
### جستجو — `GET /api/v1/authorities?searchTerm=&isActive=&page=1&pageSize=20` → `200` + PagedResult

### انتساب به پست — `POST /api/v1/authorities/assignments`
Request: `{ "authorityCode": "8f14e45f-…", "postId": "…", "startDate": null, "endDate": null }`
- موفق: `201 Created` + شناسه انتساب — ناموفق: `400` / `404` / `409` (تکراری جاری یا حق امضای غیرفعال)

### پایان انتساب — `POST /api/v1/authorities/assignments/{assignmentId}/end`
Request: `{ "endDate": "2026-09-30" }` (بدون حذف فیزیکی)
- موفق: `204` — ناموفق: `400` / `404` / `409`

### حق امضاهای پست — `GET /api/v1/posts/{postId}/authorities?onlyActive=true` → `200` + آرایه / `404`

---

## Versioning
- نسخه‌بندی مسیر: `/api/v1/...`