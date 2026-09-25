using MediatR;
using Microsoft.EntityFrameworkCore;
using OrganizationalStructure.Application.Authorization;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Common.Interfaces;
using OrganizationalStructure.Application.Organizations.DTOs;
using OrganizationalStructure.Domain.Abstractions;

namespace OrganizationalStructure.Application.Organizations.GetOrganizationStructure;

/// <summary>
/// پردازش‌گر دریافت ساختار کامل یک سازمان برای مصرف سامانه ارجاعات (§3.5).
/// </summary>
/// <remarks>
/// خروجی: لیست تخت پست‌ها با رابطه والد + مسئولیت‌های جاری + پرچم صاحب‌امضا.
/// مصرف‌کننده (سامانه ارجاعات) خود درخت را از روی این لیست می‌سازد.
/// </remarks>
public sealed class GetOrganizationStructureQueryHandler
    : IRequestHandler<GetOrganizationStructureQuery, Result<OrganizationStructureDto>>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUser _currentUser;

    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    public GetOrganizationStructureQueryHandler(IAppDbContext db, ICurrentUser currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    /// <inheritdoc />
    public async Task<Result<OrganizationStructureDto>> Handle(
        GetOrganizationStructureQuery request,
        CancellationToken cancellationToken)
    {
        // کنترل دسترسی سازمانی
        if (!_currentUser.VisibleOrganizationIds.Contains(request.OrganizationId))
        {
            return Result<OrganizationStructureDto>.Failure(AccessErrors.Forbidden());
        }

        // نام سازمان از محدوده کاربر
        var orgRef = _currentUser.VisibleOrganizations
            .FirstOrDefault(o => o.Id == request.OrganizationId);

        // خواندن پست‌ها
        var posts = await _db.Posts
            .AsNoTracking()
            .Where(p => p.OrganizationId == request.OrganizationId)
            .Select(p => new { p.Id, p.Code, p.Title, p.ParentId, p.IsActive })
            .ToListAsync(cancellationToken);

        // پست‌های صاحب‌امضا (انتساب جاری حق امضا)
        var signingPostIds = await _db.AuthorityAssignments
            .AsNoTracking()
            .Where(a => a.OrganizationId == request.OrganizationId
                && a.IsActive && a.EndDate == null)
            .Select(a => a.PostId)
            .ToListAsync(cancellationToken);
        var signingSet = signingPostIds.ToHashSet();

        // مسئولیت‌های جاری هر پست
        var responsibilityByPost = await (
            from ra in _db.ResponsibilityAssignments.AsNoTracking()
            join r in _db.Responsibilities.AsNoTracking() on ra.ResponsibilityId equals r.Id
            where ra.OrganizationId == request.OrganizationId
                && ra.IsActive && ra.EndDate == null
            select new { ra.PostId, Code = r.Code }
        ).ToListAsync(cancellationToken);

        var respMap = responsibilityByPost
            .GroupBy(x => x.PostId)
            .ToDictionary(g => g.Key, g => g.Select(x => x.Code).ToList());

        // ساخت نتیجه
        var postDtos = posts.Select(p => new OrganizationPostStructureDto
        {
            Id = p.Id,
            Code = p.Code,
            Title = p.Title,
            ParentId = p.ParentId,
            IsActive = p.IsActive,
            HasSigningAuthority = signingSet.Contains(p.Id),
            ResponsibilityCodes = respMap.TryGetValue(p.Id, out var codes)
                ? codes
                : Array.Empty<string>()
        }).ToList();

        var dto = new OrganizationStructureDto
        {
            OrganizationId = request.OrganizationId,
            OrganizationName = orgRef?.Name,
            Posts = postDtos
        };

        return Result<OrganizationStructureDto>.Success(dto);
    }
}
