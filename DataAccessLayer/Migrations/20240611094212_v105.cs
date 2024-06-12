using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v105 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "ArmedId",
                table: "UserProfile",
                type: "tinyint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReasonTokenWaiver",
                table: "UserProfile",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserProfile_ArmedId",
                table: "UserProfile",
                column: "ArmedId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfile_MArmedType_ArmedId",
                table: "UserProfile",
                column: "ArmedId",
                principalTable: "MArmedType",
                principalColumn: "ArmedId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserProfile_MArmedType_ArmedId",
                table: "UserProfile");

            migrationBuilder.DropIndex(
                name: "IX_UserProfile_ArmedId",
                table: "UserProfile");

            migrationBuilder.DropColumn(
                name: "ArmedId",
                table: "UserProfile");

            migrationBuilder.DropColumn(
                name: "ReasonTokenWaiver",
                table: "UserProfile");
        }
    }
}
