using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BasicDetails_MArmedType_ArmedId1",
                table: "BasicDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_BasicDetails_MRank_RankId1",
                table: "BasicDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_BasicDetails_MRegimental_RegimentalRegId",
                table: "BasicDetails");

            migrationBuilder.DropIndex(
                name: "IX_BasicDetails_ArmedId1",
                table: "BasicDetails");

            migrationBuilder.DropIndex(
                name: "IX_BasicDetails_RankId1",
                table: "BasicDetails");

            migrationBuilder.DropIndex(
                name: "IX_BasicDetails_RegimentalRegId",
                table: "BasicDetails");

            migrationBuilder.DeleteData(
                table: "MRegistration",
                keyColumn: "RegistrationId",
                keyValue: (short)1);

            migrationBuilder.DeleteData(
                table: "MRegistration",
                keyColumn: "RegistrationId",
                keyValue: (short)2);

            migrationBuilder.DeleteData(
                table: "MRegistration",
                keyColumn: "RegistrationId",
                keyValue: (short)3);

            migrationBuilder.DeleteData(
                table: "MRegistration",
                keyColumn: "RegistrationId",
                keyValue: (short)4);

            migrationBuilder.DeleteData(
                table: "MRegistration",
                keyColumn: "RegistrationId",
                keyValue: (short)5);

            migrationBuilder.DropColumn(
                name: "ArmedId1",
                table: "BasicDetails");

            migrationBuilder.DropColumn(
                name: "RankId1",
                table: "BasicDetails");

            migrationBuilder.DropColumn(
                name: "RegimentalRegId",
                table: "BasicDetails");

            migrationBuilder.DropColumn(
                name: "RegistrationType",
                table: "BasicDetails");

            migrationBuilder.AlterColumn<byte>(
                name: "RegistrationId",
                table: "MRegistration",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<byte>(
                name: "RegId",
                table: "MRegimental",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<byte>(
                name: "RegistrationId",
                table: "BasicDetails",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.AlterColumn<byte>(
                name: "RegimentalId",
                table: "BasicDetails",
                type: "tinyint",
                nullable: true,
                oldClrType: typeof(short),
                oldType: "smallint",
                oldNullable: true);

            migrationBuilder.AlterColumn<byte>(
                name: "RankId",
                table: "BasicDetails",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.AlterColumn<byte>(
                name: "ArmedId",
                table: "BasicDetails",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.InsertData(
                table: "MRegistration",
                columns: new[] { "RegistrationId", "IsActive", "Name", "Order", "Type", "UpdatedOn", "Updatedby" },
                values: new object[,]
                {
                    { (byte)1, false, "Apply for Self (Officer)", 1, 1, new DateTime(2023, 12, 5, 11, 47, 26, 465, DateTimeKind.Unspecified).AddTicks(8652), 1 },
                    { (byte)2, false, "Apply for Unit Officer", 2, 1, new DateTime(2023, 12, 5, 11, 47, 26, 465, DateTimeKind.Unspecified).AddTicks(8659), 1 },
                    { (byte)3, false, "Apply for Other Unit Officer", 3, 1, new DateTime(2023, 12, 5, 11, 47, 26, 465, DateTimeKind.Unspecified).AddTicks(8663), 1 },
                    { (byte)4, false, "Apply for Unit JCOs/OR", 4, 2, new DateTime(2023, 12, 5, 11, 47, 26, 465, DateTimeKind.Unspecified).AddTicks(8668), 1 },
                    { (byte)5, false, "Apply for Other Unit JCOs/OR", 5, 2, new DateTime(2023, 12, 5, 11, 47, 26, 465, DateTimeKind.Unspecified).AddTicks(8672), 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_BasicDetails_ArmedId",
                table: "BasicDetails",
                column: "ArmedId");

            migrationBuilder.CreateIndex(
                name: "IX_BasicDetails_RankId",
                table: "BasicDetails",
                column: "RankId");

            migrationBuilder.CreateIndex(
                name: "IX_BasicDetails_RegimentalId",
                table: "BasicDetails",
                column: "RegimentalId");

            migrationBuilder.AddForeignKey(
                name: "FK_BasicDetails_MArmedType_ArmedId",
                table: "BasicDetails",
                column: "ArmedId",
                principalTable: "MArmedType",
                principalColumn: "ArmedId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BasicDetails_MRank_RankId",
                table: "BasicDetails",
                column: "RankId",
                principalTable: "MRank",
                principalColumn: "RankId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BasicDetails_MRegimental_RegimentalId",
                table: "BasicDetails",
                column: "RegimentalId",
                principalTable: "MRegimental",
                principalColumn: "RegId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BasicDetails_MArmedType_ArmedId",
                table: "BasicDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_BasicDetails_MRank_RankId",
                table: "BasicDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_BasicDetails_MRegimental_RegimentalId",
                table: "BasicDetails");

            migrationBuilder.DropIndex(
                name: "IX_BasicDetails_ArmedId",
                table: "BasicDetails");

            migrationBuilder.DropIndex(
                name: "IX_BasicDetails_RankId",
                table: "BasicDetails");

            migrationBuilder.DropIndex(
                name: "IX_BasicDetails_RegimentalId",
                table: "BasicDetails");

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

            migrationBuilder.AlterColumn<short>(
                name: "RegistrationId",
                table: "MRegistration",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<byte>(
                name: "RegId",
                table: "MRegimental",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<short>(
                name: "RegistrationId",
                table: "BasicDetails",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AlterColumn<short>(
                name: "RegimentalId",
                table: "BasicDetails",
                type: "smallint",
                nullable: true,
                oldClrType: typeof(byte),
                oldType: "tinyint",
                oldNullable: true);

            migrationBuilder.AlterColumn<short>(
                name: "RankId",
                table: "BasicDetails",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AlterColumn<short>(
                name: "ArmedId",
                table: "BasicDetails",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AddColumn<byte>(
                name: "ArmedId1",
                table: "BasicDetails",
                type: "tinyint",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "RankId1",
                table: "BasicDetails",
                type: "tinyint",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "RegimentalRegId",
                table: "BasicDetails",
                type: "tinyint",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RegistrationType",
                table: "BasicDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "MRegistration",
                columns: new[] { "RegistrationId", "IsActive", "Name", "Order", "Type", "UpdatedOn", "Updatedby" },
                values: new object[,]
                {
                    { (short)1, false, "Apply for Self (Officer)", 1, 1, new DateTime(2023, 12, 5, 10, 18, 19, 435, DateTimeKind.Unspecified).AddTicks(5387), 1 },
                    { (short)2, false, "Apply for Unit Officer", 2, 1, new DateTime(2023, 12, 5, 10, 18, 19, 435, DateTimeKind.Unspecified).AddTicks(5392), 1 },
                    { (short)3, false, "Apply for Other Unit Officer", 3, 1, new DateTime(2023, 12, 5, 10, 18, 19, 435, DateTimeKind.Unspecified).AddTicks(5396), 1 },
                    { (short)4, false, "Apply for Unit JCOs/OR", 4, 2, new DateTime(2023, 12, 5, 10, 18, 19, 435, DateTimeKind.Unspecified).AddTicks(5399), 1 },
                    { (short)5, false, "Apply for Other Unit JCOs/OR", 5, 2, new DateTime(2023, 12, 5, 10, 18, 19, 435, DateTimeKind.Unspecified).AddTicks(5403), 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_BasicDetails_ArmedId1",
                table: "BasicDetails",
                column: "ArmedId1");

            migrationBuilder.CreateIndex(
                name: "IX_BasicDetails_RankId1",
                table: "BasicDetails",
                column: "RankId1");

            migrationBuilder.CreateIndex(
                name: "IX_BasicDetails_RegimentalRegId",
                table: "BasicDetails",
                column: "RegimentalRegId");

            migrationBuilder.AddForeignKey(
                name: "FK_BasicDetails_MArmedType_ArmedId1",
                table: "BasicDetails",
                column: "ArmedId1",
                principalTable: "MArmedType",
                principalColumn: "ArmedId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BasicDetails_MRank_RankId1",
                table: "BasicDetails",
                column: "RankId1",
                principalTable: "MRank",
                principalColumn: "RankId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BasicDetails_MRegimental_RegimentalRegId",
                table: "BasicDetails",
                column: "RegimentalRegId",
                principalTable: "MRegimental",
                principalColumn: "RegId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
