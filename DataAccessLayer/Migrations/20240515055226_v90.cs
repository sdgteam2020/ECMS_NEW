using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v90 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OROMapping",
                columns: table => new
                {
                    OROMappingId = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArmedIdList = table.Column<string>(type: "varchar(100)", nullable: false),
                    RecordOfficeId = table.Column<byte>(type: "tinyint", nullable: false),
                    TDMId = table.Column<int>(type: "int", nullable: true),
                    UnitId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OROMapping", x => x.OROMappingId);
                    table.ForeignKey(
                        name: "FK_OROMapping_MapUnit_UnitId",
                        column: x => x.UnitId,
                        principalTable: "MapUnit",
                        principalColumn: "UnitMapId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OROMapping_TrnDomainMapping_TDMId",
                        column: x => x.TDMId,
                        principalTable: "TrnDomainMapping",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OROMapping_TDMId",
                table: "OROMapping",
                column: "TDMId");

            migrationBuilder.CreateIndex(
                name: "IX_OROMapping_UnitId",
                table: "OROMapping",
                column: "UnitId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OROMapping");
        }
    }
}
