using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v168 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UpdatedbyUserId",
                table: "TrnLostCards",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedbyUserId",
                table: "TrnHotlistCards",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TrnHotlistCards_UpdatedbyUserId",
                table: "TrnHotlistCards",
                column: "UpdatedbyUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TrnHotlistCards_UserProfile_UpdatedbyUserId",
                table: "TrnHotlistCards",
                column: "UpdatedbyUserId",
                principalTable: "UserProfile",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrnHotlistCards_UserProfile_UpdatedbyUserId",
                table: "TrnHotlistCards");

            migrationBuilder.DropIndex(
                name: "IX_TrnHotlistCards_UpdatedbyUserId",
                table: "TrnHotlistCards");

            migrationBuilder.DropColumn(
                name: "UpdatedbyUserId",
                table: "TrnLostCards");

            migrationBuilder.DropColumn(
                name: "UpdatedbyUserId",
                table: "TrnHotlistCards");
        }
    }
}
