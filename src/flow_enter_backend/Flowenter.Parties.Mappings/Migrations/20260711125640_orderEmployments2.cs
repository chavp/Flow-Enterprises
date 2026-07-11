using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flowenter.Parties.Mappings.Migrations
{
    /// <inheritdoc />
    public partial class orderEmployments2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmploymentRelationships",
                schema: "parties");

            migrationBuilder.CreateTable(
                name: "Employments",
                schema: "parties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employments_PartyRelationships_Id",
                        column: x => x.Id,
                        principalSchema: "parties",
                        principalTable: "PartyRelationships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Employments_PartyRoles_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "parties",
                        principalTable: "PartyRoles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Employments_PartyRoles_EmployerId",
                        column: x => x.EmployerId,
                        principalSchema: "parties",
                        principalTable: "PartyRoles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Employments_EmployeeId",
                schema: "parties",
                table: "Employments",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Employments_EmployerId",
                schema: "parties",
                table: "Employments",
                column: "EmployerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Employments",
                schema: "parties");

            migrationBuilder.CreateTable(
                name: "EmploymentRelationships",
                schema: "parties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EmployerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
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
                name: "IX_EmploymentRelationships_EmployeeId",
                schema: "parties",
                table: "EmploymentRelationships",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmploymentRelationships_EmployerId",
                schema: "parties",
                table: "EmploymentRelationships",
                column: "EmployerId");
        }
    }
}
