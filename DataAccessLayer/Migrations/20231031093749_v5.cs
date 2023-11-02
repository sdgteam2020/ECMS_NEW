using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserProfile_MBde_BdeId",
                table: "UserProfile");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProfile_MComd_ComdId",
                table: "UserProfile");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProfile_MCorps_CorpsId",
                table: "UserProfile");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProfile_MDiv_DivId",
                table: "UserProfile");

            migrationBuilder.DropIndex(
                name: "IX_UserProfile_BdeId",
                table: "UserProfile");

            migrationBuilder.DropIndex(
                name: "IX_UserProfile_ComdId",
                table: "UserProfile");

            migrationBuilder.DropIndex(
                name: "IX_UserProfile_CorpsId",
                table: "UserProfile");

            migrationBuilder.DropIndex(
                name: "IX_UserProfile_DivId",
                table: "UserProfile");

            migrationBuilder.DropColumn(
                name: "ArmyNo_Old",
                table: "UserProfile");

            migrationBuilder.DropColumn(
                name: "BdeId",
                table: "UserProfile");

            migrationBuilder.DropColumn(
                name: "ComdId",
                table: "UserProfile");

            migrationBuilder.DropColumn(
                name: "CorpsId",
                table: "UserProfile");

            migrationBuilder.DropColumn(
                name: "DivId",
                table: "UserProfile");

            migrationBuilder.DropColumn(
                name: "GISOfficerArmyNo",
                table: "UserProfile");

            migrationBuilder.DropColumn(
                name: "IOArmyNo",
                table: "UserProfile");

            migrationBuilder.DropColumn(
                name: "InitOffrsArmyNo",
                table: "UserProfile");

            migrationBuilder.RenameColumn(
                name: "TypeOfUnit",
                table: "UserProfile",
                newName: "Updatedby");

            migrationBuilder.RenameColumn(
                name: "IsSubmit",
                table: "UserProfile",
                newName: "IsActive");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedOn",
                table: "UserProfile",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "MMappingProfile",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UnitId = table.Column<int>(type: "int", nullable: false),
                    IOId = table.Column<int>(type: "int", nullable: false),
                    GSOId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MMappingProfile", x => x.UserId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MMappingProfile");

            migrationBuilder.DropColumn(
                name: "UpdatedOn",
                table: "UserProfile");

            migrationBuilder.RenameColumn(
                name: "Updatedby",
                table: "UserProfile",
                newName: "TypeOfUnit");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "UserProfile",
                newName: "IsSubmit");

            migrationBuilder.AddColumn<string>(
                name: "ArmyNo_Old",
                table: "UserProfile",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "BdeId",
                table: "UserProfile",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ComdId",
                table: "UserProfile",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CorpsId",
                table: "UserProfile",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DivId",
                table: "UserProfile",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "GISOfficerArmyNo",
                table: "UserProfile",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IOArmyNo",
                table: "UserProfile",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InitOffrsArmyNo",
                table: "UserProfile",
                type: "nvarchar(max)",
                nullable: true);

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

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfile_MBde_BdeId",
                table: "UserProfile",
                column: "BdeId",
                principalTable: "MBde",
                principalColumn: "BdeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfile_MComd_ComdId",
                table: "UserProfile",
                column: "ComdId",
                principalTable: "MComd",
                principalColumn: "ComdId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfile_MCorps_CorpsId",
                table: "UserProfile",
                column: "CorpsId",
                principalTable: "MCorps",
                principalColumn: "CorpsId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfile_MDiv_DivId",
                table: "UserProfile",
                column: "DivId",
                principalTable: "MDiv",
                principalColumn: "DivId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
