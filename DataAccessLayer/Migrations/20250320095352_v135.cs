using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v135 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateTable(
                name: "TrnFaultyCard",
                columns: table => new
                {
                    TrnFaultyCardId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RemarksIds = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    OtherRemark = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    FaultyStageId = table.Column<byte>(type: "tinyint", nullable: false),
                    RequestId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrnFaultyCard", x => x.TrnFaultyCardId);
                    table.ForeignKey(
                        name: "FK_TrnFaultyCard_AspNetUsers_Updatedby",
                        column: x => x.Updatedby,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrnFaultyCard_MFaultyStage_FaultyStageId",
                        column: x => x.FaultyStageId,
                        principalTable: "MFaultyStage",
                        principalColumn: "FaultyStageId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrnFaultyCard_TrnICardRequest_RequestId",
                        column: x => x.RequestId,
                        principalTable: "TrnICardRequest",
                        principalColumn: "RequestId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TrnFaultyCard_FaultyStageId",
                table: "TrnFaultyCard",
                column: "FaultyStageId");

            migrationBuilder.CreateIndex(
                name: "IX_TrnFaultyCard_RequestId",
                table: "TrnFaultyCard",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_TrnFaultyCard_Updatedby",
                table: "TrnFaultyCard",
                column: "Updatedby");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrnFaultyCard");

            migrationBuilder.DropTable(
                name: "MFaultyStage");
        }
    }
}
