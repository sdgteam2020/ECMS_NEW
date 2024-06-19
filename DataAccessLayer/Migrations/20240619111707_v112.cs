using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v112 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
    }
}
