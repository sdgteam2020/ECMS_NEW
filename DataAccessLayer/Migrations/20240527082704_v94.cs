using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v94 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "TrnFwds");

            migrationBuilder.AddColumn<byte>(
                name: "FwdStatusId",
                table: "TrnFwds",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.CreateTable(
                name: "MTrnFwdStatus",
                columns: table => new
                {
                    FwdStatusId = table.Column<byte>(type: "tinyint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MTrnFwdStatus", x => x.FwdStatusId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TrnFwds_FwdStatusId",
                table: "TrnFwds",
                column: "FwdStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_TrnFwds_MTrnFwdStatus_FwdStatusId",
                table: "TrnFwds",
                column: "FwdStatusId",
                principalTable: "MTrnFwdStatus",
                principalColumn: "FwdStatusId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrnFwds_MTrnFwdStatus_FwdStatusId",
                table: "TrnFwds");

            migrationBuilder.DropTable(
                name: "MTrnFwdStatus");

            migrationBuilder.DropIndex(
                name: "IX_TrnFwds_FwdStatusId",
                table: "TrnFwds");

            migrationBuilder.DropColumn(
                name: "FwdStatusId",
                table: "TrnFwds");

            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "TrnFwds",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
