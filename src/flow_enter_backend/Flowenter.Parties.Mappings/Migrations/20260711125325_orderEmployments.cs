using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flowenter.Parties.Mappings.Migrations
{
    /// <inheritdoc />
    public partial class orderEmployments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "FromDate",
                schema: "parties",
                table: "PartyRelationships",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<Guid>(
                name: "PartyRelationshipTypeId",
                schema: "parties",
                table: "PartyRelationships",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateOnly>(
                name: "ThruDate",
                schema: "parties",
                table: "PartyRelationships",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.CreateTable(
                name: "EmploymentRelationships",
                schema: "parties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmploymentRelationships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmploymentRelationships_PartyRelationships_Id",
                        column: x => x.Id,
                        principalSchema: "parties",
                        principalTable: "PartyRelationships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmploymentRelationships_PartyRoles_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "parties",
                        principalTable: "PartyRoles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EmploymentRelationships_PartyRoles_EmployerId",
                        column: x => x.EmployerId,
                        principalSchema: "parties",
                        principalTable: "PartyRoles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PartyRelationships_PartyRelationshipTypeId",
                schema: "parties",
                table: "PartyRelationships",
                column: "PartyRelationshipTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmploymentRelationships_EmployeeId",
                schema: "parties",
                table: "EmploymentRelationships",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmploymentRelationships_EmployerId",
                schema: "parties",
                table: "EmploymentRelationships",
                column: "EmployerId");

            migrationBuilder.AddForeignKey(
                name: "FK_PartyRelationships_PartyRelationshipTypes_PartyRelationshipTypeId",
                schema: "parties",
                table: "PartyRelationships",
                column: "PartyRelationshipTypeId",
                principalSchema: "parties",
                principalTable: "PartyRelationshipTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PartyRelationships_PartyRelationshipTypes_PartyRelationshipTypeId",
                schema: "parties",
                table: "PartyRelationships");

            migrationBuilder.DropTable(
                name: "EmploymentRelationships",
                schema: "parties");

            migrationBuilder.DropIndex(
                name: "IX_PartyRelationships_PartyRelationshipTypeId",
                schema: "parties",
                table: "PartyRelationships");

            migrationBuilder.DropColumn(
                name: "FromDate",
                schema: "parties",
                table: "PartyRelationships");

            migrationBuilder.DropColumn(
                name: "PartyRelationshipTypeId",
                schema: "parties",
                table: "PartyRelationships");

            migrationBuilder.DropColumn(
                name: "ThruDate",
                schema: "parties",
                table: "PartyRelationships");
        }
    }
}
