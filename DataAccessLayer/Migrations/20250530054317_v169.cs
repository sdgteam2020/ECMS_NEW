using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v169 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TrnDestructionCards",
                columns: table => new
                {
                    DestructedCardId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestId = table.Column<int>(type: "int", nullable: false),
                    RemarksIds = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    DestructedOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    Remark = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    UpdatedbyUserId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrnDestructionCards", x => x.DestructedCardId);
                    table.ForeignKey(
                        name: "FK_TrnDestructionCards_AspNetUsers_Updatedby",
                        column: x => x.Updatedby,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrnDestructionCards_TrnICardRequest_RequestId",
                        column: x => x.RequestId,
                        principalTable: "TrnICardRequest",
                        principalColumn: "RequestId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrnDestructionCards_UserProfile_UpdatedbyUserId",
                        column: x => x.UpdatedbyUserId,
                        principalTable: "UserProfile",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TrnDestructionCards_RequestId",
                table: "TrnDestructionCards",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_TrnDestructionCards_Updatedby",
                table: "TrnDestructionCards",
                column: "Updatedby");

            migrationBuilder.CreateIndex(
                name: "IX_TrnDestructionCards_UpdatedbyUserId",
                table: "TrnDestructionCards",
                column: "UpdatedbyUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrnDestructionCards");
        }
    }
}
