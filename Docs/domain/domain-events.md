# Domain Events

**آخرین به‌روزرسانی:** `2026-09-15`
**وضعیت:** منجمد Phase 2 + اصلاحیه ADR-011

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
| `EmployeeAssignmentEnded` | Assignment | پایان انتساب پرسنل به پست |
| `ResponsibilityCreated` / `Updated` / `Deactivated` | Responsibility | چرخه حیات مسئولیت |
| `ResponsibilityAssigned` | Responsibility | انتساب مسئولیت به پست |
| `ResponsibilityAssignmentEnded` | Responsibility | پایان انتساب مسئولیت |
| `AuthorityCreated` / `Updated` / `Deactivated` | Authority | چرخه حیات اختیار |
| `AuthorityAssigned` | Authority | انتساب اختیار به پست |
| `AuthorityAssignmentEnded` | Authority | پایان انتساب اختیار |

## قواعد

- رویدادها فقط پس از ثبت موفق در Transaction منتشر می‌شوند.
- استفاده از Abstraction `IDomainEventDispatcher` (الگوی خواهر).
- مصرف داخلی بین ماژول‌ها از طریق قرارداد داخلی مجاز است.
- انتشار خارجی از طریق Outbox در Future Phase فعال می‌شود (ADR-007).

> فهرست فوق، پیاده‌سازی‌شده در کد (`Domain/Events/`) و منجمد است.