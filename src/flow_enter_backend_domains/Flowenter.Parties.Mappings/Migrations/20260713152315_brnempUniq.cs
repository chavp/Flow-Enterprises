using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flowenter.Parties.Mappings.Migrations
{
    /// <inheritdoc />
    public partial class brnempUniq : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BranchEmployments_BranchId",
                schema: "parties",
                table: "BranchEmployments");

            migrationBuilder.CreateIndex(
                name: "IX_BranchEmployments_BranchId_EmployeeId",
                schema: "parties",
                table: "BranchEmployments",
                columns: new[] { "BranchId", "EmployeeId" },
                unique: true,
                filter: "[BranchId] IS NOT NULL AND [EmployeeId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BranchEmployments_BranchId_EmployeeId",
                schema: "parties",
                table: "BranchEmployments");

            migrationBuilder.CreateIndex(
                name: "IX_BranchEmployments_BranchId",
                schema: "parties",
                table: "BranchEmployments",
                column: "BranchId");
        }
    }
}
