using MediatR;
using OrganizationalStructure.Application.Common;

namespace OrganizationalStructure.Application.Responsibilities.UpdateResponsibility;

/// <summary>
/// دستور ویرایش مسئولیت سازمانی.
/// </summary>
/// <param name="ResponsibilityId">شناسه مسئولیت</param>
/// <param name="Title">عنوان جدید</param>
/// <param name="Description">شرح جدید</param>
public sealed record UpdateResponsibilityCommand(
    Guid ResponsibilityId,
    string Title,
    string? Description) : IRequest<Result>;