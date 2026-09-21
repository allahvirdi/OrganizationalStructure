using MediatR;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Import.DTOs;

namespace OrganizationalStructure.Application.Import;

/// <summary>
/// دستور بارگذاری دسته‌جمعی پرسنل یک سازمان از فایل اکسل/CSV.
/// </summary>
/// <param name="OrganizationId">شناسه سازمان مقصد (مرجع IAM)</param>
/// <param name="Rows">ردیف‌های پرسنل پارس‌شده از فایل</param>
public sealed record ImportEmployeesCommand(
    Guid OrganizationId,
    IReadOnlyList<ImportEmployeeRowDto> Rows) : IRequest<Result<ImportEmployeesResultDto>>;