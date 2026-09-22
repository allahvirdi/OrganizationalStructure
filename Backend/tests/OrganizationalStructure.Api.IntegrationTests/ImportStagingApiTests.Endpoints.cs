using System.Net;
using FluentAssertions;

namespace OrganizationalStructure.Api.IntegrationTests;

/// <summary>
/// تست‌های اندپوینت‌های بارگذاری واسط.
/// </summary>
public sealed partial class ImportStagingApiTests
{
    /// <summary>
    /// بارگذاری فایل معتبر در جدول واسط باید ۲۰۱ برگرداند.
    /// </summary>
    [Fact]
    public async Task StagingUpload_ValidExcel_ShouldReturn201()
    {
        var client = _factory.CreateClient();
        var organizationId = TestAuthHandler.TestOrganizationId;
        var code = NextPersonnelCode();
        var national = MakeNationalCode(code + "0");

        var file = CreateExcelFile(new[]
        {
            code, "علی", "رضایی", national, "09120000000", "1360/05/12", "12", "3", ""
        });

        var response = await client.PostAsync(
            "/api/v1/import/employees/staging/upload",
            CreateMultipart(organizationId, file));

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("batchId");
        body.Should().Contain("validRows");
    }

    /// <summary>
    /// ایجاد بارگذاری بیرونی باید ۲۰۱ با شناسه برگرداند.
    /// </summary>
    [Fact]
    public async Task StagingCreateExternalBatch_ShouldReturn201()
    {
        var client = _factory.CreateClient();
        var organizationId = TestAuthHandler.TestOrganizationId;

        var multipart = new System.Net.Http.MultipartFormDataContent();
        multipart.Add(new System.Net.Http.StringContent(organizationId.ToString()), "organizationId");

        var response = await client.PostAsync(
            "/api/v1/import/employees/staging/batches", multipart);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("batchId");
    }

    /// <summary>
    /// فهرست بارگذاری‌ها باید ۲۰۰ برگرداند.
    /// </summary>
    [Fact]
    public async Task StagingListBatches_ShouldReturn200()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync(
            "/api/v1/import/employees/staging/batches?page=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("items");
        body.Should().Contain("totalCount");
    }

    /// <summary>
    /// بدون مجوز باید ۴۰۳ برگرداند.
    /// </summary>
    [Fact]
    public async Task StagingUpload_WithoutPermission_ShouldReturn403()
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Test-Role", "User");
        var organizationId = TestAuthHandler.TestOrganizationId;
        var code = NextPersonnelCode();
        var national = MakeNationalCode(code + "0");

        var file = CreateExcelFile(new[]
        {
            code, "علی", "رضایی", national, "09120000000", "", "", "", ""
        });

        var response = await client.PostAsync(
            "/api/v1/import/employees/staging/upload",
            CreateMultipart(organizationId, file));

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    /// <summary>
    /// ردیف‌های بارگذاری یافت‌نشده باید ۴۰۴ برگرداند.
    /// </summary>
    [Fact]
    public async Task StagingGetRows_NotFound_ShouldReturn404()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync(
            $"/api/v1/import/employees/staging/{Guid.NewGuid()}/rows?page=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}