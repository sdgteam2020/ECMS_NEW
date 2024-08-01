using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v122 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClaimsStore",
                columns: table => new
                {
                    ClaimsStoreId = table.Column<byte>(type: "tinyint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClaimType = table.Column<string>(type: "varchar(50)", nullable: false),
                    ClaimValue = table.Column<string>(type: "varchar(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClaimsStore", x => x.ClaimsStoreId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BasicDetails_ServiceNo",
                table: "BasicDetails",
                column: "ServiceNo",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClaimsStore");

            migrationBuilder.DropIndex(
                name: "IX_BasicDetails_ServiceNo",
                table: "BasicDetails");
        }
    }
}
