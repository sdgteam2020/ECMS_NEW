using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v28 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "TrnPostingOut",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedOn",
                table: "TrnPostingOut",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Updatedby",
                table: "TrnPostingOut",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TrnPostingOut_Updatedby",
                table: "TrnPostingOut",
                column: "Updatedby");

            migrationBuilder.AddForeignKey(
                name: "FK_TrnPostingOut_AspNetUsers_Updatedby",
                table: "TrnPostingOut",
                column: "Updatedby",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrnPostingOut_AspNetUsers_Updatedby",
                table: "TrnPostingOut");

            migrationBuilder.DropIndex(
                name: "IX_TrnPostingOut_Updatedby",
                table: "TrnPostingOut");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "TrnPostingOut");

            migrationBuilder.DropColumn(
                name: "UpdatedOn",
                table: "TrnPostingOut");

            migrationBuilder.DropColumn(
                name: "Updatedby",
                table: "TrnPostingOut");
        }
    }
}
