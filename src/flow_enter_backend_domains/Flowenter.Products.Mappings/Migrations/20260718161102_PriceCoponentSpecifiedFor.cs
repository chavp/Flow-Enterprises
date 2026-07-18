using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flowenter.Products.Mappings.Migrations
{
    /// <inheritdoc />
    public partial class PriceCoponentSpecifiedFor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SpecifiedForPartyName",
                schema: "products",
                table: "PriceCoponents",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SpecifiedForPartyRoleTypeCode",
                schema: "products",
                table: "PriceCoponents",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SpecifiedForPartyName",
                schema: "products",
                table: "PriceCoponents");

            migrationBuilder.DropColumn(
                name: "SpecifiedForPartyRoleTypeCode",
                schema: "products",
                table: "PriceCoponents");
        }
    }
}
