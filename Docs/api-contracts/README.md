# API Contracts — قراردادهای API

> این پوشه شامل قراردادهای API سیستم است.
> طبق اصل API-First (Baseline §۶)، **قبل از شروع هر فیچر Frontend** قرارداد API باید اینجا تعریف و ثبت شود.

**آخرین به‌روزرسانی:** `2026-09-14`
**وضعیت:** اسکلت — مستندات در Phase 3/5 ایجاد می‌شوند.

---

## مسیرهای پایه برنامه‌ریزی‌شده

- `/api/v1/organizations`
- `/api/v1/posts`
- `/api/v1/posts/{id}/parent`
- `/api/v1/posts/{id}/children`
- `/api/v1/posts/{id}/subtree`
- `/api/v1/employees`
- `/api/v1/employees/{id}/posts`
- `/api/v1/posts/{id}/authority` (Responsibility / SigningAuthority)
- `/api/v1/dashboard`
- `/api/v1/import` (CSV/Excel/Staging — MVP Secondary)

## الزامات هر Resource (حداقل)

- Request DTO
- Response DTO
- Error Contract (یکپارچه)
- Pagination (Offset/Limit استاندارد)
- Filtering و Sorting
- Versioning (`/api/v1/...`)
- مثال‌های موفق/ناموفق

## فایل‌های قراردادی

- `posts.md` — (Phase 3)
- `employees.md` — (Phase 3)
- `assignments.md` — (Phase 3)
- `authority.md` — (Phase 3)
- `organizations.md` — (Phase 5)
- `import.md` — (Phase 7)

> اسکلت در فاز ۰؛ تکمیل دقیق با تعریف Vertical Slice در فاز ۳ و قبل از Frontend (فاز ۶).