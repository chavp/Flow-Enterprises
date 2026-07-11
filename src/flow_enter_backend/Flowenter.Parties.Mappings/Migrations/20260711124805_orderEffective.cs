using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flowenter.Parties.Mappings.Migrations
{
    /// <inheritdoc />
    public partial class orderEffective : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExpiryDate",
                schema: "parties",
                table: "PersonNames",
                newName: "ThruDate");

            migrationBuilder.RenameColumn(
                name: "EffectiveDate",
                schema: "parties",
                table: "PersonNames",
                newName: "FromDate");

            migrationBuilder.CreateTable(
                name: "PartyRelationships",
                schema: "parties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Revision = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartyRelationships", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PartyRelationshipTypes",
                schema: "parties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Revision = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartyRelationshipTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PartyRelationshipTypes_Code",
                schema: "parties",
                table: "PartyRelationshipTypes",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PartyRelationships",
                schema: "parties");

            migrationBuilder.DropTable(
                name: "PartyRelationshipTypes",
                schema: "parties");

            migrationBuilder.RenameColumn(
                name: "ThruDate",
                schema: "parties",
                table: "PersonNames",
                newName: "ExpiryDate");

            migrationBuilder.RenameColumn(
                name: "FromDate",
                schema: "parties",
                table: "PersonNames",
                newName: "EffectiveDate");
        }
    }
}
