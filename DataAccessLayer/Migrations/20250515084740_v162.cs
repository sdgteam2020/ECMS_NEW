using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v162 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFIRLogged",
                table: "TrnLostCards",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SignedXML",
                table: "TrnLostCards",
                type: "NVARCHAR(MAX)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SupportDocPath",
                table: "TrnLostCards",
                type: "VARCHAR(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFIRLogged",
                table: "TrnLostCards");

            migrationBuilder.DropColumn(
                name: "SignedXML",
                table: "TrnLostCards");

            migrationBuilder.DropColumn(
                name: "SupportDocPath",
                table: "TrnLostCards");
        }
    }
}
