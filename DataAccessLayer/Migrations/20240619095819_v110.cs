using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v110 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "RegimentalId",
                table: "BasicDetails",
                type: "tinyint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BasicDetails_RegimentalId",
                table: "BasicDetails",
                column: "RegimentalId");

            migrationBuilder.AddForeignKey(
                name: "FK_BasicDetails_MRegimental_RegimentalId",
                table: "BasicDetails",
                column: "RegimentalId",
                principalTable: "MRegimental",
                principalColumn: "RegId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BasicDetails_MRegimental_RegimentalId",
                table: "BasicDetails");

            migrationBuilder.DropIndex(
                name: "IX_BasicDetails_RegimentalId",
                table: "BasicDetails");

            migrationBuilder.DropColumn(
                name: "RegimentalId",
                table: "BasicDetails");
        }
    }
}
