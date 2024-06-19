using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v113 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte>(
                name: "ArmedId",
                table: "MRegimental",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0,
                oldClrType: typeof(byte),
                oldType: "tinyint",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MRegimental_ArmedId",
                table: "MRegimental",
                column: "ArmedId");

            migrationBuilder.AddForeignKey(
                name: "FK_MRegimental_MArmedType_ArmedId",
                table: "MRegimental",
                column: "ArmedId",
                principalTable: "MArmedType",
                principalColumn: "ArmedId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MRegimental_MArmedType_ArmedId",
                table: "MRegimental");

            migrationBuilder.DropIndex(
                name: "IX_MRegimental_ArmedId",
                table: "MRegimental");

            migrationBuilder.AlterColumn<byte>(
                name: "ArmedId",
                table: "MRegimental",
                type: "tinyint",
                nullable: true,
                oldClrType: typeof(byte),
                oldType: "tinyint");
        }
    }
}
