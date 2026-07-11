using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

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
                    table.PrimaryKey("PK_ContactMechanismTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FacilityRoleTypes",
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
                    table.PrimaryKey("PK_FacilityRoleTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FacilityTypes",
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
                    table.PrimaryKey("PK_FacilityTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GenderTypes",
                schema: "parties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Revision = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenderTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GeographicBoundaryTypes",
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
                    table.PrimaryKey("PK_GeographicBoundaryTypes", x => x.Id);
                });

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
                name: "LegalStructures",
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
                    table.PrimaryKey("PK_LegalStructures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PartyCategories",
                schema: "parties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    GroupById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Revision = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "PartyRoleTypes",
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
                    table.PrimaryKey("PK_PartyRoleTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PartyTypes",
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
                    table.PrimaryKey("PK_PartyTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContactMechanisms",
                schema: "parties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Revision = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
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
                });

            migrationBuilder.CreateTable(
                name: "FacilityRoles",
                schema: "parties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FacilityRoleTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Revision = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    FromDateUtc = table.Column<DateOnly>(type: "date", nullable: false),
                    ThruDateUtc = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacilityRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FacilityRoles_FacilityRoleTypes_FacilityRoleTypeId",
                        column: x => x.FacilityRoleTypeId,
                        principalSchema: "parties",
                        principalTable: "FacilityRoleTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Facilities",
                schema: "parties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Number = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FacilityTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PartOfId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Revision = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Facilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Facilities_Facilities_PartOfId",
                        column: x => x.PartOfId,
                        principalSchema: "parties",
                        principalTable: "Facilities",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Facilities_FacilityTypes_FacilityTypeId",
                        column: x => x.FacilityTypeId,
                        principalSchema: "parties",
                        principalTable: "FacilityTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GeographicBoundaries",
                schema: "parties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Revision = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
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
                name: "PartyRelationships",
                schema: "parties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PartyRelationshipTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Revision = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    FromDateUtc = table.Column<DateOnly>(type: "date", nullable: false),
                    ThruDateUtc = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartyRelationships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PartyRelationships_PartyRelationshipTypes_PartyRelationshipTypeId",
                        column: x => x.PartyRelationshipTypeId,
                        principalSchema: "parties",
                        principalTable: "PartyRelationshipTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Parties",
                schema: "parties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Revision = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
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
                name: "ElectronicAddresses",
                schema: "parties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElectronicAddresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ElectronicAddresses_ContactMechanisms_Id",
                        column: x => x.Id,
                        principalSchema: "parties",
                        principalTable: "ContactMechanisms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                name: "Countries",
                schema: "parties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Nationality = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Numerric = table.Column<int>(type: "int", nullable: true),
                    IsoCode2 = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    IsoCode3 = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Countries_GeographicBoundaries_Id",
                        column: x => x.Id,
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Number = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Revision = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
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
                name: "Organizations",
                schema: "parties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Organizations_Parties_Id",
                        column: x => x.Id,
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PartyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Revision = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
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
                name: "PartyContactMechanisms",
                schema: "parties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PartyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PartyRoleTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContactMechanismId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Revision = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    FromDateUtc = table.Column<DateOnly>(type: "date", nullable: false),
                    ThruDateUtc = table.Column<DateOnly>(type: "date", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "PartyRoles",
                schema: "parties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PartyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Revision = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    FromDateUtc = table.Column<DateOnly>(type: "date", nullable: false),
                    ThruDateUtc = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartyRoles", x => x.Id);
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
                name: "People",
                schema: "parties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GenderTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_People", x => x.Id);
                    table.ForeignKey(
                        name: "FK_People_GenderTypes_GenderTypeId",
                        column: x => x.GenderTypeId,
                        principalSchema: "parties",
                        principalTable: "GenderTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_People_Parties_Id",
                        column: x => x.Id,
                        principalSchema: "parties",
                        principalTable: "Parties",
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

            migrationBuilder.CreateTable(
                name: "PostalAddresses",
                schema: "parties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CountryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    ZipCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Street = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    City = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    State = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    House = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    HouseNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    District = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    SubDistrict = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Province = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostalAddresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostalAddresses_ContactMechanisms_Id",
                        column: x => x.Id,
                        principalSchema: "parties",
                        principalTable: "ContactMechanisms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostalAddresses_Countries_CountryId",
                        column: x => x.CountryId,
                        principalSchema: "parties",
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TelecommunicationNumbers",
                schema: "parties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CountryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Number = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    CountryCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    AreaCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    AskForName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TelecommunicationNumbers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TelecommunicationNumbers_ContactMechanisms_Id",
                        column: x => x.Id,
                        principalSchema: "parties",
                        principalTable: "ContactMechanisms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TelecommunicationNumbers_Countries_CountryId",
                        column: x => x.CountryId,
                        principalSchema: "parties",
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                schema: "parties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Customers_PartyRoles_Id",
                        column: x => x.Id,
                        principalSchema: "parties",
                        principalTable: "PartyRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Employments",
                schema: "parties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "Enterprises",
                schema: "parties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LegalName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Information = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Logo = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    BrandName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LegalStructureId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BusinessRegistrationNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TaxId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FiscalYearStartMonth = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enterprises", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Enterprises_LegalStructures_LegalStructureId",
                        column: x => x.LegalStructureId,
                        principalSchema: "parties",
                        principalTable: "LegalStructures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Enterprises_PartyRoles_Id",
                        column: x => x.Id,
                        principalSchema: "parties",
                        principalTable: "PartyRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    LanguageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Revision = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    FromDateUtc = table.Column<DateOnly>(type: "date", nullable: false),
                    ThruDateUtc = table.Column<DateOnly>(type: "date", nullable: false)
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

            migrationBuilder.InsertData(
                schema: "parties",
                table: "FacilityRoleTypes",
                columns: new[] { "Id", "Code", "CreatedAtUtc", "CreatedBy", "Description", "Name", "Revision", "UpdatedAtUtc", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("51515151-5151-5151-5151-515151515151"), "OWN", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", null, "Own", 0m, null, null },
                    { new Guid("62626262-6262-6262-6262-626262626262"), "RENT", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", null, "Rent", 0m, null, null },
                    { new Guid("73737373-7373-7373-7373-737373737373"), "LEASE", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", null, "Lease", 0m, null, null },
                    { new Guid("84848484-8484-8484-8484-848484848484"), "USE", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", null, "Use", 0m, null, null }
                });

            migrationBuilder.InsertData(
                schema: "parties",
                table: "FacilityTypes",
                columns: new[] { "Id", "Code", "CreatedAtUtc", "CreatedBy", "Description", "Name", "Revision", "UpdatedAtUtc", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("12121212-1212-1212-1212-121212121212"), "ROOM", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", null, "Room", 0m, null, null },
                    { new Guid("34343434-3434-3434-3434-343434343434"), "BED", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", null, "Bed", 0m, null, null }
                });

            migrationBuilder.InsertData(
                schema: "parties",
                table: "GeographicBoundaryTypes",
                columns: new[] { "Id", "Code", "CreatedAtUtc", "CreatedBy", "Description", "Name", "Revision", "UpdatedAtUtc", "UpdatedBy" },
                values: new object[] { new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), "COUNTRY", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", null, "Country", 0m, null, null });

            migrationBuilder.InsertData(
                schema: "parties",
                table: "Languages",
                columns: new[] { "Id", "Code", "CreatedAtUtc", "CreatedBy", "Description", "Revision", "UpdatedAtUtc", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("88888888-8888-8888-8888-888888888888"), "EN", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", null, 0m, null, null },
                    { new Guid("99999999-9999-9999-9999-999999999999"), "TH", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", null, 0m, null, null }
                });

            migrationBuilder.InsertData(
                schema: "parties",
                table: "LegalStructures",
                columns: new[] { "Id", "Code", "CreatedAtUtc", "CreatedBy", "Description", "Name", "Revision", "UpdatedAtUtc", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "SOLE_PROPRIETORSHIP", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", null, "กิจการเจ้าของคนเดียว", 0m, null, null },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "PARTNERSHIP", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", null, "ห้างหุ้นส่วน", 0m, null, null },
                    { new Guid("33333333-3333-3333-3333-333333333333"), "CORPORATION", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", null, "บริษัทมหาชน (หรือบริษัทจำกัดขนาดใหญ่)", 0m, null, null },
                    { new Guid("44444444-4444-4444-4444-444444444444"), "LIMITED_LIABILITY_COMPANY", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", null, "บริษัทจำกัด", 0m, null, null }
                });

            migrationBuilder.InsertData(
                schema: "parties",
                table: "PartyRelationshipTypes",
                columns: new[] { "Id", "Code", "CreatedAtUtc", "CreatedBy", "Description", "Name", "Revision", "UpdatedAtUtc", "UpdatedBy" },
                values: new object[] { new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff"), "EMPLOYMENT", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", null, "การจ้างงาน", 0m, null, null });

            migrationBuilder.InsertData(
                schema: "parties",
                table: "PartyRoleTypes",
                columns: new[] { "Id", "Code", "CreatedAtUtc", "CreatedBy", "Description", "Name", "Revision", "UpdatedAtUtc", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("aaaaaaaa-1111-1111-1111-111111111111"), "ADMINISTRATOR", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", null, "ผู้บริหารหรือผู้จัดการสถานดูแล", 0m, null, null },
                    { new Guid("aaaaaaaa-2222-2222-2222-222222222222"), "CARE_MANAGER", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", null, "ผู้จัดการดูแลผู้ป่วยหรือผู้จัดการเคส", 0m, null, null },
                    { new Guid("aaaaaaaa-3333-3333-3333-333333333333"), "NURSE", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", null, "พยาบาล", 0m, null, null },
                    { new Guid("aaaaaaaa-4444-4444-4444-444444444444"), "CAREGIVER", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", null, "ผู้ดูแล", 0m, null, null },
                    { new Guid("aaaaaaaa-5555-5555-5555-555555555555"), "PHYSICIAN", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", null, "แพทย์", 0m, null, null },
                    { new Guid("aaaaaaaa-6666-6666-6666-666666666666"), "PHARMACIST", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", null, "เภสัชกร", 0m, null, null },
                    { new Guid("aaaaaaaa-7777-7777-7777-777777777777"), "DIETITIAN", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", null, "นักกำหนดอาหารหรือนักโภชนาการ", 0m, null, null },
                    { new Guid("aaaaaaaa-8888-8888-8888-888888888888"), "KITCHEN_STAFF", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", null, "พนักงานครัว", 0m, null, null },
                    { new Guid("aaaaaaaa-9999-9999-9999-999999999999"), "HOUSEKEEPING_STAFF", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", null, "พนักงานทำความสะอาด", 0m, null, null },
                    { new Guid("bbbbbbbb-1111-1111-1111-111111111111"), "MAINTENANCE_STAFF", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", null, "ช่างซ่อมบำรุง", 0m, null, null },
                    { new Guid("bbbbbbbb-2222-2222-2222-222222222222"), "LAUNDRY_STAFF", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", null, "พนักงานซักรีด", 0m, null, null },
                    { new Guid("bbbbbbbb-3333-3333-3333-333333333333"), "RECEPTIONIST", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", null, "พนักงานต้อนรับ", 0m, null, null },
                    { new Guid("bbbbbbbb-4444-4444-4444-444444444444"), "PATIENT", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", null, "ผู้ป่วย", 0m, null, null },
                    { new Guid("bbbbbbbb-5555-5555-5555-555555555555"), "SECURITY_GUARD", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", null, "เจ้าหน้าที่รักษาความปลอดภัย", 0m, null, null },
                    { new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), "ENTERPRISE", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", null, "กิจการ/นิติบุคคล", 0m, null, null },
                    { new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), "CUSTOMER", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", null, "ลูกค้า", 0m, null, null }
                });

            migrationBuilder.InsertData(
                schema: "parties",
                table: "PartyTypes",
                columns: new[] { "Id", "Code", "CreatedAtUtc", "CreatedBy", "Description", "Name", "Revision", "UpdatedAtUtc", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "PERSON", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", null, "Person", 0m, null, null },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), "ORGANIZATION", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", null, "Organization", 0m, null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Beds_RoomId",
                schema: "parties",
                table: "Beds",
                column: "RoomId");

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
                name: "IX_Countries_Name",
                schema: "parties",
                table: "Countries",
                column: "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ElectronicAddresses_Address",
                schema: "parties",
                table: "ElectronicAddresses",
                column: "Address");

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

            migrationBuilder.CreateIndex(
                name: "IX_Enterprises_LegalName",
                schema: "parties",
                table: "Enterprises",
                column: "LegalName",
                unique: true,
                filter: "[LegalName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Enterprises_LegalStructureId",
                schema: "parties",
                table: "Enterprises",
                column: "LegalStructureId");

            migrationBuilder.CreateIndex(
                name: "IX_Facilities_FacilityTypeId",
                schema: "parties",
                table: "Facilities",
                column: "FacilityTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Facilities_Number",
                schema: "parties",
                table: "Facilities",
                column: "Number");

            migrationBuilder.CreateIndex(
                name: "IX_Facilities_PartOfId",
                schema: "parties",
                table: "Facilities",
                column: "PartOfId");

            migrationBuilder.CreateIndex(
                name: "IX_FacilityRoles_FacilityRoleTypeId",
                schema: "parties",
                table: "FacilityRoles",
                column: "FacilityRoleTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_FacilityRoleTypes_Code",
                schema: "parties",
                table: "FacilityRoleTypes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FacilityTypes_Code",
                schema: "parties",
                table: "FacilityTypes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FinancialAccounts_OwnerId_Number",
                schema: "parties",
                table: "FinancialAccounts",
                columns: new[] { "OwnerId", "Number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GenderTypes_Code",
                schema: "parties",
                table: "GenderTypes",
                column: "Code",
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
                name: "IX_Languages_Code",
                schema: "parties",
                table: "Languages",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LegalStructures_Code",
                schema: "parties",
                table: "LegalStructures",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_Name",
                schema: "parties",
                table: "Organizations",
                column: "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");

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
                name: "IX_PartyContactMechanisms_PartyId_PartyRoleTypeId_ContactMechanismId_FromDateUtc",
                schema: "parties",
                table: "PartyContactMechanisms",
                columns: new[] { "PartyId", "PartyRoleTypeId", "ContactMechanismId", "FromDateUtc" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PartyContactMechanisms_PartyRoleTypeId",
                schema: "parties",
                table: "PartyContactMechanisms",
                column: "PartyRoleTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PartyRelationships_PartyRelationshipTypeId",
                schema: "parties",
                table: "PartyRelationships",
                column: "PartyRelationshipTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PartyRelationshipTypes_Code",
                schema: "parties",
                table: "PartyRelationshipTypes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PartyRoles_PartyId",
                schema: "parties",
                table: "PartyRoles",
                column: "PartyId");

            migrationBuilder.CreateIndex(
                name: "IX_PartyRoles_TypeId_PartyId_FromDateUtc",
                schema: "parties",
                table: "PartyRoles",
                columns: new[] { "TypeId", "PartyId", "FromDateUtc" },
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

            migrationBuilder.CreateIndex(
                name: "IX_People_GenderTypeId",
                schema: "parties",
                table: "People",
                column: "GenderTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonNames_FirstName_MiddleName_LastName",
                schema: "parties",
                table: "PersonNames",
                columns: new[] { "FirstName", "MiddleName", "LastName" });

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

            migrationBuilder.CreateIndex(
                name: "IX_PostalAddresses_CountryId",
                schema: "parties",
                table: "PostalAddresses",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_TelecommunicationNumbers_CountryId_Number",
                schema: "parties",
                table: "TelecommunicationNumbers",
                columns: new[] { "CountryId", "Number" },
                unique: true,
                filter: "[CountryId] IS NOT NULL AND [Number] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Beds",
                schema: "parties");

            migrationBuilder.DropTable(
                name: "Customers",
                schema: "parties");

            migrationBuilder.DropTable(
                name: "ElectronicAddresses",
                schema: "parties");

            migrationBuilder.DropTable(
                name: "Employments",
                schema: "parties");

            migrationBuilder.DropTable(
                name: "Enterprises",
                schema: "parties");

            migrationBuilder.DropTable(
                name: "FacilityRoles",
                schema: "parties");

            migrationBuilder.DropTable(
                name: "FinancialAccounts",
                schema: "parties");

            migrationBuilder.DropTable(
                name: "Organizations",
                schema: "parties");

            migrationBuilder.DropTable(
                name: "PartyClassifications",
                schema: "parties");

            migrationBuilder.DropTable(
                name: "PartyContactMechanisms",
                schema: "parties");

            migrationBuilder.DropTable(
                name: "PersonNames",
                schema: "parties");

            migrationBuilder.DropTable(
                name: "PostalAddresses",
                schema: "parties");

            migrationBuilder.DropTable(
                name: "TelecommunicationNumbers",
                schema: "parties");

            migrationBuilder.DropTable(
                name: "Rooms",
                schema: "parties");

            migrationBuilder.DropTable(
                name: "PartyRelationships",
                schema: "parties");

            migrationBuilder.DropTable(
                name: "LegalStructures",
                schema: "parties");

            migrationBuilder.DropTable(
                name: "PartyRoles",
                schema: "parties");

            migrationBuilder.DropTable(
                name: "FacilityRoleTypes",
                schema: "parties");

            migrationBuilder.DropTable(
                name: "PartyCategories",
                schema: "parties");

            migrationBuilder.DropTable(
                name: "Languages",
                schema: "parties");

            migrationBuilder.DropTable(
                name: "People",
                schema: "parties");

            migrationBuilder.DropTable(
                name: "ContactMechanisms",
                schema: "parties");

            migrationBuilder.DropTable(
                name: "Countries",
                schema: "parties");

            migrationBuilder.DropTable(
                name: "Facilities",
                schema: "parties");

            migrationBuilder.DropTable(
                name: "PartyRelationshipTypes",
                schema: "parties");

            migrationBuilder.DropTable(
                name: "PartyRoleTypes",
                schema: "parties");

            migrationBuilder.DropTable(
                name: "GenderTypes",
                schema: "parties");

            migrationBuilder.DropTable(
                name: "Parties",
                schema: "parties");

            migrationBuilder.DropTable(
                name: "ContactMechanismTypes",
                schema: "parties");

            migrationBuilder.DropTable(
                name: "GeographicBoundaries",
                schema: "parties");

            migrationBuilder.DropTable(
                name: "FacilityTypes",
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
