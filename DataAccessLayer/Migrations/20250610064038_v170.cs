using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v170 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_TrnLostCards_UpdatedbyUserId",
                table: "TrnLostCards",
                column: "UpdatedbyUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TrnLostCards_UserProfile_UpdatedbyUserId",
                table: "TrnLostCards",
                column: "UpdatedbyUserId",
                principalTable: "UserProfile",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrnLostCards_UserProfile_UpdatedbyUserId",
                table: "TrnLostCards");

            migrationBuilder.DropIndex(
                name: "IX_TrnLostCards_UpdatedbyUserId",
                table: "TrnLostCards");
        }
    }
}
