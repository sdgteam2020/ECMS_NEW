using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v119 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "TrnICardRequest");

            migrationBuilder.AddColumn<byte>(
                name: "StatusId",
                table: "TrnICardRequest",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.CreateTable(
                name: "MTrnICardStatus",
                columns: table => new
                {
                    StatusId = table.Column<byte>(type: "tinyint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MTrnICardStatus", x => x.StatusId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TrnICardRequest_StatusId",
                table: "TrnICardRequest",
                column: "StatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_TrnICardRequest_MTrnICardStatus_StatusId",
                table: "TrnICardRequest",
                column: "StatusId",
                principalTable: "MTrnICardStatus",
                principalColumn: "StatusId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrnICardRequest_MTrnICardStatus_StatusId",
                table: "TrnICardRequest");

            migrationBuilder.DropTable(
                name: "MTrnICardStatus");

            migrationBuilder.DropIndex(
                name: "IX_TrnICardRequest_StatusId",
                table: "TrnICardRequest");

            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "TrnICardRequest");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "TrnICardRequest",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
