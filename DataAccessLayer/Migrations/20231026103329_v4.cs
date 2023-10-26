using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MapUnit_MUnit_UnitId",
                table: "MapUnit");

            migrationBuilder.DropIndex(
                name: "IX_MapUnit_UnitId",
                table: "MapUnit");

            migrationBuilder.DropColumn(
                name: "UnitId",
                table: "MapUnit");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UnitId",
                table: "MapUnit",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_MapUnit_UnitId",
                table: "MapUnit",
                column: "UnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_MapUnit_MUnit_UnitId",
                table: "MapUnit",
                column: "UnitId",
                principalTable: "MUnit",
                principalColumn: "UnitId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
