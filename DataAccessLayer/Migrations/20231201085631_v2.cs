using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MUnit_MArmedType_ArmedId",
                table: "MUnit");

            migrationBuilder.DropIndex(
                name: "IX_MUnit_ArmedId",
                table: "MUnit");

            migrationBuilder.DropColumn(
                name: "ArmedId",
                table: "MUnit");

            migrationBuilder.AddColumn<int>(
                name: "RegistrationId",
                table: "BasicDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UnitId",
                table: "BasicDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "MRegistration",
                columns: table => new
                {
                    RegistrationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MRegistration", x => x.RegistrationId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BasicDetails_RegimentalId",
                table: "BasicDetails",
                column: "RegimentalId");

            migrationBuilder.CreateIndex(
                name: "IX_BasicDetails_RegistrationId",
                table: "BasicDetails",
                column: "RegistrationId");

            migrationBuilder.CreateIndex(
                name: "IX_BasicDetails_UnitId",
                table: "BasicDetails",
                column: "UnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_BasicDetails_MRegimental_RegimentalId",
                table: "BasicDetails",
                column: "RegimentalId",
                principalTable: "MRegimental",
                principalColumn: "RegId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BasicDetails_MRegistration_RegistrationId",
                table: "BasicDetails",
                column: "RegistrationId",
                principalTable: "MRegistration",
                principalColumn: "RegistrationId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BasicDetails_MUnit_UnitId",
                table: "BasicDetails",
                column: "UnitId",
                principalTable: "MUnit",
                principalColumn: "UnitId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BasicDetails_MRegimental_RegimentalId",
                table: "BasicDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_BasicDetails_MRegistration_RegistrationId",
                table: "BasicDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_BasicDetails_MUnit_UnitId",
                table: "BasicDetails");

            migrationBuilder.DropTable(
                name: "MRegistration");

            migrationBuilder.DropIndex(
                name: "IX_BasicDetails_RegimentalId",
                table: "BasicDetails");

            migrationBuilder.DropIndex(
                name: "IX_BasicDetails_RegistrationId",
                table: "BasicDetails");

            migrationBuilder.DropIndex(
                name: "IX_BasicDetails_UnitId",
                table: "BasicDetails");

            migrationBuilder.DropColumn(
                name: "RegistrationId",
                table: "BasicDetails");

            migrationBuilder.DropColumn(
                name: "UnitId",
                table: "BasicDetails");

            migrationBuilder.AddColumn<int>(
                name: "ArmedId",
                table: "MUnit",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_MUnit_ArmedId",
                table: "MUnit",
                column: "ArmedId");

            migrationBuilder.AddForeignKey(
                name: "FK_MUnit_MArmedType_ArmedId",
                table: "MUnit",
                column: "ArmedId",
                principalTable: "MArmedType",
                principalColumn: "ArmedId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
