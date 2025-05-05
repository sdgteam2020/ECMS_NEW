using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v157 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DispatchUpdatedBy",
                table: "TrnPostingOut",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DispatchUpdatedOn",
                table: "TrnPostingOut",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DispatchedOn",
                table: "TrnPostingOut",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RefNo",
                table: "TrnPostingOut",
                type: "varchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Remark",
                table: "TrnMapUnitChangeRequest",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "AdminRemark",
                table: "TrnMapUnitChangeRequest",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "TrnLostCards",
                columns: table => new
                {
                    LostCardId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestId = table.Column<int>(type: "int", nullable: false),
                    Remark = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    LostOn = table.Column<DateTime>(type: "datetime", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrnLostCards", x => x.LostCardId);
                    table.ForeignKey(
                        name: "FK_TrnLostCards_AspNetUsers_Updatedby",
                        column: x => x.Updatedby,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrnLostCards_TrnICardRequest_RequestId",
                        column: x => x.RequestId,
                        principalTable: "TrnICardRequest",
                        principalColumn: "RequestId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TrnLostCards_RequestId",
                table: "TrnLostCards",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_TrnLostCards_Updatedby",
                table: "TrnLostCards",
                column: "Updatedby");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrnLostCards");

            migrationBuilder.DropColumn(
                name: "DispatchUpdatedBy",
                table: "TrnPostingOut");

            migrationBuilder.DropColumn(
                name: "DispatchUpdatedOn",
                table: "TrnPostingOut");

            migrationBuilder.DropColumn(
                name: "DispatchedOn",
                table: "TrnPostingOut");

            migrationBuilder.DropColumn(
                name: "RefNo",
                table: "TrnPostingOut");

            migrationBuilder.AlterColumn<string>(
                name: "Remark",
                table: "TrnMapUnitChangeRequest",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "AdminRemark",
                table: "TrnMapUnitChangeRequest",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldNullable: true);
        }
    }
}
