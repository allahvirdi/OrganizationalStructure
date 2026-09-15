using MediatR;
using Microsoft.EntityFrameworkCore;
using OrganizationalStructure.Application.Authorities.DTOs;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Common.Interfaces;
using OrganizationalStructure.Application.Posts.DTOs;
using OrganizationalStructure.Application.Responsibilities.DTOs;
using OrganizationalStructure.Domain.Constants;

namespace OrganizationalStructure.Application.Posts.GetPostById;

/// <summary>
/// پردازش‌گر پرس‌وجوی دریافت تفصیلی پست با شناسه (همراه انتساب‌های جاری).
/// </summary>
public sealed class GetPostByIdQueryHandler : IRequestHandler<GetPostByIdQuery, Result<PostDto>>
{
    private readonly IAppDbContext _db;

    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    public GetPostByIdQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    /// <inheritdoc />
    public async Task<Result<PostDto>> Handle(GetPostByIdQuery request, CancellationToken cancellationToken)
    {
        var post = await _db.Posts
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == request.PostId, cancellationToken);

        if (post is null)
        {
            return Result<PostDto>.Failure(PostErrors.NotFound(request.PostId));
        }

        var responsibilities = await (
            from a in _db.ResponsibilityAssignments.AsNoTracking()
            join r in _db.Responsibilities.AsNoTracking() on a.ResponsibilityId equals r.Id
            where a.PostId == post.Id && a.IsActive && a.EndDate == null
            orderby r.Code
            select new ResponsibilityAssignmentDto
            {
                Id = a.Id,
                ResponsibilityId = r.Id,
                ResponsibilityCode = r.Code,
                ResponsibilityTitle = r.Title,
                OrganizationId = a.OrganizationId,
                PostId = post.Id,
                PostCode = post.Code,
                PostTitle = post.Title,
                StartDate = a.StartDate,
                EndDate = a.EndDate,
                IsActive = a.IsActive
            }).ToListAsync(cancellationToken);

        var authorities = await (
            from a in _db.AuthorityAssignments.AsNoTracking()
            join u in _db.Authorities.AsNoTracking() on a.AuthorityId equals u.Id
            where a.PostId == post.Id && a.IsActive && a.EndDate == null
            orderby u.Code
            select new AuthorityAssignmentDto
            {
                Id = a.Id,
                AuthorityId = u.Id,
                AuthorityCode = u.Code,
                AuthorityTitle = u.Title,
                OrganizationId = a.OrganizationId,
                PostId = post.Id,
                PostCode = post.Code,
                PostTitle = post.Title,
                StartDate = a.StartDate,
                EndDate = a.EndDate,
                IsActive = a.IsActive
            }).ToListAsync(cancellationToken);

        var dto = new PostDto
        {
            Id = post.Id,
            OrganizationId = post.OrganizationId,
            Code = post.Code,
            Title = post.Title,
            Description = post.Description,
            ParentId = post.ParentId,
            IsActive = post.IsActive,
            HasSigningAuthority = authorities.Any(a =>
                string.Equals(a.AuthorityCode, AuthorityCodes.SigningAuthority, StringComparison.Ordinal)),
            Responsibilities = responsibilities,
            Authorities = authorities
        };

        return Result<PostDto>.Success(dto);
    }
}