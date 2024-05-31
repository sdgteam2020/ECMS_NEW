using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v101 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "TypeId",
                table: "MStepCounterStep",
                type: "tinyint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MStepCounterStep_TypeId",
                table: "MStepCounterStep",
                column: "TypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_MStepCounterStep_MFwdType_TypeId",
                table: "MStepCounterStep",
                column: "TypeId",
                principalTable: "MFwdType",
                principalColumn: "TypeId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MStepCounterStep_MFwdType_TypeId",
                table: "MStepCounterStep");

            migrationBuilder.DropIndex(
                name: "IX_MStepCounterStep_TypeId",
                table: "MStepCounterStep");

            migrationBuilder.DropColumn(
                name: "TypeId",
                table: "MStepCounterStep");
        }
    }
}
