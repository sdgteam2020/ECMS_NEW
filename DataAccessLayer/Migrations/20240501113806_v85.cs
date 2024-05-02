using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v85 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DialingCode",
                table: "TrnDomainMapping",
                type: "varchar(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Extension",
                table: "TrnDomainMapping",
                type: "varchar(5)",
                maxLength: 5,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsCO",
                table: "TrnDomainMapping",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsIO",
                table: "TrnDomainMapping",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsORO",
                table: "TrnDomainMapping",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsRO",
                table: "TrnDomainMapping",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DialingCode",
                table: "TrnDomainMapping");

            migrationBuilder.DropColumn(
                name: "Extension",
                table: "TrnDomainMapping");

            migrationBuilder.DropColumn(
                name: "IsCO",
                table: "TrnDomainMapping");

            migrationBuilder.DropColumn(
                name: "IsIO",
                table: "TrnDomainMapping");

            migrationBuilder.DropColumn(
                name: "IsORO",
                table: "TrnDomainMapping");

            migrationBuilder.DropColumn(
                name: "IsRO",
                table: "TrnDomainMapping");
        }
    }
}
