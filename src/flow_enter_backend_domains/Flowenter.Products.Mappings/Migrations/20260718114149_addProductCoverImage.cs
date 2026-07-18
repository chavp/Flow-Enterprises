using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flowenter.Products.Mappings.Migrations
{
    /// <inheritdoc />
    public partial class addProductCoverImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "CoverImage",
                schema: "products",
                table: "Products",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<string>(
                name: "CoverImageName",
                schema: "products",
                table: "Products",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CoverImage",
                schema: "products",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CoverImageName",
                schema: "products",
                table: "Products");
        }
    }
}
