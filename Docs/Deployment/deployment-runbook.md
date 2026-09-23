# مستندات استقرار و Runbook — OrganizationalStructure API

> سند عملیاتی برای استقرار، پیکربندی، پشتیبان‌گیری، بازیابی و عیب‌یابی سرویس بک‌اند.
> فرانت‌اند (Next.js) به‌صورت ایستا (`next build`) یا روی یک سرویس‌دهندهٔ Node اجرا می‌شود و فقط پروکسی به بک‌اند است.

---

## ۱. معماری استقرار

```
کاربر ──► فرانت‌اند (Next.js) ──► بک‌اند (.NET 10 API)
                                     │
                                     ├──► SQL Server (دیتابیس چندمستأجری)
                                     └──► Redis (اختیاری — نشست‌های چندنمونه‌ای)
```

| مؤلفه | فناوری | توضیح |
|---|---|---|
| بک‌اند | .NET 10 / ASP.NET Core (Kestrel) | Clean Architecture، چندمستأجری با فیلترهای سراسری |
| دیتابیس | SQL Server (یا LocalDB برای توسعه) | Migrationهای EF Core |
| Redis | اختیاری | فقط اگر بیش از یک نمونهٔ بک‌اند اجرا می‌شود |
| فرانت‌اند | Node.js 22 / Next.js | پروکسی درخواست‌ها به بک‌اند (مطابق `next.config.ts`) |
| CI/CD | GitHub Actions | `.github/workflows/ci.yml` — هر پوش به `main` |

---

## ۲. پیش‌نیازهای محیطی

| مورد | نسخه/شرط |
|---|---|
| .NET SDK/Runtime | 10.x |
| SQL Server | 2019+ (یا LocalDB برای توسعه) |
| Redis | 6+ — فقط در حالت چندنمونه‌ای |
| پورت پیش‌فرض توسعه | `5197` (در `launchSettings.json`) — **توجه:** از پورتهای رزروشدهٔ محدودهٔ 5272–5371 ویندوز استفاده نکنید |

---

## ۳. پیکربندی (appsettings / متغیرهای محیطی)

کلیدها را در `appsettings.Production.json` یا متغیرهای محیطی مقدار دهید. هیچ‌گاه مقدار واقعی را در مخزن ذخیره نکنید.

| کلید | اجباری؟ | توضیح |
|---|---|---|
| `ConnectionStrings:OrganizationalStructureDb` | ✅ بله | رشتهٔ اتصال دیتابیس اصلی |
| `ConnectionStrings:Redis` | اختیاری | در صورت تنظیم → `RedisBffSessionStore`؛ در غیر این صورت `InMemoryBffSessionStore` (فقط تک‌نمونه) |
| `Iam:*` (طبق `IamOptions`) | ✅ بله | پیکربندی IAM. **هشدار:** در صورت ناقص بودن، تمام ورودها به‌صورت امن (fail-closed) رد می‌شوند و در لاگ هنگام راه‌اندازی هشدار داده می‌شود |
| `ASPNETCORE_URLS` | اختیاری | آدرس گوش‌دادن (پیشنهاد: پشت پروکسی معکوس) |
| `Serilog:MinimumLevel:Default` | اختیاری | پیش‌فرض `Information` |

---

## ۴. مراحل استقرار (پاسخ‌گو به‌صورت Publish)

```bash
# ۱. بازیابی و انتشار
cd Backend/src
dotnet publish OrganizationalStructure.API -c Release -o ./artifacts

# ۲. اعمال مایگریشن‌ها (روی دیتابیس هدف)
dotnet ef database update --project OrganizationalStructure.Infrastructure --startup-project OrganizationalStructure.API

# ۳. اجرا (بهتر است به‌صورت سرویس یا در کانتینر)
cd artifacts
dotnet OrganizationalStructure.API.dll
```

### وارسی سلامت (Health Check)

```bash
curl -fsS http://<host>:<port>/health
# انتظار: HTTP 200 + "Healthy"
```

---

## ۵. اقدامات امنیتی فعال (تسک ۴)

- **Rate Limit سراسری:** ۱۰۰ درخواست در دقیقه برای هر IP؛ پاسخ 429 با RFC 7807.
- **هدرهای امنیتی (میان‌افزار `SecurityHeadersMiddleware`):**
  - `X-Content-Type-Options: nosniff`
  - `X-Frame-Options: DENY`
  - `X-XSS-Protection: 0`
  - `Referrer-Policy: strict-origin-when-cross-origin`
  - `Permissions-Policy: camera=(), microphone=(), geolocation=()`
  - `Content-Security-Policy: default-src 'self'; frame-ancestors 'none'; base-uri 'self'; form-action 'self'`
- **حذف هدر `Server`** (Kestrel) برای عدم افشای فناوری.
- **مدیریت خطای یکپارچه:** تمام خطاها به `application/problem+json` تبدیل می‌شوند و جزئیات داخلی به کاربر نمایش داده نمی‌شود.

---

## ۶. پشتیبان‌گیری و بازیابی (Backup / DR)

| مورد | سیاست |
|---|---|
| دیتابیس | پشتیبان کامل روزانه (Full) + پشتیبان لاگ تراکنش هر ۱۵ دقیقه؛ نگهداری کامل ۷ روز، هفتگی ۴ هفته |
| فایل‌های بارگذاری‌شده (Import) | همگام با پشتیبان دیتابیس |
| پیکربندی/رمزها | در مخزن امن جداگانه یا مدیریت رمز؛ همراه با روال چرخش |
| RPO | حداکثر ۱۵ دقیقه |
| RTO | حداکثر ۱ ساعت |

**روال بازیابی:**
1. بازیابی آخرین پشتیبان کامل، سپس اعمال لاگ‌های تراکنش.
2. اجرای `dotnet ef database update` برای اطمینان از هم‌راستایی مایگریشن‌ها.
3. وارسی `/health` و تست دستی یک سناریوی خواندن/نوشتن.

---

## ۷. عیب‌یابی سریع (Troubleshooting)

| علامت | علل رایج | اقدام |
|---|---|---|
| خطای `SocketException 10013` | پورت در محدودهٔ رزروی ویندوز | `netsh interface ipv4 show excludedportrange protocol=tcp` — پورت را تغییر دهید |
| همهٔ ورودها 401/403 | تنظیمات `Iam:*` خالی | هشدارهای «تنظیم … مقدار ندارد» را در لاگ راه‌اندازی بررسی و رفع کنید |
| خطاهای نشست در چند نمونه | `ConnectionStrings:Redis` تنظیم نشده | اتصال Redis را پیکربندی کنید؛ سپس سرویس‌ها را راه‌اندازی مجدد کنید |
| پاسخ 429 مکرر | نرخ واقعی بالاتر از حد مجاز | محدودیت `PermitLimit` در `Program.cs` را بر اساس نیاز بازنگری کنید |
| خطای ۵۰۰ با جزئیات مبهم | استثناهای مدیریت‌نشده | لاگ‌های `Serilog` را با کلیدواژهٔ «Unhandled exception» بررسی کنید |

---

## ۸. SLO و هشداردهی (پیشنهادی)

| شاخص | هدف |
|---|---|
| در دسترس بودن (`/health`) | ≥ 99.5% ماهانه |
| پاسخ ۹۵امینک درخواست‌ها | < 800ms |
| نرخ خطای ۵۰۰ | < 1% |
| هشدار | `health` سه بار متوالی ناموفق → اعلان فوری |

---

## ۹. چک‌لیست پیش از استقرار

- [ ] گذر کامل پایپ‌لاین CI (بیلد + ۵ پروژهٔ تست + اسکن آسیب‌پذیری)
- [ ] اعمال مایگریشن‌ها روی دیتابیس هدف و وارسی
- [ ] مقداردهی کامل `Iam:*` و رشته‌های اتصال در محیط مقصد
- [ ] وارسی `/health` و پاسخ صحیح هدرهای امنیتی
- [ ] اطمینان از پیکربندی پشتیبان‌گیری دیتابیس
- [ ] ثبت تغییرات در `Docs/05_ChangeLog.md`