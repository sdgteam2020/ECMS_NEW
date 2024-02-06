using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v50 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MApiDataOffrs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplyForId = table.Column<int>(type: "int", nullable: true),
                    Pers_Army_No = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pers_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pers_Rank = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pers_Father_Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pers_birth_dt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pers_enrol_dt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pers_District = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pers_State = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pers_Regt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pers_Height = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pers_UID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pers_Blood_Gp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pers_House_no = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pers_Moh_st = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pers_Village = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pers_Tehsil = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pers_Post_office = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pers_Police_stn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pers_Pin_code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pers_Iden_mark_1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pers_Iden_mark_2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pers_Gender = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MApiDataOffrs", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MApiDataOffrs");
        }
    }
}
