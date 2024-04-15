using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v78 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TDMId",
                table: "MRecordOffice",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MRecordOffice_TDMId",
                table: "MRecordOffice",
                column: "TDMId");

            migrationBuilder.AddForeignKey(
                name: "FK_MRecordOffice_TrnDomainMapping_TDMId",
                table: "MRecordOffice",
                column: "TDMId",
                principalTable: "TrnDomainMapping",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MRecordOffice_TrnDomainMapping_TDMId",
                table: "MRecordOffice");

            migrationBuilder.DropIndex(
                name: "IX_MRecordOffice_TDMId",
                table: "MRecordOffice");

            migrationBuilder.DropColumn(
                name: "TDMId",
                table: "MRecordOffice");
        }
    }
}
