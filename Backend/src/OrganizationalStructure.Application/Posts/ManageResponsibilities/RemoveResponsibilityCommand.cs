using MediatR;
using OrganizationalStructure.Application.Common;

namespace OrganizationalStructure.Application.Posts.ManageResponsibilities;

/// <summary>
/// دستور حذف مسئولیت از پست.
/// </summary>
/// <param name="PostId">شناسه پست</param>
/// <param name="Title">عنوان مسئولیت</param>
public sealed record RemoveResponsibilityCommand(
    Guid PostId,
    string Title) : IRequest<Result>;