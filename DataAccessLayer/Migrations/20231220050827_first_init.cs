using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class first_init : Migration
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
                    UpdatedOn = table.Column<DateTime>(type: "datetime", nullable: false)
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
                    UpdatedOn = table.Column<DateTime>(type: "datetime", nullable: false)
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
                    Created = table.Column<DateTime>(type: "datetime", nullable: false)
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
                name: "MApplyFor",
                columns: table => new
                {
                    ApplyForId = table.Column<byte>(type: "tinyint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "VARCHAR(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MApplyFor", x => x.ApplyForId);
                });

            migrationBuilder.CreateTable(
                name: "MArmedCats",
                columns: table => new
                {
                    ArmedCatId = table.Column<byte>(type: "tinyint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Order = table.Column<byte>(type: "tinyint", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MArmedCats", x => x.ArmedCatId);
                });

            migrationBuilder.CreateTable(
                name: "MComd",
                columns: table => new
                {
                    ComdId = table.Column<byte>(type: "tinyint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ComdName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ComdAbbreviation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Orderby = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MComd", x => x.ComdId);
                });

            migrationBuilder.CreateTable(
                name: "MFormation",
                columns: table => new
                {
                    FormationId = table.Column<byte>(type: "tinyint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FormationName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MFormation", x => x.FormationId);
                });

            migrationBuilder.CreateTable(
                name: "MFwdType",
                columns: table => new
                {
                    TypeId = table.Column<byte>(type: "tinyint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
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
                    TypeId = table.Column<byte>(type: "tinyint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
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
                    UpdatedOn = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MMappingProfile", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MRank",
                columns: table => new
                {
                    RankId = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RankName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RankAbbreviation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Orderby = table.Column<short>(type: "smallint", nullable: false),
                    Type = table.Column<byte>(type: "tinyint", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MRank", x => x.RankId);
                });

            migrationBuilder.CreateTable(
                name: "MRegistration",
                columns: table => new
                {
                    RegistrationId = table.Column<byte>(type: "tinyint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    ApplyForId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MRegistration", x => x.RegistrationId);
                });

            migrationBuilder.CreateTable(
                name: "MStepCounterStep",
                columns: table => new
                {
                    StepId = table.Column<byte>(type: "tinyint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
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
                    UpdatedOn = table.Column<DateTime>(type: "datetime", nullable: false)
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
                    ArmedId = table.Column<byte>(type: "tinyint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArmedName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Abbreviation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FlagInf = table.Column<bool>(type: "bit", nullable: false),
                    ArmedCatId = table.Column<byte>(type: "tinyint", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime", nullable: false)
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
                    CorpsId = table.Column<byte>(type: "tinyint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CorpsName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ComdId = table.Column<byte>(type: "tinyint", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime", nullable: false)
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
                    ApptId = table.Column<byte>(type: "tinyint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppointmentName = table.Column<string>(type: "VARCHAR(50)", nullable: false),
                    FormationId = table.Column<byte>(type: "tinyint", nullable: false),
                    mFormationFormationId = table.Column<byte>(type: "tinyint", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime", nullable: false)
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
                    RegId = table.Column<byte>(type: "tinyint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Abbreviation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ArmedId = table.Column<byte>(type: "tinyint", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime", nullable: false)
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
                    DivId = table.Column<byte>(type: "tinyint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DivName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ComdId = table.Column<byte>(type: "tinyint", nullable: false),
                    CorpsId = table.Column<byte>(type: "tinyint", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime", nullable: false)
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
                    ArmyNo = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    RankId = table.Column<short>(type: "smallint", nullable: false),
                    Name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    ApptId = table.Column<byte>(type: "tinyint", nullable: false),
                    IntOffr = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime", nullable: false)
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
                        name: "FK_UserProfile_MRank_RankId",
                        column: x => x.RankId,
                        principalTable: "MRank",
                        principalColumn: "RankId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BasicDetails",
                columns: table => new
                {
                    BasicDetailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false),
                    ArmedId = table.Column<byte>(type: "tinyint", nullable: false),
                    RankId = table.Column<short>(type: "smallint", nullable: false),
                    ArmyNo = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false),
                    DOB = table.Column<DateTime>(type: "datetime", nullable: false),
                    PlaceOfIssue = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    DateOfIssue = table.Column<DateTime>(type: "datetime", nullable: false),
                    IssuingAuth = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    DateOfCommissioning = table.Column<DateTime>(type: "datetime", nullable: false),
                    RegimentalId = table.Column<byte>(type: "tinyint", nullable: true),
                    ApplyForId = table.Column<byte>(type: "tinyint", nullable: false),
                    UnitId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BasicDetails", x => x.BasicDetailId);
                    table.ForeignKey(
                        name: "FK_BasicDetails_MApplyFor_ApplyForId",
                        column: x => x.ApplyForId,
                        principalTable: "MApplyFor",
                        principalColumn: "ApplyForId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BasicDetails_MArmedType_ArmedId",
                        column: x => x.ArmedId,
                        principalTable: "MArmedType",
                        principalColumn: "ArmedId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BasicDetails_MRank_RankId",
                        column: x => x.RankId,
                        principalTable: "MRank",
                        principalColumn: "RankId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BasicDetails_MRegimental_RegimentalId",
                        column: x => x.RegimentalId,
                        principalTable: "MRegimental",
                        principalColumn: "RegId",
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
                    BdeId = table.Column<byte>(type: "tinyint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BdeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ComdId = table.Column<byte>(type: "tinyint", nullable: false),
                    CorpsId = table.Column<byte>(type: "tinyint", nullable: false),
                    DivId = table.Column<byte>(type: "tinyint", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime", nullable: false)
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
                name: "TrnAddress",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BasicDetailId = table.Column<int>(type: "int", nullable: false),
                    State = table.Column<string>(type: "VARCHAR(50)", nullable: true),
                    District = table.Column<string>(type: "VARCHAR(50)", nullable: true),
                    PS = table.Column<string>(type: "VARCHAR(50)", nullable: true),
                    PO = table.Column<string>(type: "VARCHAR(50)", nullable: true),
                    Tehsil = table.Column<string>(type: "VARCHAR(50)", nullable: true),
                    Village = table.Column<string>(type: "VARCHAR(50)", nullable: true),
                    PinCode = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrnAddress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrnAddress_BasicDetails_BasicDetailId",
                        column: x => x.BasicDetailId,
                        principalTable: "BasicDetails",
                        principalColumn: "BasicDetailId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TrnIdentityInfo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BasicDetailId = table.Column<int>(type: "int", nullable: false),
                    IdenMark1 = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false),
                    IdenMark2 = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false),
                    AadhaarNo = table.Column<int>(type: "int", maxLength: 12, nullable: false),
                    Height = table.Column<float>(type: "real", nullable: false),
                    BloodGroup = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrnIdentityInfo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrnIdentityInfo_BasicDetails_BasicDetailId",
                        column: x => x.BasicDetailId,
                        principalTable: "BasicDetails",
                        principalColumn: "BasicDetailId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TrnUpload",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BasicDetailId = table.Column<int>(type: "int", nullable: false),
                    SignatureImagePath = table.Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: false),
                    PhotoImagePath = table.Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrnUpload", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrnUpload_BasicDetails_BasicDetailId",
                        column: x => x.BasicDetailId,
                        principalTable: "BasicDetails",
                        principalColumn: "BasicDetailId",
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
                    UpdatedOn = table.Column<DateTime>(type: "datetime", nullable: false)
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
                name: "TrnICardRequest",
                columns: table => new
                {
                    RequestId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BasicDetailId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    TypeId = table.Column<byte>(type: "tinyint", nullable: false),
                    TrnDomainMappingId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime", nullable: false)
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
                    table.ForeignKey(
                        name: "FK_TrnICardRequest_TrnDomainMapping_TrnDomainMappingId",
                        column: x => x.TrnDomainMappingId,
                        principalTable: "TrnDomainMapping",
                        principalColumn: "Id",
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
                    IsComplete = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime", nullable: false)
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
                    UpdatedOn = table.Column<DateTime>(type: "datetime", nullable: false)
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
                name: "IX_BasicDetails_ApplyForId",
                table: "BasicDetails",
                column: "ApplyForId");

            migrationBuilder.CreateIndex(
                name: "IX_BasicDetails_ArmedId",
                table: "BasicDetails",
                column: "ArmedId");

            migrationBuilder.CreateIndex(
                name: "IX_BasicDetails_RankId",
                table: "BasicDetails",
                column: "RankId");

            migrationBuilder.CreateIndex(
                name: "IX_BasicDetails_RegimentalId",
                table: "BasicDetails",
                column: "RegimentalId");

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
                name: "IX_TrnAddress_BasicDetailId",
                table: "TrnAddress",
                column: "BasicDetailId");

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
                name: "IX_TrnICardRequest_TrnDomainMappingId",
                table: "TrnICardRequest",
                column: "TrnDomainMappingId");

            migrationBuilder.CreateIndex(
                name: "IX_TrnICardRequest_TypeId",
                table: "TrnICardRequest",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TrnIdentityInfo_BasicDetailId",
                table: "TrnIdentityInfo",
                column: "BasicDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_TrnStepCounter_RequestId",
                table: "TrnStepCounter",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_TrnStepCounter_StepId",
                table: "TrnStepCounter",
                column: "StepId");

            migrationBuilder.CreateIndex(
                name: "IX_TrnUpload_BasicDetailId",
                table: "TrnUpload",
                column: "BasicDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfile_ApptId",
                table: "UserProfile",
                column: "ApptId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfile_RankId",
                table: "UserProfile",
                column: "RankId");
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
                name: "MRegistration");

            migrationBuilder.DropTable(
                name: "TrnAddress");

            migrationBuilder.DropTable(
                name: "TrnFwds");

            migrationBuilder.DropTable(
                name: "TrnIdentityInfo");

            migrationBuilder.DropTable(
                name: "TrnStepCounter");

            migrationBuilder.DropTable(
                name: "TrnUpload");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "MFwdType");

            migrationBuilder.DropTable(
                name: "MStepCounterStep");

            migrationBuilder.DropTable(
                name: "TrnICardRequest");

            migrationBuilder.DropTable(
                name: "BasicDetails");

            migrationBuilder.DropTable(
                name: "MICardType");

            migrationBuilder.DropTable(
                name: "TrnDomainMapping");

            migrationBuilder.DropTable(
                name: "MApplyFor");

            migrationBuilder.DropTable(
                name: "MRegimental");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "MapUnit");

            migrationBuilder.DropTable(
                name: "UserProfile");

            migrationBuilder.DropTable(
                name: "MArmedType");

            migrationBuilder.DropTable(
                name: "MBde");

            migrationBuilder.DropTable(
                name: "MUnit");

            migrationBuilder.DropTable(
                name: "MAppointment");

            migrationBuilder.DropTable(
                name: "MRank");

            migrationBuilder.DropTable(
                name: "MArmedCats");

            migrationBuilder.DropTable(
                name: "MDiv");

            migrationBuilder.DropTable(
                name: "MFormation");

            migrationBuilder.DropTable(
                name: "MCorps");

            migrationBuilder.DropTable(
                name: "MComd");
        }
    }
}
