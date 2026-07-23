using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v203 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DestructedCardId",
                table: "TrnApplClose",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TrnApplClose_DestructedCardId",
                table: "TrnApplClose",
                column: "DestructedCardId");

            migrationBuilder.AddForeignKey(
                name: "FK_TrnApplClose_TrnDestructionCards_DestructedCardId",
                table: "TrnApplClose",
                column: "DestructedCardId",
                principalTable: "TrnDestructionCards",
                principalColumn: "DestructedCardId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrnApplClose_TrnDestructionCards_DestructedCardId",
                table: "TrnApplClose");

            migrationBuilder.DropIndex(
                name: "IX_TrnApplClose_DestructedCardId",
                table: "TrnApplClose");

            migrationBuilder.DropColumn(
                name: "DestructedCardId",
                table: "TrnApplClose");
        }
    }
}
