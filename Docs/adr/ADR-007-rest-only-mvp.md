# ADR-007: تعلیق RabbitMQ/Outbox و انتخاب REST-only برای MVP

- **وضعیت:** Accepted
- **تاریخ:** 2026-09-14
- **نویسنده:** Technical Lead / Architecture (تأیید کارفرما)

## Context
Architecture Baseline بخش ۳.۱ و Phase 1 سند مسیر توسعه، **RabbitMQ + Outbox Pattern** را به‌عنوان قابلیت‌های منجمد/الزامی تعریف کردهاند. کارفرما در MVP تعیین کرد: Integration فقط **REST API** است و Event-driven Integration در MVP پیاده‌سازی نمیشود؛ RabbitMQ و Outbox **Deferred** هستند. همچنین LDAP/AD در این پروژه وجود ندارد.

## Decision
- **MVP فقط REST:** ارائه خدمات ساختار سازمانی به سامانه‌های مصرف‌کننده از طریق REST API نسخه‌بندی‌شده (`/api/v1/...`).
- **RabbitMQ و Outbox Pattern در MVP پیاده‌سازی نمیشوند** (Deferred به Future Phase).
- **Event-driven Integration در MVP نیست.**
- **LDAP / Active Directory از Scope اجرایی حذف میشود** (Identity در IAM است).
- قابلیت‌های Deferred در Roadmap و اسناد معماری به‌عنوان «آینده» حفظ میشوند و افزودن آن‌ها نیازمند ADR جدید است.

## Rationale
- کاهش ریسک و زمان MVP یک‌ماهه با تمرکز بر قابلیت‌های هسته.
- Identity/Auth در IAM متمرکز است؛ نیاز به LDAP/AD منتفی است.
- REST برای سامانه‌های مصرفکننده در MVP کافی است.

## Considered and rejected alternatives
- پیاده‌سازی کامل Outbox/RabbitMQ در MVP: رد شد — افزایش دامنه و ریسک زمانی بدون نیاز تأییدشده.
- حذف کامل Outbox از اسناد: رد شد — به‌عنوان Future Phase حفظ میشود.

## Consequences
- مثبت: کاهش دامنه MVP، تمرکز بر Core، سرعت تحویل.
- منفی: انحراف موقت از Baseline (ثبت‌شده)؛ در صورت نیاز آینده به Event Integration، افزودن آن نیازمند ADR و کار اضافه است.
- هشدار: `Hangfire` برای Background Jobs (مثل Import) در MVP حفظ میشود و Deferred نیست.

**منابع:** `Docs/PROJECT-BASELINE-v0.1.md` (DEC-010)، `Docs/01-Architecture-Baseline-FA.md` (§۳.۱)، `Docs/02-AI-Development-Path-0-to-100-FA.md` (Phase 1)