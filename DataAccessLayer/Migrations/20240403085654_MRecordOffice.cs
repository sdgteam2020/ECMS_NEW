using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class MRecordOffice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MRecordOffice",
                columns: table => new
                {
                    RecordOfficeId = table.Column<byte>(type: "tinyint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    Abbreviation = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false),
                    ArmedId = table.Column<byte>(type: "tinyint", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MRecordOffice", x => x.RecordOfficeId);
                    table.ForeignKey(
                        name: "FK_MRecordOffice_AspNetUsers_Updatedby",
                        column: x => x.Updatedby,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MRecordOffice_MArmedType_ArmedId",
                        column: x => x.ArmedId,
                        principalTable: "MArmedType",
                        principalColumn: "ArmedId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MRecordOffice_ArmedId",
                table: "MRecordOffice",
                column: "ArmedId");

            migrationBuilder.CreateIndex(
                name: "IX_MRecordOffice_Updatedby",
                table: "MRecordOffice",
                column: "Updatedby");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MRecordOffice");
        }
    }
}
