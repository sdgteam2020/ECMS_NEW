using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v25 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "UnregdUserId",
                table: "MUnit",
                type: "smallint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TrnUnregdUser",
                columns: table => new
                {
                    UnregdUserId = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    ServiceNo = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false),
                    Rank = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    DomainId = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrnUnregdUser", x => x.UnregdUserId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MUnit_UnregdUserId",
                table: "MUnit",
                column: "UnregdUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_MUnit_TrnUnregdUser_UnregdUserId",
                table: "MUnit",
                column: "UnregdUserId",
                principalTable: "TrnUnregdUser",
                principalColumn: "UnregdUserId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MUnit_TrnUnregdUser_UnregdUserId",
                table: "MUnit");

            migrationBuilder.DropTable(
                name: "TrnUnregdUser");

            migrationBuilder.DropIndex(
                name: "IX_MUnit_UnregdUserId",
                table: "MUnit");

            migrationBuilder.DropColumn(
                name: "UnregdUserId",
                table: "MUnit");
        }
    }
}
