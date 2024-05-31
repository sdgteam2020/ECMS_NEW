using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v102 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "StepId",
                table: "TrnFwds",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.CreateIndex(
                name: "IX_TrnFwds_StepId",
                table: "TrnFwds",
                column: "StepId");

            migrationBuilder.AddForeignKey(
                name: "FK_TrnFwds_MStepCounterStep_StepId",
                table: "TrnFwds",
                column: "StepId",
                principalTable: "MStepCounterStep",
                principalColumn: "StepId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrnFwds_MStepCounterStep_StepId",
                table: "TrnFwds");

            migrationBuilder.DropIndex(
                name: "IX_TrnFwds_StepId",
                table: "TrnFwds");

            migrationBuilder.DropColumn(
                name: "StepId",
                table: "TrnFwds");
        }
    }
}
