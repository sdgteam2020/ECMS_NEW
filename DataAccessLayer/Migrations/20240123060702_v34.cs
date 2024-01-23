using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v34 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_TrnPostingOut_BasicDetailId",
                table: "TrnPostingOut",
                column: "BasicDetailId");

            migrationBuilder.AddForeignKey(
                name: "FK_TrnPostingOut_BasicDetails_BasicDetailId",
                table: "TrnPostingOut",
                column: "BasicDetailId",
                principalTable: "BasicDetails",
                principalColumn: "BasicDetailId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrnPostingOut_BasicDetails_BasicDetailId",
                table: "TrnPostingOut");

            migrationBuilder.DropIndex(
                name: "IX_TrnPostingOut_BasicDetailId",
                table: "TrnPostingOut");
        }
    }
}
