# Phase 6 — تعریف پست، سازمان و والد

## درخواست و پیاده‌سازی
- نمایش سازمان پست در فرم ایجاد: شناسه organizationId نشست، فقط‌خواندنی. API نشست نام سازمان ندارد؛ نام ساختگی اضافه نشد.
- انتخاب والد با MUI Autocomplete، جستجوی سروری کد/عنوان، صفحه‌بندی ۲۰تایی و فیلتر سازمان نشست. والد اختیاری و نبود آن به معنی ریشه است.
- ارسال organizationId و parentId با قرارداد موجود POST /api/v1/posts؛ سازمان والد در کلاینت و هندلر سرور بررسی می‌شود.
- مجوز Create و View برای OrganizationStructureAdmin افزوده شد؛ SystemAdmin و مجوزهای صریح حفظ شدند. سایر مجوزها برای نقش جدید گسترش نیافتند؛ scope و tenant bypass نشدند.
- guard فرم و دکمه ایجاد و منوی پست‌ها هماهنگ شدند. جابه‌جایی پست‌های قبلی حذف نشده است.

## فایل‌های تغییرکرده (نسبت به شروع این درخواست)
- Frontend/app/(protected)/posts/new/page.tsx
- Frontend/app/(protected)/posts/page.tsx
- Frontend/src/lib/permissions.ts (جدید)
- Frontend/src/components/RequirePermission.tsx
- Frontend/src/config/menu.tsx
- Backend/src/OrganizationalStructure.API/Security/AuthorizationPolicies.cs
- Backend/src/OrganizationalStructure.API/Controllers/PostsController.cs
- Backend/tests/OrganizationalStructure.Api.IntegrationTests/PostsApiTests.cs
- Backend/tests/OrganizationalStructure.Api.IntegrationTests/TestAuthHandler.cs (هدر نقش فقط در پروژه تست)
- Docs/04_Progress.md، Docs/05_ChangeLog.md و همین گزارش.

## اعتبارسنجی
- tsc --noEmit، npm run lint (بدون هشدار)، npm run build موفق.
- API IntegrationTests: ۵۸ موفق؛ Application UnitTests: ۳۲ موفق، هر دو Release.
- تست‌های جدید دو نقش بدون permission، ایجاد والد/فرزند و بازیابی شناسه سازمان/والد، جستجوی والد و رد سازمان خارج scope و نقش غیرمجاز را پوشش می‌دهند.
- تست قدیمی جستجو با افزایش داده‌ها به صفحه اول وابسته بود؛ searchTerm دقیق اضافه شد.
- اجرای اولیه dotnet test در Backend به دلیل نبود solution انجام نشد؛ اجرای Debug به قفل فایل توسط API زنده و build موازی برخورد کرد. اجرای ترتیبی Release موفق بود؛ API زنده متوقف نشد.
- git diff --check بدون خطای whitespace (هشدار LF/CRLF).

## محدودیت و ادامه
- تست تعاملی فرم در مرورگر و آزمون نشست واقعی نقش OrganizationStructureAdmin انجام نشده است؛ نتیجه HTTP مربوط به میزبان تست است.
- API زنده برای دریافت policy جدید نیاز به rebuild/restart دارد؛ در این درخواست انجام نشد.
- نام سازمان نیازمند قرارداد معتبر IAM است؛ فعلاً شناسه نشان داده می‌شود.
- هیچ کامیتی برای این درخواست انجام نشد. تغییرات قبلی تم/مجوز/پوسته حفظ شده‌اند و بعضی فایل‌ها تغییرات مشترک دارند؛ هنگام کامیت دامنه بازبینی شود.
