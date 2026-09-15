using MediatR;
using OrganizationalStructure.Application.Common;

namespace OrganizationalStructure.Application.Posts.ManageResponsibilities;

/// <summary>
/// دستور افزودن مسئولیت به پست.
/// </summary>
/// <param name="PostId">شناسه پست</param>
/// <param name="Title">عنوان مسئولیت</param>
/// <param name="Description">شرح اختیاری</param>
public sealed record AddResponsibilityCommand(
    Guid PostId,
    string Title,
    string? Description) : IRequest<Result>;