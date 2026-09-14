# Domain Events

**آخرین به‌روزرسانی:** `2026-09-14`
**وضعیت:** طراحی اولیه Phase 0 — نهایی شدن در Phase 2

> **توجه MVP:** با توجه به ADR-007، Integration رویدادمحور در MVP نیست و فقط REST است. رویدادهای دامنه در MVP عمدتاً برای یکپارچگی داخلی ماژول‌ها (درون Modular Monolith) و لاگ/Audit به‌کار می‌روند؛ انتشار به Bus خارجی (RabbitMQ/Outbox) Deferred است.

---

## رویدادهای اولیه پیشنهادی

| رویداد | ماژول مبدأ | وقوع |
|--------|-----------|------|
| `PostCreated` | Organization Structure | ایجاد Post |
| `PostUpdated` | Organization Structure | ویرایش Post (شامل جابجایی Parent) |
| `PostDeactivated` | Organization Structure | غیرفعال کردن Post |
| `PostActivated` | Organization Structure | فعال کردن Post |
| `PostMoved` | Organization Structure | تغییر Parent درون درخت |
| `EmployeeCreated` | Employee | ایجاد پرسنل |
| `EmployeeUpdated` | Employee | ویرایش اطلاعات پرسنلی |
| `EmployeeDeactivated` | Employee | غیرفعال کردن پرسنل |
| `EmployeeAssignedToPost` | Assignment | انتساب پرسنل به پست |
| `EmployeeUnassignedFromPost` | Assignment | قطع انتساب |
| `SigningAuthorityChanged` | Authority | تغییر اختیار امضای Post |
| `ResponsibilityChanged` | Authority | تغییر مسئولیت Post |

## قواعد

- رویدادها فقط پس از ثبت موفق در Transaction منتشر می‌شوند.
- استفاده از Abstraction `IDomainEventDispatcher` (الگوی خواهر).
- مصرف داخلی بین ماژول‌ها از طریق قرارداد داخلی مجاز است.
- انتشار خارجی از طریق Outbox در Future Phase فعال می‌شود (ADR-007).

> این فهرست مقدماتی است و در Phase 2 با Event Storming نهایی می‌شود.