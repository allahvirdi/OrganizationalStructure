using MediatR;
using OrganizationalStructure.Application.Authorization;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Common.Interfaces;
using OrganizationalStructure.Domain.Abstractions;
using OrganizationalStructure.Domain.Entities;

namespace OrganizationalStructure.Application.Import.Staging;

/// <summary>
/// پردازش‌گر ایجاد بارگذاری بیرونی با وضعیت AwaitingRows.
/// </summary>
public sealed class CreateExternalBatchCommandHandler
    : IRequestHandler<CreateExternalBatchCommand, Result<Guid>>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUser _currentUser;

    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    /// <param name="db">زمینه داده</param>
    /// <param name="currentUser">کاربر جاری</param>
    public CreateExternalBatchCommandHandler(IAppDbContext db, ICurrentUser currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    /// <inheritdoc />
    public async Task<Result<Guid>> Handle(
        CreateExternalBatchCommand request,
        CancellationToken cancellationToken)
    {
        if (request.OrganizationId == Guid.Empty)
        {
            return Result<Guid>.Failure(
                new Error("Import.InvalidOrganization", "شناسه سازمان معتبر نیست.", ErrorType.Validation));
        }

        if (!_currentUser.VisibleOrganizationIds.Contains(request.OrganizationId))
        {
            return Result<Guid>.Failure(AccessErrors.Forbidden());
        }

        var batchId = Guid.NewGuid();
        var batch = ImportBatch.CreateExternal(batchId, request.OrganizationId);
        _db.ImportBatches.Add(batch);

        await _db.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(batchId);
    }
}