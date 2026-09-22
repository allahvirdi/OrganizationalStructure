using MediatR;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Import.Staging.DTOs;

namespace OrganizationalStructure.Application.Import.Staging;

/// <summary>
/// دستور اعلام آماده بودن بارگذاری بیرونی پس از درج ردیف‌ها.
/// </summary>
/// <param name="BatchId">شناسه بارگذاری</param>
public sealed record MarkBatchReadyCommand(
    Guid BatchId) : IRequest<Result<StagingUploadResultDto>>;