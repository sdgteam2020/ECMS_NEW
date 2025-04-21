using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v147 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "FlagForHotlist",
                table: "TrnICardRequest",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "FlagForLost",
                table: "TrnICardRequest",
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
                name: "FlagForHotlist",
                table: "TrnICardRequest");

            migrationBuilder.DropColumn(
                name: "FlagForLost",
                table: "TrnICardRequest");

            migrationBuilder.DropColumn(
                name: "DbInvalidRecords",
                table: "CSVImports");

            migrationBuilder.DropColumn(
                name: "SheetInvalidRecords",
                table: "CSVImports");
        }
    }
}
