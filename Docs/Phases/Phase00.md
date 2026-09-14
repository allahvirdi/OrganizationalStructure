# Phase 0 — Project Baseline

> قالب استاندارد فاز طبق `Operational-Templates/PhaseXX_Template.md`.

**وضعیت:** In Progress
**درصد پیشرفت:** ~۸۰٪
**وابستگی‌ها:** —

---

## ۱. اهداف فاز
- شناخت کامل اسناد موجود و Repository.
- تثبیت Context کسب‌وکار و مرز Master Data با IAM.
- تکمیل Domain Vision، Bounded Contextها و ثبت ابهامات.
- ایجاد ساختار ریشه (`Docs` / `Backend` / `Frontend`) و اسناد عملیاتی `00` تا `06`.
- تکمیل `PROJECT-BASELINE-v0.1.md`، `decision-log.md`، `open-questions.md`.
- ثبت ADRهای اولیه (ADR-001 تا ADR-009).
- ایجاد اسکلت `Phases/`، `SessionReports/`، `domain/`، `adr/`، `Architecture/`، `api-contracts/`.

## ۲. Deliverables (خروجی‌های قابل تحویل)
- [x] ساختار ریشه پروژه (Docs/Backend/Frontend + فایل‌های ریشه)
- [x] اسناد عملیاتی `00` تا `06`
- [x] `Docs/PROJECT-BASELINE-v0.1.md`
- [x] `Docs/decision-log.md` + `Docs/open-questions.md`
- [x] ADR-001 تا ADR-009
- [x] اسناد دامنه (`Docs/domain/`)
- [x] `Docs/Phases/Phase00.md` + اسکلت فازهای بعدی
- [ ] `Docs/Architecture/` + `Docs/api-contracts/` (اسکلت)
- [ ] اولین `SessionReport`
- [ ] کامیت پس از تأیید انسان

## ۳. Definition of Done (فاز)
- [ ] تمام Deliverables تکمیل شده
- [ ] قابلیت ادامه توسط فرد/AI جدید بدون تاریخچه گفتگو
- [ ] Progress و Session Report به‌روز است
- [ ] تمام تسک‌ها با قانون ۱۵ دقیقه‌ای اجرا و کامیت‌ها پس از تأیید انسان ثبت شده‌اند

## ۴. پیش‌نیازها و وابستگی‌ها
- آماده بودن پاسخ‌های کارفرما به C-1..C-9 (انجام شد).
- دسترسی فقط‌خواندنی به `Enterprise-IAM-V2`.

## ۵. ریسک‌ها
| ریسک | احتمال | تأثیر | mitigation |
|------|--------|-------|------------|
| نگاشت چهار سطح بدون Region=3 در IAM | بالا | بالا | هماهنگی مالک IAM (Q-001) |
| همپوشانی Employee با IAM | متوسط | بالا | ADR-003 + حل Q-007 |

## ۶. لیست کلاس‌ها / پروژه‌های موردنیاز
- (Phase 0 فقط اسناد؛ بدون کد)

## ۷. APIها / DTOها / Commands / Queries / Validators
- (Phase 1/2)

## ۸. Eventها / Background Jobs (Hangfire)
- (Phase 2/7 — Import)

## ۹. تست‌های موردنیاز
- (Phase 1 به بعد)

## ۱۰. شکست به تسک‌های ≤ ۱۵ دقیقه‌ای
1. `تسک ۱: ساختار ریشه + فایل‌های ریشه` — انجام شد
2. `تسک ۲: اسناد 00..06` — انجام شد
3. `تسک ۳: PROJECT-BASELINE` — انجام شد
4. `تسک ۴: decision-log + open-questions` — انجام شد
5. `تسک ۵: ADR-001..009` — انجام شد (کامیت آتی)
6. `تسک ۶: domain docs` — انجام شد
7. `تسک ۷: Phases (Phase00 + اسکلت)` — در حال اجرا
8. `تسک ۸: Architecture + api-contracts` — برنامه‌ریزی‌شده
9. `تسک ۹: اولین SessionReport` — برنامه‌ریزی‌شده
10. `تسک ۱۰: به‌روزرسانی Progress + ChangeLog` — برنامه‌ریزی‌شده

## ۱۱. فایل‌های ایجاد/تغییر یافته (در طول فاز پر شود)
- `.gitignore`, `.editorconfig`, `README.md`
- `Docs/00_ProjectContext.md`, `Docs/01_Architecture.md`, `Docs/02_CodingStandards.md`, `Docs/03_Roadmap.md`, `Docs/04_Progress.md`, `Docs/05_ChangeLog.md`, `Docs/06_DevelopmentRules.md`
- `Docs/PROJECT-BASELINE-v0.1.md`, `Docs/decision-log.md`, `Docs/open-questions.md`
- `Docs/adr/ADR-001..009`
- `Docs/domain/*` (فعال‌سازی)

## ۱۲. نکات خاص فاز
- خروجی اصلی این فاز «تثبیت Context» است، نه کد.
- انتقال ابهامات کسب‌وکاری به `open-questions.md` و تصویب تصمیمات در `decision-log.md` الزامی است.