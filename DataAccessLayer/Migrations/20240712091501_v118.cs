using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v118 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AfsacCellMapping",
                columns: table => new
                {
                    AfsacCellMappingId = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TDMId = table.Column<int>(type: "int", nullable: true),
                    UnitId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AfsacCellMapping", x => x.AfsacCellMappingId);
                    table.ForeignKey(
                        name: "FK_AfsacCellMapping_MapUnit_UnitId",
                        column: x => x.UnitId,
                        principalTable: "MapUnit",
                        principalColumn: "UnitMapId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AfsacCellMapping_TrnDomainMapping_TDMId",
                        column: x => x.TDMId,
                        principalTable: "TrnDomainMapping",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AfsacCellMapping_TDMId",
                table: "AfsacCellMapping",
                column: "TDMId");

            migrationBuilder.CreateIndex(
                name: "IX_AfsacCellMapping_UnitId",
                table: "AfsacCellMapping",
                column: "UnitId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AfsacCellMapping");
        }
    }
}
