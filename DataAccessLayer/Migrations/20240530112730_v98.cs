using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v98 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MStepCounterStep_MFwdType_MTrnFwdTypeTypeId",
                table: "MStepCounterStep");

            migrationBuilder.DropIndex(
                name: "IX_MStepCounterStep_MTrnFwdTypeTypeId",
                table: "MStepCounterStep");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "MStepCounterStep");

            migrationBuilder.DropColumn(
                name: "MTrnFwdTypeTypeId",
                table: "MStepCounterStep");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "GroupId",
                table: "MStepCounterStep",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "MTrnFwdTypeTypeId",
                table: "MStepCounterStep",
                type: "tinyint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MStepCounterStep_MTrnFwdTypeTypeId",
                table: "MStepCounterStep",
                column: "MTrnFwdTypeTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_MStepCounterStep_MFwdType_MTrnFwdTypeTypeId",
                table: "MStepCounterStep",
                column: "MTrnFwdTypeTypeId",
                principalTable: "MFwdType",
                principalColumn: "TypeId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
