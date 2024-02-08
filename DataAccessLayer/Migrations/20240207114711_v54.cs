using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v54 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_BasicDetailTemps_RankId",
                table: "BasicDetailTemps",
                column: "RankId");

            migrationBuilder.AddForeignKey(
                name: "FK_BasicDetailTemps_MRank_RankId",
                table: "BasicDetailTemps",
                column: "RankId",
                principalTable: "MRank",
                principalColumn: "RankId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BasicDetailTemps_MRank_RankId",
                table: "BasicDetailTemps");

            migrationBuilder.DropIndex(
                name: "IX_BasicDetailTemps_RankId",
                table: "BasicDetailTemps");
        }
    }
}
