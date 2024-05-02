using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v86 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DialingCode",
                table: "UserProfile");

            migrationBuilder.DropColumn(
                name: "Extension",
                table: "UserProfile");

            migrationBuilder.DropColumn(
                name: "IsCO",
                table: "UserProfile");

            migrationBuilder.DropColumn(
                name: "IsIO",
                table: "UserProfile");

            migrationBuilder.DropColumn(
                name: "IsORO",
                table: "UserProfile");

            migrationBuilder.DropColumn(
                name: "IsRO",
                table: "UserProfile");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DialingCode",
                table: "UserProfile",
                type: "varchar(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Extension",
                table: "UserProfile",
                type: "varchar(5)",
                maxLength: 5,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsCO",
                table: "UserProfile",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsIO",
                table: "UserProfile",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsORO",
                table: "UserProfile",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsRO",
                table: "UserProfile",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
