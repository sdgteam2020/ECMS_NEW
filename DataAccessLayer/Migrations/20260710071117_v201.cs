using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v201 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RankAbbreviation",
                table: "TrnApplClose");

            migrationBuilder.DropColumn(
                name: "RankAbbreviation",
                table: "CompletedICardRequests");

            migrationBuilder.AddColumn<byte>(
                name: "ApplyForId",
                table: "TrnApplClose",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<short>(
                name: "RankId",
                table: "TrnApplClose",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<byte>(
                name: "ApplyForId",
                table: "CompletedICardRequests",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<short>(
                name: "RankId",
                table: "CompletedICardRequests",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.CreateIndex(
                name: "IX_TrnApplClose_ApplyForId",
                table: "TrnApplClose",
                column: "ApplyForId");

            migrationBuilder.CreateIndex(
                name: "IX_TrnApplClose_RankId",
                table: "TrnApplClose",
                column: "RankId");

            migrationBuilder.CreateIndex(
                name: "IX_CompletedICardRequests_ApplyForId",
                table: "CompletedICardRequests",
                column: "ApplyForId");

            migrationBuilder.CreateIndex(
                name: "IX_CompletedICardRequests_RankId",
                table: "CompletedICardRequests",
                column: "RankId");

            migrationBuilder.AddForeignKey(
                name: "FK_CompletedICardRequests_MApplyFor_ApplyForId",
                table: "CompletedICardRequests",
                column: "ApplyForId",
                principalTable: "MApplyFor",
                principalColumn: "ApplyForId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CompletedICardRequests_MRank_RankId",
                table: "CompletedICardRequests",
                column: "RankId",
                principalTable: "MRank",
                principalColumn: "RankId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TrnApplClose_MApplyFor_ApplyForId",
                table: "TrnApplClose",
                column: "ApplyForId",
                principalTable: "MApplyFor",
                principalColumn: "ApplyForId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TrnApplClose_MRank_RankId",
                table: "TrnApplClose",
                column: "RankId",
                principalTable: "MRank",
                principalColumn: "RankId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompletedICardRequests_MApplyFor_ApplyForId",
                table: "CompletedICardRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_CompletedICardRequests_MRank_RankId",
                table: "CompletedICardRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_TrnApplClose_MApplyFor_ApplyForId",
                table: "TrnApplClose");

            migrationBuilder.DropForeignKey(
                name: "FK_TrnApplClose_MRank_RankId",
                table: "TrnApplClose");

            migrationBuilder.DropIndex(
                name: "IX_TrnApplClose_ApplyForId",
                table: "TrnApplClose");

            migrationBuilder.DropIndex(
                name: "IX_TrnApplClose_RankId",
                table: "TrnApplClose");

            migrationBuilder.DropIndex(
                name: "IX_CompletedICardRequests_ApplyForId",
                table: "CompletedICardRequests");

            migrationBuilder.DropIndex(
                name: "IX_CompletedICardRequests_RankId",
                table: "CompletedICardRequests");

            migrationBuilder.DropColumn(
                name: "ApplyForId",
                table: "TrnApplClose");

            migrationBuilder.DropColumn(
                name: "RankId",
                table: "TrnApplClose");

            migrationBuilder.DropColumn(
                name: "ApplyForId",
                table: "CompletedICardRequests");

            migrationBuilder.DropColumn(
                name: "RankId",
                table: "CompletedICardRequests");

            migrationBuilder.AddColumn<string>(
                name: "RankAbbreviation",
                table: "TrnApplClose",
                type: "varchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RankAbbreviation",
                table: "CompletedICardRequests",
                type: "varchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");
        }
    }
}
