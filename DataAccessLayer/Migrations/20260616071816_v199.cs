using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v199 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrnApplClose_BasicDetails_BasicDetailId",
                table: "TrnApplClose");

            migrationBuilder.DropForeignKey(
                name: "FK_TrnPostingOut_BasicDetails_BasicDetailId",
                table: "TrnPostingOut");

            migrationBuilder.DropIndex(
                name: "IX_TrnPostingOut_BasicDetailId",
                table: "TrnPostingOut");

            migrationBuilder.DropIndex(
                name: "IX_TrnApplClose_BasicDetailId",
                table: "TrnApplClose");

            migrationBuilder.DropColumn(
                name: "BasicDetailId",
                table: "TrnPostingOut");

            migrationBuilder.DropColumn(
                name: "BasicDetailId",
                table: "TrnApplClose");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BasicDetailId",
                table: "TrnPostingOut",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BasicDetailId",
                table: "TrnApplClose",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TrnPostingOut_BasicDetailId",
                table: "TrnPostingOut",
                column: "BasicDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_TrnApplClose_BasicDetailId",
                table: "TrnApplClose",
                column: "BasicDetailId");

            migrationBuilder.AddForeignKey(
                name: "FK_TrnApplClose_BasicDetails_BasicDetailId",
                table: "TrnApplClose",
                column: "BasicDetailId",
                principalTable: "BasicDetails",
                principalColumn: "BasicDetailId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TrnPostingOut_BasicDetails_BasicDetailId",
                table: "TrnPostingOut",
                column: "BasicDetailId",
                principalTable: "BasicDetails",
                principalColumn: "BasicDetailId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
