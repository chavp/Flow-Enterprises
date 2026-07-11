using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flowenter.Parties.Mappings.Migrations
{
    /// <inheritdoc />
    public partial class initOrders2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_People_FirstName_MiddleName_LastName",
                schema: "parties",
                table: "People");

            migrationBuilder.DropColumn(
                name: "BirthDate",
                schema: "parties",
                table: "People");

            migrationBuilder.DropColumn(
                name: "FirstName",
                schema: "parties",
                table: "People");

            migrationBuilder.DropColumn(
                name: "LastName",
                schema: "parties",
                table: "People");

            migrationBuilder.DropColumn(
                name: "MiddleName",
                schema: "parties",
                table: "People");

            migrationBuilder.AddColumn<DateOnly>(
                name: "DateOfBirth",
                schema: "parties",
                table: "People",
                type: "date",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Languages",
                schema: "parties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Revision = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PersonNames",
                schema: "parties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PersonId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    MiddleName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    EffectiveDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ExpiryDate = table.Column<DateOnly>(type: "date", nullable: false),
                    LanguageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Revision = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonNames", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonNames_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalSchema: "parties",
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PersonNames_People_PersonId",
                        column: x => x.PersonId,
                        principalSchema: "parties",
                        principalTable: "People",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Languages_Code",
                schema: "parties",
                table: "Languages",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PersonNames_FirstName_MiddleName_LastName_LanguageId",
                schema: "parties",
                table: "PersonNames",
                columns: new[] { "FirstName", "MiddleName", "LastName", "LanguageId" },
                unique: true,
                filter: "[MiddleName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PersonNames_LanguageId",
                schema: "parties",
                table: "PersonNames",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonNames_PersonId",
                schema: "parties",
                table: "PersonNames",
                column: "PersonId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PersonNames",
                schema: "parties");

            migrationBuilder.DropTable(
                name: "Languages",
                schema: "parties");

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                schema: "parties",
                table: "People");

            migrationBuilder.AddColumn<DateTime>(
                name: "BirthDate",
                schema: "parties",
                table: "People",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                schema: "parties",
                table: "People",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                schema: "parties",
                table: "People",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MiddleName",
                schema: "parties",
                table: "People",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_People_FirstName_MiddleName_LastName",
                schema: "parties",
                table: "People",
                columns: new[] { "FirstName", "MiddleName", "LastName" },
                unique: true,
                filter: "[FirstName] IS NOT NULL AND [MiddleName] IS NOT NULL AND [LastName] IS NOT NULL");
        }
    }
}
