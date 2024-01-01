using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MRemarksApply",
                columns: table => new
                {
                    RemarksApplyId = table.Column<byte>(type: "tinyint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RemarksApply = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MRemarksApply", x => x.RemarksApplyId);
                });

            migrationBuilder.CreateTable(
                name: "MRemarkType",
                columns: table => new
                {
                    RemarkTypeId = table.Column<byte>(type: "tinyint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RemarksType = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MRemarkType", x => x.RemarkTypeId);
                });

            migrationBuilder.CreateTable(
                name: "MRemarks",
                columns: table => new
                {
                    RemarksId = table.Column<byte>(type: "tinyint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Remarks = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false),
                    RemarkTypeId = table.Column<byte>(type: "tinyint", nullable: false),
                    RemarksApplyId = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MRemarks", x => x.RemarksId);
                    table.ForeignKey(
                        name: "FK_MRemarks_MRemarkType_RemarkTypeId",
                        column: x => x.RemarkTypeId,
                        principalTable: "MRemarkType",
                        principalColumn: "RemarkTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MRemarks_MRemarksApply_RemarksApplyId",
                        column: x => x.RemarksApplyId,
                        principalTable: "MRemarksApply",
                        principalColumn: "RemarksApplyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MRemarks_RemarksApplyId",
                table: "MRemarks",
                column: "RemarksApplyId");

            migrationBuilder.CreateIndex(
                name: "IX_MRemarks_RemarkTypeId",
                table: "MRemarks",
                column: "RemarkTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MRemarks");

            migrationBuilder.DropTable(
                name: "MRemarkType");

            migrationBuilder.DropTable(
                name: "MRemarksApply");
        }
    }
}
