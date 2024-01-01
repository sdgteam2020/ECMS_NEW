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
            migrationBuilder.DropForeignKey(
                name: "FK_MRemarks_MRemarksApply_RemarksApplyId",
                table: "MRemarks");

            migrationBuilder.RenameColumn(
                name: "RemarksApplyId",
                table: "MRemarksApply",
                newName: "RemarkApplyId");

            migrationBuilder.RenameColumn(
                name: "RemarksApplyId",
                table: "MRemarks",
                newName: "RemarkApplyId");

            migrationBuilder.RenameIndex(
                name: "IX_MRemarks_RemarksApplyId",
                table: "MRemarks",
                newName: "IX_MRemarks_RemarkApplyId");

            migrationBuilder.AddForeignKey(
                name: "FK_MRemarks_MRemarksApply_RemarkApplyId",
                table: "MRemarks",
                column: "RemarkApplyId",
                principalTable: "MRemarksApply",
                principalColumn: "RemarkApplyId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MRemarks_MRemarksApply_RemarkApplyId",
                table: "MRemarks");

            migrationBuilder.RenameColumn(
                name: "RemarkApplyId",
                table: "MRemarksApply",
                newName: "RemarksApplyId");

            migrationBuilder.RenameColumn(
                name: "RemarkApplyId",
                table: "MRemarks",
                newName: "RemarksApplyId");

            migrationBuilder.RenameIndex(
                name: "IX_MRemarks_RemarkApplyId",
                table: "MRemarks",
                newName: "IX_MRemarks_RemarksApplyId");

            migrationBuilder.AddForeignKey(
                name: "FK_MRemarks_MRemarksApply_RemarksApplyId",
                table: "MRemarks",
                column: "RemarksApplyId",
                principalTable: "MRemarksApply",
                principalColumn: "RemarksApplyId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
