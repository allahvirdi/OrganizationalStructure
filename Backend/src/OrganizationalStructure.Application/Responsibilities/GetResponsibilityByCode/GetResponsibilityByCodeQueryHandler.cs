using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Common.Interfaces;
using OrganizationalStructure.Application.Responsibilities.DTOs;

namespace OrganizationalStructure.Application.Responsibilities.GetResponsibilityByCode;

/// <summary>
/// اعتبارسنج پرس‌وجوی مسئولیت با کد.
/// </summary>
public sealed class GetResponsibilityByCodeQueryValidator
    : AbstractValidator<GetResponsibilityByCodeQuery>
{
    /// <summary>
    /// تعریف قواعد اعتبارسنجی.
    /// </summary>
    public GetResponsibilityByCodeQueryValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage("کد مسئولیت الزامی است.");
    }
}

/// <summary>
/// پردازش‌گر پرس‌وجوی دریافت مسئولیت با کد.
/// </summary>
public sealed class GetResponsibilityByCodeQueryHandler
    : IRequestHandler<GetResponsibilityByCodeQuery, Result<ResponsibilityDto>>
{
    private readonly IAppDbContext _db;

    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    public GetResponsibilityByCodeQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    /// <inheritdoc />
    public async Task<Result<ResponsibilityDto>> Handle(
        GetResponsibilityByCodeQuery request,
        CancellationToken cancellationToken)
    {
        var dto = await _db.Responsibilities
            .AsNoTracking()
            .Where(r => r.Code == request.Code.Trim())
            .Select(r => new ResponsibilityDto
            {
                Id = r.Id,
                Code = r.Code,
                Title = r.Title,
                Description = r.Description,
                IsActive = r.IsActive
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (dto is null)
        {
            return Result<ResponsibilityDto>.Failure(
                ResponsibilityErrors.NotFoundByCode(request.Code.Trim()));
        }

        return Result<ResponsibilityDto>.Success(dto);
    }
}