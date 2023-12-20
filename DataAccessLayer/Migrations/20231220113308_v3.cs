using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BasicDetails_MUnit_UnitId",
                table: "BasicDetails");

            migrationBuilder.AlterColumn<long>(
                name: "AadhaarNo",
                table: "TrnIdentityInfo",
                type: "bigint",
                maxLength: 12,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 12);

            migrationBuilder.AddForeignKey(
                name: "FK_BasicDetails_MapUnit_UnitId",
                table: "BasicDetails",
                column: "UnitId",
                principalTable: "MapUnit",
                principalColumn: "UnitMapId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BasicDetails_MapUnit_UnitId",
                table: "BasicDetails");

            migrationBuilder.AlterColumn<int>(
                name: "AadhaarNo",
                table: "TrnIdentityInfo",
                type: "int",
                maxLength: 12,
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldMaxLength: 12);

            migrationBuilder.AddForeignKey(
                name: "FK_BasicDetails_MUnit_UnitId",
                table: "BasicDetails",
                column: "UnitId",
                principalTable: "MUnit",
                principalColumn: "UnitId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
