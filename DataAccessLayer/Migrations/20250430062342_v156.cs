using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v156 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrnMapUnitChangeRequest_AspNetUsers_ToUpdatedby",
                table: "TrnMapUnitChangeRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_TrnMapUnitChangeRequest_UserProfile_ToUserId",
                table: "TrnMapUnitChangeRequest");

            migrationBuilder.RenameColumn(
                name: "ToUserId",
                table: "TrnMapUnitChangeRequest",
                newName: "ApproverUserId");

            migrationBuilder.RenameColumn(
                name: "ToUpdatedby",
                table: "TrnMapUnitChangeRequest",
                newName: "ApproverUpdatedby");

            migrationBuilder.RenameColumn(
                name: "ToUpdatedOn",
                table: "TrnMapUnitChangeRequest",
                newName: "ApproverUpdatedOn");

            migrationBuilder.RenameColumn(
                name: "ChangeMapUnitId",
                table: "TrnMapUnitChangeRequest",
                newName: "MapUnitChangeRequestId");

            migrationBuilder.RenameIndex(
                name: "IX_TrnMapUnitChangeRequest_ToUserId",
                table: "TrnMapUnitChangeRequest",
                newName: "IX_TrnMapUnitChangeRequest_ApproverUserId");

            migrationBuilder.RenameIndex(
                name: "IX_TrnMapUnitChangeRequest_ToUpdatedby",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                newName: "ToUserId");

            migrationBuilder.RenameColumn(
                name: "ApproverUpdatedby",
                table: "TrnMapUnitChangeRequest",
                newName: "ToUpdatedby");

            migrationBuilder.RenameColumn(
                name: "ApproverUpdatedOn",
                table: "TrnMapUnitChangeRequest",
                newName: "ToUpdatedOn");

            migrationBuilder.RenameColumn(
                name: "MapUnitChangeRequestId",
                table: "TrnMapUnitChangeRequest",
                newName: "ChangeMapUnitId");

            migrationBuilder.RenameIndex(
                name: "IX_TrnMapUnitChangeRequest_ApproverUserId",
                table: "TrnMapUnitChangeRequest",
                newName: "IX_TrnMapUnitChangeRequest_ToUserId");

            migrationBuilder.RenameIndex(
                name: "IX_TrnMapUnitChangeRequest_ApproverUpdatedby",
                table: "TrnMapUnitChangeRequest",
                newName: "IX_TrnMapUnitChangeRequest_ToUpdatedby");

            migrationBuilder.AddForeignKey(
                name: "FK_TrnMapUnitChangeRequest_AspNetUsers_ToUpdatedby",
                table: "TrnMapUnitChangeRequest",
                column: "ToUpdatedby",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TrnMapUnitChangeRequest_UserProfile_ToUserId",
                table: "TrnMapUnitChangeRequest",
                column: "ToUserId",
                principalTable: "UserProfile",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
