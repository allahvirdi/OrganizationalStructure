using System.Threading.RateLimiting;
using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OrganizationalStructure.API.BackgroundJobs;
using OrganizationalStructure.API.Middleware;
using OrganizationalStructure.API.Security;
using OrganizationalStructure.Application;
using OrganizationalStructure.Domain.Abstractions;
using OrganizationalStructure.Infrastructure;
using OrganizationalStructure.Infrastructure.Security;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// حذف هدر Server (Kestrel) برای جلوگیری از افشای اطلاعات سرور
builder.WebHost.ConfigureKestrel(options => options.AddServerHeader = false);

builder.Host.UseSerilog((context, services, configuration) =>
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .WriteTo.Console());

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, HttpContextCurrentUser>();

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

builder.Services.AddScoped<OrganizationScopeResolver>();

builder.Services
    .AddAuthentication(AuthenticationSchemes.Smart)
    .AddPolicyScheme(
        AuthenticationSchemes.Smart,
        "انتخاب هوشمند کوکی/Bearer",
        options => options.ForwardDefaultSelector = AuthenticationSchemes.SelectScheme)
    .AddScheme<AuthenticationSchemeOptions, BffSessionAuthenticationHandler>(
        AuthenticationSchemes.Bff, null)
    .AddScheme<AuthenticationSchemeOptions, IamBearerAuthenticationHandler>(
        AuthenticationSchemes.IamBearer, null);

builder.Services.AddOrgAuthorization();

builder.Services.AddHostedService<StagingCleanupBackgroundService>();

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.OnRejected = async (context, ct) =>
    {
        context.HttpContext.Response.ContentType = "application/problem+json";
        await context.HttpContext.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Title = "درخواست بیش از حد مجاز",
            Detail = "لطفاً پس از چند ثانیه دوباره تلاش کنید.",
            Status = StatusCodes.Status429TooManyRequests
        }, ct);
    };
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1)
            }));
});

var app = builder.Build();

// هشدار صریح در صورت ناقص بودن پیکربندی IAM تا شکست خاموش ورود رخ ندهد.
var iamOptions = app.Services.GetRequiredService<IOptions<IamOptions>>().Value;
foreach (var missingIamKey in IamConfigurationValidator.GetMissingSettings(iamOptions))
{
    app.Logger.LogWarning(
        "تنظیم «{Key}» در پیکربندی IAM مقدار ندارد؛ تمام ورودها به‌صورت fail-closed رد می‌شوند.",
        missingIamKey);
}

app.UseExceptionHandling();
app.UseSecurityHeaders();
app.UseRateLimiter();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => Results.Ok(new { Name = "Organizational Structure API", Status = "OK" }))
    .AllowAnonymous();
app.MapHealthChecks("/health").AllowAnonymous();

app.Run();