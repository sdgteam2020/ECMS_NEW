using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v16 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MappedBy",
                table: "TrnDomainMapping",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "MappedDate",
                table: "TrnDomainMapping",
                type: "datetime",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MappedBy",
                table: "TrnDomainMapping");

            migrationBuilder.DropColumn(
                name: "MappedDate",
                table: "TrnDomainMapping");
        }
    }
}
