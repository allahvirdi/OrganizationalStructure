using MediatR;
using Microsoft.EntityFrameworkCore;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Common.Interfaces;
using OrganizationalStructure.Domain.Abstractions;
using OrganizationalStructure.Domain.Entities;

namespace OrganizationalStructure.Application.Responsibilities.CreateResponsibility;

/// <summary>
/// پردازش‌گر دستور تعریف مسئولیت سازمانی.
/// </summary>
public sealed class CreateResponsibilityCommandHandler
    : IRequestHandler<CreateResponsibilityCommand, Result<Guid>>
{
    private readonly IAppDbContext _db;
    private readonly IClock _clock;
    private readonly ICurrentUser _currentUser;

    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    public CreateResponsibilityCommandHandler(
        IAppDbContext db,
        IClock clock,
        ICurrentUser currentUser)
    {
        _db = db;
        _clock = clock;
        _currentUser = currentUser;
    }

    /// <inheritdoc />
    public async Task<Result<Guid>> Handle(
        CreateResponsibilityCommand request,
        CancellationToken cancellationToken)
    {
        var code = request.Code.Trim();

        var duplicate = await _db.Responsibilities.AnyAsync(
            r => r.Code == code,
            cancellationToken);

        if (duplicate)
        {
            return Result<Guid>.Failure(ResponsibilityErrors.DuplicateCode(code));
        }

        var responsibility = Responsibility.Create(
            Guid.NewGuid(),
            _currentUser.TenantId,
            code,
            request.Title,
            request.Description,
            _clock.UtcNow);

        _db.Responsibilities.Add(responsibility);
        await _db.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(responsibility.Id);
    }
}