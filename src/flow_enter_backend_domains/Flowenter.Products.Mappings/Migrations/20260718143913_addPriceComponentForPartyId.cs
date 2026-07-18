using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flowenter.Products.Mappings.Migrations
{
    /// <inheritdoc />
    public partial class addPriceComponentForPartyId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SpecifiedForPartyId",
                schema: "products",
                table: "PriceCoponents",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SpecifiedForPartyId",
                schema: "products",
                table: "PriceCoponents");
        }
    }
}
