using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v29 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FromDomainId",
                table: "TrnPostingOut");

            migrationBuilder.RenameColumn(
                name: "ToDomainId",
                table: "TrnPostingOut",
                newName: "ToAspNetUsersId");

            migrationBuilder.RenameColumn(
                name: "SOSDate",
                table: "TrnPostingOut",
                newName: "FromAspNetUsersId");

            migrationBuilder.AlterColumn<string>(
                name: "Authority",
                table: "TrnPostingOut",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_TrnPostingOut_FromAspNetUsersId",
                table: "TrnPostingOut",
                column: "FromAspNetUsersId");

            migrationBuilder.CreateIndex(
                name: "IX_TrnPostingOut_FromUnitID",
                table: "TrnPostingOut",
                column: "FromUnitID");

            migrationBuilder.CreateIndex(
                name: "IX_TrnPostingOut_FromUserID",
                table: "TrnPostingOut",
                column: "FromUserID");

            migrationBuilder.CreateIndex(
                name: "IX_TrnPostingOut_ToAspNetUsersId",
                table: "TrnPostingOut",
                column: "ToAspNetUsersId");

            migrationBuilder.CreateIndex(
                name: "IX_TrnPostingOut_ToUnitID",
                table: "TrnPostingOut",
                column: "ToUnitID");

            migrationBuilder.CreateIndex(
                name: "IX_TrnPostingOut_ToUserID",
                table: "TrnPostingOut",
                column: "ToUserID");

            migrationBuilder.AddForeignKey(
                name: "FK_TrnPostingOut_AspNetUsers_FromAspNetUsersId",
                table: "TrnPostingOut",
                column: "FromAspNetUsersId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TrnPostingOut_AspNetUsers_ToAspNetUsersId",
                table: "TrnPostingOut",
                column: "ToAspNetUsersId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TrnPostingOut_MapUnit_FromUnitID",
                table: "TrnPostingOut",
                column: "FromUnitID",
                principalTable: "MapUnit",
                principalColumn: "UnitMapId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TrnPostingOut_MapUnit_ToUnitID",
                table: "TrnPostingOut",
                column: "ToUnitID",
                principalTable: "MapUnit",
                principalColumn: "UnitMapId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TrnPostingOut_UserProfile_FromUserID",
                table: "TrnPostingOut",
                column: "FromUserID",
                principalTable: "UserProfile",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TrnPostingOut_UserProfile_ToUserID",
                table: "TrnPostingOut",
                column: "ToUserID",
                principalTable: "UserProfile",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrnPostingOut_AspNetUsers_FromAspNetUsersId",
                table: "TrnPostingOut");

            migrationBuilder.DropForeignKey(
                name: "FK_TrnPostingOut_AspNetUsers_ToAspNetUsersId",
                table: "TrnPostingOut");

            migrationBuilder.DropForeignKey(
                name: "FK_TrnPostingOut_MapUnit_FromUnitID",
                table: "TrnPostingOut");

            migrationBuilder.DropForeignKey(
                name: "FK_TrnPostingOut_MapUnit_ToUnitID",
                table: "TrnPostingOut");

            migrationBuilder.DropForeignKey(
                name: "FK_TrnPostingOut_UserProfile_FromUserID",
                table: "TrnPostingOut");

            migrationBuilder.DropForeignKey(
                name: "FK_TrnPostingOut_UserProfile_ToUserID",
                table: "TrnPostingOut");

            migrationBuilder.DropIndex(
                name: "IX_TrnPostingOut_FromAspNetUsersId",
                table: "TrnPostingOut");

            migrationBuilder.DropIndex(
                name: "IX_TrnPostingOut_FromUnitID",
                table: "TrnPostingOut");

            migrationBuilder.DropIndex(
                name: "IX_TrnPostingOut_FromUserID",
                table: "TrnPostingOut");

            migrationBuilder.DropIndex(
                name: "IX_TrnPostingOut_ToAspNetUsersId",
                table: "TrnPostingOut");

            migrationBuilder.DropIndex(
                name: "IX_TrnPostingOut_ToUnitID",
                table: "TrnPostingOut");

            migrationBuilder.DropIndex(
                name: "IX_TrnPostingOut_ToUserID",
                table: "TrnPostingOut");

            migrationBuilder.RenameColumn(
                name: "ToAspNetUsersId",
                table: "TrnPostingOut",
                newName: "ToDomainId");

            migrationBuilder.RenameColumn(
                name: "FromAspNetUsersId",
                table: "TrnPostingOut",
                newName: "SOSDate");

            migrationBuilder.AlterColumn<int>(
                name: "Authority",
                table: "TrnPostingOut",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "FromDomainId",
                table: "TrnPostingOut",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
