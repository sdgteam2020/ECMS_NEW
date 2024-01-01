using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RequestId",
                table: "TrnNotification",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TrnNotification_RequestId",
                table: "TrnNotification",
                column: "RequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_TrnNotification_TrnICardRequest_RequestId",
                table: "TrnNotification",
                column: "RequestId",
                principalTable: "TrnICardRequest",
                principalColumn: "RequestId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrnNotification_TrnICardRequest_RequestId",
                table: "TrnNotification");

            migrationBuilder.DropIndex(
                name: "IX_TrnNotification_RequestId",
                table: "TrnNotification");

            migrationBuilder.DropColumn(
                name: "RequestId",
                table: "TrnNotification");
        }
    }
}
