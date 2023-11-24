using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TrnFwds",
                columns: table => new
                {
                    TrnFwdId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BasicDetailId = table.Column<int>(type: "int", nullable: false),
                    FromProfileId = table.Column<int>(type: "int", nullable: false),
                    FlagSUS = table.Column<bool>(type: "bit", nullable: false),
                    SusNo = table.Column<int>(type: "int", nullable: false),
                    ToProfileId = table.Column<int>(type: "int", nullable: true),
                    ActionRemark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FlagAccepted = table.Column<bool>(type: "bit", nullable: false),
                    StatusLevel = table.Column<byte>(type: "tinyint", nullable: false),
                    CancelDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrnFwds", x => x.TrnFwdId);
                    table.ForeignKey(
                        name: "FK_TrnFwds_BasicDetails_BasicDetailId",
                        column: x => x.BasicDetailId,
                        principalTable: "BasicDetails",
                        principalColumn: "BasicDetailId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TrnFwds_BasicDetailId",
                table: "TrnFwds",
                column: "BasicDetailId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrnFwds");
        }
    }
}
