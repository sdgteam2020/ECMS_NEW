using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "MRegistration",
                columns: new[] { "RegistrationId", "IsActive", "Name", "Order", "UpdatedOn", "Updatedby" },
                values: new object[,]
                {
                    { 1, false, "Apply for Self (Officer)", 1, new DateTime(2023, 12, 1, 15, 34, 57, 368, DateTimeKind.Unspecified).AddTicks(6944), 1 },
                    { 2, false, "Apply for Unit Officer", 2, new DateTime(2023, 12, 1, 15, 34, 57, 368, DateTimeKind.Unspecified).AddTicks(6950), 1 },
                    { 3, false, "Apply for Other Unit Officer", 3, new DateTime(2023, 12, 1, 15, 34, 57, 368, DateTimeKind.Unspecified).AddTicks(6954), 1 },
                    { 4, false, "Apply for Unit JCOs/OR", 4, new DateTime(2023, 12, 1, 15, 34, 57, 368, DateTimeKind.Unspecified).AddTicks(6958), 1 },
                    { 5, false, "Apply for Other Unit JCOs/OR", 5, new DateTime(2023, 12, 1, 15, 34, 57, 368, DateTimeKind.Unspecified).AddTicks(6962), 1 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "MRegistration",
                keyColumn: "RegistrationId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "MRegistration",
                keyColumn: "RegistrationId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "MRegistration",
                keyColumn: "RegistrationId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "MRegistration",
                keyColumn: "RegistrationId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "MRegistration",
                keyColumn: "RegistrationId",
                keyValue: 5);
        }
    }
}
