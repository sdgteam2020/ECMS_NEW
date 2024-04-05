using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v74 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RequestId",
                table: "TrnPostingOut",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TrnPostingOut_RequestId",
                table: "TrnPostingOut",
                column: "RequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_TrnPostingOut_TrnICardRequest_RequestId",
                table: "TrnPostingOut",
                column: "RequestId",
                principalTable: "TrnICardRequest",
                principalColumn: "RequestId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrnPostingOut_TrnICardRequest_RequestId",
                table: "TrnPostingOut");

            migrationBuilder.DropIndex(
                name: "IX_TrnPostingOut_RequestId",
                table: "TrnPostingOut");

            migrationBuilder.DropColumn(
                name: "RequestId",
                table: "TrnPostingOut");
        }
    }
}
