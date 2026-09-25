using MediatR;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Posts.DTOs;

namespace OrganizationalStructure.Application.Responsibilities.ResolveResponsibility;

/// <summary>
/// پرس‌وجوی مسیریابی مسئولیت (FindResponsible):
/// یافتن پست و پرسنل مسئول برای یک کد مسئولیت در یک سازمان مشخص.
/// </summary>
/// <param name="OrganizationId">شناسه سازمان (Scope)</param>
/// <param name="ResponsibilityCode">کد مسئولیت (Business Routing Key)</param>
public sealed record ResolveResponsibilityQuery(Guid OrganizationId, string ResponsibilityCode)
    : IRequest<Result<IReadOnlyList<ResolvedResponsibleDto>>>;
