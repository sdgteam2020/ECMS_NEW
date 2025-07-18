using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v177 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RequestId",
                table: "TrnDispatchCardMapping",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TrnDispatchCardMapping_RequestId",
                table: "TrnDispatchCardMapping",
                column: "RequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_TrnDispatchCardMapping_TrnICardRequest_RequestId",
                table: "TrnDispatchCardMapping",
                column: "RequestId",
                principalTable: "TrnICardRequest",
                principalColumn: "RequestId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrnDispatchCardMapping_TrnICardRequest_RequestId",
                table: "TrnDispatchCardMapping");

            migrationBuilder.DropIndex(
                name: "IX_TrnDispatchCardMapping_RequestId",
                table: "TrnDispatchCardMapping");

            migrationBuilder.DropColumn(
                name: "RequestId",
                table: "TrnDispatchCardMapping");
        }
    }
}
