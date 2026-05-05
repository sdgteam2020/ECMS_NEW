using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class CreateMArmyPrefixRuleTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MArmyPrefixRule",
                columns: table => new
                {
                    Id = table.Column<byte>(type: "tinyint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Prefix = table.Column<string>(type: "varchar(3)", maxLength: 3, nullable: false),
                    ApplyForId = table.Column<byte>(type: "tinyint", nullable: false),
                    MinDigits = table.Column<byte>(type: "tinyint", nullable: false),
                    MaxDigits = table.Column<byte>(type: "tinyint", nullable: false),
                    Order = table.Column<byte>(type: "tinyint", nullable: false),
                    StorePrefix = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MArmyPrefixRule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MArmyPrefixRule_AspNetUsers_Updatedby",
                        column: x => x.Updatedby,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MArmyPrefixRule_Updatedby",
                table: "MArmyPrefixRule",
                column: "Updatedby");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MArmyPrefixRule");
        }
    }
}
