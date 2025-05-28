using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v164 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Remark",
                table: "TrnLostCards",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(250)",
                oldMaxLength: 250,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "TrnDistributeCards",
                columns: table => new
                {
                    DistributeCardId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestId = table.Column<int>(type: "int", nullable: false),
                    DistributedOn = table.Column<DateTime>(type: "datetime", nullable: false),
                    Remark = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    UpdatedbyUserId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrnDistributeCards", x => x.DistributeCardId);
                    table.ForeignKey(
                        name: "FK_TrnDistributeCards_AspNetUsers_Updatedby",
                        column: x => x.Updatedby,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrnDistributeCards_TrnICardRequest_RequestId",
                        column: x => x.RequestId,
                        principalTable: "TrnICardRequest",
                        principalColumn: "RequestId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrnDistributeCards_UserProfile_UpdatedbyUserId",
                        column: x => x.UpdatedbyUserId,
                        principalTable: "UserProfile",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TrnDistributeCards_RequestId",
                table: "TrnDistributeCards",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_TrnDistributeCards_Updatedby",
                table: "TrnDistributeCards",
                column: "Updatedby");

            migrationBuilder.CreateIndex(
                name: "IX_TrnDistributeCards_UpdatedbyUserId",
                table: "TrnDistributeCards",
                column: "UpdatedbyUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrnDistributeCards");

            migrationBuilder.AlterColumn<string>(
                name: "Remark",
                table: "TrnLostCards",
                type: "varchar(250)",
                maxLength: 250,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldNullable: true);
        }
    }
}
