using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v80 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "MPostingReason",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "TrnApplClose",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BasicDetailId = table.Column<int>(type: "int", nullable: false),
                    ReasonId = table.Column<byte>(type: "tinyint", nullable: false),
                    Authority = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    Remarks = table.Column<string>(type: "varchar(50)", nullable: false),
                    RequestId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrnApplClose", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrnApplClose_AspNetUsers_Updatedby",
                        column: x => x.Updatedby,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrnApplClose_BasicDetails_BasicDetailId",
                        column: x => x.BasicDetailId,
                        principalTable: "BasicDetails",
                        principalColumn: "BasicDetailId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrnApplClose_MPostingReason_ReasonId",
                        column: x => x.ReasonId,
                        principalTable: "MPostingReason",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrnApplClose_TrnICardRequest_RequestId",
                        column: x => x.RequestId,
                        principalTable: "TrnICardRequest",
                        principalColumn: "RequestId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TrnApplClose_BasicDetailId",
                table: "TrnApplClose",
                column: "BasicDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_TrnApplClose_ReasonId",
                table: "TrnApplClose",
                column: "ReasonId");

            migrationBuilder.CreateIndex(
                name: "IX_TrnApplClose_RequestId",
                table: "TrnApplClose",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_TrnApplClose_Updatedby",
                table: "TrnApplClose",
                column: "Updatedby");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrnApplClose");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "MPostingReason");
        }
    }
}
