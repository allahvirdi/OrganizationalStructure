using System.Security.Claims;
using OrganizationalStructure.Domain.Abstractions;

namespace OrganizationalStructure.API.Security;

/// <summary>
/// پیاده‌سازی کاربر جاری بر پایه <see cref="HttpContext"/> و ادعاهای هویت معتبر (الگوی BFF).
/// </summary>
/// <remarks>
/// در الگوی BFF (Q-002) مرورگر تنها کوکی نشست دارد و این سامانه هویت را از IAM اعتبارسنجی می‌کند؛
/// ادعاهای احرازشده به <see cref="HttpContext.User"/> تزریق و از اینجا خوانده می‌شوند.
/// </remarks>
public sealed class HttpContextCurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    /// <param name="httpContextAccessor">دسترس‌دهنده HttpContext</param>
    public HttpContextCurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <inheritdoc />
    public bool IsAuthenticated =>
        _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated == true;

    /// <inheritdoc />
    public Guid UserId => ReadGuidClaim(ClaimNames.UserId) ?? Guid.Empty;

    /// <inheritdoc />
    public Guid TenantId => ReadGuidClaim(ClaimNames.TenantId) ?? Guid.Empty;

    /// <inheritdoc />
    public Guid? OrganizationId => ReadGuidClaim(ClaimNames.OrganizationId);

    /// <inheritdoc />
    public IReadOnlyCollection<string> Roles => ReadRoles();

    /// <inheritdoc />
    public IReadOnlyCollection<Guid> VisibleOrganizationIds => ReadScope();

    /// <inheritdoc />
    public IReadOnlyCollection<OrganizationReference> VisibleOrganizations => ReadVisibleOrganizations();

    private Guid? ReadGuidClaim(string claimType)
    {
        var value = _httpContextAccessor.HttpContext?.User.FindFirstValue(claimType);
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return Guid.TryParse(value, out var guid) ? guid : null;
    }

    private IReadOnlyCollection<string> ReadRoles()
    {
        if (_httpContextAccessor.HttpContext?.User is not ClaimsPrincipal principal)
        {
            return Array.Empty<string>();
        }

        var roles = principal.FindAll(ClaimNames.Role)
            .Select(c => c.Value)
            .Distinct(StringComparer.Ordinal)
            .ToArray();

        return roles;
    }

    private IReadOnlyCollection<Guid> ReadScope()
    {
        if (_httpContextAccessor.HttpContext?.User is not ClaimsPrincipal principal)
        {
            return Array.Empty<Guid>();
        }

        return principal.FindAll(ClaimNames.OrganizationScope)
            .Select(c => c.Value)
            .Where(v => Guid.TryParse(v, out _))
            .Select(Guid.Parse)
            .Distinct()
            .ToArray();
    }

    private IReadOnlyCollection<OrganizationReference> ReadVisibleOrganizations()
    {
        if (_httpContextAccessor.HttpContext?.User is not ClaimsPrincipal principal)
        {
            return Array.Empty<OrganizationReference>();
        }

        return principal.FindAll(ClaimNames.OrganizationScopeNode)
            .Select(claim => OrganizationScopeClaim.Parse(claim.Value))
            .Where(reference => reference is not null)
            .Select(reference => reference!)
            .DistinctBy(reference => reference.Id)
            .ToArray();
    }
}