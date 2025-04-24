using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v150 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FlagForFaulty",
                table: "TrnICardRequest");

            migrationBuilder.AddColumn<bool>(
                name: "IsComplete",
                table: "TrnFaultyCard",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "DbInvalidRecords",
                table: "CSVImports",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SheetInvalidRecords",
                table: "CSVImports",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsComplete",
                table: "TrnFaultyCard");

            migrationBuilder.DropColumn(
                name: "DbInvalidRecords",
                table: "CSVImports");

            migrationBuilder.DropColumn(
                name: "SheetInvalidRecords",
                table: "CSVImports");

            migrationBuilder.AddColumn<bool>(
                name: "FlagForFaulty",
                table: "TrnICardRequest",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
