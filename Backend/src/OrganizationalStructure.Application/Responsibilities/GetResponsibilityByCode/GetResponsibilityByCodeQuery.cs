using MediatR;
using Microsoft.EntityFrameworkCore;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Common.Interfaces;
using OrganizationalStructure.Application.Responsibilities.DTOs;

namespace OrganizationalStructure.Application.Responsibilities.GetResponsibilityByCode;

/// <summary>
/// پرس‌وجوی دریافت مسئولیت با کد.
/// </summary>
/// <param name="Code">کد مسئولیت</param>
public sealed record GetResponsibilityByCodeQuery(string Code)
    : IRequest<Result<ResponsibilityDto>>;