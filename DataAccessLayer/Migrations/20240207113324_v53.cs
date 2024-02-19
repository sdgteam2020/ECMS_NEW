using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v53 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Authority",
                table: "TrnPostingOut",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Remark",
                table: "TrnFwds",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Village",
                table: "BasicDetailTemps",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Tehsil",
                table: "BasicDetailTemps",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "State",
                table: "BasicDetailTemps",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ServiceNo",
                table: "BasicDetailTemps",
                type: "varchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "PS",
                table: "BasicDetailTemps",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PO",
                table: "BasicDetailTemps",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Observations",
                table: "BasicDetailTemps",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "BasicDetailTemps",
                type: "varchar(36)",
                maxLength: 36,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(36)",
                oldMaxLength: 36);

            migrationBuilder.AlterColumn<string>(
                name: "District",
                table: "BasicDetailTemps",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

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

            migrationBuilder.AlterColumn<string>(
                name: "Authority",
                table: "TrnPostingOut",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Remark",
                table: "TrnFwds",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Village",
                table: "BasicDetailTemps",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Tehsil",
                table: "BasicDetailTemps",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "State",
                table: "BasicDetailTemps",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ServiceNo",
                table: "BasicDetailTemps",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "PS",
                table: "BasicDetailTemps",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PO",
                table: "BasicDetailTemps",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Observations",
                table: "BasicDetailTemps",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "BasicDetailTemps",
                type: "nvarchar(36)",
                maxLength: 36,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(36)",
                oldMaxLength: 36);

            migrationBuilder.AlterColumn<string>(
                name: "District",
                table: "BasicDetailTemps",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true);
        }
    }
}
