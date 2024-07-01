using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v117 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "BasicDetailTemps");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "BasicDetails");

            migrationBuilder.AddColumn<string>(
                name: "FName",
                table: "BasicDetailTemps",
                type: "varchar(18)",
                maxLength: 18,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LName",
                table: "BasicDetailTemps",
                type: "varchar(18)",
                maxLength: 18,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FName",
                table: "BasicDetails",
                type: "varchar(18)",
                maxLength: 18,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LName",
                table: "BasicDetails",
                type: "varchar(18)",
                maxLength: 18,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FName",
                table: "BasicDetailTemps");

            migrationBuilder.DropColumn(
                name: "LName",
                table: "BasicDetailTemps");

            migrationBuilder.DropColumn(
                name: "FName",
                table: "BasicDetails");

            migrationBuilder.DropColumn(
                name: "LName",
                table: "BasicDetails");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "BasicDetailTemps",
                type: "varchar(36)",
                maxLength: 36,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "BasicDetails",
                type: "varchar(36)",
                maxLength: 36,
                nullable: false,
                defaultValue: "");
        }
    }
}
