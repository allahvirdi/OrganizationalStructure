using MediatR;
using Microsoft.EntityFrameworkCore;
using OrganizationalStructure.Application.Authorization;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Common.Interfaces;
using OrganizationalStructure.Application.Import.DTOs;
using OrganizationalStructure.Application.Posts;
using OrganizationalStructure.Domain.Abstractions;
using OrganizationalStructure.Domain.Entities;

namespace OrganizationalStructure.Application.Import;

/// <summary>
/// پردازش‌گر دستور بارگذاری ساختار پست‌ها از فایل اکسل.
/// </summary>
/// <remarks>
/// عملیات اتمیک است: اگر هر ردیف خطا داشته باشد کل بارگذاری رد می‌شود.
/// سلسله‌مراتب با کد والد ساخته می‌شود (والد در فایل یا دیتابیس موجود).
/// </remarks>
public sealed class ImportPostsCommandHandler : IRequestHandler<ImportPostsCommand, Result<ImportPostsResultDto>>
{
    private readonly IAppDbContext _db;
    private readonly IClock _clock;
    private readonly ICurrentUser _currentUser;

    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    public ImportPostsCommandHandler(
        IAppDbContext db,
        IClock clock,
        ICurrentUser currentUser)
    {
        _db = db;
        _clock = clock;
        _currentUser = currentUser;
    }

    /// <inheritdoc />
    public async Task<Result<ImportPostsResultDto>> Handle(
        ImportPostsCommand request,
        CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId;

        // بررسی محدوده سازمانی
        if (!_currentUser.VisibleOrganizationIds.Contains(request.OrganizationId))
        {
            return Result<ImportPostsResultDto>.Failure(AccessErrors.Forbidden());
        }

        // بارگذاری پست‌های موجود سازمان برای بررسی تکراری و والد
        var existingPosts = await _db.Posts
            .Where(p => p.OrganizationId == request.OrganizationId)
            .Select(p => new { p.Code, p.Id })
            .ToListAsync(cancellationToken);

        var existingCodes = new HashSet<string>(
            existingPosts.Select(p => p.Code),
            StringComparer.OrdinalIgnoreCase);

        var existingCodeToId = existingPosts.ToDictionary(
            p => p.Code,
            p => p.Id,
            StringComparer.OrdinalIgnoreCase);

        // بررسی تکراری بودن کدها با دیتابیس
        foreach (var row in request.Rows)
        {
            if (existingCodes.Contains(row.Code))
            {
                return Result<ImportPostsResultDto>.Failure(
                    PostErrors.DuplicateCode(row.Code));
            }
        }

        // ساخت نگاشت کد به شناسه برای والد‌های درون فایل
        var fileCodeToId = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);
        foreach (var row in request.Rows)
        {
            fileCodeToId[row.Code] = Guid.NewGuid();
        }

        // اعتبارسنجی والد‌ها
        for (var i = 0; i < request.Rows.Count; i++)
        {
            var row = request.Rows[i];
            if (row.ParentCode is not null)
            {
                var parentExistsInFile = fileCodeToId.ContainsKey(row.ParentCode);
                var parentExistsInDb = existingCodeToId.ContainsKey(row.ParentCode);

                if (!parentExistsInFile && !parentExistsInDb)
                {
                    return Result<ImportPostsResultDto>.Failure(
                        ImportErrors.ParentNotFound(i + 1, row.ParentCode));
                }
            }
        }

        // ایجاد پست‌ها
        var createdPosts = new List<Post>(request.Rows.Count);
        foreach (var row in request.Rows)
        {
            Guid? parentId = null;
            if (row.ParentCode is not null)
            {
                parentId = fileCodeToId.TryGetValue(row.ParentCode, out var fileParentId)
                    ? fileParentId
                    : existingCodeToId[row.ParentCode];
            }

            var post = Post.Create(
                fileCodeToId[row.Code],
                tenantId,
                request.OrganizationId,
                row.Code,
                row.Title,
                row.Description,
                parentId,
                _clock.UtcNow);

            createdPosts.Add(post);
        }

        foreach (var post in createdPosts)
        {
            _db.Posts.Add(post);
        }

        await _db.SaveChangesAsync(cancellationToken);

        return Result<ImportPostsResultDto>.Success(
            new ImportPostsResultDto(request.OrganizationId, createdPosts.Count));
    }
}