using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Flowenter.Parties.Mappings.Migrations
{
    /// <inheritdoc />
    public partial class SeedLegalStructures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "parties",
                table: "LegalStructures",
                columns: new[] { "Id", "Code", "CreatedAtUtc", "CreatedBy", "Description", "Name", "Revision", "UpdatedAtUtc", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "SOLE_PROPRIETORSHIP", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", null, "Sole Proprietorship", 0m, null, null },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "PARTNERSHIP", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", null, "Partnership", 0m, null, null },
                    { new Guid("33333333-3333-3333-3333-333333333333"), "CORPORATION", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", null, "Corporation", 0m, null, null },
                    { new Guid("44444444-4444-4444-4444-444444444444"), "LIMITED_LIABILITY_COMPANY", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", null, "Limited Liability Company", 0m, null, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "parties",
                table: "LegalStructures",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                schema: "parties",
                table: "LegalStructures",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                schema: "parties",
                table: "LegalStructures",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                schema: "parties",
                table: "LegalStructures",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"));
        }
    }
}
