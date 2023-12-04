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
                name: "FK_TrnFwds_MTrnFwdType_MTrnFwdTypeTypeId",
                table: "TrnFwds");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MTrnFwdType",
                table: "MTrnFwdType");

            migrationBuilder.RenameTable(
                name: "MTrnFwdType",
                newName: "MFwdType");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MFwdType",
                table: "MFwdType",
                column: "TypeId");

            migrationBuilder.UpdateData(
                table: "MRegistration",
                keyColumn: "RegistrationId",
                keyValue: (short)1,
                column: "UpdatedOn",
                value: new DateTime(2023, 12, 4, 12, 1, 4, 966, DateTimeKind.Unspecified).AddTicks(651));

            migrationBuilder.UpdateData(
                table: "MRegistration",
                keyColumn: "RegistrationId",
                keyValue: (short)2,
                column: "UpdatedOn",
                value: new DateTime(2023, 12, 4, 12, 1, 4, 966, DateTimeKind.Unspecified).AddTicks(657));

            migrationBuilder.UpdateData(
                table: "MRegistration",
                keyColumn: "RegistrationId",
                keyValue: (short)3,
                column: "UpdatedOn",
                value: new DateTime(2023, 12, 4, 12, 1, 4, 966, DateTimeKind.Unspecified).AddTicks(660));

            migrationBuilder.UpdateData(
                table: "MRegistration",
                keyColumn: "RegistrationId",
                keyValue: (short)4,
                column: "UpdatedOn",
                value: new DateTime(2023, 12, 4, 12, 1, 4, 966, DateTimeKind.Unspecified).AddTicks(663));

            migrationBuilder.UpdateData(
                table: "MRegistration",
                keyColumn: "RegistrationId",
                keyValue: (short)5,
                column: "UpdatedOn",
                value: new DateTime(2023, 12, 4, 12, 1, 4, 966, DateTimeKind.Unspecified).AddTicks(666));

            migrationBuilder.AddForeignKey(
                name: "FK_TrnFwds_MFwdType_MTrnFwdTypeTypeId",
                table: "TrnFwds",
                column: "MTrnFwdTypeTypeId",
                principalTable: "MFwdType",
                principalColumn: "TypeId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrnFwds_MFwdType_MTrnFwdTypeTypeId",
                table: "TrnFwds");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MFwdType",
                table: "MFwdType");

            migrationBuilder.RenameTable(
                name: "MFwdType",
                newName: "MTrnFwdType");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MTrnFwdType",
                table: "MTrnFwdType",
                column: "TypeId");

            migrationBuilder.UpdateData(
                table: "MRegistration",
                keyColumn: "RegistrationId",
                keyValue: (short)1,
                column: "UpdatedOn",
                value: new DateTime(2023, 12, 4, 11, 57, 19, 13, DateTimeKind.Unspecified).AddTicks(8066));

            migrationBuilder.UpdateData(
                table: "MRegistration",
                keyColumn: "RegistrationId",
                keyValue: (short)2,
                column: "UpdatedOn",
                value: new DateTime(2023, 12, 4, 11, 57, 19, 13, DateTimeKind.Unspecified).AddTicks(8072));

            migrationBuilder.UpdateData(
                table: "MRegistration",
                keyColumn: "RegistrationId",
                keyValue: (short)3,
                column: "UpdatedOn",
                value: new DateTime(2023, 12, 4, 11, 57, 19, 13, DateTimeKind.Unspecified).AddTicks(8076));

            migrationBuilder.UpdateData(
                table: "MRegistration",
                keyColumn: "RegistrationId",
                keyValue: (short)4,
                column: "UpdatedOn",
                value: new DateTime(2023, 12, 4, 11, 57, 19, 13, DateTimeKind.Unspecified).AddTicks(8080));

            migrationBuilder.UpdateData(
                table: "MRegistration",
                keyColumn: "RegistrationId",
                keyValue: (short)5,
                column: "UpdatedOn",
                value: new DateTime(2023, 12, 4, 11, 57, 19, 13, DateTimeKind.Unspecified).AddTicks(8084));

            migrationBuilder.AddForeignKey(
                name: "FK_TrnFwds_MTrnFwdType_MTrnFwdTypeTypeId",
                table: "TrnFwds",
                column: "MTrnFwdTypeTypeId",
                principalTable: "MTrnFwdType",
                principalColumn: "TypeId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
