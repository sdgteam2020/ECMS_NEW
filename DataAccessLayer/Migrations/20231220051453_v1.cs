using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "RegistrationId",
                table: "TrnICardRequest",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.CreateIndex(
                name: "IX_TrnICardRequest_RegistrationId",
                table: "TrnICardRequest",
                column: "RegistrationId");

            migrationBuilder.AddForeignKey(
                name: "FK_TrnICardRequest_MRegistration_RegistrationId",
                table: "TrnICardRequest",
                column: "RegistrationId",
                principalTable: "MRegistration",
                principalColumn: "RegistrationId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrnICardRequest_MRegistration_RegistrationId",
                table: "TrnICardRequest");

            migrationBuilder.DropIndex(
                name: "IX_TrnICardRequest_RegistrationId",
                table: "TrnICardRequest");

            migrationBuilder.DropColumn(
                name: "RegistrationId",
                table: "TrnICardRequest");
        }
    }
}
