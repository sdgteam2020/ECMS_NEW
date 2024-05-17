using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v92 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ArmedIdList",
                table: "OROMapping",
                type: "varchar(100)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)");

            migrationBuilder.AddColumn<short>(
                name: "RankId",
                table: "OROMapping",
                type: "smallint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OROMapping_RankId",
                table: "OROMapping",
                column: "RankId");

            migrationBuilder.AddForeignKey(
                name: "FK_OROMapping_MRank_RankId",
                table: "OROMapping",
                column: "RankId",
                principalTable: "MRank",
                principalColumn: "RankId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OROMapping_MRank_RankId",
                table: "OROMapping");

            migrationBuilder.DropIndex(
                name: "IX_OROMapping_RankId",
                table: "OROMapping");

            migrationBuilder.DropColumn(
                name: "RankId",
                table: "OROMapping");

            migrationBuilder.AlterColumn<string>(
                name: "ArmedIdList",
                table: "OROMapping",
                type: "varchar(100)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldNullable: true);
        }
    }
}
