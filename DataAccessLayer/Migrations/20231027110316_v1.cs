using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DomainId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    AdminFlag = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Fd1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Fd2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DocUploads",
                columns: table => new
                {
                    DocUploadId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DocPath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocUploads", x => x.DocUploadId);
                });

            migrationBuilder.CreateTable(
                name: "Errors",
                columns: table => new
                {
                    ErrorId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Values = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Errors", x => x.ErrorId);
                });

            migrationBuilder.CreateTable(
                name: "MArmedType",
                columns: table => new
                {
                    ArmedId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArmedName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Abbreviation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MArmedType", x => x.ArmedId);
                });

            migrationBuilder.CreateTable(
                name: "MComd",
                columns: table => new
                {
                    ComdId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ComdName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ComdAbbreviation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MComd", x => x.ComdId);
                });

            migrationBuilder.CreateTable(
                name: "MFormation",
                columns: table => new
                {
                    FormationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FormationName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MFormation", x => x.FormationId);
                });

            migrationBuilder.CreateTable(
                name: "MRank",
                columns: table => new
                {
                    RankId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RankName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ComdAbbreviation = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MRank", x => x.RankId);
                });

            migrationBuilder.CreateTable(
                name: "MStates",
                columns: table => new
                {
                    StateId = table.Column<int>(type: "int", nullable: false),
                    StateCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StateName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MStates", x => x.StateId);
                });

            migrationBuilder.CreateTable(
                name: "MUnit",
                columns: table => new
                {
                    UnitId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Sus_no = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    Suffix = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    Unit_desc = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsVerify = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MUnit", x => x.UnitId);
                });

            migrationBuilder.CreateTable(
                name: "ProfileData",
                columns: table => new
                {
                    ProfileDataId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArmyNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArmyNumberPart1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArmyNumberPart2 = table.Column<int>(type: "int", nullable: true),
                    ArmyNumberPart3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Rank = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Appointment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DomainId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UnitSusNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UnitSusNoPart1 = table.Column<int>(type: "int", nullable: true),
                    UnitSusNoPart2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UnitName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TypeOfUnit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ComdId = table.Column<int>(type: "int", nullable: false),
                    Corps = table.Column<int>(type: "int", nullable: false),
                    Div = table.Column<int>(type: "int", nullable: false),
                    Bde = table.Column<int>(type: "int", nullable: false),
                    InitiatingOfficerArmyNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IOArmyNumberPart1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IOArmyNumberPart2 = table.Column<int>(type: "int", nullable: true),
                    IOArmyNumberPart3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IORank = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IOName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IOAppointment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IOUnitFormation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GISOfficerArmyNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GISArmyNumberPart1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GISArmyNumberPart2 = table.Column<int>(type: "int", nullable: true),
                    GISArmyNumberPart3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GISRank = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GISName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GISAppointment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GISUnitFormation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsSubmit = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileData", x => x.ProfileDataId);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MCorps",
                columns: table => new
                {
                    CorpsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CorpsName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ComdId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MCorps", x => x.CorpsId);
                    table.ForeignKey(
                        name: "FK_MCorps_MComd_ComdId",
                        column: x => x.ComdId,
                        principalTable: "MComd",
                        principalColumn: "ComdId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MAppointment",
                columns: table => new
                {
                    ApptId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppointmentName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FormationId = table.Column<int>(type: "int", nullable: false),
                    mFormationFormationId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MAppointment", x => x.ApptId);
                    table.ForeignKey(
                        name: "FK_MAppointment_MFormation_mFormationFormationId",
                        column: x => x.mFormationFormationId,
                        principalTable: "MFormation",
                        principalColumn: "FormationId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MDistricts",
                columns: table => new
                {
                    DistrictId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StateId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MDistricts", x => x.DistrictId);
                    table.ForeignKey(
                        name: "FK_MDistricts_MStates_StateId",
                        column: x => x.StateId,
                        principalTable: "MStates",
                        principalColumn: "StateId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MDiv",
                columns: table => new
                {
                    DivId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DivName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ComdId = table.Column<int>(type: "int", nullable: false),
                    CorpsId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MDiv", x => x.DivId);
                    table.ForeignKey(
                        name: "FK_MDiv_MComd_ComdId",
                        column: x => x.ComdId,
                        principalTable: "MComd",
                        principalColumn: "ComdId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MDiv_MCorps_CorpsId",
                        column: x => x.CorpsId,
                        principalTable: "MCorps",
                        principalColumn: "CorpsId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BasicDetails",
                columns: table => new
                {
                    BasicDetailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    Rank = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ArmService = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ServiceNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IdentityMark = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    DOB = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Height = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AadhaarNo = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: true),
                    BloodGroup = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    PlaceOfIssue = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DateOfIssue = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IssuingAuth = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SignatureImagePath = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PhotoImagePath = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DateOfCommissioning = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PermanentAddress = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsSubmit = table.Column<bool>(type: "bit", nullable: false),
                    DistrictId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BasicDetails", x => x.BasicDetailId);
                    table.ForeignKey(
                        name: "FK_BasicDetails_MDistricts_DistrictId",
                        column: x => x.DistrictId,
                        principalTable: "MDistricts",
                        principalColumn: "DistrictId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MBde",
                columns: table => new
                {
                    BdeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BdeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ComdId = table.Column<int>(type: "int", nullable: false),
                    CorpsId = table.Column<int>(type: "int", nullable: false),
                    DivId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MBde", x => x.BdeId);
                    table.ForeignKey(
                        name: "FK_MBde_MComd_ComdId",
                        column: x => x.ComdId,
                        principalTable: "MComd",
                        principalColumn: "ComdId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MBde_MCorps_CorpsId",
                        column: x => x.CorpsId,
                        principalTable: "MCorps",
                        principalColumn: "CorpsId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MBde_MDiv_DivId",
                        column: x => x.DivId,
                        principalTable: "MDiv",
                        principalColumn: "DivId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MapUnit",
                columns: table => new
                {
                    UnitMapId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UnitName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UnitId = table.Column<int>(type: "int", nullable: false),
                    ComdId = table.Column<int>(type: "int", nullable: false),
                    CorpsId = table.Column<int>(type: "int", nullable: false),
                    DivId = table.Column<int>(type: "int", nullable: false),
                    BdeId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MapUnit", x => x.UnitMapId);
                    table.ForeignKey(
                        name: "FK_MapUnit_MBde_BdeId",
                        column: x => x.BdeId,
                        principalTable: "MBde",
                        principalColumn: "BdeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MapUnit_MComd_ComdId",
                        column: x => x.ComdId,
                        principalTable: "MComd",
                        principalColumn: "ComdId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MapUnit_MCorps_CorpsId",
                        column: x => x.CorpsId,
                        principalTable: "MCorps",
                        principalColumn: "CorpsId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MapUnit_MDiv_DivId",
                        column: x => x.DivId,
                        principalTable: "MDiv",
                        principalColumn: "DivId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MapUnit_MUnit_UnitId",
                        column: x => x.UnitId,
                        principalTable: "MUnit",
                        principalColumn: "UnitId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserProfile",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArmyNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ArmyNo_Old = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RankId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApptId = table.Column<int>(type: "int", nullable: false),
                    TypeOfUnit = table.Column<int>(type: "int", nullable: false),
                    UnitId = table.Column<int>(type: "int", nullable: false),
                    MUnitUnitId = table.Column<int>(type: "int", nullable: true),
                    ComdId = table.Column<int>(type: "int", nullable: false),
                    CorpsId = table.Column<int>(type: "int", nullable: false),
                    DivId = table.Column<int>(type: "int", nullable: false),
                    BdeId = table.Column<int>(type: "int", nullable: false),
                    InitOffrsArmyNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IOArmyNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GISOfficerArmyNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsSubmit = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfile", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_UserProfile_MAppointment_ApptId",
                        column: x => x.ApptId,
                        principalTable: "MAppointment",
                        principalColumn: "ApptId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserProfile_MBde_BdeId",
                        column: x => x.BdeId,
                        principalTable: "MBde",
                        principalColumn: "BdeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserProfile_MComd_ComdId",
                        column: x => x.ComdId,
                        principalTable: "MComd",
                        principalColumn: "ComdId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserProfile_MCorps_CorpsId",
                        column: x => x.CorpsId,
                        principalTable: "MCorps",
                        principalColumn: "CorpsId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserProfile_MDiv_DivId",
                        column: x => x.DivId,
                        principalTable: "MDiv",
                        principalColumn: "DivId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserProfile_MUnit_MUnitUnitId",
                        column: x => x.MUnitUnitId,
                        principalTable: "MUnit",
                        principalColumn: "UnitId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BasicDetails_DistrictId",
                table: "BasicDetails",
                column: "DistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_MAppointment_mFormationFormationId",
                table: "MAppointment",
                column: "mFormationFormationId");

            migrationBuilder.CreateIndex(
                name: "IX_MapUnit_BdeId",
                table: "MapUnit",
                column: "BdeId");

            migrationBuilder.CreateIndex(
                name: "IX_MapUnit_ComdId",
                table: "MapUnit",
                column: "ComdId");

            migrationBuilder.CreateIndex(
                name: "IX_MapUnit_CorpsId",
                table: "MapUnit",
                column: "CorpsId");

            migrationBuilder.CreateIndex(
                name: "IX_MapUnit_DivId",
                table: "MapUnit",
                column: "DivId");

            migrationBuilder.CreateIndex(
                name: "IX_MapUnit_UnitId",
                table: "MapUnit",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_MBde_ComdId",
                table: "MBde",
                column: "ComdId");

            migrationBuilder.CreateIndex(
                name: "IX_MBde_CorpsId",
                table: "MBde",
                column: "CorpsId");

            migrationBuilder.CreateIndex(
                name: "IX_MBde_DivId",
                table: "MBde",
                column: "DivId");

            migrationBuilder.CreateIndex(
                name: "IX_MCorps_ComdId",
                table: "MCorps",
                column: "ComdId");

            migrationBuilder.CreateIndex(
                name: "IX_MDistricts_StateId",
                table: "MDistricts",
                column: "StateId");

            migrationBuilder.CreateIndex(
                name: "IX_MDiv_ComdId",
                table: "MDiv",
                column: "ComdId");

            migrationBuilder.CreateIndex(
                name: "IX_MDiv_CorpsId",
                table: "MDiv",
                column: "CorpsId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfile_ApptId",
                table: "UserProfile",
                column: "ApptId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfile_BdeId",
                table: "UserProfile",
                column: "BdeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfile_ComdId",
                table: "UserProfile",
                column: "ComdId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfile_CorpsId",
                table: "UserProfile",
                column: "CorpsId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfile_DivId",
                table: "UserProfile",
                column: "DivId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfile_MUnitUnitId",
                table: "UserProfile",
                column: "MUnitUnitId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "BasicDetails");

            migrationBuilder.DropTable(
                name: "DocUploads");

            migrationBuilder.DropTable(
                name: "Errors");

            migrationBuilder.DropTable(
                name: "MapUnit");

            migrationBuilder.DropTable(
                name: "MArmedType");

            migrationBuilder.DropTable(
                name: "MRank");

            migrationBuilder.DropTable(
                name: "ProfileData");

            migrationBuilder.DropTable(
                name: "UserProfile");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "MDistricts");

            migrationBuilder.DropTable(
                name: "MAppointment");

            migrationBuilder.DropTable(
                name: "MBde");

            migrationBuilder.DropTable(
                name: "MUnit");

            migrationBuilder.DropTable(
                name: "MStates");

            migrationBuilder.DropTable(
                name: "MFormation");

            migrationBuilder.DropTable(
                name: "MDiv");

            migrationBuilder.DropTable(
                name: "MCorps");

            migrationBuilder.DropTable(
                name: "MComd");
        }
    }
}
