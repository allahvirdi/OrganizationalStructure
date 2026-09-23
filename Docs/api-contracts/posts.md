# API Contract — Posts (پست‌های سازمانی)

**نسخه:** `v1`
**پیشوند:** `/api/v1/posts`
**وضعیت:** مصوب Phase 3 (Slice پست)
**تصریح‌دهی:** فعلاً بدون احراز هویت؛ Policyها در Phase 4 (DEC-008/ADR-008)

---

## مدل‌ها

### PostDto (Response — تفصیلی)
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "organizationId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "code": "MGR-001",
  "title": "مدیر اداره",
  "description": null,
  "parentId": null,
  "isActive": true,
  "hasSigningAuthority": true,
  "responsibilities": [{ "id": "…", "responsibilityCode": "…", "…": "انتساب جاری" }],
  "authorities": [{ "id": "…", "authorityCode": "…", "…": "انتساب جاری" }]
}
```

> `hasSigningAuthority`: وجود حداقل یک انتساب جاری **حق امضا** (Authority) برای پست (DEC-035).
> پیش از DEC-035 این نشان به کد ثابت `SIGNING_AUTHORITY` گره خورده بود؛ با خودکارشدن کد (GUID) مبنای آن به «وجود انتساب جاری» تغییر کرد.

### PostSummaryDto (Response — فهرست/جستجو)
```json
{
  "id": "...",
  "organizationId": "...",
  "code": "MGR-001",
  "title": "مدیر اداره",
  "parentId": null,
  "isActive": true
}
```

### PostTreeDto (Response — subtree)
```json
{
  "id": "...",
  "code": "MGR-001",
  "title": "مدیر اداره",
  "hasSigningAuthority": true,
  "isActive": true,
  "children": [ { "...": "گره فرزند بازگشتی" } ]
}
```

### PagedResult (Response — search)
```json
{
  "items": [ { "...": "PostDto" } ],
  "totalCount": 42,
  "page": 1,
  "pageSize": 20,
  "totalPages": 3
}
```

### Error Contract (یکپارچه — ProblemDetails)
```json
{
  "title": "Post.DuplicateCode",
  "detail": "پستی با کد MGR-001 در این سازمان از قبل وجود دارد.",
  "status": 409
}
```

| کد خطا | وضعیت HTTP |
|---|---|
| `Post.NotFound` / `Post.ParentNotFound` | 404 |
| `Post.DuplicateCode` / `Post.CrossOrganizationMove` / `Post.CycleDetected` | 409 |
| `Access.Forbidden` (سازمان خارج از محدوده کاربر) | 403 |
| خطای اعتبارسنجی (`Validation.Failed`) | 400 |

> **محدوده سازمانی (Phase 6):** تمام Endpointهای پست به Scope مشاهده کاربر (خود سازمان +
> زیرمجموعهها — از درخت IAM) محدود شده‌اند. خواندن/ویرایش/جابجایی/وضعیت پست خارج از محدوده و
> همچنین `organizationId` خارج از محدوده در جستجو/ایجاد، `403 Access.Forbidden` میدهد.
> برای نمایش «نام» سازمان در فرم‌ها (به‌جای شناسه خام) از `GET /api/v1/organizations` استفاده کنید
> (قرارداد: `organizations.md`).

---

## Endpointها

### ایجاد پست — `POST /api/v1/posts`
Request:
```json
{
  "organizationId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "code": "MGR-001",
  "title": "مدیر اداره",
  "description": null,
  "parentId": null
}
```
- موفق: `201 Created` + شناسه (GUID) + هدر `Location`
- ناموفق: `400` (اعتبارسنجی) / `404` (والد ناموجود) / `409` (کد تکراری یا ناسازگاری سازمانی)

### ویرایش پست — `PUT /api/v1/posts/{id}`
Request: `{ "code": "...", "title": "...", "description": "..." }`
- موفق: `204 No Content`
- ناموفق: `400` / `404` / `409`

### جابجایی پست — `POST /api/v1/posts/{id}/move`
Request: `{ "newParentId": "..." }` (خالی یعنی ریشه)
- موفق: `204 No Content`
- ناموفق: `400` / `404` / `409` (چرخه یا بین‌سازمانی)

### تعیین وضعیت — `PATCH /api/v1/posts/{id}/status`
Request: `{ "isActive": false }`
- موفق: `204 No Content`
- ناموفق: `400` / `404`

> **تغییر Breaking (ADR-011, DEC-024):** Endpointهای `PATCH .../signing-authority` و `POST/DELETE .../responsibilities` (title-based) حذف شدند و با مدل Assignment جایگزین شدند — مراجعه به `responsibilities.md` و `authorities.md`. هیچ Consumer خارجی نداشتند.

### دریافت پست — `GET /api/v1/posts/{id}`
- موفق: `200` + PostDto
- ناموفق: `404`

### فرزندان مستقیم — `GET /api/v1/posts/{id}/children`
- موفق: `200` + آرایه PostDto (مرتب‌سازی با Code)
- ناموفق: `404`

### زیرشاخه — `GET /api/v1/posts/{id}/subtree?maxDepth=5`
- موفق: `200` + PostTreeDto (سقف عمق ۱..۲۰، پیش‌فرض امن ۲۰)
- ناموفق: `400` (عمق نامعتبر) / `404`

### جستجو — `GET /api/v1/posts?organizationId=&searchTerm=&isActive=&page=1&pageSize=20`
- موفق: `200` + PagedResult (pageSize حداکثر ۱۰۰)
- ناموفق: `400` (صفحه‌بندی نامعتبر)

---

## Versioning و Pagination
- نسخه‌بندی مسیر: `/api/v1/...` (تغییر ناسازگار فقط با نسخه جدید)
- صفحه‌بندی: `page` (از ۱) + `pageSize` (۱..۱۰۰) + `totalCount`/`totalPages` در پاسخ
- مرتب‌سازی پیش‌فرض جستجو: `Code` صعودی