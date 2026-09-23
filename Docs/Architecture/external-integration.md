# راهنمای Integration برای سامانه‌های مصرف‌کننده

**آخرین به‌روزرسانی:** `2026-09-15`
**وضعیت:** مصوب Phase 5 (ADR-007، ADR-012)

> این سامانه مرجع معتبر ساختار سازمانی است؛ سامانه‌های دیگر منطق ساختار را پیاده‌سازی نمی‌کنند و از این API مصرف می‌کنند.

---

## ۱. دو الگوی احراز هویت

| مصرف‌کننده | الگو | جزئیات |
|---|---|---|
| مرورگر (کاربر انسانی) | کوکی نشست BFF | `POST /api/v1/auth/login` → کوکی HttpOnly؛ توکن IAM هرگز به مرورگر نمی‌رسد |
| سامانه ماشینی (BPMS/HR) | Bearer JWT معتبر IAM | هدر `Authorization: Bearer <access-token>`؛ اعتبارسنجی introspection سمت IAM (fail-closed) |

هر دو الگو Claimها و Policyهای یکسان می‌سازند؛ تفاوتی در Authorization نیست.

## ۲. قراردادهای پایه (`/api/v1/...`)

- `GET /api/v1/posts/{id}` — پست با جزئیات و انتساب‌های جاری
- `GET /api/v1/posts/{id}/children` — فرزندان مستقیم
- `GET /api/v1/posts/{id}/subtree?maxDepth=` — زیرشاخه چندسطحی
- `GET /api/v1/posts?organizationId=&searchTerm=` — جستجو
- `GET /api/v1/employees/{id}` — پرسنل با جزئیات
- `GET /api/v1/employees/{id}/posts` — پست‌های پرسنل
- `GET /api/v1/posts/{id}/employees` — پرسنل پست
- `GET /api/v1/responsibilities/{code}` — مسئولیت با کد (GUID خودکار)
- `GET /api/v1/posts/{id}/responsibilities` — مسئولیت‌های پست
- `GET /api/v1/authorities/{code}` — حق امضا با کد (GUID خودکار)
- `GET /api/v1/posts/{id}/authorities` — حق امضاهای پست

جزئیات کامل: `Docs/api-contracts/*.md`. نسخه‌بندی مسیر؛ تغییر ناسازگار فقط با نسخه جدید.

## ۳. الگوی Routing بر اساس مسئولیت (توصیه‌شده)

به‌جای `UserId` ثابت، بر اساس مسئولیت ارجاع دهید:

```text
OrganizationId + ResponsibilityCode (مثلاً SECRETARIAT)
  → GET پست‌های دارای آن مسئولیت (از طریق posts/{id}/responsibilities)
  → پرسنل جاری آن پست‌ها
```

> `FindResponsible(OrganizationId, ResponsibilityCode)` در MVP پیاده‌سازی نشده؛ با همین Endpointها قابل ترکیب است.

## ۴. خطاها و محدودیت‌ها

- Error Contract یکپارچه (ProblemDetails): `400` اعتبارسنجی، `401` احراز، `403` دسترسی/Scope، `404` عدم وجود، `409` تعارض.
- صفحه‌بندی: `page` (از ۱) + `pageSize` (۱..۱۰۰).
- در دسترس‌نبودن IAM یعنی 401/403 (fail-closed) — در مصرف‌کننده Retry با Backoff بگذارید.

---

**منابع:** ADR-007، ADR-012، `Docs/api-contracts/`