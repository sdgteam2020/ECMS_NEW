using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v143 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UnitId",
                table: "MRegimental",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MRegimental_UnitId",
                table: "MRegimental",
                column: "UnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_MRegimental_MapUnit_UnitId",
                table: "MRegimental",
                column: "UnitId",
                principalTable: "MapUnit",
                principalColumn: "UnitMapId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MRegimental_MapUnit_UnitId",
                table: "MRegimental");

            migrationBuilder.DropIndex(
                name: "IX_MRegimental_UnitId",
                table: "MRegimental");

            migrationBuilder.DropColumn(
                name: "UnitId",
                table: "MRegimental");
        }
    }
}
