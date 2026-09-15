using MediatR;
using Microsoft.EntityFrameworkCore;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Common.Interfaces;
using OrganizationalStructure.Domain.Abstractions;

namespace OrganizationalStructure.Application.Employees.LinkEmployeeUser;

/// <summary>
/// پردازش‌گر دستور اتصال پرسنل به حساب کاربری IAM.
/// </summary>
public sealed class LinkEmployeeUserCommandHandler : IRequestHandler<LinkEmployeeUserCommand, Result>
{
    private readonly IAppDbContext _db;
    private readonly IClock _clock;

    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    public LinkEmployeeUserCommandHandler(IAppDbContext db, IClock clock)
    {
        _db = db;
        _clock = clock;
    }

    /// <inheritdoc />
    public async Task<Result> Handle(LinkEmployeeUserCommand request, CancellationToken cancellationToken)
    {
        var employee = await _db.Employees.FirstOrDefaultAsync(
            e => e.Id == request.EmployeeId,
            cancellationToken);

        if (employee is null)
        {
            return Result.Failure(EmployeeErrors.NotFound(request.EmployeeId));
        }

        employee.LinkToUser(request.UserId, _clock.UtcNow);
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}