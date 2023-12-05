using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrnFwds_UserProfile_UserId",
                table: "TrnFwds");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "TrnFwds",
                newName: "ToUserId");

            migrationBuilder.RenameIndex(
                name: "IX_TrnFwds_UserId",
                table: "TrnFwds",
                newName: "IX_TrnFwds_ToUserId");

            migrationBuilder.AddColumn<int>(
                name: "FromUserId",
                table: "TrnFwds",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "MRegistration",
                keyColumn: "RegistrationId",
                keyValue: (short)1,
                column: "UpdatedOn",
                value: new DateTime(2023, 12, 4, 16, 56, 19, 151, DateTimeKind.Unspecified).AddTicks(7489));

            migrationBuilder.UpdateData(
                table: "MRegistration",
                keyColumn: "RegistrationId",
                keyValue: (short)2,
                column: "UpdatedOn",
                value: new DateTime(2023, 12, 4, 16, 56, 19, 151, DateTimeKind.Unspecified).AddTicks(7495));

            migrationBuilder.UpdateData(
                table: "MRegistration",
                keyColumn: "RegistrationId",
                keyValue: (short)3,
                column: "UpdatedOn",
                value: new DateTime(2023, 12, 4, 16, 56, 19, 151, DateTimeKind.Unspecified).AddTicks(7499));

            migrationBuilder.UpdateData(
                table: "MRegistration",
                keyColumn: "RegistrationId",
                keyValue: (short)4,
                column: "UpdatedOn",
                value: new DateTime(2023, 12, 4, 16, 56, 19, 151, DateTimeKind.Unspecified).AddTicks(7502));

            migrationBuilder.UpdateData(
                table: "MRegistration",
                keyColumn: "RegistrationId",
                keyValue: (short)5,
                column: "UpdatedOn",
                value: new DateTime(2023, 12, 4, 16, 56, 19, 151, DateTimeKind.Unspecified).AddTicks(7506));

            migrationBuilder.CreateIndex(
                name: "IX_TrnFwds_FromUserId",
                table: "TrnFwds",
                column: "FromUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TrnFwds_UserProfile_FromUserId",
                table: "TrnFwds",
                column: "FromUserId",
                principalTable: "UserProfile",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TrnFwds_UserProfile_ToUserId",
                table: "TrnFwds",
                column: "ToUserId",
                principalTable: "UserProfile",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrnFwds_UserProfile_FromUserId",
                table: "TrnFwds");

            migrationBuilder.DropForeignKey(
                name: "FK_TrnFwds_UserProfile_ToUserId",
                table: "TrnFwds");

            migrationBuilder.DropIndex(
                name: "IX_TrnFwds_FromUserId",
                table: "TrnFwds");

            migrationBuilder.DropColumn(
                name: "FromUserId",
                table: "TrnFwds");

            migrationBuilder.RenameColumn(
                name: "ToUserId",
                table: "TrnFwds",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_TrnFwds_ToUserId",
                table: "TrnFwds",
                newName: "IX_TrnFwds_UserId");

            migrationBuilder.UpdateData(
                table: "MRegistration",
                keyColumn: "RegistrationId",
                keyValue: (short)1,
                column: "UpdatedOn",
                value: new DateTime(2023, 12, 4, 16, 52, 49, 131, DateTimeKind.Unspecified).AddTicks(309));

            migrationBuilder.UpdateData(
                table: "MRegistration",
                keyColumn: "RegistrationId",
                keyValue: (short)2,
                column: "UpdatedOn",
                value: new DateTime(2023, 12, 4, 16, 52, 49, 131, DateTimeKind.Unspecified).AddTicks(314));

            migrationBuilder.UpdateData(
                table: "MRegistration",
                keyColumn: "RegistrationId",
                keyValue: (short)3,
                column: "UpdatedOn",
                value: new DateTime(2023, 12, 4, 16, 52, 49, 131, DateTimeKind.Unspecified).AddTicks(317));

            migrationBuilder.UpdateData(
                table: "MRegistration",
                keyColumn: "RegistrationId",
                keyValue: (short)4,
                column: "UpdatedOn",
                value: new DateTime(2023, 12, 4, 16, 52, 49, 131, DateTimeKind.Unspecified).AddTicks(321));

            migrationBuilder.UpdateData(
                table: "MRegistration",
                keyColumn: "RegistrationId",
                keyValue: (short)5,
                column: "UpdatedOn",
                value: new DateTime(2023, 12, 4, 16, 52, 49, 131, DateTimeKind.Unspecified).AddTicks(324));

            migrationBuilder.AddForeignKey(
                name: "FK_TrnFwds_UserProfile_UserId",
                table: "TrnFwds",
                column: "UserId",
                principalTable: "UserProfile",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
