using MediatR;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Organizations.DTOs;
using OrganizationalStructure.Domain.Abstractions;

namespace OrganizationalStructure.Application.Organizations.GetOrganizationOptions;

/// <summary>
/// پردازش‌گر پرس‌وجوی گزینه‌های سازمان.
/// </summary>
/// <remarks>
/// این پرس‌وجو هیچ دسترسی مستقیم به پایگاه‌داده ندارد؛ منبع آن محدوده سازمانی کاربر جاری است
/// که در زمان ورود/اعتبارسنجی از درخت IAM محاسبه و در نشست نگهداری می‌شود
/// (Master سازمان در IAM است — ADR-002). بنابراین خروجی هرگز از محدوده کاربر فراتر نمی‌رود.
/// </remarks>
public sealed class GetOrganizationOptionsQueryHandler
    : IRequestHandler<GetOrganizationOptionsQuery, Result<IReadOnlyList<OrganizationOptionDto>>>
{
    private readonly ICurrentUser _currentUser;

    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    /// <param name="currentUser">کاربر جاری (منبع محدوده سازمانی).</param>
    public GetOrganizationOptionsQueryHandler(ICurrentUser currentUser)
    {
        _currentUser = currentUser;
    }

    /// <inheritdoc />
    public Task<Result<IReadOnlyList<OrganizationOptionDto>>> Handle(
        GetOrganizationOptionsQuery request,
        CancellationToken cancellationToken)
    {
        var currentOrganizationId = _currentUser.OrganizationId;
        var term = request.SearchTerm?.Trim();

        var options = _currentUser.VisibleOrganizations
            .Where(organization =>
                string.IsNullOrEmpty(term)
                || organization.Name.Contains(term, StringComparison.OrdinalIgnoreCase)
                || organization.Code.Contains(term, StringComparison.OrdinalIgnoreCase))
            .OrderBy(organization => organization.Depth)
            .ThenBy(organization => organization.Name, StringComparer.CurrentCulture)
            .Select(organization => new OrganizationOptionDto(
                organization.Id,
                organization.Name,
                organization.Code,
                organization.ParentId,
                organization.Depth,
                currentOrganizationId == organization.Id))
            .ToArray();

        return Task.FromResult(Result<IReadOnlyList<OrganizationOptionDto>>.Success(options));
    }
}
