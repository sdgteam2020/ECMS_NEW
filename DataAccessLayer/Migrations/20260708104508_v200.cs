using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v200 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CardRequestHistoryJson",
                table: "TrnApplClose",
                type: "varchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "TrnApplClose",
                type: "varchar(36)",
                maxLength: 36,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RankAbbreviation",
                table: "TrnApplClose",
                type: "varchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ServiceNo",
                table: "TrnApplClose",
                type: "varchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "CompletedICardRequests",
                type: "varchar(36)",
                maxLength: 36,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RankAbbreviation",
                table: "CompletedICardRequests",
                type: "varchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ServiceNo",
                table: "CompletedICardRequests",
                type: "varchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "CompletedICardRequestMapping",
                columns: table => new
                {
                    CompletedMappingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompletedId = table.Column<int>(type: "int", nullable: false),
                    AspNetUsersId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompletedICardRequestMapping", x => x.CompletedMappingId);
                    table.ForeignKey(
                        name: "FK_CompletedICardRequestMapping_AspNetUsers_AspNetUsersId",
                        column: x => x.AspNetUsersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompletedICardRequestMapping_CompletedICardRequests_CompletedId",
                        column: x => x.CompletedId,
                        principalTable: "CompletedICardRequests",
                        principalColumn: "CompletedId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TrnApplCloseMapping",
                columns: table => new
                {
                    ApplCloseMappingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CloseId = table.Column<int>(type: "int", nullable: false),
                    AspNetUsersId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrnApplCloseMapping", x => x.ApplCloseMappingId);
                    table.ForeignKey(
                        name: "FK_TrnApplCloseMapping_AspNetUsers_AspNetUsersId",
                        column: x => x.AspNetUsersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrnApplCloseMapping_TrnApplClose_CloseId",
                        column: x => x.CloseId,
                        principalTable: "TrnApplClose",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompletedICardRequestMapping_AspNetUsersId",
                table: "CompletedICardRequestMapping",
                column: "AspNetUsersId");

            migrationBuilder.CreateIndex(
                name: "IX_CompletedICardRequestMapping_CompletedId",
                table: "CompletedICardRequestMapping",
                column: "CompletedId");

            migrationBuilder.CreateIndex(
                name: "IX_TrnApplCloseMapping_AspNetUsersId",
                table: "TrnApplCloseMapping",
                column: "AspNetUsersId");

            migrationBuilder.CreateIndex(
                name: "IX_TrnApplCloseMapping_CloseId",
                table: "TrnApplCloseMapping",
                column: "CloseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompletedICardRequestMapping");

            migrationBuilder.DropTable(
                name: "TrnApplCloseMapping");

            migrationBuilder.DropColumn(
                name: "CardRequestHistoryJson",
                table: "TrnApplClose");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "TrnApplClose");

            migrationBuilder.DropColumn(
                name: "RankAbbreviation",
                table: "TrnApplClose");

            migrationBuilder.DropColumn(
                name: "ServiceNo",
                table: "TrnApplClose");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "CompletedICardRequests");

            migrationBuilder.DropColumn(
                name: "RankAbbreviation",
                table: "CompletedICardRequests");

            migrationBuilder.DropColumn(
                name: "ServiceNo",
                table: "CompletedICardRequests");
        }
    }
}
