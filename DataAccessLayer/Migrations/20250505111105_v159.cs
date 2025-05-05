using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v159 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "TrnApplClose",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TrnApplClose_UserId",
                table: "TrnApplClose",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TrnApplClose_UserProfile_UserId",
                table: "TrnApplClose",
                column: "UserId",
                principalTable: "UserProfile",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrnApplClose_UserProfile_UserId",
                table: "TrnApplClose");

            migrationBuilder.DropIndex(
                name: "IX_TrnApplClose_UserId",
                table: "TrnApplClose");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "TrnApplClose");

        }
    }
}
