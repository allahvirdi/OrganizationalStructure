using FluentAssertions;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Import;
using OrganizationalStructure.Application.Import.DTOs;
using OrganizationalStructure.Infrastructure.Persistence;

namespace OrganizationalStructure.Application.UnitTests;

/// <summary>
/// تست‌های پارسر اکسل و پردازش‌گر بارگذاری ساختار پست‌ها از فایل.
/// </summary>
public sealed class ImportPostsTests
{
    private readonly PostsExcelParser _parser = new();

    private static (OrganizationalStructureDbContext Db, TestClock Clock, TestCurrentUser User)
        CreateContext(IEnumerable<Guid> scope)
    {
        var tenantId = Guid.NewGuid();
        var db = TestDbContextFactory.Create(tenantId);
        return (db, new TestClock(), new TestCurrentUser(tenantId, scope));
    }

    private static MemoryStream CreateExcelBytes(params string[][] rows)
    {
        using var workbook = new ClosedXML.Excel.XLWorkbook();
        var ws = workbook.Worksheets.Add("ساختار پست‌ها");
        ws.Cell(1, 1).Value = "کد پست";
        ws.Cell(1, 2).Value = "عنوان پست";
        ws.Cell(1, 3).Value = "شرح";
        ws.Cell(1, 4).Value = "کد پست والد";

        for (var i = 0; i < rows.Length; i++)
        {
            var row = rows[i];
            for (var c = 0; c < row.Length && c < 4; c++)
                ws.Cell(i + 2, c + 1).Value = row[c];
        }

        var stream = new MemoryStream();
        workbook.SaveAs(stream);
        stream.Position = 0;
        return stream;
    }

    /// <summary>
    /// پارس فایل معتبر باید ردیف‌ها را درست برگرداند.
    /// </summary>
    [Fact]
    public void Parser_ValidFile_ShouldReturnRows()
    {
        var bytes = CreateExcelBytes(
            new[] { "MGR-001", "مدیرعامل", "بالاترین سطح", "" },
            new[] { "MGR-002", "معاون فنی", "", "MGR-001" });

        var result = _parser.Parse(bytes);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value[0].Code.Should().Be("MGR-001");
        result.Value[0].Title.Should().Be("مدیرعامل");
        result.Value[0].ParentCode.Should().BeNull();
        result.Value[1].ParentCode.Should().Be("MGR-001");
    }

    /// <summary>
    /// فایل بدون ردیف داده (فقط هدر) باید خطا بدهد.
    /// </summary>
    [Fact]
    public void Parser_HeaderOnly_ShouldReturnError()
    {
        var bytes = CreateExcelBytes();

        var result = _parser.Parse(bytes);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("Import.InvalidFile");
    }

    /// <summary>
    /// کد تکراری در فایل باید خطای DuplicateCodeInFile بدهد.
    /// </summary>
    [Fact]
    public void Parser_DuplicateCodeInFile_ShouldReturnError()
    {
        var bytes = CreateExcelBytes(
            new[] { "MGR-001", "مدیرعامل", "", "" },
            new[] { "MGR-001", "مدیرعامل دوم", "", "" });

        var result = _parser.Parse(bytes);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("Import.DuplicateCodeInFile");
    }

    /// <summary>
    /// ساخت قالب نمونه باید هدرهای فارسی داشته باشد.
    /// </summary>
    [Fact]
    public void Template_ShouldContainPersianHeaders()
    {
        var templateBytes = PostsExcelParser.BuildTemplate();
        using var stream = new MemoryStream(templateBytes);
        using var workbook = new ClosedXML.Excel.XLWorkbook(stream);
        var ws = workbook.Worksheet(1);
        ws.Cell(1, 1).GetString().Should().Be("کد پست");
        ws.Cell(1, 2).GetString().Should().Be("عنوان پست");
        ws.Cell(1, 3).GetString().Should().Be("شرح");
        ws.Cell(1, 4).GetString().Should().Be("کد پست والد");
        ws.Cell(2, 1).GetString().Should().Be("MGR-001");
        ws.Cell(3, 4).GetString().Should().Be("MGR-001");
    }

    /// <summary>
    /// Handler با داده معتبر باید پست‌ها را ایجاد کند.
    /// </summary>
    [Fact]
    public async Task Handle_ValidRows_ShouldCreatePosts()
    {
        var organizationId = Guid.NewGuid();
        var (db, clock, user) = CreateContext(new[] { organizationId });
        var handler = new ImportPostsCommandHandler(db, clock, user);

        var result = await handler.Handle(
            new ImportPostsCommand(organizationId, new[]
            {
                new ImportPostRowDto("MGR-001", "مدیرعامل", "بالاترین سطح", null),
                new ImportPostRowDto("MGR-002", "معاون فنی", null, "MGR-001")
            }),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.ImportedCount.Should().Be(2);
        db.Posts.Should().HaveCount(2);
    }

    /// <summary>
    /// کد تکراری با پست موجود در دیتابیس باید خطای Conflict بدهد.
    /// </summary>
    [Fact]
    public async Task Handle_DuplicateCodeWithDatabase_ShouldReturnConflict()
    {
        var organizationId = Guid.NewGuid();
        var (db, clock, user) = CreateContext(new[] { organizationId });
        var handler = new ImportPostsCommandHandler(db, clock, user);

        // بار اول: ایجاد پست
        await handler.Handle(
            new ImportPostsCommand(organizationId, new[]
            {
                new ImportPostRowDto("MGR-001", "مدیرعامل", null, null)
            }),
            CancellationToken.None);

        // بار دوم: همان کد تکراری
        var result = await handler.Handle(
            new ImportPostsCommand(organizationId, new[]
            {
                new ImportPostRowDto("MGR-001", "مدیرعامل جدید", null, null)
            }),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("Post.DuplicateCode");
    }

    /// <summary>
    /// والد ناموجود باید خطای NotFound بدهد.
    /// </summary>
    [Fact]
    public async Task Handle_ParentNotFound_ShouldReturnNotFound()
    {
        var organizationId = Guid.NewGuid();
        var (db, clock, user) = CreateContext(new[] { organizationId });
        var handler = new ImportPostsCommandHandler(db, clock, user);

        var result = await handler.Handle(
            new ImportPostsCommand(organizationId, new[]
            {
                new ImportPostRowDto("MGR-001", "مدیرعامل", null, "NOT-EXIST")
            }),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("Import.ParentNotFound");
    }

    /// <summary>
    /// سازمان خارج از Scope باید Forbidden بدهد.
    /// </summary>
    [Fact]
    public async Task Handle_OutOfScope_ShouldReturnForbidden()
    {
        var (db, clock, user) = CreateContext(Array.Empty<Guid>());
        var handler = new ImportPostsCommandHandler(db, clock, user);

        var result = await handler.Handle(
            new ImportPostsCommand(Guid.NewGuid(), new[]
            {
                new ImportPostRowDto("MGR-001", "مدیرعامل", null, null)
            }),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Type.Should().Be(ErrorType.Forbidden);
    }

    /// <summary>
    /// والد ارجاع‌شده به پست ایجادشده در همان فایل باید درست لینک شود.
    /// </summary>
    [Fact]
    public async Task Handle_ParentWithinFile_ShouldLinkCorrectly()
    {
        var organizationId = Guid.NewGuid();
        var (db, clock, user) = CreateContext(new[] { organizationId });
        var handler = new ImportPostsCommandHandler(db, clock, user);

        var result = await handler.Handle(
            new ImportPostsCommand(organizationId, new[]
            {
                new ImportPostRowDto("MGR-001", "مدیرعامل", null, null),
                new ImportPostRowDto("MGR-002", "معاون فنی", null, "MGR-001")
            }),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        var parent = db.Posts.Single(post => post.Code == "MGR-001");
        var child = db.Posts.Single(post => post.Code == "MGR-002");
        child.ParentId.Should().Be(parent.Id);
    }

    /// <summary>
    /// Validator باید لیست ردیف خالی را رد کند.
    /// </summary>
    [Fact]
    public void Validator_EmptyRows_ShouldBeInvalid()
    {
        var validator = new ImportPostsCommandValidator();
        var result = validator.Validate(
            new ImportPostsCommand(Guid.NewGuid(), Array.Empty<ImportPostRowDto>()));
        result.IsValid.Should().BeFalse();
    }

    /// <summary>
    /// Validator باید شناسه سازمان خالی را رد کند.
    /// </summary>
    [Fact]
    public void Validator_EmptyOrganizationId_ShouldBeInvalid()
    {
        var validator = new ImportPostsCommandValidator();
        var result = validator.Validate(
            new ImportPostsCommand(Guid.Empty, new[]
            {
                new ImportPostRowDto("MGR-001", "مدیرعامل", null, null)
            }));
        result.IsValid.Should().BeFalse();
    }
}