using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v158 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "DispatchedOn",
                table: "TrnPostingOut",
                type: "datetime",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DispatchUpdatedOn",
                table: "TrnPostingOut",
                type: "datetime",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TrnPostingOut_DispatchUpdatedBy",
                table: "TrnPostingOut",
                column: "DispatchUpdatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_TrnPostingOut_AspNetUsers_DispatchUpdatedBy",
                table: "TrnPostingOut",
                column: "DispatchUpdatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrnPostingOut_AspNetUsers_DispatchUpdatedBy",
                table: "TrnPostingOut");

            migrationBuilder.DropIndex(
                name: "IX_TrnPostingOut_DispatchUpdatedBy",
                table: "TrnPostingOut");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DispatchedOn",
                table: "TrnPostingOut",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DispatchUpdatedOn",
                table: "TrnPostingOut",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true);
        }
    }
}
