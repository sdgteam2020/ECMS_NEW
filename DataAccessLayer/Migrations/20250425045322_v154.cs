using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v154 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrnMapUnitChangeRequest_MUnit_UnitId",
                table: "TrnMapUnitChangeRequest");

            migrationBuilder.DropIndex(
                name: "IX_TrnMapUnitChangeRequest_UnitId",
                table: "TrnMapUnitChangeRequest");

            migrationBuilder.DropColumn(
                name: "UnitId",
                table: "TrnMapUnitChangeRequest");

            migrationBuilder.RenameColumn(
                name: "ToUnit",
                table: "TrnMapUnitChangeRequest",
                newName: "RequestCh");

            migrationBuilder.RenameColumn(
                name: "FromUnit",
                table: "TrnMapUnitChangeRequest",
                newName: "ExistingCh");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RequestCh",
                table: "TrnMapUnitChangeRequest",
                newName: "ToUnit");

            migrationBuilder.RenameColumn(
                name: "ExistingCh",
                table: "TrnMapUnitChangeRequest",
                newName: "FromUnit");

            migrationBuilder.AddColumn<int>(
                name: "UnitId",
                table: "TrnMapUnitChangeRequest",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TrnMapUnitChangeRequest_UnitId",
                table: "TrnMapUnitChangeRequest",
                column: "UnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_TrnMapUnitChangeRequest_MUnit_UnitId",
                table: "TrnMapUnitChangeRequest",
                column: "UnitId",
                principalTable: "MUnit",
                principalColumn: "UnitId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
