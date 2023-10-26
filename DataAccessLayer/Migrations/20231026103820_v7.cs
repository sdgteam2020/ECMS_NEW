using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MapUnit",
                table: "MapUnit");

            migrationBuilder.DropIndex(
                name: "IX_MapUnit_UnitId",
                table: "MapUnit");

            migrationBuilder.AlterColumn<int>(
                name: "UnitMapId",
                table: "MapUnit",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MapUnit",
                table: "MapUnit",
                column: "UnitId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MapUnit",
                table: "MapUnit");

            migrationBuilder.AlterColumn<int>(
                name: "UnitMapId",
                table: "MapUnit",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MapUnit",
                table: "MapUnit",
                column: "UnitMapId");

            migrationBuilder.CreateIndex(
                name: "IX_MapUnit_UnitId",
                table: "MapUnit",
                column: "UnitId");
        }
    }
}
