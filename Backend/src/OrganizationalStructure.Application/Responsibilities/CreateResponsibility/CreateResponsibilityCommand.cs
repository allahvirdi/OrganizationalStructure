using MediatR;
using OrganizationalStructure.Application.Common;

namespace OrganizationalStructure.Application.Responsibilities.CreateResponsibility;

/// <summary>
/// دستور تعریف مسئولیت سازمانی جدید.
/// </summary>
/// <param name="Code">کد یکتا (Business Routing Key)</param>
/// <param name="Title">عنوان</param>
/// <param name="Description">شرح اختیاری</param>
public sealed record CreateResponsibilityCommand(
    string Code,
    string Title,
    string? Description) : IRequest<Result<Guid>>;