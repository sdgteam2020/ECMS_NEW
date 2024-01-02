using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte>(
                name: "SubDteId",
                table: "MapUnit",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0,
                oldClrType: typeof(byte),
                oldType: "tinyint",
                oldNullable: true);

            migrationBuilder.AlterColumn<byte>(
                name: "PsoId",
                table: "MapUnit",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0,
                oldClrType: typeof(byte),
                oldType: "tinyint",
                oldNullable: true);

            migrationBuilder.AlterColumn<byte>(
                name: "FmnBranchID",
                table: "MapUnit",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0,
                oldClrType: typeof(byte),
                oldType: "tinyint",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MapUnit_FmnBranchID",
                table: "MapUnit",
                column: "FmnBranchID");

            migrationBuilder.CreateIndex(
                name: "IX_MapUnit_PsoId",
                table: "MapUnit",
                column: "PsoId");

            migrationBuilder.CreateIndex(
                name: "IX_MapUnit_SubDteId",
                table: "MapUnit",
                column: "SubDteId");

            migrationBuilder.AddForeignKey(
                name: "FK_MapUnit_MFmnBranches_FmnBranchID",
                table: "MapUnit",
                column: "FmnBranchID",
                principalTable: "MFmnBranches",
                principalColumn: "FmnBranchID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MapUnit_MPso_PsoId",
                table: "MapUnit",
                column: "PsoId",
                principalTable: "MPso",
                principalColumn: "PsoId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MapUnit_MSubDte_SubDteId",
                table: "MapUnit",
                column: "SubDteId",
                principalTable: "MSubDte",
                principalColumn: "SubDteId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MapUnit_MFmnBranches_FmnBranchID",
                table: "MapUnit");

            migrationBuilder.DropForeignKey(
                name: "FK_MapUnit_MPso_PsoId",
                table: "MapUnit");

            migrationBuilder.DropForeignKey(
                name: "FK_MapUnit_MSubDte_SubDteId",
                table: "MapUnit");

            migrationBuilder.DropIndex(
                name: "IX_MapUnit_FmnBranchID",
                table: "MapUnit");

            migrationBuilder.DropIndex(
                name: "IX_MapUnit_PsoId",
                table: "MapUnit");

            migrationBuilder.DropIndex(
                name: "IX_MapUnit_SubDteId",
                table: "MapUnit");

            migrationBuilder.AlterColumn<byte>(
                name: "SubDteId",
                table: "MapUnit",
                type: "tinyint",
                nullable: true,
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AlterColumn<byte>(
                name: "PsoId",
                table: "MapUnit",
                type: "tinyint",
                nullable: true,
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AlterColumn<byte>(
                name: "FmnBranchID",
                table: "MapUnit",
                type: "tinyint",
                nullable: true,
                oldClrType: typeof(byte),
                oldType: "tinyint");
        }
    }
}
