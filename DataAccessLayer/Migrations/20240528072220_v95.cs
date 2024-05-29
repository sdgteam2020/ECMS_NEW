using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v95 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "TrnDomainMapping",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedOn",
                table: "TrnDomainMapping",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Updatedby",
                table: "TrnDomainMapping",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TrnDomainMapping_Updatedby",
                table: "TrnDomainMapping",
                column: "Updatedby");

            migrationBuilder.AddForeignKey(
                name: "FK_TrnDomainMapping_AspNetUsers_Updatedby",
                table: "TrnDomainMapping",
                column: "Updatedby",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrnDomainMapping_AspNetUsers_Updatedby",
                table: "TrnDomainMapping");

            migrationBuilder.DropIndex(
                name: "IX_TrnDomainMapping_Updatedby",
                table: "TrnDomainMapping");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "TrnDomainMapping");

            migrationBuilder.DropColumn(
                name: "UpdatedOn",
                table: "TrnDomainMapping");

            migrationBuilder.DropColumn(
                name: "Updatedby",
                table: "TrnDomainMapping");
        }
    }
}
