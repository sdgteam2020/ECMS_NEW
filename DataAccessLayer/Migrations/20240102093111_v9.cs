using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v9 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MFmnBranches",
                columns: table => new
                {
                    FmnBranchID = table.Column<byte>(type: "tinyint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BranchName = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    Abvr = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MFmnBranches", x => x.FmnBranchID);
                });

            migrationBuilder.CreateTable(
                name: "MPso",
                columns: table => new
                {
                    PsoId = table.Column<byte>(type: "tinyint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PSOName = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    PSOAbvr = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MPso", x => x.PsoId);
                });

            migrationBuilder.CreateTable(
                name: "MSubDte",
                columns: table => new
                {
                    SubDteId = table.Column<byte>(type: "tinyint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubDteName = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    SubDteAbvr = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MSubDte", x => x.SubDteId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MFmnBranches");

            migrationBuilder.DropTable(
                name: "MPso");

            migrationBuilder.DropTable(
                name: "MSubDte");
        }
    }
}
