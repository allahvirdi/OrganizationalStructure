using MediatR;
using OrganizationalStructure.Application.Common;

namespace OrganizationalStructure.Application.Responsibilities.CreateResponsibility;

/// <summary>
/// دستور تعریف مسئولیت سازمانی جدید.
/// </summary>
/// <remarks>
/// کد (Routing Key) به‌صورت خودکار در پردازش‌گر تولید می‌شود (GUID).
/// </remarks>
/// <param name="Title">عنوان</param>
/// <param name="Description">شرح اختیاری</param>
public sealed record CreateResponsibilityCommand(
    string Title,
    string? Description) : IRequest<Result<Guid>>;