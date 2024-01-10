using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v19 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DOB",
                table: "MApiData");

            migrationBuilder.DropColumn(
                name: "DateOfCommissioning",
                table: "MApiData");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "MApiData");

            migrationBuilder.DropColumn(
                name: "PermanentAddress",
                table: "MApiData");

            migrationBuilder.DropColumn(
                name: "ServiceNo",
                table: "MApiData");

            migrationBuilder.AddColumn<string>(
                name: "Pers_Army_No",
                table: "MApiData",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Pers_Blood_Gp",
                table: "MApiData",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Pers_District",
                table: "MApiData",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Pers_Father_Name",
                table: "MApiData",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Pers_Gender",
                table: "MApiData",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Pers_Height",
                table: "MApiData",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Pers_House_no",
                table: "MApiData",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Pers_Iden_mark_1",
                table: "MApiData",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Pers_Iden_mark_2",
                table: "MApiData",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Pers_Moh_st",
                table: "MApiData",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Pers_Pin_code",
                table: "MApiData",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Pers_Police_stn",
                table: "MApiData",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Pers_Post_office",
                table: "MApiData",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Pers_Rank",
                table: "MApiData",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Pers_Regt",
                table: "MApiData",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Pers_State",
                table: "MApiData",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Pers_Tehsil",
                table: "MApiData",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Pers_UID",
                table: "MApiData",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Pers_Village",
                table: "MApiData",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Pers_birth_dt",
                table: "MApiData",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Pers_enrol_dt",
                table: "MApiData",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Pers_name",
                table: "MApiData",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Pers_Army_No",
                table: "MApiData");

            migrationBuilder.DropColumn(
                name: "Pers_Blood_Gp",
                table: "MApiData");

            migrationBuilder.DropColumn(
                name: "Pers_District",
                table: "MApiData");

            migrationBuilder.DropColumn(
                name: "Pers_Father_Name",
                table: "MApiData");

            migrationBuilder.DropColumn(
                name: "Pers_Gender",
                table: "MApiData");

            migrationBuilder.DropColumn(
                name: "Pers_Height",
                table: "MApiData");

            migrationBuilder.DropColumn(
                name: "Pers_House_no",
                table: "MApiData");

            migrationBuilder.DropColumn(
                name: "Pers_Iden_mark_1",
                table: "MApiData");

            migrationBuilder.DropColumn(
                name: "Pers_Iden_mark_2",
                table: "MApiData");

            migrationBuilder.DropColumn(
                name: "Pers_Moh_st",
                table: "MApiData");

            migrationBuilder.DropColumn(
                name: "Pers_Pin_code",
                table: "MApiData");

            migrationBuilder.DropColumn(
                name: "Pers_Police_stn",
                table: "MApiData");

            migrationBuilder.DropColumn(
                name: "Pers_Post_office",
                table: "MApiData");

            migrationBuilder.DropColumn(
                name: "Pers_Rank",
                table: "MApiData");

            migrationBuilder.DropColumn(
                name: "Pers_Regt",
                table: "MApiData");

            migrationBuilder.DropColumn(
                name: "Pers_State",
                table: "MApiData");

            migrationBuilder.DropColumn(
                name: "Pers_Tehsil",
                table: "MApiData");

            migrationBuilder.DropColumn(
                name: "Pers_UID",
                table: "MApiData");

            migrationBuilder.DropColumn(
                name: "Pers_Village",
                table: "MApiData");

            migrationBuilder.DropColumn(
                name: "Pers_birth_dt",
                table: "MApiData");

            migrationBuilder.DropColumn(
                name: "Pers_enrol_dt",
                table: "MApiData");

            migrationBuilder.DropColumn(
                name: "Pers_name",
                table: "MApiData");

            migrationBuilder.AddColumn<DateTime>(
                name: "DOB",
                table: "MApiData",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfCommissioning",
                table: "MApiData",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "MApiData",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PermanentAddress",
                table: "MApiData",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ServiceNo",
                table: "MApiData",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
