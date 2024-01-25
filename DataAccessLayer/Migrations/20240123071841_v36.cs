using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v36 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PermanentAddress",
                table: "BasicDetailTemps");

            migrationBuilder.AddColumn<string>(
                name: "District",
                table: "BasicDetailTemps",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PO",
                table: "BasicDetailTemps",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PS",
                table: "BasicDetailTemps",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PinCode",
                table: "BasicDetailTemps",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "BasicDetailTemps",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tehsil",
                table: "BasicDetailTemps",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Village",
                table: "BasicDetailTemps",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "District",
                table: "BasicDetailTemps");

            migrationBuilder.DropColumn(
                name: "PO",
                table: "BasicDetailTemps");

            migrationBuilder.DropColumn(
                name: "PS",
                table: "BasicDetailTemps");

            migrationBuilder.DropColumn(
                name: "PinCode",
                table: "BasicDetailTemps");

            migrationBuilder.DropColumn(
                name: "State",
                table: "BasicDetailTemps");

            migrationBuilder.DropColumn(
                name: "Tehsil",
                table: "BasicDetailTemps");

            migrationBuilder.DropColumn(
                name: "Village",
                table: "BasicDetailTemps");

            migrationBuilder.AddColumn<string>(
                name: "PermanentAddress",
                table: "BasicDetailTemps",
                type: "nvarchar(Max)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }
    }
}
