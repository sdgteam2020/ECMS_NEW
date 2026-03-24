using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v195 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "RecordOfficeId",
                table: "BasicDetailTemps",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.CreateIndex(
                name: "IX_BasicDetailTemps_RecordOfficeId",
                table: "BasicDetailTemps",
                column: "RecordOfficeId");

            migrationBuilder.AddForeignKey(
                name: "FK_BasicDetailTemps_MRecordOffice_RecordOfficeId",
                table: "BasicDetailTemps",
                column: "RecordOfficeId",
                principalTable: "MRecordOffice",
                principalColumn: "RecordOfficeId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BasicDetailTemps_MRecordOffice_RecordOfficeId",
                table: "BasicDetailTemps");

            migrationBuilder.DropIndex(
                name: "IX_BasicDetailTemps_RecordOfficeId",
                table: "BasicDetailTemps");

            migrationBuilder.DropColumn(
                name: "RecordOfficeId",
                table: "BasicDetailTemps");
        }
    }
}
