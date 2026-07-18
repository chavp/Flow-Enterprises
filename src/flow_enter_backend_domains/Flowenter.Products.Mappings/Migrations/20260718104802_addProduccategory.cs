using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flowenter.Products.Mappings.Migrations
{
    /// <inheritdoc />
    public partial class addProduccategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Products_Name_ProviderPartyId",
                schema: "products",
                table: "Products");

            migrationBuilder.CreateTable(
                name: "ProductFeatureCategories",
                schema: "products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProviderPartyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Revision = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductFeatureCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductFeatures",
                schema: "products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProviderPartyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProductFeatureType = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ProductFeatureCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Revision = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductFeatures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductFeatures_ProductFeatureCategories_ProductFeatureCategoryId",
                        column: x => x.ProductFeatureCategoryId,
                        principalSchema: "products",
                        principalTable: "ProductFeatureCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductFeatureApplicabilities",
                schema: "products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductFeatureId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductFeatureApplicabilityType = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Revision = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    FromDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ThruDate = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductFeatureApplicabilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductFeatureApplicabilities_ProductFeatures_ProductFeatureId",
                        column: x => x.ProductFeatureId,
                        principalSchema: "products",
                        principalTable: "ProductFeatures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductFeatureApplicabilities_Products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "products",
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_ProviderPartyId_Name",
                schema: "products",
                table: "Products",
                columns: new[] { "ProviderPartyId", "Name" },
                unique: true,
                filter: "[ProviderPartyId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ProductFeatureApplicabilities_ProductFeatureId",
                schema: "products",
                table: "ProductFeatureApplicabilities",
                column: "ProductFeatureId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductFeatureApplicabilities_ProductId",
                schema: "products",
                table: "ProductFeatureApplicabilities",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductFeatureCategories_ProviderPartyId_Name",
                schema: "products",
                table: "ProductFeatureCategories",
                columns: new[] { "ProviderPartyId", "Name" },
                unique: true,
                filter: "[ProviderPartyId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ProductFeatures_ProductFeatureCategoryId",
                schema: "products",
                table: "ProductFeatures",
                column: "ProductFeatureCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductFeatures_ProviderPartyId_ProductFeatureType_ProductFeatureCategoryId_Code",
                schema: "products",
                table: "ProductFeatures",
                columns: new[] { "ProviderPartyId", "ProductFeatureType", "ProductFeatureCategoryId", "Code" },
                unique: true,
                filter: "[ProviderPartyId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductFeatureApplicabilities",
                schema: "products");

            migrationBuilder.DropTable(
                name: "ProductFeatures",
                schema: "products");

            migrationBuilder.DropTable(
                name: "ProductFeatureCategories",
                schema: "products");

            migrationBuilder.DropIndex(
                name: "IX_Products_ProviderPartyId_Name",
                schema: "products",
                table: "Products");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Name_ProviderPartyId",
                schema: "products",
                table: "Products",
                columns: new[] { "Name", "ProviderPartyId" },
                unique: true,
                filter: "[ProviderPartyId] IS NOT NULL");
        }
    }
}
