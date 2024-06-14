using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v107 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsTokenWaiver",
                table: "UserProfile",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<byte>(
                name: "ArmedId",
                table: "BasicDetailTemps",
                type: "tinyint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BasicDetailTemps_ArmedId",
                table: "BasicDetailTemps",
                column: "ArmedId");

            migrationBuilder.AddForeignKey(
                name: "FK_BasicDetailTemps_MArmedType_ArmedId",
                table: "BasicDetailTemps",
                column: "ArmedId",
                principalTable: "MArmedType",
                principalColumn: "ArmedId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BasicDetailTemps_MArmedType_ArmedId",
                table: "BasicDetailTemps");

            migrationBuilder.DropIndex(
                name: "IX_BasicDetailTemps_ArmedId",
                table: "BasicDetailTemps");

            migrationBuilder.DropColumn(
                name: "IsTokenWaiver",
                table: "UserProfile");

            migrationBuilder.DropColumn(
                name: "ArmedId",
                table: "BasicDetailTemps");
        }
    }
}
