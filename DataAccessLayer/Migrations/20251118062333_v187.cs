using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v187 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Pers_Blood_Gp",
                table: "MApiDataOffrs");

            migrationBuilder.DropColumn(
                name: "Pers_Father_Name",
                table: "MApiDataOffrs");

            migrationBuilder.DropColumn(
                name: "Pers_Gender",
                table: "MApiDataOffrs");

            migrationBuilder.DropColumn(
                name: "Pers_Height",
                table: "MApiDataOffrs");

            migrationBuilder.DropColumn(
                name: "Pers_Iden_mark_1",
                table: "MApiDataOffrs");

            migrationBuilder.DropColumn(
                name: "Pers_Iden_mark_2",
                table: "MApiDataOffrs");

            migrationBuilder.DropColumn(
                name: "Pers_Rank",
                table: "MApiDataOffrs");

            migrationBuilder.DropColumn(
                name: "Pers_Regt",
                table: "MApiDataOffrs");

            migrationBuilder.DropColumn(
                name: "Pers_UID",
                table: "MApiDataOffrs");

            migrationBuilder.DropColumn(
                name: "Pers_Blood_Gp",
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
                name: "Pers_Iden_mark_1",
                table: "MApiData");

            migrationBuilder.DropColumn(
                name: "Pers_Iden_mark_2",
                table: "MApiData");

            migrationBuilder.DropColumn(
                name: "Pers_Rank",
                table: "MApiData");

            migrationBuilder.DropColumn(
                name: "Pers_Regt",
                table: "MApiData");

            migrationBuilder.DropColumn(
                name: "Pers_UID",
                table: "MApiData");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Pers_Blood_Gp",
                table: "MApiDataOffrs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Pers_Father_Name",
                table: "MApiDataOffrs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Pers_Gender",
                table: "MApiDataOffrs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Pers_Height",
                table: "MApiDataOffrs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Pers_Iden_mark_1",
                table: "MApiDataOffrs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Pers_Iden_mark_2",
                table: "MApiDataOffrs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Pers_Rank",
                table: "MApiDataOffrs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Pers_Regt",
                table: "MApiDataOffrs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Pers_UID",
                table: "MApiDataOffrs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Pers_Blood_Gp",
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
                name: "Pers_UID",
                table: "MApiData",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
