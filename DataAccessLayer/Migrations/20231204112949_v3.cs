using System;
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
            migrationBuilder.RenameColumn(
                name: "SusNo",
                table: "TrnFwds",
                newName: "UnitId");

            migrationBuilder.UpdateData(
                table: "MRegistration",
                keyColumn: "RegistrationId",
                keyValue: (short)1,
                column: "UpdatedOn",
                value: new DateTime(2023, 12, 4, 16, 59, 49, 4, DateTimeKind.Unspecified).AddTicks(1375));

            migrationBuilder.UpdateData(
                table: "MRegistration",
                keyColumn: "RegistrationId",
                keyValue: (short)2,
                column: "UpdatedOn",
                value: new DateTime(2023, 12, 4, 16, 59, 49, 4, DateTimeKind.Unspecified).AddTicks(1380));

            migrationBuilder.UpdateData(
                table: "MRegistration",
                keyColumn: "RegistrationId",
                keyValue: (short)3,
                column: "UpdatedOn",
                value: new DateTime(2023, 12, 4, 16, 59, 49, 4, DateTimeKind.Unspecified).AddTicks(1384));

            migrationBuilder.UpdateData(
                table: "MRegistration",
                keyColumn: "RegistrationId",
                keyValue: (short)4,
                column: "UpdatedOn",
                value: new DateTime(2023, 12, 4, 16, 59, 49, 4, DateTimeKind.Unspecified).AddTicks(1412));

            migrationBuilder.UpdateData(
                table: "MRegistration",
                keyColumn: "RegistrationId",
                keyValue: (short)5,
                column: "UpdatedOn",
                value: new DateTime(2023, 12, 4, 16, 59, 49, 4, DateTimeKind.Unspecified).AddTicks(1416));

            migrationBuilder.CreateIndex(
                name: "IX_TrnFwds_UnitId",
                table: "TrnFwds",
                column: "UnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_TrnFwds_MapUnit_UnitId",
                table: "TrnFwds",
                column: "UnitId",
                principalTable: "MapUnit",
                principalColumn: "UnitMapId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrnFwds_MapUnit_UnitId",
                table: "TrnFwds");

            migrationBuilder.DropIndex(
                name: "IX_TrnFwds_UnitId",
                table: "TrnFwds");

            migrationBuilder.RenameColumn(
                name: "UnitId",
                table: "TrnFwds",
                newName: "SusNo");

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
        }
    }
}
