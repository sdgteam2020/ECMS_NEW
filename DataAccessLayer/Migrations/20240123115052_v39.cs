using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v39 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "ApplyForId",
                table: "BasicDetailTemps",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.CreateIndex(
                name: "IX_BasicDetailTemps_ApplyForId",
                table: "BasicDetailTemps",
                column: "ApplyForId");

            migrationBuilder.AddForeignKey(
                name: "FK_BasicDetailTemps_MApplyFor_ApplyForId",
                table: "BasicDetailTemps",
                column: "ApplyForId",
                principalTable: "MApplyFor",
                principalColumn: "ApplyForId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BasicDetailTemps_MApplyFor_ApplyForId",
                table: "BasicDetailTemps");

            migrationBuilder.DropIndex(
                name: "IX_BasicDetailTemps_ApplyForId",
                table: "BasicDetailTemps");

            migrationBuilder.DropColumn(
                name: "ApplyForId",
                table: "BasicDetailTemps");
        }
    }
}
