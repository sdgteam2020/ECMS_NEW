using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v22 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_TrnLogin_Log_RoleId",
                table: "TrnLogin_Log",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_TrnLogin_Log_AspNetRoles_RoleId",
                table: "TrnLogin_Log",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrnLogin_Log_AspNetRoles_RoleId",
                table: "TrnLogin_Log");

            migrationBuilder.DropIndex(
                name: "IX_TrnLogin_Log_RoleId",
                table: "TrnLogin_Log");
        }
    }
}
