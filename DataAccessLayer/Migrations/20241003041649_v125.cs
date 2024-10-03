using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v125 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PaperIcardNo",
                table: "BasicDetails",
                type: "varchar(12)",
                maxLength: 12,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50);

            migrationBuilder.CreateIndex(
                name: "IX_AadhaarNo",
                table: "TrnIdentityInfo",
                column: "AadhaarNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaperIcardNo",
                table: "BasicDetails",
                column: "PaperIcardNo",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AadhaarNo",
                table: "TrnIdentityInfo");

            migrationBuilder.DropIndex(
                name: "IX_PaperIcardNo",
                table: "BasicDetails");

            migrationBuilder.AlterColumn<string>(
                name: "PaperIcardNo",
                table: "BasicDetails",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(12)",
                oldMaxLength: 12);
        }
    }
}
