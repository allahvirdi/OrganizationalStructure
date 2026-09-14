# Container Diagram

**آخرین به‌روزرسانی:** `2026-09-14`
**وضعیت:** اسکلت Phase 0 (نهایی‌سازی در Phase 1)

---

## سطح Container

```
Browser (Frontend: Next.js + MUI RTL)
      │ HTML/JSON (کوکی نشست HttpOnly از طریق BFF)
      ▼
Backend  OrganizationalStructure.API  (Modular Monolith, /api/v1)
      ├── OrganizationalStructure.Domain
      ├── OrganizationalStructure.Application
      ├── OrganizationalStructure.Infrastructure
      │       ├── SQL Server (OrgStructureDb)
      │       ├── Redis (Cache/Lock)
      │       ├── MinIO (فایل/مستند)
      │       ├── Elasticsearch/OpenSearch
      │       └── Hangfire (Background Jobs — Import و ...)
      └── (اعتبارسنجی JWT ↔ IAM — فقط‌خواندنی)
```

## تصمیم‌های کلیدی
- Modular Monolith در یک واحد استقرار.
- ۴ پروژه لایه‌ای.
- BFF: مرورگر توکن IAM را نمی‌بیند؛ کوکی نشست HttpOnly (تأیید در Phase 1 — Q-002).
- DB مستقل از IAM؛ تماس فقط از طریق Reference ID.

## نکات
- Diagرا احراز/ونهای Data Flow در Phase 1 با حل Q-002 تکمیل می‌شود.