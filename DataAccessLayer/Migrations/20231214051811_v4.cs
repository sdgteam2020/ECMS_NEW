using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte>(
                name: "RankId",
                table: "UserProfile",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfile_RankId",
                table: "UserProfile",
                column: "RankId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfile_MRank_RankId",
                table: "UserProfile",
                column: "RankId",
                principalTable: "MRank",
                principalColumn: "RankId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserProfile_MRank_RankId",
                table: "UserProfile");

            migrationBuilder.DropIndex(
                name: "IX_UserProfile_RankId",
                table: "UserProfile");

            migrationBuilder.AlterColumn<int>(
                name: "RankId",
                table: "UserProfile",
                type: "int",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint");
        }
    }
}
