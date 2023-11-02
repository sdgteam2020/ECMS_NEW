using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserProfile_MAppointment_ApptId",
                table: "UserProfile");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProfile_MUnit_MUnitUnitId",
                table: "UserProfile");

            migrationBuilder.DropIndex(
                name: "IX_UserProfile_ApptId",
                table: "UserProfile");

            migrationBuilder.DropIndex(
                name: "IX_UserProfile_MUnitUnitId",
                table: "UserProfile");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MMappingProfile",
                table: "MMappingProfile");

            migrationBuilder.DropColumn(
                name: "MUnitUnitId",
                table: "UserProfile");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "MMappingProfile",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "MMappingProfile",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MMappingProfile",
                table: "MMappingProfile",
                column: "Id");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MApiData");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MMappingProfile",
                table: "MMappingProfile");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "MMappingProfile");

            migrationBuilder.AddColumn<int>(
                name: "MUnitUnitId",
                table: "UserProfile",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "MMappingProfile",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MMappingProfile",
                table: "MMappingProfile",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfile_ApptId",
                table: "UserProfile",
                column: "ApptId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfile_MUnitUnitId",
                table: "UserProfile",
                column: "MUnitUnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfile_MAppointment_ApptId",
                table: "UserProfile",
                column: "ApptId",
                principalTable: "MAppointment",
                principalColumn: "ApptId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfile_MUnit_MUnitUnitId",
                table: "UserProfile",
                column: "MUnitUnitId",
                principalTable: "MUnit",
                principalColumn: "UnitId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
