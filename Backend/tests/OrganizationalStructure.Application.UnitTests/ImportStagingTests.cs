using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Import;
using OrganizationalStructure.Application.Import.DTOs;
using OrganizationalStructure.Application.Import.Staging;
using OrganizationalStructure.Domain.Enums;
using OrganizationalStructure.Infrastructure.Persistence;

namespace OrganizationalStructure.Application.UnitTests;

/// <summary>
/// تست‌های واحد پردازش‌گرهای بارگذاری واسط (Staging) — DEC-030.
/// </summary>
public sealed partial class ImportStagingTests
{
    private static (OrganizationalStructureDbContext Db, TestClock Clock, TestCurrentUser User)
        CreateContext(IEnumerable<Guid> scope)
    {
        var tenantId = Guid.NewGuid();
        var db = TestDbContextFactory.Create(tenantId);
        return (db, new TestClock(), new TestCurrentUser(tenantId, scope));
    }

    private static string MakeNationalCode(string prefix9)
    {
        var sum = 0;
        for (var i = 0; i < 9; i++)
        {
            sum += (prefix9[i] - '0') * (10 - i);
        }

        var remainder = sum % 11;
        var checkDigit = remainder < 2 ? remainder : 11 - remainder;
        return prefix9 + checkDigit.ToString();
    }

    private static ImportEmployeeRowDto ValidRow(
        int rowNumber = 1,
        string personnelCode = "00000001")
    {
        return new ImportEmployeeRowDto(
            rowNumber,
            personnelCode,
            "علی",
            "رضایی",
            MakeNationalCode(personnelCode + "0"),
            "09120000000",
            "1360/05/12",
            "12",
            "3",
            null);
    }
}