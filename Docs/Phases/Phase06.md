# Phase 6 — Frontend Development

**وضعیت:** Not Started
**درصد پیشرفت:** 0%
**وابستگی‌ها:** Phase 3 + Phase 4

---

## ۱. اهداف فاز
- توسعه Frontend بر اساس استک منجمد (Next.js App Router + TS + TanStack Query + RHF + Zod + MUI RTL + Tailwind) و با رعایت SampleAdminPanel به‌عنوان مرجع بصری/UX.
- صفحات: چارت سازمانی، مدیریت Post، مدیریت پرسنل، انتساب، صاحب امضا/مسئولیت، مدیریت دسترسی.

## ۲. Deliverables
- [ ] اسکلت Next.js + MUI RTL + اتصال Auth (کوکی/Token از طریق Backend)
- [ ] صفحه چارت سازمانی (Tree + نمایش صاحب امضا/مسئولیت متمایز)
- [ ] مدیریت Post (CRUD + جابجایی + فعال/غیرفعال)
- [ ] مدیریت Employee (CRUD + PII + انتساب چندپستی)
- [ ] مدیریت Responsibility/SigningAuthority
- [ ] مدیریت User/Role/Permission/Scope
- [ ] Permission Guards
- [ ] حالت‌های Loading/Error/Empty

## ۳. Definition of Done
- [ ] `npm run build`, `tsc --noEmit`, lint پاس شده
- [ ] Permission Guard پیاده‌سازی شده
- [ ] RTL فارسی صحیح

## ۴. پیش‌نیازها
- Phase 3 + Phase 4
- API Contracts مصوب

---

## تسک‌های ≤ ۱۵ دقیقه (اولویت اصلی — DEC-026)
1. `اسکلت Next.js + TS + MUI RTL + Tailwind + TanStack/RHF/Zod` (استک DEC-003)
2. `Auth: ورود BFF (کوکی) + Guard صفحات + مدیریت نشست`
3. `انتخاب کتابخانه Tree چارت (ADR در صورت انحراف از MUI)`
4. `صفحه چارت سازمانی (درخت + نشان امضا/مسئولیت)`
5. `صفحات Post (فهرست/ایجاد/ویرایش/جابجایی/وضعیت)`
6. `صفحات Employee (فهرست/ثبت/ویرایش/انتساب/وضعیت) + ماسک PII`
7. `صفحات Responsibility/Authority (مدیریت + انتساب)`
8. `تست‌ها + Progress/ChangeLog/SessionReport + بستن فاز ۶`