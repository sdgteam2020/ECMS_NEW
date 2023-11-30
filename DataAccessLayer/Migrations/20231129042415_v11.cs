using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ArmedCatId",
                table: "MArmedType",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_MArmedType_ArmedCatId",
                table: "MArmedType",
                column: "ArmedCatId");

            migrationBuilder.AddForeignKey(
                name: "FK_MArmedType_MArmedCats_ArmedCatId",
                table: "MArmedType",
                column: "ArmedCatId",
                principalTable: "MArmedCats",
                principalColumn: "ArmedCatId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MArmedType_MArmedCats_ArmedCatId",
                table: "MArmedType");

            migrationBuilder.DropIndex(
                name: "IX_MArmedType_ArmedCatId",
                table: "MArmedType");

            migrationBuilder.DropColumn(
                name: "ArmedCatId",
                table: "MArmedType");
        }
    }
}
