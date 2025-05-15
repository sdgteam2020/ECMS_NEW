using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v161 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrnMapUnitChangeRequest_AspNetUsers_ApproverUpdatedby",
                table: "TrnMapUnitChangeRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_TrnMapUnitChangeRequest_UserProfile_ApproverUserId",
                table: "TrnMapUnitChangeRequest");

            migrationBuilder.RenameColumn(
                name: "ApproverUserId",
                table: "TrnMapUnitChangeRequest",
                newName: "AdminUserId");

            migrationBuilder.RenameColumn(
                name: "ApproverUpdatedby",
                table: "TrnMapUnitChangeRequest",
                newName: "AdminUpdatedby");

            migrationBuilder.RenameColumn(
                name: "ApproverUpdatedOn",
                table: "TrnMapUnitChangeRequest",
                newName: "AdminUpdatedOn");

            migrationBuilder.RenameIndex(
                name: "IX_TrnMapUnitChangeRequest_ApproverUserId",
                table: "TrnMapUnitChangeRequest",
                newName: "IX_TrnMapUnitChangeRequest_AdminUserId");

            migrationBuilder.RenameIndex(
                name: "IX_TrnMapUnitChangeRequest_ApproverUpdatedby",
                table: "TrnMapUnitChangeRequest",
                newName: "IX_TrnMapUnitChangeRequest_AdminUpdatedby");

            migrationBuilder.AddForeignKey(
                name: "FK_TrnMapUnitChangeRequest_AspNetUsers_AdminUpdatedby",
                table: "TrnMapUnitChangeRequest",
                column: "AdminUpdatedby",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TrnMapUnitChangeRequest_UserProfile_AdminUserId",
                table: "TrnMapUnitChangeRequest",
                column: "AdminUserId",
                principalTable: "UserProfile",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrnMapUnitChangeRequest_AspNetUsers_AdminUpdatedby",
                table: "TrnMapUnitChangeRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_TrnMapUnitChangeRequest_UserProfile_AdminUserId",
                table: "TrnMapUnitChangeRequest");

            migrationBuilder.RenameColumn(
                name: "AdminUserId",
                table: "TrnMapUnitChangeRequest",
                newName: "ApproverUserId");

            migrationBuilder.RenameColumn(
                name: "AdminUpdatedby",
                table: "TrnMapUnitChangeRequest",
                newName: "ApproverUpdatedby");

            migrationBuilder.RenameColumn(
                name: "AdminUpdatedOn",
                table: "TrnMapUnitChangeRequest",
                newName: "ApproverUpdatedOn");

            migrationBuilder.RenameIndex(
                name: "IX_TrnMapUnitChangeRequest_AdminUserId",
                table: "TrnMapUnitChangeRequest",
                newName: "IX_TrnMapUnitChangeRequest_ApproverUserId");

            migrationBuilder.RenameIndex(
                name: "IX_TrnMapUnitChangeRequest_AdminUpdatedby",
                table: "TrnMapUnitChangeRequest",
                newName: "IX_TrnMapUnitChangeRequest_ApproverUpdatedby");

            migrationBuilder.AddForeignKey(
                name: "FK_TrnMapUnitChangeRequest_AspNetUsers_ApproverUpdatedby",
                table: "TrnMapUnitChangeRequest",
                column: "ApproverUpdatedby",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TrnMapUnitChangeRequest_UserProfile_ApproverUserId",
                table: "TrnMapUnitChangeRequest",
                column: "ApproverUserId",
                principalTable: "UserProfile",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
