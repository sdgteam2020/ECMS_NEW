using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v13 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Abbreviation",
                table: "MUnit",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ArmedId",
                table: "MUnit",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_MUnit_ArmedId",
                table: "MUnit",
                column: "ArmedId");

            migrationBuilder.AddForeignKey(
                name: "FK_MUnit_MArmedType_ArmedId",
                table: "MUnit",
                column: "ArmedId",
                principalTable: "MArmedType",
                principalColumn: "ArmedId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MUnit_MArmedType_ArmedId",
                table: "MUnit");

            migrationBuilder.DropIndex(
                name: "IX_MUnit_ArmedId",
                table: "MUnit");

            migrationBuilder.DropColumn(
                name: "Abbreviation",
                table: "MUnit");

            migrationBuilder.DropColumn(
                name: "ArmedId",
                table: "MUnit");
        }
    }
}
