using OrganizationalStructure.Application.Authorization;
using OrganizationalStructure.Application.Integration.Iam;
using OrganizationalStructure.Domain.Abstractions;

namespace OrganizationalStructure.API.Security;

/// <summary>
/// حل‌کننده مشترک Scope سازمانی از درخت IAM (مورد استفاده هر دو هندلر BFF و Bearer).
/// </summary>
public sealed class OrganizationScopeResolver
{
    private readonly IIamClient _iam;

    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    /// <param name="iam">کلاینت IAM</param>
    public OrganizationScopeResolver(IIamClient iam)
    {
        _iam = iam;
    }

    /// <summary>
    /// حل Scope (خود سازمان + زیرمجموعه‌ها) با نام/کد؛ خالی در صورت شکست (fail-closed در مصرفکننده).
    /// </summary>
    /// <param name="accessToken">توکن دسترسی برای واکشی درخت</param>
    /// <param name="organizationId">شناسه سازمان کاربر</param>
    /// <param name="cancellationToken">توکن لغو</param>
    /// <returns>مراجع سازمان‌های داخل Scope</returns>
    public async Task<IReadOnlyList<OrganizationReference>> ResolveScopeAsync(
        string accessToken,
        string? organizationId,
        CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(organizationId, out var orgId))
        {
            return Array.Empty<OrganizationReference>();
        }

        var tree = await _iam.GetOrganizationTreeAsync(accessToken, cancellationToken);
        return OrganizationScope.ComputeVisibleOrganizations(tree, orgId);
    }
}