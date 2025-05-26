using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class _165 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CompletedICardRequests",
                columns: table => new
                {
                    CompletedId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestId = table.Column<int>(type: "int", nullable: false),
                    CardRequestHistoryJson = table.Column<string>(type: "varchar(max)", nullable: false),
                    UpdatedbyUserId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompletedICardRequests", x => x.CompletedId);
                    table.ForeignKey(
                        name: "FK_CompletedICardRequests_AspNetUsers_Updatedby",
                        column: x => x.Updatedby,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompletedICardRequests_TrnICardRequest_RequestId",
                        column: x => x.RequestId,
                        principalTable: "TrnICardRequest",
                        principalColumn: "RequestId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompletedICardRequests_UserProfile_UpdatedbyUserId",
                        column: x => x.UpdatedbyUserId,
                        principalTable: "UserProfile",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompletedICardRequests_RequestId",
                table: "CompletedICardRequests",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_CompletedICardRequests_Updatedby",
                table: "CompletedICardRequests",
                column: "Updatedby");

            migrationBuilder.CreateIndex(
                name: "IX_CompletedICardRequests_UpdatedbyUserId",
                table: "CompletedICardRequests",
                column: "UpdatedbyUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropTable(
                name: "CompletedICardRequests");
        }
    }
}
