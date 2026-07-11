using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flowenter.Parties.Mappings.Migrations
{
    /// <inheritdoc />
    public partial class facRoomBed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Facilities_Facilities_RoomId",
                schema: "parties",
                table: "Facilities");

            migrationBuilder.DropIndex(
                name: "IX_Facilities_RoomId",
                schema: "parties",
                table: "Facilities");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                schema: "parties",
                table: "Facilities");

            migrationBuilder.DropColumn(
                name: "RoomId",
                schema: "parties",
                table: "Facilities");

            migrationBuilder.CreateTable(
                name: "Rooms",
                schema: "parties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rooms_Facilities_Id",
                        column: x => x.Id,
                        principalSchema: "parties",
                        principalTable: "Facilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Beds",
                schema: "parties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoomId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Beds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Beds_Facilities_Id",
                        column: x => x.Id,
                        principalSchema: "parties",
                        principalTable: "Facilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Beds_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalSchema: "parties",
                        principalTable: "Rooms",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Beds_RoomId",
                schema: "parties",
                table: "Beds",
                column: "RoomId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Beds",
                schema: "parties");

            migrationBuilder.DropTable(
                name: "Rooms",
                schema: "parties");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                schema: "parties",
                table: "Facilities",
                type: "nvarchar(8)",
                maxLength: 8,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "RoomId",
                schema: "parties",
                table: "Facilities",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Facilities_RoomId",
                schema: "parties",
                table: "Facilities",
                column: "RoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_Facilities_Facilities_RoomId",
                schema: "parties",
                table: "Facilities",
                column: "RoomId",
                principalSchema: "parties",
                principalTable: "Facilities",
                principalColumn: "Id");
        }
    }
}
