using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v148 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FlagForHotlist",
                table: "TrnICardRequest");

            migrationBuilder.DropColumn(
                name: "FlagForLost",
                table: "TrnICardRequest");

            migrationBuilder.CreateTable(
                name: "TrnHotlistCards",
                columns: table => new
                {
                    TrnFaultyCardId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestId = table.Column<int>(type: "int", nullable: false),
                    RemarksIds = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    Remark = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrnHotlistCards", x => x.TrnFaultyCardId);
                    table.ForeignKey(
                        name: "FK_TrnHotlistCards_AspNetUsers_Updatedby",
                        column: x => x.Updatedby,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrnHotlistCards_TrnICardRequest_RequestId",
                        column: x => x.RequestId,
                        principalTable: "TrnICardRequest",
                        principalColumn: "RequestId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TrnHotlistCards_RequestId",
                table: "TrnHotlistCards",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_TrnHotlistCards_Updatedby",
                table: "TrnHotlistCards",
                column: "Updatedby");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrnHotlistCards");

            migrationBuilder.AddColumn<bool>(
                name: "FlagForHotlist",
                table: "TrnICardRequest",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "FlagForLost",
                table: "TrnICardRequest",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
