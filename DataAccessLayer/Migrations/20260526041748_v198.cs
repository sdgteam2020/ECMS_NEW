using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v198 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrnICardRequest_BasicDetails_BasicDetailId",
                table: "TrnICardRequest");

            migrationBuilder.DropIndex(
                name: "IX_TrnICardRequest_BasicDetailId",
                table: "TrnICardRequest");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_TrnICardRequest_BasicDetailId",
                table: "TrnICardRequest",
                column: "BasicDetailId");

            migrationBuilder.AddForeignKey(
                name: "FK_TrnICardRequest_BasicDetails_BasicDetailId",
                table: "TrnICardRequest",
                column: "BasicDetailId",
                principalTable: "BasicDetails",
                principalColumn: "BasicDetailId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
