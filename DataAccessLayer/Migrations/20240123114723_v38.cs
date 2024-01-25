using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v38 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "RegistrationId",
                table: "BasicDetailTemps",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "TypeId",
                table: "BasicDetailTemps",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.CreateIndex(
                name: "IX_BasicDetailTemps_RegistrationId",
                table: "BasicDetailTemps",
                column: "RegistrationId");

            migrationBuilder.CreateIndex(
                name: "IX_BasicDetailTemps_TypeId",
                table: "BasicDetailTemps",
                column: "TypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_BasicDetailTemps_MICardType_TypeId",
                table: "BasicDetailTemps",
                column: "TypeId",
                principalTable: "MICardType",
                principalColumn: "TypeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BasicDetailTemps_MRegistration_RegistrationId",
                table: "BasicDetailTemps",
                column: "RegistrationId",
                principalTable: "MRegistration",
                principalColumn: "RegistrationId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BasicDetailTemps_MICardType_TypeId",
                table: "BasicDetailTemps");

            migrationBuilder.DropForeignKey(
                name: "FK_BasicDetailTemps_MRegistration_RegistrationId",
                table: "BasicDetailTemps");

            migrationBuilder.DropIndex(
                name: "IX_BasicDetailTemps_RegistrationId",
                table: "BasicDetailTemps");

            migrationBuilder.DropIndex(
                name: "IX_BasicDetailTemps_TypeId",
                table: "BasicDetailTemps");

            migrationBuilder.DropColumn(
                name: "RegistrationId",
                table: "BasicDetailTemps");

            migrationBuilder.DropColumn(
                name: "TypeId",
                table: "BasicDetailTemps");
        }
    }
}
