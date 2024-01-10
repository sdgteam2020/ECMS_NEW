using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v17 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "ApplyForId",
                table: "TrnStepCounter",
                type: "tinyint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TrnStepCounter_ApplyForId",
                table: "TrnStepCounter",
                column: "ApplyForId");

            migrationBuilder.AddForeignKey(
                name: "FK_TrnStepCounter_MApplyFor_ApplyForId",
                table: "TrnStepCounter",
                column: "ApplyForId",
                principalTable: "MApplyFor",
                principalColumn: "ApplyForId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrnStepCounter_MApplyFor_ApplyForId",
                table: "TrnStepCounter");

            migrationBuilder.DropIndex(
                name: "IX_TrnStepCounter_ApplyForId",
                table: "TrnStepCounter");

            migrationBuilder.DropColumn(
                name: "ApplyForId",
                table: "TrnStepCounter");
        }
    }
}
