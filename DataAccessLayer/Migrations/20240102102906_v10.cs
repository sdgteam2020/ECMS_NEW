using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "FmnBranchID",
                table: "MapUnit",
                type: "tinyint",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "PsoId",
                table: "MapUnit",
                type: "tinyint",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "SubDteId",
                table: "MapUnit",
                type: "tinyint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FmnBranchID",
                table: "MapUnit");

            migrationBuilder.DropColumn(
                name: "PsoId",
                table: "MapUnit");

            migrationBuilder.DropColumn(
                name: "SubDteId",
                table: "MapUnit");
        }
    }
}
