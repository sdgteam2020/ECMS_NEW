using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v176 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ModeOfDispatch",
                table: "TrnDispatchCard");

            migrationBuilder.AddColumn<byte>(
                name: "DispatchModeId",
                table: "TrnDispatchCard",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.CreateTable(
                name: "MDispatchMode",
                columns: table => new
                {
                    DispatchModeId = table.Column<byte>(type: "tinyint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MDispatchMode", x => x.DispatchModeId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TrnDispatchCard_DispatchModeId",
                table: "TrnDispatchCard",
                column: "DispatchModeId");

            migrationBuilder.AddForeignKey(
                name: "FK_TrnDispatchCard_MDispatchMode_DispatchModeId",
                table: "TrnDispatchCard",
                column: "DispatchModeId",
                principalTable: "MDispatchMode",
                principalColumn: "DispatchModeId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrnDispatchCard_MDispatchMode_DispatchModeId",
                table: "TrnDispatchCard");

            migrationBuilder.DropTable(
                name: "MDispatchMode");

            migrationBuilder.DropIndex(
                name: "IX_TrnDispatchCard_DispatchModeId",
                table: "TrnDispatchCard");

            migrationBuilder.DropColumn(
                name: "DispatchModeId",
                table: "TrnDispatchCard");

            migrationBuilder.AddColumn<string>(
                name: "ModeOfDispatch",
                table: "TrnDispatchCard",
                type: "VARCHAR(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
