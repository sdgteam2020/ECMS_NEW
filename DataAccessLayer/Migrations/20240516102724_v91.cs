using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v91 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_OROMapping_RecordOfficeId",
                table: "OROMapping",
                column: "RecordOfficeId");

            migrationBuilder.AddForeignKey(
                name: "FK_OROMapping_MRecordOffice_RecordOfficeId",
                table: "OROMapping",
                column: "RecordOfficeId",
                principalTable: "MRecordOffice",
                principalColumn: "RecordOfficeId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OROMapping_MRecordOffice_RecordOfficeId",
                table: "OROMapping");

            migrationBuilder.DropIndex(
                name: "IX_OROMapping_RecordOfficeId",
                table: "OROMapping");
        }
    }
}
