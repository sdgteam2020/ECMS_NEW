using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v172 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AadhaarNo",
                table: "TrnIdentityInfo");

            migrationBuilder.DropIndex(
                name: "IX_BasicDetails_ServiceNo",
                table: "BasicDetails");

            migrationBuilder.DropIndex(
                name: "IX_PaperIcardNo",
                table: "BasicDetails");

            migrationBuilder.DropColumn(
                name: "OldServiceNo",
                table: "BasicDetails");

            migrationBuilder.AddColumn<int>(
                name: "PreviousBasicDetailId",
                table: "BasicDetails",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PreviousBasicDetailId",
                table: "BasicDetails");

            migrationBuilder.AddColumn<string>(
                name: "OldServiceNo",
                table: "BasicDetails",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AadhaarNo",
                table: "TrnIdentityInfo",
                column: "AadhaarNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BasicDetails_ServiceNo",
                table: "BasicDetails",
                column: "ServiceNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaperIcardNo",
                table: "BasicDetails",
                column: "PaperIcardNo",
                unique: true);
        }
    }
}
