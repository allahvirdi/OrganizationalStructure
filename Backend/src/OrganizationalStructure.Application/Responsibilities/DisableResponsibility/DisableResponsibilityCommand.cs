using MediatR;
using OrganizationalStructure.Application.Common;

namespace OrganizationalStructure.Application.Responsibilities.DisableResponsibility;

/// <summary>
/// دستور غیرفعال‌سازی مسئولیت سازمانی.
/// </summary>
/// <param name="ResponsibilityId">شناسه مسئولیت</param>
public sealed record DisableResponsibilityCommand(Guid ResponsibilityId) : IRequest<Result>;