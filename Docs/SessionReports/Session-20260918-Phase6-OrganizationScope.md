# Phase 6 — نام سازمان در UI و یکسان‌سازی محدوده سازمانی

## درخواست و پیاده‌سازی
- مشکل قبلی: فرم ایجاد پست سازمان را با «شناسه خام» (`organizationId` نشست) نشان می‌داد و نام سازمان در قرارداد نشست موجود نبود.
- راه‌حل: Endpoint جدید `GET /api/v1/organizations` (فقط احراز هویت، بدون Permission دامنه‌ای) که سازمان‌های مجاز کاربر را با **نام/کد/والد/عمق/`isCurrent`** برمی‌گرداند؛ منبع داده همان محدوده سازمانی نشست است (Master سازمان در IAM — ADR-002) و پایگاه‌داده‌ای در این سرویس درگیر نمی‌شود.
- Scope مشاهده یکدست شد: `OrganizationScope.ComputeVisibleOrganizations` همیشه «خود سازمان کاربر (عمق ۰) + تمام زیرمجموعه‌ها» را برمی‌گرداند و در نشست علاوه بر `organization_scope` (شناسه‌ها) به‌صورت `organization_scope_node` (هر مقدار یک JSON شامل شناسه/نام/کد/والد/عمق) هم نگهداری می‌شود؛ JSON مانع شکست روی نویسه‌های خاص در نام سازمان می‌شود (`OrganizationScopeClaim`).
- حل Scope در `OrganizationScopeResolver` مشترک هندلرهای BFF و Bearer شد تا هر دو مسیر احراز هویت یک Scope تولید کنند، نه دو پیاده‌سازی موازی.
- فرم ایجاد پست: انتخاب سازمان با Autocomplete جستجوپذیر روی نام/کد (جستجوی سروری)، نمایش تودرتو با تورفتگی بر اساس عمق، پیش‌فرض «سازمان خود کاربر»، پاک‌شدن والد با تغییر سازمان، و پیام هشدار وقتی سازمانی برای انتخاب وجود ندارد (اجرای ثبت غیرفعال می‌شود).
- امنیت: خروجی Endpoint هرگز از محدوده کاربر فراتر نمی‌رود؛ نبود سازمان کاربر در درخت IAM ⇒ فهرست خالی (fail-closed)؛ Fallback Deny و Policyهای دامنه‌ای پست‌ها دست‌نخورده ماندند. سازمان خارج از محدوده در جستجو/ایجاد پست `403 Access.Forbidden` می‌دهد.

## فایل‌های این درخواست
### Backend — جدید
- `Backend/src/OrganizationalStructure.API/Controllers/OrganizationsController.cs`
- `Backend/src/OrganizationalStructure.API/Security/OrganizationScopeClaim.cs`
- `Backend/src/OrganizationalStructure.Application/Organizations/**` (DTO + Query/Handler/Validator)
- `Backend/src/OrganizationalStructure.Domain/Abstractions/OrganizationReference.cs`
### Backend — تغییر
- `Application/Authorization/OrganizationScope.cs`، `Application/Integration/Iam/{IBffSessionStore,IIamClient}.cs`
- `Domain/Abstractions/ICurrentUser.cs`
- `API/Security/{OrganizationScopeResolver,ClaimNames,HttpContextCurrentUser,BffSessionAuthenticationHandler,IamBearerAuthenticationHandler,AuthorizationPolicies}.cs`
- `API/Controllers/AuthController.cs` (ذخیره مراجع سازمان در نشست)
### Frontend — جدید
- `Frontend/src/features/organizations/api.ts`، `Frontend/src/features/organizations/useOrganizations.ts`
### Frontend — تغییر
- `Frontend/app/(protected)/posts/new/page.tsx`
### تست — جدید
- `OrganizationsApiTests.cs`، `OrganizationOptionsTests.cs`، `SystemAdminPolicyTests.cs`
### تست — تغییر
- `OrganizationScopeTests.cs`، `TestDoubles.cs`، `IamClientTests.cs`، `TestAuthHandler.cs`، `BffHandlerTests.cs`، `PostsApiTests.cs`
### اسناد
- جدید: `Docs/api-contracts/organizations.md`، همین گزارش
- تغییر: `Docs/api-contracts/README.md`، `Docs/api-contracts/posts.md`، `Docs/Architecture/iam-integration.md`، `Docs/04_Progress.md`، `Docs/05_ChangeLog.md`

> توجه: `Frontend/app/(protected)/posts/page.tsx`، `Frontend/app/page.tsx`، `Frontend/app/login/page.tsx`، `Frontend/src/theme/theme.ts`، `Frontend/src/components/*`، `Frontend/src/config/menu.tsx`، `Frontend/src/features/employees/usePermissions.ts` و `Backend/.../AuthorizationPolicies.cs` تغییرات کامیت‌نشدهٔ Sessionهای قبلی (تم/لندینگ/مجوز) را هم در خود دارند.

## اعتبارسنجی
- `npx tsc --noEmit` بدون خطا؛ `npm run lint` بدون هشدار؛ `npm run build` موفق (۱۵ مسیر تولید شد).
- `dotnet build` پروژه API در Release: ۰ خطا، ۰ هشدار.
- تست‌ها در Release: Domain ۳۵، Application ۳۹، Infrastructure ۲۴، Architecture ۴، API Integration ۶۶ — جمعاً ۱۶۸ تست، همه سبز.
- پوشش تست‌های جدید: نام/کد/عمق/والد و `isCurrent`، نبود سازمان بیگانه در فهرست، جستجو روی نام و کد، `400` برای `searchTerm` بلند، یکسان‌بودن پاسخ برای نقش‌های مختلف، و اعتبارسنجی محاسبه مراجع سازمان در `OrganizationScopeTests`/`IamClientTests`/`BffHandlerTests`.

## محدودیت و ادامه
- تست تعاملی مرورگر (انتخاب سازمان و سپس والد) با نشست واقعی IAM انجام نشده است؛ شواهد HTTP مربوط به میزبان تست (`TestAuthHandler`) است.
- نام سازمان از درخت IAM می‌آید؛ اگر IAM در دسترس نباشد ورود fail-closed می‌شود (تغییر عمدی، بدون کش نام سازمان).
- هیچ کامیتی در این ادامه انجام نشد؛ دامنهٔ کامیت هنگام بازبینی باید از تغییرات قبلیِ کامیت‌نشده (تم/مجوز) تفکیک شود.
