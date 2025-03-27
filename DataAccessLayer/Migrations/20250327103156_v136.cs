using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v136 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrnFaultyCard_MFaultyStage_FaultyStageId",
                table: "TrnFaultyCard");

            migrationBuilder.DropTable(
                name: "MFaultyStage");

            migrationBuilder.DropIndex(
                name: "IX_TrnFaultyCard_FaultyStageId",
                table: "TrnFaultyCard");

            migrationBuilder.RenameColumn(
                name: "OtherRemark",
                table: "TrnFaultyCard",
                newName: "ToRemark");

            migrationBuilder.RenameColumn(
                name: "FaultyStageId",
                table: "TrnFaultyCard",
                newName: "CategoryId");

            migrationBuilder.AddColumn<string>(
                name: "FromRemark",
                table: "TrnFaultyCard",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "MFaultyStageCategoryId",
                table: "TrnFaultyCard",
                type: "tinyint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MCategory",
                columns: table => new
                {
                    CategoryId = table.Column<byte>(type: "tinyint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MCategory", x => x.CategoryId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TrnFaultyCard_MFaultyStageCategoryId",
                table: "TrnFaultyCard",
                column: "MFaultyStageCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_TrnFaultyCard_MCategory_MFaultyStageCategoryId",
                table: "TrnFaultyCard",
                column: "MFaultyStageCategoryId",
                principalTable: "MCategory",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrnFaultyCard_MCategory_MFaultyStageCategoryId",
                table: "TrnFaultyCard");

            migrationBuilder.DropTable(
                name: "MCategory");

            migrationBuilder.DropIndex(
                name: "IX_TrnFaultyCard_MFaultyStageCategoryId",
                table: "TrnFaultyCard");

            migrationBuilder.DropColumn(
                name: "FromRemark",
                table: "TrnFaultyCard");

            migrationBuilder.DropColumn(
                name: "MFaultyStageCategoryId",
                table: "TrnFaultyCard");

            migrationBuilder.RenameColumn(
                name: "ToRemark",
                table: "TrnFaultyCard",
                newName: "OtherRemark");

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "TrnFaultyCard",
                newName: "FaultyStageId");

            migrationBuilder.CreateTable(
                name: "MFaultyStage",
                columns: table => new
                {
                    FaultyStageId = table.Column<byte>(type: "tinyint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MFaultyStage", x => x.FaultyStageId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TrnFaultyCard_FaultyStageId",
                table: "TrnFaultyCard",
                column: "FaultyStageId");

            migrationBuilder.AddForeignKey(
                name: "FK_TrnFaultyCard_MFaultyStage_FaultyStageId",
                table: "TrnFaultyCard",
                column: "FaultyStageId",
                principalTable: "MFaultyStage",
                principalColumn: "FaultyStageId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
