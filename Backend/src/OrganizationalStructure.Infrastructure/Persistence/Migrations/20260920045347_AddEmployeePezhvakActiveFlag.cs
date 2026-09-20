using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrganizationalStructure.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeePezhvakActiveFlag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PezhvakIsActive",
                table: "Employees",
                type: "bit",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_TenantId_OrganizationId",
                table: "Employees",
                columns: new[] { "TenantId", "OrganizationId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Employees_TenantId_OrganizationId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "PezhvakIsActive",
                table: "Employees");
        }
    }
}
