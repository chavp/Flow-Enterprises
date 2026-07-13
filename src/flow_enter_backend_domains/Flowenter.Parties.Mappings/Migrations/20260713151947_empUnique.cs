using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flowenter.Parties.Mappings.Migrations
{
    /// <inheritdoc />
    public partial class empUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Employments_EnterpriseId",
                schema: "parties",
                table: "Employments");

            migrationBuilder.CreateIndex(
                name: "IX_Employments_EnterpriseId_EmployeeId_Number",
                schema: "parties",
                table: "Employments",
                columns: new[] { "EnterpriseId", "EmployeeId", "Number" },
                unique: true,
                filter: "[EnterpriseId] IS NOT NULL AND [EmployeeId] IS NOT NULL AND [Number] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Employments_EnterpriseId_EmployeeId_Number",
                schema: "parties",
                table: "Employments");

            migrationBuilder.CreateIndex(
                name: "IX_Employments_EnterpriseId",
                schema: "parties",
                table: "Employments",
                column: "EnterpriseId");
        }
    }
}
