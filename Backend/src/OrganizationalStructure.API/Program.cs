using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using OrganizationalStructure.API.Middleware;
using OrganizationalStructure.API.Security;
using OrganizationalStructure.Application;
using OrganizationalStructure.Domain.Abstractions;
using OrganizationalStructure.Infrastructure;
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

builder.Services
    .AddAuthentication(BffSessionAuthenticationHandler.SchemeName)
    .AddScheme<AuthenticationSchemeOptions, BffSessionAuthenticationHandler>(
        BffSessionAuthenticationHandler.SchemeName, null);

var app = builder.Build();

app.UseExceptionHandling();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => Results.Ok(new { Name = "Organizational Structure API", Status = "OK" }));
app.MapHealthChecks("/health");

app.Run();