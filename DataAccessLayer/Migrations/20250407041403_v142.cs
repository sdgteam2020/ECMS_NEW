using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v142 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "TrnFaultyCard",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TrnFaultyCard_UserId",
                table: "TrnFaultyCard",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TrnFaultyCard_UserProfile_UserId",
                table: "TrnFaultyCard",
                column: "UserId",
                principalTable: "UserProfile",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrnFaultyCard_UserProfile_UserId",
                table: "TrnFaultyCard");

            migrationBuilder.DropIndex(
                name: "IX_TrnFaultyCard_UserId",
                table: "TrnFaultyCard");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "TrnFaultyCard");
        }
    }
}
