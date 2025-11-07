using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flowenter.Parties.Mappings.Migrations
{
    /// <inheritdoc />
    public partial class initParties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "parties");

            migrationBuilder.CreateTable(
                name: "ContactMechanismTypes",
                schema: "parties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    Revision = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactMechanismTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GeographicBoundaryTypes",
                schema: "parties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    Revision = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeographicBoundaryTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LegalStructures",
                schema: "parties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    Revision = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LegalStructures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PartyCategories",
                schema: "parties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    GroupById = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    Revision = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartyCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PartyCategories_PartyCategories_GroupById",
                        column: x => x.GroupById,
                        principalSchema: "parties",
                        principalTable: "PartyCategories",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PartyRoleTypes",
                schema: "parties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    Revision = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartyRoleTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PartyTypes",
                schema: "parties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    Revision = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartyTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GeographicBoundaries",
                schema: "parties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TypeId = table.Column<Guid>(type: "uuid", nullable: true),
                    Discriminator = table.Column<string>(type: "character varying(21)", maxLength: 21, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    IsoCode2 = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    IsoCode3 = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    Revision = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeographicBoundaries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GeographicBoundaries_GeographicBoundaryTypes_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "parties",
                        principalTable: "GeographicBoundaryTypes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Parties",
                schema: "parties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Discriminator = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    FirstName = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    MiddleName = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    LastName = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    BirthDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    Revision = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Parties_PartyTypes_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "parties",
                        principalTable: "PartyTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContactMechanisms",
                schema: "parties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Discriminator = table.Column<string>(type: "character varying(34)", maxLength: 34, nullable: false),
                    Address = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: true),
                    CountryId = table.Column<Guid>(type: "uuid", nullable: true),
                    PostalAddress_Address = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    ZipCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Street = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    City = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    State = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    House = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    HouseNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    District = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    SubDistrict = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    Province = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    TelecommunicationNumber_CountryId = table.Column<Guid>(type: "uuid", nullable: true),
                    Number = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                    CountryCode = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                    AreaCode = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                    AskForName = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    Revision = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactMechanisms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContactMechanisms_ContactMechanismTypes_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "parties",
                        principalTable: "ContactMechanismTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContactMechanisms_GeographicBoundaries_CountryId",
                        column: x => x.CountryId,
                        principalSchema: "parties",
                        principalTable: "GeographicBoundaries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContactMechanisms_GeographicBoundaries_TelecommunicationNum~",
                        column: x => x.TelecommunicationNumber_CountryId,
                        principalSchema: "parties",
                        principalTable: "GeographicBoundaries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FinancialAccounts",
                schema: "parties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Type = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Currency = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    Balance = table.Column<decimal>(type: "numeric", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    Revision = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinancialAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FinancialAccounts_Parties_OwnerId",
                        column: x => x.OwnerId,
                        principalSchema: "parties",
                        principalTable: "Parties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PartyClassifications",
                schema: "parties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PartyId = table.Column<Guid>(type: "uuid", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    Revision = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartyClassifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PartyClassifications_Parties_PartyId",
                        column: x => x.PartyId,
                        principalSchema: "parties",
                        principalTable: "Parties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PartyClassifications_PartyCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "parties",
                        principalTable: "PartyCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PartyRoles",
                schema: "parties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    PartyId = table.Column<Guid>(type: "uuid", nullable: false),
                    Discriminator = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: false),
                    LegalName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Information = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Logo = table.Column<byte[]>(type: "bytea", nullable: true),
                    BrandName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    LegalStructureId = table.Column<Guid>(type: "uuid", nullable: true),
                    BusinessRegistrationNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    TaxId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    FiscalYearStartMonth = table.Column<int>(type: "integer", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    Revision = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    FromDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ThruDate = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartyRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PartyRoles_LegalStructures_LegalStructureId",
                        column: x => x.LegalStructureId,
                        principalSchema: "parties",
                        principalTable: "LegalStructures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PartyRoles_Parties_PartyId",
                        column: x => x.PartyId,
                        principalSchema: "parties",
                        principalTable: "Parties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PartyRoles_PartyRoleTypes_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "parties",
                        principalTable: "PartyRoleTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PartyContactMechanisms",
                schema: "parties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PartyId = table.Column<Guid>(type: "uuid", nullable: false),
                    PartyRoleTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    ContactMechanismId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    Revision = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    FromDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ThruDate = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartyContactMechanisms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PartyContactMechanisms_ContactMechanisms_ContactMechanismId",
                        column: x => x.ContactMechanismId,
                        principalSchema: "parties",
                        principalTable: "ContactMechanisms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PartyContactMechanisms_Parties_PartyId",
                        column: x => x.PartyId,
                        principalSchema: "parties",
                        principalTable: "Parties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PartyContactMechanisms_PartyRoleTypes_PartyRoleTypeId",
                        column: x => x.PartyRoleTypeId,
                        principalSchema: "parties",
                        principalTable: "PartyRoleTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContactMechanisms_Address",
                schema: "parties",
                table: "ContactMechanisms",
                column: "Address");

            migrationBuilder.CreateIndex(
                name: "IX_ContactMechanisms_CountryId",
                schema: "parties",
                table: "ContactMechanisms",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_ContactMechanisms_TelecommunicationNumber_CountryId_Number",
                schema: "parties",
                table: "ContactMechanisms",
                columns: new[] { "TelecommunicationNumber_CountryId", "Number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContactMechanisms_TypeId",
                schema: "parties",
                table: "ContactMechanisms",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ContactMechanismTypes_Code",
                schema: "parties",
                table: "ContactMechanismTypes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FinancialAccounts_OwnerId_Number",
                schema: "parties",
                table: "FinancialAccounts",
                columns: new[] { "OwnerId", "Number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GeographicBoundaries_Name",
                schema: "parties",
                table: "GeographicBoundaries",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GeographicBoundaries_TypeId",
                schema: "parties",
                table: "GeographicBoundaries",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_GeographicBoundaryTypes_Code",
                schema: "parties",
                table: "GeographicBoundaryTypes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LegalStructures_Code",
                schema: "parties",
                table: "LegalStructures",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Parties_FirstName_MiddleName_LastName",
                schema: "parties",
                table: "Parties",
                columns: new[] { "FirstName", "MiddleName", "LastName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Parties_Name",
                schema: "parties",
                table: "Parties",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Parties_TypeId",
                schema: "parties",
                table: "Parties",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PartyCategories_GroupById",
                schema: "parties",
                table: "PartyCategories",
                column: "GroupById");

            migrationBuilder.CreateIndex(
                name: "IX_PartyCategories_Name",
                schema: "parties",
                table: "PartyCategories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PartyClassifications_CategoryId",
                schema: "parties",
                table: "PartyClassifications",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_PartyClassifications_PartyId",
                schema: "parties",
                table: "PartyClassifications",
                column: "PartyId");

            migrationBuilder.CreateIndex(
                name: "IX_PartyContactMechanisms_ContactMechanismId",
                schema: "parties",
                table: "PartyContactMechanisms",
                column: "ContactMechanismId");

            migrationBuilder.CreateIndex(
                name: "IX_PartyContactMechanisms_PartyId_PartyRoleTypeId_ContactMecha~",
                schema: "parties",
                table: "PartyContactMechanisms",
                columns: new[] { "PartyId", "PartyRoleTypeId", "ContactMechanismId", "FromDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PartyContactMechanisms_PartyRoleTypeId",
                schema: "parties",
                table: "PartyContactMechanisms",
                column: "PartyRoleTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PartyRoles_LegalName",
                schema: "parties",
                table: "PartyRoles",
                column: "LegalName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PartyRoles_LegalStructureId",
                schema: "parties",
                table: "PartyRoles",
                column: "LegalStructureId");

            migrationBuilder.CreateIndex(
                name: "IX_PartyRoles_PartyId",
                schema: "parties",
                table: "PartyRoles",
                column: "PartyId");

            migrationBuilder.CreateIndex(
                name: "IX_PartyRoles_TypeId_PartyId_FromDate",
                schema: "parties",
                table: "PartyRoles",
                columns: new[] { "TypeId", "PartyId", "FromDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PartyRoleTypes_Code",
                schema: "parties",
                table: "PartyRoleTypes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PartyTypes_Code",
                schema: "parties",
                table: "PartyTypes",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FinancialAccounts",
                schema: "parties");

            migrationBuilder.DropTable(
                name: "PartyClassifications",
                schema: "parties");

            migrationBuilder.DropTable(
                name: "PartyContactMechanisms",
                schema: "parties");

            migrationBuilder.DropTable(
                name: "PartyRoles",
                schema: "parties");

            migrationBuilder.DropTable(
                name: "PartyCategories",
                schema: "parties");

            migrationBuilder.DropTable(
                name: "ContactMechanisms",
                schema: "parties");

            migrationBuilder.DropTable(
                name: "LegalStructures",
                schema: "parties");

            migrationBuilder.DropTable(
                name: "Parties",
                schema: "parties");

            migrationBuilder.DropTable(
                name: "PartyRoleTypes",
                schema: "parties");

            migrationBuilder.DropTable(
                name: "ContactMechanismTypes",
                schema: "parties");

            migrationBuilder.DropTable(
                name: "GeographicBoundaries",
                schema: "parties");

            migrationBuilder.DropTable(
                name: "PartyTypes",
                schema: "parties");

            migrationBuilder.DropTable(
                name: "GeographicBoundaryTypes",
                schema: "parties");
        }
    }
}
