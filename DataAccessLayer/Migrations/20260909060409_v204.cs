using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v204 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DialingCode",
                table: "TrnUnregdUser");

            migrationBuilder.DropColumn(
                name: "Extension",
                table: "TrnUnregdUser");

            migrationBuilder.DropColumn(
                name: "MobileNo",
                table: "TrnUnregdUser");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DialingCode",
                table: "TrnUnregdUser",
                type: "varchar(6)",
                maxLength: 6,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Extension",
                table: "TrnUnregdUser",
                type: "varchar(5)",
                maxLength: 5,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MobileNo",
                table: "TrnUnregdUser",
                type: "varchar(10)",
                maxLength: 10,
                nullable: true);
        }
    }
}
