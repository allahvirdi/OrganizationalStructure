using NetArchTest.Rules;
using OrganizationalStructure.API.Security;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Domain.Common;
using OrganizationalStructure.Infrastructure.Persistence;

namespace OrganizationalStructure.ArchitectureTests;

/// <summary>
/// تست‌های معماری (Fitness Functions) برای تضمین رعایت Clean Architecture و جریان وابستگی لایه‌ها.
/// </summary>
/// <remarks>
/// مطابق Architecture Baseline §۱۱ و ADR-005/ADR-002، جریان وابستگی باید به شکل زیر باشد:
/// <code>Domain ← Application ← Infrastructure ← API</code>
/// و هیچ وابستگی معکوسی مجاز نیست.
/// </remarks>
public sealed class LayerDependencyTests
{
    /// <summary>
    /// بررسی می‌کند که لایه Domain به هیچ لایه بیرونی (Application / Infrastructure / API) وابسته نیست.
    /// </summary>
    [Fact]
    public void Domain_Should_Not_DependOn_OuterLayers()
    {
        var result = Types.InAssembly(typeof(BaseEntity).Assembly)
            .That()
            .ResideInNamespace("OrganizationalStructure.Domain")
            .Should()
            .NotHaveDependencyOn("OrganizationalStructure.Application")
            .And()
            .NotHaveDependencyOn("OrganizationalStructure.Infrastructure")
            .And()
            .NotHaveDependencyOn("OrganizationalStructure.API")
            .GetResult();

        Assert.True(result.IsSuccessful,
            "لایه Domain نباید به Application یا Infrastructure یا API وابسته باشد.");
    }

    /// <summary>
    /// بررسی می‌کند که لایه Application فقط به Domain وابسته است و به Infrastructure یا API وابسته نیست.
    /// </summary>
    [Fact]
    public void Application_Should_Not_DependOn_InfrastructureOrApi()
    {
        var result = Types.InAssembly(typeof(Error).Assembly)
            .That()
            .ResideInNamespace("OrganizationalStructure.Application")
            .Should()
            .NotHaveDependencyOn("OrganizationalStructure.Infrastructure")
            .And()
            .NotHaveDependencyOn("OrganizationalStructure.API")
            .GetResult();

        Assert.True(result.IsSuccessful,
            "لایه Application نباید به Infrastructure یا API وابسته باشد.");
    }

    /// <summary>
    /// بررسی می‌کند که لایه Infrastructure به API وابسته نیست.
    /// </summary>
    [Fact]
    public void Infrastructure_Should_Not_DependOn_Api()
    {
        var result = Types.InAssembly(typeof(OrganizationalStructureDbContext).Assembly)
            .That()
            .ResideInNamespace("OrganizationalStructure.Infrastructure")
            .Should()
            .NotHaveDependencyOn("OrganizationalStructure.API")
            .GetResult();

        Assert.True(result.IsSuccessful,
            "لایه Infrastructure نباید به API وابسته باشد.");
    }

    /// <summary>
    /// بررسی می‌کند که کد لایه API مستقیماً به DbContext دسترسی ندارد
    /// (دسترسی به پایگاه داده فقط از طریق لایه Infrastructure مجاز است).
    /// </summary>
    [Fact]
    public void Api_Should_Not_Reference_DbContext_Directly()
    {
        var result = Types.InAssembly(typeof(HttpContextCurrentUser).Assembly)
            .That()
            .ResideInNamespace("OrganizationalStructure.API")
            .Should()
            .NotHaveDependencyOn(typeof(OrganizationalStructureDbContext).FullName!)
            .GetResult();

        Assert.True(result.IsSuccessful,
            "لایه API نباید مستقیماً به DbContext دسترسی داشته باشد.");
    }
}
