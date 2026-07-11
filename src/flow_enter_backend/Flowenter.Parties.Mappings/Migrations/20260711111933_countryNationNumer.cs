using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flowenter.Parties.Mappings.Migrations
{
    /// <inheritdoc />
    public partial class countryNationNumer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Nationality",
                schema: "parties",
                table: "Countries",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Numerric",
                schema: "parties",
                table: "Countries",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Nationality",
                schema: "parties",
                table: "Countries");

            migrationBuilder.DropColumn(
                name: "Numerric",
                schema: "parties",
                table: "Countries");
        }
    }
}
