using MediatR;
using Microsoft.EntityFrameworkCore;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Common.Interfaces;
using OrganizationalStructure.Domain.Abstractions;

namespace OrganizationalStructure.Application.Employees.AssignPost;

/// <summary>
/// پردازش‌گر دستور انتساب پرسنل به پست.
/// </summary>
public sealed class AssignPostCommandHandler : IRequestHandler<AssignPostCommand, Result>
{
    private readonly IAppDbContext _db;
    private readonly IClock _clock;

    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    public AssignPostCommandHandler(IAppDbContext db, IClock clock)
    {
        _db = db;
        _clock = clock;
    }

    /// <inheritdoc />
    public async Task<Result> Handle(AssignPostCommand request, CancellationToken cancellationToken)
    {
        var employee = await _db.Employees
            .Include(e => e.Assignments)
            .FirstOrDefaultAsync(e => e.Id == request.EmployeeId, cancellationToken);

        if (employee is null)
        {
            return Result.Failure(EmployeeErrors.NotFound(request.EmployeeId));
        }

        var postExists = await _db.Posts.AnyAsync(p => p.Id == request.PostId, cancellationToken);
        if (!postExists)
        {
            return Result.Failure(EmployeeErrors.PostNotFound(request.PostId));
        }

        try
        {
            employee.AssignToPost(
                request.PostId,
                request.FromDate,
                request.ToDate,
                request.IsPrimary,
                _clock.UtcNow);
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(EmployeeErrors.AssignmentConflict(ex.Message));
        }

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}