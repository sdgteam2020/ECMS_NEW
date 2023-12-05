using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
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
                name: "BasicDetailTemps",
                columns: table => new
                {
                    BasicDetailTempId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    ServiceNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DOB = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateOfCommissioning = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PermanentAddress = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Observations = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BasicDetailTemps", x => x.BasicDetailTempId);
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
                name: "MApiData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ServiceNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DOB = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateOfCommissioning = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PermanentAddress = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MApiData", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MArmedCats",
                columns: table => new
                {
                    ArmedCatId = table.Column<byte>(type: "tinyint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Order = table.Column<byte>(type: "tinyint", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MArmedCats", x => x.ArmedCatId);
                });

            migrationBuilder.CreateTable(
                name: "MComd",
                columns: table => new
                {
                    ComdId = table.Column<byte>(type: "tinyint", nullable: false),
                    ComdName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ComdAbbreviation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Orderby = table.Column<int>(type: "int", nullable: false),
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
                    FormationId = table.Column<byte>(type: "tinyint", nullable: false),
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
                name: "MFwdType",
                columns: table => new
                {
                    TypeId = table.Column<byte>(type: "tinyint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MFwdType", x => x.TypeId);
                });

            migrationBuilder.CreateTable(
                name: "MICardType",
                columns: table => new
                {
                    TypeId = table.Column<byte>(type: "tinyint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MICardType", x => x.TypeId);
                });

            migrationBuilder.CreateTable(
                name: "MMappingProfile",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    UnitId = table.Column<int>(type: "int", nullable: false),
                    IOId = table.Column<int>(type: "int", nullable: false),
                    GSOId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MMappingProfile", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MRank",
                columns: table => new
                {
                    RankId = table.Column<byte>(type: "tinyint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RankName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RankAbbreviation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Orderby = table.Column<byte>(type: "tinyint", nullable: false),
                    Type = table.Column<byte>(type: "tinyint", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MRank", x => x.RankId);
                });

            migrationBuilder.CreateTable(
                name: "MRegistration",
                columns: table => new
                {
                    RegistrationId = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MRegistration", x => x.RegistrationId);
                });

            migrationBuilder.CreateTable(
                name: "MStepCounterStep",
                columns: table => new
                {
                    StepId = table.Column<byte>(type: "tinyint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MStepCounterStep", x => x.StepId);
                });

            migrationBuilder.CreateTable(
                name: "MUnit",
                columns: table => new
                {
                    UnitId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Sus_no = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    Suffix = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    UnitName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Abbreviation = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                name: "MArmedType",
                columns: table => new
                {
                    ArmedId = table.Column<byte>(type: "tinyint", nullable: false),
                    ArmedName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Abbreviation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FlagInf = table.Column<bool>(type: "bit", nullable: false),
                    ArmedCatId = table.Column<byte>(type: "tinyint", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MArmedType", x => x.ArmedId);
                    table.ForeignKey(
                        name: "FK_MArmedType_MArmedCats_ArmedCatId",
                        column: x => x.ArmedCatId,
                        principalTable: "MArmedCats",
                        principalColumn: "ArmedCatId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MCorps",
                columns: table => new
                {
                    CorpsId = table.Column<byte>(type: "tinyint", nullable: false),
                    CorpsName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ComdId = table.Column<byte>(type: "tinyint", nullable: false),
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
                    ApptId = table.Column<byte>(type: "tinyint", nullable: false),
                    AppointmentName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FormationId = table.Column<byte>(type: "tinyint", nullable: false),
                    mFormationFormationId = table.Column<byte>(type: "tinyint", nullable: true),
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
                name: "MRegimental",
                columns: table => new
                {
                    RegId = table.Column<byte>(type: "tinyint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Abbreviation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ArmedId = table.Column<byte>(type: "tinyint", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MRegimental", x => x.RegId);
                    table.ForeignKey(
                        name: "FK_MRegimental_MArmedType_ArmedId",
                        column: x => x.ArmedId,
                        principalTable: "MArmedType",
                        principalColumn: "ArmedId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MDiv",
                columns: table => new
                {
                    DivId = table.Column<byte>(type: "tinyint", nullable: false),
                    DivName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ComdId = table.Column<byte>(type: "tinyint", nullable: false),
                    CorpsId = table.Column<byte>(type: "tinyint", nullable: false),
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
                name: "UserProfile",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArmyNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RankId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApptId = table.Column<int>(type: "int", nullable: false),
                    MAppointmentApptId = table.Column<byte>(type: "tinyint", nullable: true),
                    IntOffr = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfile", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_UserProfile_MAppointment_MAppointmentApptId",
                        column: x => x.MAppointmentApptId,
                        principalTable: "MAppointment",
                        principalColumn: "ApptId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BasicDetails",
                columns: table => new
                {
                    BasicDetailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RegistrationType = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    ArmedId = table.Column<short>(type: "smallint", nullable: false),
                    ArmedId1 = table.Column<byte>(type: "tinyint", nullable: true),
                    RankId = table.Column<short>(type: "smallint", nullable: false),
                    RankId1 = table.Column<byte>(type: "tinyint", nullable: true),
                    ServiceNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IdentityMark = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DOB = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Height = table.Column<int>(type: "int", nullable: false),
                    AadhaarNo = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    BloodGroup = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    PlaceOfIssue = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DateOfIssue = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IssuingAuth = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SignatureImagePath = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PhotoImagePath = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DateOfCommissioning = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PermanentAddress = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StatusLevel = table.Column<byte>(type: "tinyint", nullable: false),
                    RegimentalId = table.Column<short>(type: "smallint", nullable: true),
                    RegimentalRegId = table.Column<byte>(type: "tinyint", nullable: true),
                    RegistrationId = table.Column<short>(type: "smallint", nullable: false),
                    UnitId = table.Column<int>(type: "int", nullable: false),
                    Step = table.Column<int>(type: "int", nullable: false),
                    IsSubmit = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BasicDetails", x => x.BasicDetailId);
                    table.ForeignKey(
                        name: "FK_BasicDetails_MArmedType_ArmedId1",
                        column: x => x.ArmedId1,
                        principalTable: "MArmedType",
                        principalColumn: "ArmedId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BasicDetails_MRank_RankId1",
                        column: x => x.RankId1,
                        principalTable: "MRank",
                        principalColumn: "RankId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BasicDetails_MRegimental_RegimentalRegId",
                        column: x => x.RegimentalRegId,
                        principalTable: "MRegimental",
                        principalColumn: "RegId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BasicDetails_MRegistration_RegistrationId",
                        column: x => x.RegistrationId,
                        principalTable: "MRegistration",
                        principalColumn: "RegistrationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BasicDetails_MUnit_UnitId",
                        column: x => x.UnitId,
                        principalTable: "MUnit",
                        principalColumn: "UnitId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MBde",
                columns: table => new
                {
                    BdeId = table.Column<byte>(type: "tinyint", nullable: false),
                    BdeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ComdId = table.Column<byte>(type: "tinyint", nullable: false),
                    CorpsId = table.Column<byte>(type: "tinyint", nullable: false),
                    DivId = table.Column<byte>(type: "tinyint", nullable: false),
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
                name: "TrnICardRequest",
                columns: table => new
                {
                    RequestId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BasicDetailId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    TypeId = table.Column<byte>(type: "tinyint", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrnICardRequest", x => x.RequestId);
                    table.ForeignKey(
                        name: "FK_TrnICardRequest_BasicDetails_BasicDetailId",
                        column: x => x.BasicDetailId,
                        principalTable: "BasicDetails",
                        principalColumn: "BasicDetailId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrnICardRequest_MICardType_TypeId",
                        column: x => x.TypeId,
                        principalTable: "MICardType",
                        principalColumn: "TypeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MapUnit",
                columns: table => new
                {
                    UnitMapId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UnitId = table.Column<int>(type: "int", nullable: false),
                    FmnBrach = table.Column<bool>(type: "bit", nullable: false),
                    ComdId = table.Column<byte>(type: "tinyint", nullable: false),
                    CorpsId = table.Column<byte>(type: "tinyint", nullable: false),
                    DivId = table.Column<byte>(type: "tinyint", nullable: false),
                    BdeId = table.Column<byte>(type: "tinyint", nullable: false),
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
                name: "TrnStepCounter",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestId = table.Column<int>(type: "int", nullable: false),
                    StepId = table.Column<byte>(type: "tinyint", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrnStepCounter", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrnStepCounter_MStepCounterStep_StepId",
                        column: x => x.StepId,
                        principalTable: "MStepCounterStep",
                        principalColumn: "StepId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrnStepCounter_TrnICardRequest_RequestId",
                        column: x => x.RequestId,
                        principalTable: "TrnICardRequest",
                        principalColumn: "RequestId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TrnDomainMapping",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AspNetUsersId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    UnitId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrnDomainMapping", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrnDomainMapping_AspNetUsers_AspNetUsersId",
                        column: x => x.AspNetUsersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrnDomainMapping_MapUnit_UnitId",
                        column: x => x.UnitId,
                        principalTable: "MapUnit",
                        principalColumn: "UnitMapId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrnDomainMapping_UserProfile_UserId",
                        column: x => x.UserId,
                        principalTable: "UserProfile",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TrnFwds",
                columns: table => new
                {
                    TrnFwdId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestId = table.Column<int>(type: "int", nullable: false),
                    ToUserId = table.Column<int>(type: "int", nullable: false),
                    FromUserId = table.Column<int>(type: "int", nullable: false),
                    FromAspNetUsersId = table.Column<int>(type: "int", nullable: true),
                    ToAspNetUsersId = table.Column<int>(type: "int", nullable: true),
                    UnitId = table.Column<int>(type: "int", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    TypeId = table.Column<byte>(type: "tinyint", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrnFwds", x => x.TrnFwdId);
                    table.ForeignKey(
                        name: "FK_TrnFwds_AspNetUsers_FromAspNetUsersId",
                        column: x => x.FromAspNetUsersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrnFwds_AspNetUsers_ToAspNetUsersId",
                        column: x => x.ToAspNetUsersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrnFwds_MFwdType_TypeId",
                        column: x => x.TypeId,
                        principalTable: "MFwdType",
                        principalColumn: "TypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrnFwds_MapUnit_UnitId",
                        column: x => x.UnitId,
                        principalTable: "MapUnit",
                        principalColumn: "UnitMapId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrnFwds_TrnICardRequest_RequestId",
                        column: x => x.RequestId,
                        principalTable: "TrnICardRequest",
                        principalColumn: "RequestId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrnFwds_UserProfile_FromUserId",
                        column: x => x.FromUserId,
                        principalTable: "UserProfile",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrnFwds_UserProfile_ToUserId",
                        column: x => x.ToUserId,
                        principalTable: "UserProfile",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "MRegistration",
                columns: new[] { "RegistrationId", "IsActive", "Name", "Order", "Type", "UpdatedOn", "Updatedby" },
                values: new object[,]
                {
                    { (short)1, false, "Apply for Self (Officer)", 1, 1, new DateTime(2023, 12, 5, 10, 12, 28, 392, DateTimeKind.Unspecified).AddTicks(6839), 1 },
                    { (short)2, false, "Apply for Unit Officer", 2, 1, new DateTime(2023, 12, 5, 10, 12, 28, 392, DateTimeKind.Unspecified).AddTicks(6845), 1 },
                    { (short)3, false, "Apply for Other Unit Officer", 3, 1, new DateTime(2023, 12, 5, 10, 12, 28, 392, DateTimeKind.Unspecified).AddTicks(6849), 1 },
                    { (short)4, false, "Apply for Unit JCOs/OR", 4, 2, new DateTime(2023, 12, 5, 10, 12, 28, 392, DateTimeKind.Unspecified).AddTicks(6852), 1 },
                    { (short)5, false, "Apply for Other Unit JCOs/OR", 5, 2, new DateTime(2023, 12, 5, 10, 12, 28, 392, DateTimeKind.Unspecified).AddTicks(6856), 1 }
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
                name: "IX_BasicDetails_ArmedId1",
                table: "BasicDetails",
                column: "ArmedId1");

            migrationBuilder.CreateIndex(
                name: "IX_BasicDetails_RankId1",
                table: "BasicDetails",
                column: "RankId1");

            migrationBuilder.CreateIndex(
                name: "IX_BasicDetails_RegimentalRegId",
                table: "BasicDetails",
                column: "RegimentalRegId");

            migrationBuilder.CreateIndex(
                name: "IX_BasicDetails_RegistrationId",
                table: "BasicDetails",
                column: "RegistrationId");

            migrationBuilder.CreateIndex(
                name: "IX_BasicDetails_UnitId",
                table: "BasicDetails",
                column: "UnitId");

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
                name: "IX_MArmedType_ArmedCatId",
                table: "MArmedType",
                column: "ArmedCatId");

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
                name: "IX_MDiv_ComdId",
                table: "MDiv",
                column: "ComdId");

            migrationBuilder.CreateIndex(
                name: "IX_MDiv_CorpsId",
                table: "MDiv",
                column: "CorpsId");

            migrationBuilder.CreateIndex(
                name: "IX_MRegimental_ArmedId",
                table: "MRegimental",
                column: "ArmedId");

            migrationBuilder.CreateIndex(
                name: "IX_TrnDomainMapping_AspNetUsersId",
                table: "TrnDomainMapping",
                column: "AspNetUsersId");

            migrationBuilder.CreateIndex(
                name: "IX_TrnDomainMapping_UnitId",
                table: "TrnDomainMapping",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_TrnDomainMapping_UserId",
                table: "TrnDomainMapping",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TrnFwds_FromAspNetUsersId",
                table: "TrnFwds",
                column: "FromAspNetUsersId");

            migrationBuilder.CreateIndex(
                name: "IX_TrnFwds_FromUserId",
                table: "TrnFwds",
                column: "FromUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TrnFwds_RequestId",
                table: "TrnFwds",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_TrnFwds_ToAspNetUsersId",
                table: "TrnFwds",
                column: "ToAspNetUsersId");

            migrationBuilder.CreateIndex(
                name: "IX_TrnFwds_ToUserId",
                table: "TrnFwds",
                column: "ToUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TrnFwds_TypeId",
                table: "TrnFwds",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TrnFwds_UnitId",
                table: "TrnFwds",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_TrnICardRequest_BasicDetailId",
                table: "TrnICardRequest",
                column: "BasicDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_TrnICardRequest_TypeId",
                table: "TrnICardRequest",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TrnStepCounter_RequestId",
                table: "TrnStepCounter",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_TrnStepCounter_StepId",
                table: "TrnStepCounter",
                column: "StepId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfile_MAppointmentApptId",
                table: "UserProfile",
                column: "MAppointmentApptId");
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
                name: "BasicDetailTemps");

            migrationBuilder.DropTable(
                name: "DocUploads");

            migrationBuilder.DropTable(
                name: "Errors");

            migrationBuilder.DropTable(
                name: "MApiData");

            migrationBuilder.DropTable(
                name: "MMappingProfile");

            migrationBuilder.DropTable(
                name: "TrnDomainMapping");

            migrationBuilder.DropTable(
                name: "TrnFwds");

            migrationBuilder.DropTable(
                name: "TrnStepCounter");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "MFwdType");

            migrationBuilder.DropTable(
                name: "MapUnit");

            migrationBuilder.DropTable(
                name: "UserProfile");

            migrationBuilder.DropTable(
                name: "MStepCounterStep");

            migrationBuilder.DropTable(
                name: "TrnICardRequest");

            migrationBuilder.DropTable(
                name: "MBde");

            migrationBuilder.DropTable(
                name: "MAppointment");

            migrationBuilder.DropTable(
                name: "BasicDetails");

            migrationBuilder.DropTable(
                name: "MICardType");

            migrationBuilder.DropTable(
                name: "MDiv");

            migrationBuilder.DropTable(
                name: "MFormation");

            migrationBuilder.DropTable(
                name: "MRank");

            migrationBuilder.DropTable(
                name: "MRegimental");

            migrationBuilder.DropTable(
                name: "MRegistration");

            migrationBuilder.DropTable(
                name: "MUnit");

            migrationBuilder.DropTable(
                name: "MCorps");

            migrationBuilder.DropTable(
                name: "MArmedType");

            migrationBuilder.DropTable(
                name: "MComd");

            migrationBuilder.DropTable(
                name: "MArmedCats");
        }
    }
}
