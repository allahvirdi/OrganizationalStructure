# Session Report — Session-20260923-AuthoritySigningAuthority

**فاز:** رفع نقص پس از فاز ۸ (بدون تغییر فاز)
**تاریخ:** `2026-09-23`
**دستور کاربر:** «به جای ثبت اختیار، ثبت حق امضا ایجاد شود؛ کد مانند مسئولیت GUID و بدون نمایش در فرانت‌اند باشد؛ در تخصیص به پست دقیقاً مانند مسئولیت رفتار شود؛ پس از تغییر، اسناد به‌روز شود.»

---

## ۱. خلاصه Session
ماژول `Authorities` در لایهٔ UI از «اختیار» به **«حق امضا»** تغییر نام یافت و ساختار آن دقیقاً هم‌شکل ماژول مسئولیت شد: کد ورودی کاربر حذف و در بک‌اند به‌صورت خودکار معادل شناسه (GUID) تولید می‌شود، و انتساب به پست با دو دراپ‌داون جستجوپذیر (سازمان ← پست) انجام می‌گیرد.

پیامد مهمی که در این Session کشف و اصلاح شد: نشان «صاحب امضا» (`hasSigningAuthority`) به کد ثابت `SIGNING_AUTHORITY` گره خورده بود و با خودکارشدن کد (GUID) هرگز روشن نمی‌شد؛ مبنای آن به «وجود حداقل یک انتساب جاری حق امضا» تغییر کرد.

## ۲. Deliverables تکمیل‌شده
- [x] تغییر نام UI ماژول به «حق امضا» (منو، داشبورد، فهرست، صفحه ایجاد، صفحه جزئیات، برچسب‌های صفحه پست‌ها).
- [x] حذف `Code` از `CreateAuthorityCommand`/اعتبارسنج/هندلر و تولید خودکار `Code = Id.ToString()`.
- [x] اصلاح `CreatedAtAction` کنترلر برای استفاده از شناسهٔ تولیدشده.
- [x] حذف فیلد `code` از اسکیمای Zod، کلاینت API و فرم ایجاد؛ ریدایرکت با شناسهٔ برگشتی.
- [x] بازطراحی دیالوگ انتساب حق امضا به پست (سازمان + پست با جستجوی سروری، محدود به درخت دسترسی کاربر).
- [x] اصلاح مبنای `hasSigningAuthority` در سه هندلر پست + مستندسازی ثابت قدیمی.
- [x] حذف نمایش کد GUID از فرانت‌اند (ستون فهرست‌ها و زیرعنوان صفحات جزئیات).
- [x] به‌روزرسانی تست یکپارچگی و اجرای کامل مجموعهٔ تست‌ها/بیلدها.
- [x] به‌روزرسانی اسناد: `api-contracts/authorities.md`، `api-contracts/posts.md`، `api-contracts/responsibilities.md`، `decision-log.md` (DEC-035)، `adr/ADR-011`، `Architecture/external-integration.md`، `04_Progress.md`.

## ۳. فایل‌های ویرایش‌شده
| فایل | توضیح |
|---|---|
| `Backend/.../Authorities/ManageAuthorities/AuthorityCommands.cs` | حذف `Code` از دستور/اعتبارسنج؛ تولید خودکار GUID در هندلر؛ حذف بررسی کد تکراری |
| `Backend/.../API/Controllers/AuthoritiesController.cs` | `Location` بر پایهٔ شناسهٔ تولیدشده + به‌روزرسانی مستند XML |
| `Backend/.../Posts/GetPostById/GetPostByIdQueryHandler.cs` | `hasSigningAuthority` بر پایهٔ وجود انتساب جاری |
| `Backend/.../Posts/GetPostSubtree/GetPostSubtreeQueryHandler.cs` | حذف Join با `Authorities` و کد ثابت؛ نشان بر پایهٔ انتساب جاری |
| `Backend/.../Posts/SearchPosts/SearchPostsQueryHandler.cs` | همان تغییر نشان در فهرست/جستجو |
| `Backend/.../Posts/DTOs/PostDtos.cs` | به‌روزرسانی مستندات XML |
| `Backend/.../Domain/Constants/AuthorityCodes.cs` | ثبت توضیح «مرجع تاریخی» برای کد `SIGNING_AUTHORITY` |
| `Backend/tests/.../AuthorityApiTests.cs` | بازنویسی تست نشان امضا با کد خودکار |
| `Frontend/src/features/authorities/schemas.ts` + `api.ts` | حذف `code` از ورودی ایجاد |
| `Frontend/app/(protected)/authorities/new/page.tsx` | حذف فیلد کد + ریدایرکت با شناسه + عنوان «حق امضای جدید» |
| `Frontend/app/(protected)/authorities/[code]/page.tsx` | دیالوگ انتساب سازمان+پست؛ حذف زیرعنوان کد |
| `Frontend/app/(protected)/authorities/page.tsx` | عنوان/دکمه/جستجو + حذف ستون کد |
| `Frontend/app/(protected)/responsibilities/page.tsx` + `[code]/page.tsx` | حذف نمایش کد GUID (هم‌شکلی دو ماژول) |
| `Frontend/src/config/menu.tsx` + `app/(protected)/dashboard/page.tsx` | برچسب «حق امضاها» |
| `Frontend/app/(protected)/posts/new/page.tsx` + `posts/[id]/page.tsx` | برچسب‌های «حق امضا» در انتساب/فهرست پست |
| `Docs/api-contracts/authorities.md` | به‌روزرسانی کامل قرارداد (کد خودکار، حذف `DuplicateCode`) |
| `Docs/api-contracts/posts.md` | مبنای جدید `hasSigningAuthority` + حذف فیلد منسوخ از نمونهٔ درخواست ایجاد |
| `Docs/api-contracts/responsibilities.md` | حذف `DuplicateCode` از قرارداد خطا (هم‌راستا با کد خودکار) |
| `Docs/decision-log.md` | ثبت DEC-035 |
| `Docs/adr/ADR-011-responsibility-authority-separation.md` | یادداشت به‌روزرسانی DEC-035 |
| `Docs/Architecture/external-integration.md` | اصلاح نام‌گذاری «حق امضا»/GUID |
| `Docs/04_Progress.md` | بخش ادامه با شرح این تغییرات |

## ۴. تست‌ها (وضعیت نهایی — همه سبز)
| پروژه | تعداد | نتیجه |
|---|---|---|
| Domain.UnitTests | ۳۷ | ✅ |
| Application.UnitTests | ۹۸ | ✅ |
| Infrastructure.UnitTests | ۲۷ | ✅ |
| ArchitectureTests (Fitness Functions) | ۴ | ✅ |
| Api.IntegrationTests | ۸۱ | ✅ |
| **مجموع** | **۲۴۷** | ✅ |

بیلد بک‌اند: `Build succeeded` (۰ هشدار، ۰ خطا). فرانت‌اند: `tsc --noEmit` ✅ · `eslint` ✅ · `next build` ✅ (۱۶ روت شامل `/authorities`، `/authorities/[code]`، `/authorities/new`).

## ۵. تصمیم‌های ثبت‌شده
- **DEC-035:** تغییر نام UI به «حق امضا»، کد خودکار GUID و بدون نمایش، انتساب هم‌شکل مسئولیت، و تغییر مبنای `hasSigningAuthority` به «وجود انتساب جاری حق امضا». شناسه‌های فنی (مسیر `/api/v1/authorities`، موجودیت `Authority`، Permissionهای `OrganizationStructure.Authority.*`) بدون تغییر ماندند تا ثبت‌نام Permission در IAM نشکند.

## ۶. نکات باقی‌مانده / ریسک‌ها
- کد GUID در `GET /api/v1/authorities/{code}` باقی است؛ جستجوی انسانی روی کد بی‌معناست اما مسیرهای لینک داخلی فرانت‌اند سالم کار می‌کنند.
- در صورت نیاز آینده به تفکیک «حق امضا» از سایر انواع اختیار، افزودن فلگ دامنه‌ای `IsSigningAuthority` + Migration پیشنهاد می‌شود (در حال حاضر لازم نیست؛ همهٔ آیتم‌های ماژول «حق امضا» محسوب می‌شوند).
- تست HTTP زنده با IAM واقعی و آزمون تعاملی مرورگر در این Session انجام نشد (خارج از دامنهٔ تغییر).

## ۷. کامیت‌ها
| هش | پیام |
|---|---|
| `39e361a` | `feat(authority): rename to signing authority, auto-GUID code, org+post assignment (DEC-035)` |

کامیت با تأیید صریح کارفرما (DEC-002) انجام و به `origin/main` پوش شد.