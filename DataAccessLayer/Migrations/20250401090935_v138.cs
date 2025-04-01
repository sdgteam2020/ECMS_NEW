using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v138 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CSVImports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileName = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    TotalRecords = table.Column<int>(type: "int", nullable: false),
                    ValidRecords = table.Column<int>(type: "int", nullable: false),
                    DBUpdated = table.Column<bool>(type: "bit", nullable: false),
                    ImportedBy = table.Column<int>(type: "int", nullable: true),
                    ImportedOn = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CSVImports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CSVImports_AspNetUsers_ImportedBy",
                        column: x => x.ImportedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CSVImports_ImportedBy",
                table: "CSVImports",
                column: "ImportedBy");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CSVImports");
        }
    }
}
