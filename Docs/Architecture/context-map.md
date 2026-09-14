# Context Map

**آخرین به‌روزرسانی:** `2026-09-14`
**وضعیت:** اسکلت Phase 0 (نهایی‌سازی در Phase 1/2)

---

## تعاملات

```
┌────────────────────┐         ┌─────────────────────────┐
│  Enterprise-IAM-V2 │  JWT    │ OrganizationalStructure │
│  (Identity/Org)    │ ──────► │       (Core)            │
└────────────────────┘         └────────────┬────────────┘
                                             │ REST /api/v1
             ┌───────────────────────────────┼───────────────┐
             ▼                               ▼               ▼
      ┌─────────────┐                ┌─────────────┐  ┌─────────────┐
      │  BPMS       │                │  HR System  │  │  Other      │
      └─────────────┘                └─────────────┘  └─────────────┘
```

## شرح Bounded Contextها
| Context | مالک | رابطه |
|---------|------|-------|
| IAM — Identity/Organization | Enterprise-IAM-V2 | OrgStructure Reference می‌کند (ADR-002) |
| OrgStructure Core (Post/Employee/Assignment/Authority) | این سامانه | مالک کامل دامنه ساختار/پرسنل |
| Access & Visibility | این سامانه | مصرف Claim از IAM؛ Policy داخلی |
| Integration API | این سامانه | سرویس REST به مصرف‌کننده‌ها |

## روابط
- OrgStructure ← IAM: `organization_id`, `user_id`, `role` (Reference و Claim).
- مصرف‌کننده‌ها ← OrgStructure: REST نسخه‌بندی‌شده (MVP، ADR-007).
- در MVP خبری از Events/Outbox نیست (ADR-007).

> کامل‌شدن با دیاگرام نهایی در Phase 1/2.