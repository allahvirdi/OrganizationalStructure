using MediatR;
using Microsoft.EntityFrameworkCore;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Common.Interfaces;
using OrganizationalStructure.Domain.Abstractions;

namespace OrganizationalStructure.Application.Responsibilities.DisableResponsibility;

/// <summary>
/// پردازش‌گر دستور غیرفعال‌سازی مسئولیت سازمانی.
/// </summary>
/// <remarks>
/// غیرفعال‌سازی در صورت وجود انتساب جاری مجاز نیست (ابتدا انتساب‌ها پایان داده شوند).
/// </remarks>
public sealed class DisableResponsibilityCommandHandler
    : IRequestHandler<DisableResponsibilityCommand, Result>
{
    private readonly IAppDbContext _db;
    private readonly IClock _clock;

    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    public DisableResponsibilityCommandHandler(IAppDbContext db, IClock clock)
    {
        _db = db;
        _clock = clock;
    }

    /// <inheritdoc />
    public async Task<Result> Handle(
        DisableResponsibilityCommand request,
        CancellationToken cancellationToken)
    {
        var responsibility = await _db.Responsibilities
            .Include(r => r.Assignments)
            .FirstOrDefaultAsync(r => r.Id == request.ResponsibilityId, cancellationToken);

        if (responsibility is null)
        {
            return Result.Failure(ResponsibilityErrors.NotFound(request.ResponsibilityId));
        }

        if (responsibility.Assignments.Any(a => a.IsCurrent))
        {
            return Result.Failure(ResponsibilityErrors.HasActiveAssignments());
        }

        responsibility.Deactivate(_clock.UtcNow);
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}