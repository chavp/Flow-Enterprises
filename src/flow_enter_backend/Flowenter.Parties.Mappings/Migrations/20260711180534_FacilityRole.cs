using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flowenter.Parties.Mappings.Migrations
{
    /// <inheritdoc />
    public partial class FacilityRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PartyId",
                schema: "parties",
                table: "FacilityRoles",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "PartyRoleTypeId",
                schema: "parties",
                table: "FacilityRoles",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_FacilityRoles_PartyId",
                schema: "parties",
                table: "FacilityRoles",
                column: "PartyId");

            migrationBuilder.CreateIndex(
                name: "IX_FacilityRoles_PartyRoleTypeId",
                schema: "parties",
                table: "FacilityRoles",
                column: "PartyRoleTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_FacilityRoles_Parties_PartyId",
                schema: "parties",
                table: "FacilityRoles",
                column: "PartyId",
                principalSchema: "parties",
                principalTable: "Parties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FacilityRoles_PartyRoleTypes_PartyRoleTypeId",
                schema: "parties",
                table: "FacilityRoles",
                column: "PartyRoleTypeId",
                principalSchema: "parties",
                principalTable: "PartyRoleTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FacilityRoles_Parties_PartyId",
                schema: "parties",
                table: "FacilityRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_FacilityRoles_PartyRoleTypes_PartyRoleTypeId",
                schema: "parties",
                table: "FacilityRoles");

            migrationBuilder.DropIndex(
                name: "IX_FacilityRoles_PartyId",
                schema: "parties",
                table: "FacilityRoles");

            migrationBuilder.DropIndex(
                name: "IX_FacilityRoles_PartyRoleTypeId",
                schema: "parties",
                table: "FacilityRoles");

            migrationBuilder.DropColumn(
                name: "PartyId",
                schema: "parties",
                table: "FacilityRoles");

            migrationBuilder.DropColumn(
                name: "PartyRoleTypeId",
                schema: "parties",
                table: "FacilityRoles");
        }
    }
}
