using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v202 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CardExportedByAspNetUserId",
                table: "TrnICardRequest",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CardExportedByUserId",
                table: "TrnICardRequest",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CardPrintedByAspNetUserId",
                table: "TrnICardRequest",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CardPrintedByUserId",
                table: "TrnICardRequest",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DestructedCardId",
                table: "CompletedICardRequests",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TrnICardRequest_CardExportedByAspNetUserId",
                table: "TrnICardRequest",
                column: "CardExportedByAspNetUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TrnICardRequest_CardExportedByUserId",
                table: "TrnICardRequest",
                column: "CardExportedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TrnICardRequest_CardPrintedByAspNetUserId",
                table: "TrnICardRequest",
                column: "CardPrintedByAspNetUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TrnICardRequest_CardPrintedByUserId",
                table: "TrnICardRequest",
                column: "CardPrintedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CompletedICardRequests_DestructedCardId",
                table: "CompletedICardRequests",
                column: "DestructedCardId");

            migrationBuilder.AddForeignKey(
                name: "FK_CompletedICardRequests_TrnDestructionCards_DestructedCardId",
                table: "CompletedICardRequests",
                column: "DestructedCardId",
                principalTable: "TrnDestructionCards",
                principalColumn: "DestructedCardId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TrnICardRequest_AspNetUsers_CardExportedByAspNetUserId",
                table: "TrnICardRequest",
                column: "CardExportedByAspNetUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TrnICardRequest_AspNetUsers_CardPrintedByAspNetUserId",
                table: "TrnICardRequest",
                column: "CardPrintedByAspNetUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TrnICardRequest_UserProfile_CardExportedByUserId",
                table: "TrnICardRequest",
                column: "CardExportedByUserId",
                principalTable: "UserProfile",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TrnICardRequest_UserProfile_CardPrintedByUserId",
                table: "TrnICardRequest",
                column: "CardPrintedByUserId",
                principalTable: "UserProfile",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompletedICardRequests_TrnDestructionCards_DestructedCardId",
                table: "CompletedICardRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_TrnICardRequest_AspNetUsers_CardExportedByAspNetUserId",
                table: "TrnICardRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_TrnICardRequest_AspNetUsers_CardPrintedByAspNetUserId",
                table: "TrnICardRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_TrnICardRequest_UserProfile_CardExportedByUserId",
                table: "TrnICardRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_TrnICardRequest_UserProfile_CardPrintedByUserId",
                table: "TrnICardRequest");

            migrationBuilder.DropIndex(
                name: "IX_TrnICardRequest_CardExportedByAspNetUserId",
                table: "TrnICardRequest");

            migrationBuilder.DropIndex(
                name: "IX_TrnICardRequest_CardExportedByUserId",
                table: "TrnICardRequest");

            migrationBuilder.DropIndex(
                name: "IX_TrnICardRequest_CardPrintedByAspNetUserId",
                table: "TrnICardRequest");

            migrationBuilder.DropIndex(
                name: "IX_TrnICardRequest_CardPrintedByUserId",
                table: "TrnICardRequest");

            migrationBuilder.DropIndex(
                name: "IX_CompletedICardRequests_DestructedCardId",
                table: "CompletedICardRequests");

            migrationBuilder.DropColumn(
                name: "CardExportedByAspNetUserId",
                table: "TrnICardRequest");

            migrationBuilder.DropColumn(
                name: "CardExportedByUserId",
                table: "TrnICardRequest");

            migrationBuilder.DropColumn(
                name: "CardPrintedByAspNetUserId",
                table: "TrnICardRequest");

            migrationBuilder.DropColumn(
                name: "CardPrintedByUserId",
                table: "TrnICardRequest");

            migrationBuilder.DropColumn(
                name: "DestructedCardId",
                table: "CompletedICardRequests");
        }
    }
}
