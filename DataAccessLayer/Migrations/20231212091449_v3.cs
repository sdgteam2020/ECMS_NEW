using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserProfile_MAppointment_MAppointmentApptId",
                table: "UserProfile");

            migrationBuilder.DropIndex(
                name: "IX_UserProfile_MAppointmentApptId",
                table: "UserProfile");

            migrationBuilder.DropColumn(
                name: "MAppointmentApptId",
                table: "UserProfile");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "UserProfile",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ArmyNo",
                table: "UserProfile",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<byte>(
                name: "ApptId",
                table: "UserProfile",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<bool>(
                name: "IsComplete",
                table: "TrnFwds",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_UserProfile_ApptId",
                table: "UserProfile",
                column: "ApptId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfile_MAppointment_ApptId",
                table: "UserProfile",
                column: "ApptId",
                principalTable: "MAppointment",
                principalColumn: "ApptId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserProfile_MAppointment_ApptId",
                table: "UserProfile");

            migrationBuilder.DropIndex(
                name: "IX_UserProfile_ApptId",
                table: "UserProfile");

            migrationBuilder.DropColumn(
                name: "IsComplete",
                table: "TrnFwds");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "UserProfile",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "ArmyNo",
                table: "UserProfile",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<int>(
                name: "ApptId",
                table: "UserProfile",
                type: "int",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AddColumn<byte>(
                name: "MAppointmentApptId",
                table: "UserProfile",
                type: "tinyint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserProfile_MAppointmentApptId",
                table: "UserProfile",
                column: "MAppointmentApptId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfile_MAppointment_MAppointmentApptId",
                table: "UserProfile",
                column: "MAppointmentApptId",
                principalTable: "MAppointment",
                principalColumn: "ApptId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
