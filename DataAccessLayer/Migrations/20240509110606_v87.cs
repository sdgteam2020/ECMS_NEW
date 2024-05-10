using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v87 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "RecordOfficeId",
                table: "BasicDetails",
                type: "tinyint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BasicDetails_RecordOfficeId",
                table: "BasicDetails",
                column: "RecordOfficeId");

            migrationBuilder.AddForeignKey(
                name: "FK_BasicDetails_MRecordOffice_RecordOfficeId",
                table: "BasicDetails",
                column: "RecordOfficeId",
                principalTable: "MRecordOffice",
                principalColumn: "RecordOfficeId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BasicDetails_MRecordOffice_RecordOfficeId",
                table: "BasicDetails");

            migrationBuilder.DropIndex(
                name: "IX_BasicDetails_RecordOfficeId",
                table: "BasicDetails");

            migrationBuilder.DropColumn(
                name: "RecordOfficeId",
                table: "BasicDetails");
        }
    }
}
