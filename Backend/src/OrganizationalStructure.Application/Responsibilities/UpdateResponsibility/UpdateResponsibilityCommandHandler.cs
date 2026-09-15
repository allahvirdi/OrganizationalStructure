using MediatR;
using Microsoft.EntityFrameworkCore;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Common.Interfaces;
using OrganizationalStructure.Domain.Abstractions;

namespace OrganizationalStructure.Application.Responsibilities.UpdateResponsibility;

/// <summary>
/// پردازش‌گر دستور ویرایش مسئولیت سازمانی.
/// </summary>
public sealed class UpdateResponsibilityCommandHandler
    : IRequestHandler<UpdateResponsibilityCommand, Result>
{
    private readonly IAppDbContext _db;
    private readonly IClock _clock;

    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    public UpdateResponsibilityCommandHandler(IAppDbContext db, IClock clock)
    {
        _db = db;
        _clock = clock;
    }

    /// <inheritdoc />
    public async Task<Result> Handle(
        UpdateResponsibilityCommand request,
        CancellationToken cancellationToken)
    {
        var responsibility = await _db.Responsibilities.FirstOrDefaultAsync(
            r => r.Id == request.ResponsibilityId,
            cancellationToken);

        if (responsibility is null)
        {
            return Result.Failure(ResponsibilityErrors.NotFound(request.ResponsibilityId));
        }

        responsibility.UpdateDetails(request.Title, request.Description, _clock.UtcNow);
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}