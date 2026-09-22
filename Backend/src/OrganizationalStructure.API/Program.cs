using FluentValidation;
using Microsoft.AspNetCore.Authentication;
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