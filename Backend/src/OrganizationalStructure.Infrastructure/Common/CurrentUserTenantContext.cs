using OrganizationalStructure.Domain.Abstractions;

namespace OrganizationalStructure.Infrastructure.Common;

/// <summary>
/// متن مستأجر جاری مبتنی بر <see cref="ICurrentUser"/>.
/// </summary>
/// <remarks>
/// در الگوی BFF (Q-002)، هویت و مستأجر از نشست احرازشده در HttpContext تأمین می‌شود.
/// این کلاس <see cref="ITenantContext"/> را از <see cref="ICurrentUser"/> نگاشت می‌کند.
/// </remarks>
public sealed class CurrentUserTenantContext : ITenantContext
{
    private readonly ICurrentUser _currentUser;

    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    /// <param name="currentUser">کاربر جاری</param>
    public CurrentUserTenantContext(ICurrentUser currentUser)
    {
        _currentUser = currentUser;
    }

    /// <inheritdoc />
    public Guid TenantId => _currentUser.TenantId;
}