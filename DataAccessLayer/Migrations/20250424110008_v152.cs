using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v152 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TrnHotlistCards",
                columns: table => new
                {
                    HotlistCardId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestId = table.Column<int>(type: "int", nullable: false),
                    RemarksIds = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    Remark = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrnHotlistCards", x => x.HotlistCardId);
                    table.ForeignKey(
                        name: "FK_TrnHotlistCards_AspNetUsers_Updatedby",
                        column: x => x.Updatedby,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrnHotlistCards_TrnICardRequest_RequestId",
                        column: x => x.RequestId,
                        principalTable: "TrnICardRequest",
                        principalColumn: "RequestId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TrnMapUnitChangeRequest",
                columns: table => new
                {
                    ChangeMapUnitId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UnitMapId = table.Column<int>(type: "int", nullable: false),
                    UnitId = table.Column<int>(type: "int", nullable: false),
                    FromUnit = table.Column<string>(type: "varchar(500)", nullable: false),
                    ToUnit = table.Column<string>(type: "varchar(100)", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AdminRemark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsComplete = table.Column<bool>(type: "bit", nullable: false),
                    FromUserId = table.Column<int>(type: "int", nullable: false),
                    ToUpdatedby = table.Column<int>(type: "int", nullable: true),
                    ToUpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ToUserId = table.Column<int>(type: "int", nullable: false),
                    IsEditAction = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrnMapUnitChangeRequest", x => x.ChangeMapUnitId);
                    table.ForeignKey(
                        name: "FK_TrnMapUnitChangeRequest_AspNetUsers_ToUpdatedby",
                        column: x => x.ToUpdatedby,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrnMapUnitChangeRequest_AspNetUsers_Updatedby",
                        column: x => x.Updatedby,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrnMapUnitChangeRequest_MUnit_UnitId",
                        column: x => x.UnitId,
                        principalTable: "MUnit",
                        principalColumn: "UnitId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrnMapUnitChangeRequest_MapUnit_UnitMapId",
                        column: x => x.UnitMapId,
                        principalTable: "MapUnit",
                        principalColumn: "UnitMapId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrnMapUnitChangeRequest_UserProfile_FromUserId",
                        column: x => x.FromUserId,
                        principalTable: "UserProfile",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrnMapUnitChangeRequest_UserProfile_ToUserId",
                        column: x => x.ToUserId,
                        principalTable: "UserProfile",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TrnHotlistCards_RequestId",
                table: "TrnHotlistCards",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_TrnHotlistCards_Updatedby",
                table: "TrnHotlistCards",
                column: "Updatedby");

            migrationBuilder.CreateIndex(
                name: "IX_TrnMapUnitChangeRequest_FromUserId",
                table: "TrnMapUnitChangeRequest",
                column: "FromUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TrnMapUnitChangeRequest_ToUpdatedby",
                table: "TrnMapUnitChangeRequest",
                column: "ToUpdatedby");

            migrationBuilder.CreateIndex(
                name: "IX_TrnMapUnitChangeRequest_ToUserId",
                table: "TrnMapUnitChangeRequest",
                column: "ToUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TrnMapUnitChangeRequest_UnitId",
                table: "TrnMapUnitChangeRequest",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_TrnMapUnitChangeRequest_UnitMapId",
                table: "TrnMapUnitChangeRequest",
                column: "UnitMapId");

            migrationBuilder.CreateIndex(
                name: "IX_TrnMapUnitChangeRequest_Updatedby",
                table: "TrnMapUnitChangeRequest",
                column: "Updatedby");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrnHotlistCards");

            migrationBuilder.DropTable(
                name: "TrnMapUnitChangeRequest");
        }
    }
}
