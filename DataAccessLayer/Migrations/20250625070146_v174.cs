using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v174 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "RecordOfficeId",
                table: "TrnICardRequest",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.CreateIndex(
                name: "IX_TrnICardRequest_RecordOfficeId",
                table: "TrnICardRequest",
                column: "RecordOfficeId");

            migrationBuilder.AddForeignKey(
                name: "FK_TrnICardRequest_MRecordOffice_RecordOfficeId",
                table: "TrnICardRequest",
                column: "RecordOfficeId",
                principalTable: "MRecordOffice",
                principalColumn: "RecordOfficeId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrnICardRequest_MRecordOffice_RecordOfficeId",
                table: "TrnICardRequest");

            migrationBuilder.DropIndex(
                name: "IX_TrnICardRequest_RecordOfficeId",
                table: "TrnICardRequest");

            migrationBuilder.DropColumn(
                name: "RecordOfficeId",
                table: "TrnICardRequest");
        }
    }
}
