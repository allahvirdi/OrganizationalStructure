using MediatR;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Organizations.DTOs;

namespace OrganizationalStructure.Application.Organizations.GetOrganizationStructure;

/// <summary>
/// پرس‌وجوی دریافت ساختار کامل یک سازمان (لیست تخت پست‌ها + مسئولیت‌ها).
/// </summary>
/// <param name="OrganizationId">شناسه سازمان</param>
public sealed record GetOrganizationStructureQuery(Guid OrganizationId)
    : IRequest<Result<OrganizationStructureDto>>;
