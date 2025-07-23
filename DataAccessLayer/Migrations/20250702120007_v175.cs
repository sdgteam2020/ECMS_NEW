using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v175 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TrnDispatchCard",
                columns: table => new
                {
                    DispatchCardId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Step = table.Column<byte>(type: "tinyint", nullable: false),
                    ApplyForId = table.Column<byte>(type: "tinyint", nullable: false),
                    RegId = table.Column<byte>(type: "tinyint", nullable: true),
                    RecordOfficeId = table.Column<byte>(type: "tinyint", nullable: true),
                    OutDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReceiptDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DispatchDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModeOfDispatch = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false),
                    RefOfDispatch = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false),
                    LotNo = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false),
                    NameOfCourierIncharge = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false),
                    UploadFilePath = table.Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: false),
                    FromRemark = table.Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: true),
                    ToRemark = table.Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: true),
                    FromUnitId = table.Column<int>(type: "int", nullable: false),
                    ToUnitId = table.Column<int>(type: "int", nullable: false),
                    ToUserId = table.Column<int>(type: "int", nullable: false),
                    FromUserId = table.Column<int>(type: "int", nullable: false),
                    FromAspNetUsersId = table.Column<int>(type: "int", nullable: false),
                    ToAspNetUsersId = table.Column<int>(type: "int", nullable: false),
                    IsComplete = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrnDispatchCard", x => x.DispatchCardId);
                    table.ForeignKey(
                        name: "FK_TrnDispatchCard_AspNetUsers_FromAspNetUsersId",
                        column: x => x.FromAspNetUsersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrnDispatchCard_AspNetUsers_ToAspNetUsersId",
                        column: x => x.ToAspNetUsersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrnDispatchCard_AspNetUsers_Updatedby",
                        column: x => x.Updatedby,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrnDispatchCard_MApplyFor_ApplyForId",
                        column: x => x.ApplyForId,
                        principalTable: "MApplyFor",
                        principalColumn: "ApplyForId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrnDispatchCard_MRecordOffice_RecordOfficeId",
                        column: x => x.RecordOfficeId,
                        principalTable: "MRecordOffice",
                        principalColumn: "RecordOfficeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrnDispatchCard_MRegimental_RegId",
                        column: x => x.RegId,
                        principalTable: "MRegimental",
                        principalColumn: "RegId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrnDispatchCard_MapUnit_FromUnitId",
                        column: x => x.FromUnitId,
                        principalTable: "MapUnit",
                        principalColumn: "UnitMapId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrnDispatchCard_MapUnit_ToUnitId",
                        column: x => x.ToUnitId,
                        principalTable: "MapUnit",
                        principalColumn: "UnitMapId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrnDispatchCard_UserProfile_FromUserId",
                        column: x => x.FromUserId,
                        principalTable: "UserProfile",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrnDispatchCard_UserProfile_ToUserId",
                        column: x => x.ToUserId,
                        principalTable: "UserProfile",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TrnDispatchCardMapping",
                columns: table => new
                {
                    DispatchCardMappingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DispatchCardId = table.Column<int>(type: "int", nullable: false),
                    ChipNo = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrnDispatchCardMapping", x => x.DispatchCardMappingId);
                    table.ForeignKey(
                        name: "FK_TrnDispatchCardMapping_TrnDispatchCard_DispatchCardId",
                        column: x => x.DispatchCardId,
                        principalTable: "TrnDispatchCard",
                        principalColumn: "DispatchCardId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TrnDispatchCard_ApplyForId",
                table: "TrnDispatchCard",
                column: "ApplyForId");

            migrationBuilder.CreateIndex(
                name: "IX_TrnDispatchCard_FromAspNetUsersId",
                table: "TrnDispatchCard",
                column: "FromAspNetUsersId");

            migrationBuilder.CreateIndex(
                name: "IX_TrnDispatchCard_FromUnitId",
                table: "TrnDispatchCard",
                column: "FromUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_TrnDispatchCard_FromUserId",
                table: "TrnDispatchCard",
                column: "FromUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TrnDispatchCard_RecordOfficeId",
                table: "TrnDispatchCard",
                column: "RecordOfficeId");

            migrationBuilder.CreateIndex(
                name: "IX_TrnDispatchCard_RegId",
                table: "TrnDispatchCard",
                column: "RegId");

            migrationBuilder.CreateIndex(
                name: "IX_TrnDispatchCard_ToAspNetUsersId",
                table: "TrnDispatchCard",
                column: "ToAspNetUsersId");

            migrationBuilder.CreateIndex(
                name: "IX_TrnDispatchCard_ToUnitId",
                table: "TrnDispatchCard",
                column: "ToUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_TrnDispatchCard_ToUserId",
                table: "TrnDispatchCard",
                column: "ToUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TrnDispatchCard_Updatedby",
                table: "TrnDispatchCard",
                column: "Updatedby");

            migrationBuilder.CreateIndex(
                name: "IX_TrnDispatchCardMapping_DispatchCardId",
                table: "TrnDispatchCardMapping",
                column: "DispatchCardId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrnDispatchCardMapping");

            migrationBuilder.DropTable(
                name: "TrnDispatchCard");
        }
    }
}
