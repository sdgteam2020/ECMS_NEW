using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v103 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IssuingAuth",
                table: "BasicDetails");

            migrationBuilder.AddColumn<byte>(
                name: "IssuingAuthorityId",
                table: "BasicDetails",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.CreateTable(
                name: "MIssuingAuthority",
                columns: table => new
                {
                    IssuingAuthorityId = table.Column<byte>(type: "tinyint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "VARCHAR(50)", nullable: false),
                    ApplyForId = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MIssuingAuthority", x => x.IssuingAuthorityId);
                    table.ForeignKey(
                        name: "FK_MIssuingAuthority_MApplyFor_ApplyForId",
                        column: x => x.ApplyForId,
                        principalTable: "MApplyFor",
                        principalColumn: "ApplyForId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BasicDetails_IssuingAuthorityId",
                table: "BasicDetails",
                column: "IssuingAuthorityId");

            migrationBuilder.CreateIndex(
                name: "IX_MIssuingAuthority_ApplyForId",
                table: "MIssuingAuthority",
                column: "ApplyForId");

            migrationBuilder.AddForeignKey(
                name: "FK_BasicDetails_MIssuingAuthority_IssuingAuthorityId",
                table: "BasicDetails",
                column: "IssuingAuthorityId",
                principalTable: "MIssuingAuthority",
                principalColumn: "IssuingAuthorityId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BasicDetails_MIssuingAuthority_IssuingAuthorityId",
                table: "BasicDetails");

            migrationBuilder.DropTable(
                name: "MIssuingAuthority");

            migrationBuilder.DropIndex(
                name: "IX_BasicDetails_IssuingAuthorityId",
                table: "BasicDetails");

            migrationBuilder.DropColumn(
                name: "IssuingAuthorityId",
                table: "BasicDetails");

            migrationBuilder.AddColumn<string>(
                name: "IssuingAuth",
                table: "BasicDetails",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
