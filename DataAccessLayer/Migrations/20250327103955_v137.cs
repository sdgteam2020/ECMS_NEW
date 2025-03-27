using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v137 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrnFaultyCard_MCategory_MFaultyStageCategoryId",
                table: "TrnFaultyCard");

            migrationBuilder.DropIndex(
                name: "IX_TrnFaultyCard_MFaultyStageCategoryId",
                table: "TrnFaultyCard");

            migrationBuilder.DropColumn(
                name: "MFaultyStageCategoryId",
                table: "TrnFaultyCard");

            migrationBuilder.AddColumn<bool>(
                name: "FlagForFaulty",
                table: "TrnICardRequest",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_TrnFaultyCard_CategoryId",
                table: "TrnFaultyCard",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_TrnFaultyCard_MCategory_CategoryId",
                table: "TrnFaultyCard",
                column: "CategoryId",
                principalTable: "MCategory",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrnFaultyCard_MCategory_CategoryId",
                table: "TrnFaultyCard");

            migrationBuilder.DropIndex(
                name: "IX_TrnFaultyCard_CategoryId",
                table: "TrnFaultyCard");

            migrationBuilder.DropColumn(
                name: "FlagForFaulty",
                table: "TrnICardRequest");

            migrationBuilder.AddColumn<byte>(
                name: "MFaultyStageCategoryId",
                table: "TrnFaultyCard",
                type: "tinyint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TrnFaultyCard_MFaultyStageCategoryId",
                table: "TrnFaultyCard",
                column: "MFaultyStageCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_TrnFaultyCard_MCategory_MFaultyStageCategoryId",
                table: "TrnFaultyCard",
                column: "MFaultyStageCategoryId",
                principalTable: "MCategory",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
