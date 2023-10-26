using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MapUnit",
                table: "MapUnit");

            migrationBuilder.AlterColumn<int>(
                name: "UnitId",
                table: "MapUnit",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "UnitMapId",
                table: "MapUnit",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MapUnit",
                table: "MapUnit",
                column: "UnitMapId");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MapUnit_MUnit_UnitId",
                table: "MapUnit");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MapUnit",
                table: "MapUnit");

            migrationBuilder.DropIndex(
                name: "IX_MapUnit_UnitId",
                table: "MapUnit");

            migrationBuilder.DropColumn(
                name: "UnitMapId",
                table: "MapUnit");

            migrationBuilder.AlterColumn<int>(
                name: "UnitId",
                table: "MapUnit",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MapUnit",
                table: "MapUnit",
                column: "UnitId");
        }
    }
}
