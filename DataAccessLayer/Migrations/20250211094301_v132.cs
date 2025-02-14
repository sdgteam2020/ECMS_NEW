using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v132 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "accessKey",
                table: "MApiLogin",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "IAMSetting",
                columns: table => new
                {
                    IAMSettingId = table.Column<byte>(type: "tinyint", nullable: false),
                    WithIAMLogin = table.Column<bool>(type: "bit", nullable: false),
                    DebugWithIAM = table.Column<bool>(type: "bit", nullable: false),
                    LocalHostActive = table.Column<byte>(type: "tinyint", nullable: false),
                    HardSAMLResonoce = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IAMSetting", x => x.IAMSettingId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IAMSetting");

            migrationBuilder.DropColumn(
                name: "accessKey",
                table: "MApiLogin");
        }
    }
}
