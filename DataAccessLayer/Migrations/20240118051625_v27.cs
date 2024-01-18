using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v27 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TrnPostingOut",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReasonId = table.Column<byte>(type: "tinyint", nullable: false),
                    Authority = table.Column<int>(type: "int", nullable: false),
                    SOSDate = table.Column<int>(type: "int", nullable: false),
                    FromDomainId = table.Column<int>(type: "int", nullable: false),
                    FromUnitID = table.Column<int>(type: "int", nullable: false),
                    FromUserID = table.Column<int>(type: "int", nullable: false),
                    ToDomainId = table.Column<int>(type: "int", nullable: false),
                    ToUnitID = table.Column<int>(type: "int", nullable: false),
                    ToUserID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrnPostingOut", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrnPostingOut_MPostingReason_ReasonId",
                        column: x => x.ReasonId,
                        principalTable: "MPostingReason",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TrnPostingOut_ReasonId",
                table: "TrnPostingOut",
                column: "ReasonId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrnPostingOut");
        }
    }
}
