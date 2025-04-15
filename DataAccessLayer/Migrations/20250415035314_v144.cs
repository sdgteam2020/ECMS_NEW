using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v144 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PostingOutId",
                table: "TrnFwds");

            migrationBuilder.AddColumn<int>(
                name: "TrnFwdId",
                table: "TrnPostingOut",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TrnFwdId",
                table: "TrnFaultyCard",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TrnPostingOut_TrnFwdId",
                table: "TrnPostingOut",
                column: "TrnFwdId");

            migrationBuilder.CreateIndex(
                name: "IX_TrnFaultyCard_TrnFwdId",
                table: "TrnFaultyCard",
                column: "TrnFwdId");

            migrationBuilder.AddForeignKey(
                name: "FK_TrnFaultyCard_TrnFwds_TrnFwdId",
                table: "TrnFaultyCard",
                column: "TrnFwdId",
                principalTable: "TrnFwds",
                principalColumn: "TrnFwdId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TrnPostingOut_TrnFwds_TrnFwdId",
                table: "TrnPostingOut",
                column: "TrnFwdId",
                principalTable: "TrnFwds",
                principalColumn: "TrnFwdId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrnFaultyCard_TrnFwds_TrnFwdId",
                table: "TrnFaultyCard");

            migrationBuilder.DropForeignKey(
                name: "FK_TrnPostingOut_TrnFwds_TrnFwdId",
                table: "TrnPostingOut");

            migrationBuilder.DropIndex(
                name: "IX_TrnPostingOut_TrnFwdId",
                table: "TrnPostingOut");

            migrationBuilder.DropIndex(
                name: "IX_TrnFaultyCard_TrnFwdId",
                table: "TrnFaultyCard");

            migrationBuilder.DropColumn(
                name: "TrnFwdId",
                table: "TrnPostingOut");

            migrationBuilder.DropColumn(
                name: "TrnFwdId",
                table: "TrnFaultyCard");

            migrationBuilder.AddColumn<int>(
                name: "PostingOutId",
                table: "TrnFwds",
                type: "int",
                nullable: true);
        }
    }
}
