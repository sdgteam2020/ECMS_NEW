using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v133 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IAMSetting");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IAMSetting",
                columns: table => new
                {
                    IAMSettingId = table.Column<byte>(type: "tinyint", nullable: false),
                    DebugWithIAM = table.Column<bool>(type: "bit", nullable: false),
                    HardSAMLResonoce = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LocalHostActive = table.Column<byte>(type: "tinyint", nullable: false),
                    WithIAMLogin = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IAMSetting", x => x.IAMSettingId);
                });
        }
    }
}
