using MediatR;
using Microsoft.EntityFrameworkCore;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Common.Interfaces;
using OrganizationalStructure.Application.Employees.DTOs;

namespace OrganizationalStructure.Application.Employees.GetEmployeeById;

/// <summary>
/// پردازش‌گر پرس‌وجوی دریافت پرسنل با شناسه.
/// </summary>
public sealed class GetEmployeeByIdQueryHandler : IRequestHandler<GetEmployeeByIdQuery, Result<EmployeeDto>>
{
    private readonly IAppDbContext _db;

    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    public GetEmployeeByIdQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    /// <inheritdoc />
    public async Task<Result<EmployeeDto>> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
    {
        var dto = await _db.Employees
            .AsNoTracking()
            .Where(e => e.Id == request.EmployeeId)
            .Select(e => new EmployeeDto
            {
                Id = e.Id,
                PersonnelCode = e.PersonnelCode,
                FirstName = e.FirstName,
                LastName = e.LastName,
                NationalCode = e.NationalCode,
                Mobile = e.Mobile,
                BirthDate = e.BirthDate,
                ServiceYears = e.ServiceRecord != null ? e.ServiceRecord.Years : null,
                ServiceMonths = e.ServiceRecord != null ? e.ServiceRecord.Months : null,
                PezhvakMobile = e.PezhvakMobile,
                UserId = e.UserId,
                IsActive = e.IsActive
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (dto is null)
        {
            return Result<EmployeeDto>.Failure(EmployeeErrors.NotFound(request.EmployeeId));
        }

        return Result<EmployeeDto>.Success(dto);
    }
}