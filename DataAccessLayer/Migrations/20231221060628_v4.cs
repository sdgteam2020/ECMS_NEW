using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "TrnUpload",
                newName: "UploadId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "TrnIdentityInfo",
                newName: "InfoId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "TrnAddress",
                newName: "AddressId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UploadId",
                table: "TrnUpload",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "InfoId",
                table: "TrnIdentityInfo",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "AddressId",
                table: "TrnAddress",
                newName: "Id");
        }
    }
}
