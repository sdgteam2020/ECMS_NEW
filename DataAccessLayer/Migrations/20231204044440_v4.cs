using System;
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
            migrationBuilder.DropColumn(
                name: "UnitId",
                table: "UserProfile");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "MRegistration",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "MRegistration",
                keyColumn: "RegistrationId",
                keyValue: 1,
                columns: new[] { "Type", "UpdatedOn" },
                values: new object[] { 1, new DateTime(2023, 12, 4, 10, 14, 40, 9, DateTimeKind.Unspecified).AddTicks(2157) });

            migrationBuilder.UpdateData(
                table: "MRegistration",
                keyColumn: "RegistrationId",
                keyValue: 2,
                columns: new[] { "Type", "UpdatedOn" },
                values: new object[] { 1, new DateTime(2023, 12, 4, 10, 14, 40, 9, DateTimeKind.Unspecified).AddTicks(2163) });

            migrationBuilder.UpdateData(
                table: "MRegistration",
                keyColumn: "RegistrationId",
                keyValue: 3,
                columns: new[] { "Type", "UpdatedOn" },
                values: new object[] { 1, new DateTime(2023, 12, 4, 10, 14, 40, 9, DateTimeKind.Unspecified).AddTicks(2167) });

            migrationBuilder.UpdateData(
                table: "MRegistration",
                keyColumn: "RegistrationId",
                keyValue: 4,
                columns: new[] { "Type", "UpdatedOn" },
                values: new object[] { 2, new DateTime(2023, 12, 4, 10, 14, 40, 9, DateTimeKind.Unspecified).AddTicks(2171) });

            migrationBuilder.UpdateData(
                table: "MRegistration",
                keyColumn: "RegistrationId",
                keyValue: 5,
                columns: new[] { "Type", "UpdatedOn" },
                values: new object[] { 2, new DateTime(2023, 12, 4, 10, 14, 40, 9, DateTimeKind.Unspecified).AddTicks(2175) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "MRegistration");

            migrationBuilder.AddColumn<int>(
                name: "UnitId",
                table: "UserProfile",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "MRegistration",
                keyColumn: "RegistrationId",
                keyValue: 1,
                column: "UpdatedOn",
                value: new DateTime(2023, 12, 1, 15, 34, 57, 368, DateTimeKind.Unspecified).AddTicks(6944));

            migrationBuilder.UpdateData(
                table: "MRegistration",
                keyColumn: "RegistrationId",
                keyValue: 2,
                column: "UpdatedOn",
                value: new DateTime(2023, 12, 1, 15, 34, 57, 368, DateTimeKind.Unspecified).AddTicks(6950));

            migrationBuilder.UpdateData(
                table: "MRegistration",
                keyColumn: "RegistrationId",
                keyValue: 3,
                column: "UpdatedOn",
                value: new DateTime(2023, 12, 1, 15, 34, 57, 368, DateTimeKind.Unspecified).AddTicks(6954));

            migrationBuilder.UpdateData(
                table: "MRegistration",
                keyColumn: "RegistrationId",
                keyValue: 4,
                column: "UpdatedOn",
                value: new DateTime(2023, 12, 1, 15, 34, 57, 368, DateTimeKind.Unspecified).AddTicks(6958));

            migrationBuilder.UpdateData(
                table: "MRegistration",
                keyColumn: "RegistrationId",
                keyValue: 5,
                column: "UpdatedOn",
                value: new DateTime(2023, 12, 1, 15, 34, 57, 368, DateTimeKind.Unspecified).AddTicks(6962));
        }
    }
}
