using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v48 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MApiData",
                table: "MApiData");

            migrationBuilder.RenameTable(
                name: "MApiData",
                newName: "MApiDataOffrs");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MApiDataOffrs",
                table: "MApiDataOffrs",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MApiDataOffrs",
                table: "MApiDataOffrs");

            migrationBuilder.RenameTable(
                name: "MApiDataOffrs",
                newName: "MApiData");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MApiData",
                table: "MApiData",
                column: "Id");
        }
    }
}
