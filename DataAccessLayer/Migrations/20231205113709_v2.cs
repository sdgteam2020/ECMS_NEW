using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "MRegistration",
                keyColumn: "RegistrationId",
                keyValue: (byte)1);

            migrationBuilder.DeleteData(
                table: "MRegistration",
                keyColumn: "RegistrationId",
                keyValue: (byte)2);

            migrationBuilder.DeleteData(
                table: "MRegistration",
                keyColumn: "RegistrationId",
                keyValue: (byte)3);

            migrationBuilder.DeleteData(
                table: "MRegistration",
                keyColumn: "RegistrationId",
                keyValue: (byte)4);

            migrationBuilder.DeleteData(
                table: "MRegistration",
                keyColumn: "RegistrationId",
                keyValue: (byte)5);

            migrationBuilder.AddColumn<int>(
                name: "TrnDomainMappingId",
                table: "TrnICardRequest",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TrnICardRequest_TrnDomainMappingId",
                table: "TrnICardRequest",
                column: "TrnDomainMappingId");

            migrationBuilder.AddForeignKey(
                name: "FK_TrnICardRequest_TrnDomainMapping_TrnDomainMappingId",
                table: "TrnICardRequest",
                column: "TrnDomainMappingId",
                principalTable: "TrnDomainMapping",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrnICardRequest_TrnDomainMapping_TrnDomainMappingId",
                table: "TrnICardRequest");

            migrationBuilder.DropIndex(
                name: "IX_TrnICardRequest_TrnDomainMappingId",
                table: "TrnICardRequest");

            migrationBuilder.DropColumn(
                name: "TrnDomainMappingId",
                table: "TrnICardRequest");

            migrationBuilder.InsertData(
                table: "MRegistration",
                columns: new[] { "RegistrationId", "IsActive", "Name", "Order", "Type", "UpdatedOn", "Updatedby" },
                values: new object[,]
                {
                    { (byte)1, false, "Apply for Self (Officer)", 1, 1, new DateTime(2023, 12, 5, 12, 6, 58, 899, DateTimeKind.Unspecified).AddTicks(1308), 1 },
                    { (byte)2, false, "Apply for Unit Officer", 2, 1, new DateTime(2023, 12, 5, 12, 6, 58, 899, DateTimeKind.Unspecified).AddTicks(1315), 1 },
                    { (byte)3, false, "Apply for Other Unit Officer", 3, 1, new DateTime(2023, 12, 5, 12, 6, 58, 899, DateTimeKind.Unspecified).AddTicks(1319), 1 },
                    { (byte)4, false, "Apply for Unit JCOs/OR", 4, 2, new DateTime(2023, 12, 5, 12, 6, 58, 899, DateTimeKind.Unspecified).AddTicks(1357), 1 },
                    { (byte)5, false, "Apply for Other Unit JCOs/OR", 5, 2, new DateTime(2023, 12, 5, 12, 6, 58, 899, DateTimeKind.Unspecified).AddTicks(1362), 1 }
                });
        }
    }
}
