using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v184 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NotificationTypeId",
                table: "TrnNotification");

            migrationBuilder.AddColumn<byte>(
                name: "StepId",
                table: "TrnNotification",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.CreateIndex(
                name: "IX_TrnNotification_ReciverAspNetUsersId",
                table: "TrnNotification",
                column: "ReciverAspNetUsersId");

            migrationBuilder.CreateIndex(
                name: "IX_TrnNotification_SentAspNetUsersId",
                table: "TrnNotification",
                column: "SentAspNetUsersId");

            migrationBuilder.CreateIndex(
                name: "IX_TrnNotification_StepId",
                table: "TrnNotification",
                column: "StepId");

            migrationBuilder.AddForeignKey(
                name: "FK_TrnNotification_AspNetUsers_ReciverAspNetUsersId",
                table: "TrnNotification",
                column: "ReciverAspNetUsersId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TrnNotification_AspNetUsers_SentAspNetUsersId",
                table: "TrnNotification",
                column: "SentAspNetUsersId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TrnNotification_MStepCounterStep_StepId",
                table: "TrnNotification",
                column: "StepId",
                principalTable: "MStepCounterStep",
                principalColumn: "StepId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrnNotification_AspNetUsers_ReciverAspNetUsersId",
                table: "TrnNotification");

            migrationBuilder.DropForeignKey(
                name: "FK_TrnNotification_AspNetUsers_SentAspNetUsersId",
                table: "TrnNotification");

            migrationBuilder.DropForeignKey(
                name: "FK_TrnNotification_MStepCounterStep_StepId",
                table: "TrnNotification");

            migrationBuilder.DropIndex(
                name: "IX_TrnNotification_ReciverAspNetUsersId",
                table: "TrnNotification");

            migrationBuilder.DropIndex(
                name: "IX_TrnNotification_SentAspNetUsersId",
                table: "TrnNotification");

            migrationBuilder.DropIndex(
                name: "IX_TrnNotification_StepId",
                table: "TrnNotification");

            migrationBuilder.DropColumn(
                name: "StepId",
                table: "TrnNotification");

            migrationBuilder.AddColumn<int>(
                name: "NotificationTypeId",
                table: "TrnNotification",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
