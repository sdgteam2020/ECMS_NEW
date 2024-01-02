using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FmnBrach",
                table: "MapUnit");

            migrationBuilder.AddColumn<int>(
                name: "UnitType",
                table: "MapUnit",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UnitType",
                table: "MapUnit");

            migrationBuilder.AddColumn<bool>(
                name: "FmnBrach",
                table: "MapUnit",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
