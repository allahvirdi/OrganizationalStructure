using MediatR;
using OrganizationalStructure.Application.Common;

namespace OrganizationalStructure.Application.Employees.AssignPost;

/// <summary>
/// دستور انتساب پرسنل به پست.
/// </summary>
/// <param name="EmployeeId">شناسه پرسنل</param>
/// <param name="PostId">شناسه پست مقصد</param>
/// <param name="FromDate">تاریخ شروع (اختیاری)</param>
/// <param name="ToDate">تاریخ پایان (اختیاری)</param>
/// <param name="IsPrimary">آیا انتساب اصلی است؟</param>
public sealed record AssignPostCommand(
    Guid EmployeeId,
    Guid PostId,
    DateOnly? FromDate,
    DateOnly? ToDate,
    bool IsPrimary) : IRequest<Result>;