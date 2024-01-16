using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v23 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_UserProfile_Updatedby",
                table: "UserProfile",
                column: "Updatedby");

            migrationBuilder.CreateIndex(
                name: "IX_TrnStepCounter_Updatedby",
                table: "TrnStepCounter",
                column: "Updatedby");

            migrationBuilder.CreateIndex(
                name: "IX_TrnLogin_Log_Updatedby",
                table: "TrnLogin_Log",
                column: "Updatedby");

            migrationBuilder.CreateIndex(
                name: "IX_TrnICardRequest_Updatedby",
                table: "TrnICardRequest",
                column: "Updatedby");

            migrationBuilder.CreateIndex(
                name: "IX_TrnFwds_Updatedby",
                table: "TrnFwds",
                column: "Updatedby");

            migrationBuilder.CreateIndex(
                name: "IX_MUnit_Updatedby",
                table: "MUnit",
                column: "Updatedby");

            migrationBuilder.CreateIndex(
                name: "IX_MSubDte_Updatedby",
                table: "MSubDte",
                column: "Updatedby");

            migrationBuilder.CreateIndex(
                name: "IX_MRegistration_Updatedby",
                table: "MRegistration",
                column: "Updatedby");

            migrationBuilder.CreateIndex(
                name: "IX_MRegimental_Updatedby",
                table: "MRegimental",
                column: "Updatedby");

            migrationBuilder.CreateIndex(
                name: "IX_MRank_Updatedby",
                table: "MRank",
                column: "Updatedby");

            migrationBuilder.CreateIndex(
                name: "IX_MPso_Updatedby",
                table: "MPso",
                column: "Updatedby");

            migrationBuilder.CreateIndex(
                name: "IX_MMappingProfile_Updatedby",
                table: "MMappingProfile",
                column: "Updatedby");

            migrationBuilder.CreateIndex(
                name: "IX_MFormation_Updatedby",
                table: "MFormation",
                column: "Updatedby");

            migrationBuilder.CreateIndex(
                name: "IX_MFmnBranches_Updatedby",
                table: "MFmnBranches",
                column: "Updatedby");

            migrationBuilder.CreateIndex(
                name: "IX_MDiv_Updatedby",
                table: "MDiv",
                column: "Updatedby");

            migrationBuilder.CreateIndex(
                name: "IX_MCorps_Updatedby",
                table: "MCorps",
                column: "Updatedby");

            migrationBuilder.CreateIndex(
                name: "IX_MComd_Updatedby",
                table: "MComd",
                column: "Updatedby");

            migrationBuilder.CreateIndex(
                name: "IX_MBde_Updatedby",
                table: "MBde",
                column: "Updatedby");

            migrationBuilder.CreateIndex(
                name: "IX_MArmedType_Updatedby",
                table: "MArmedType",
                column: "Updatedby");

            migrationBuilder.CreateIndex(
                name: "IX_MArmedCats_Updatedby",
                table: "MArmedCats",
                column: "Updatedby");

            migrationBuilder.CreateIndex(
                name: "IX_MapUnit_Updatedby",
                table: "MapUnit",
                column: "Updatedby");

            migrationBuilder.CreateIndex(
                name: "IX_MAppointment_Updatedby",
                table: "MAppointment",
                column: "Updatedby");

            migrationBuilder.CreateIndex(
                name: "IX_DocUploads_Updatedby",
                table: "DocUploads",
                column: "Updatedby");

            migrationBuilder.CreateIndex(
                name: "IX_BasicDetailTemps_Updatedby",
                table: "BasicDetailTemps",
                column: "Updatedby");

            migrationBuilder.CreateIndex(
                name: "IX_BasicDetails_Updatedby",
                table: "BasicDetails",
                column: "Updatedby");

            migrationBuilder.AddForeignKey(
                name: "FK_BasicDetails_AspNetUsers_Updatedby",
                table: "BasicDetails",
                column: "Updatedby",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BasicDetailTemps_AspNetUsers_Updatedby",
                table: "BasicDetailTemps",
                column: "Updatedby",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DocUploads_AspNetUsers_Updatedby",
                table: "DocUploads",
                column: "Updatedby",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MAppointment_AspNetUsers_Updatedby",
                table: "MAppointment",
                column: "Updatedby",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MapUnit_AspNetUsers_Updatedby",
                table: "MapUnit",
                column: "Updatedby",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MArmedCats_AspNetUsers_Updatedby",
                table: "MArmedCats",
                column: "Updatedby",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MArmedType_AspNetUsers_Updatedby",
                table: "MArmedType",
                column: "Updatedby",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MBde_AspNetUsers_Updatedby",
                table: "MBde",
                column: "Updatedby",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MComd_AspNetUsers_Updatedby",
                table: "MComd",
                column: "Updatedby",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MCorps_AspNetUsers_Updatedby",
                table: "MCorps",
                column: "Updatedby",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MDiv_AspNetUsers_Updatedby",
                table: "MDiv",
                column: "Updatedby",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MFmnBranches_AspNetUsers_Updatedby",
                table: "MFmnBranches",
                column: "Updatedby",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MFormation_AspNetUsers_Updatedby",
                table: "MFormation",
                column: "Updatedby",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MMappingProfile_AspNetUsers_Updatedby",
                table: "MMappingProfile",
                column: "Updatedby",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MPso_AspNetUsers_Updatedby",
                table: "MPso",
                column: "Updatedby",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MRank_AspNetUsers_Updatedby",
                table: "MRank",
                column: "Updatedby",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MRegimental_AspNetUsers_Updatedby",
                table: "MRegimental",
                column: "Updatedby",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MRegistration_AspNetUsers_Updatedby",
                table: "MRegistration",
                column: "Updatedby",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MSubDte_AspNetUsers_Updatedby",
                table: "MSubDte",
                column: "Updatedby",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MUnit_AspNetUsers_Updatedby",
                table: "MUnit",
                column: "Updatedby",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TrnFwds_AspNetUsers_Updatedby",
                table: "TrnFwds",
                column: "Updatedby",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TrnICardRequest_AspNetUsers_Updatedby",
                table: "TrnICardRequest",
                column: "Updatedby",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TrnLogin_Log_AspNetUsers_Updatedby",
                table: "TrnLogin_Log",
                column: "Updatedby",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TrnStepCounter_AspNetUsers_Updatedby",
                table: "TrnStepCounter",
                column: "Updatedby",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfile_AspNetUsers_Updatedby",
                table: "UserProfile",
                column: "Updatedby",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BasicDetails_AspNetUsers_Updatedby",
                table: "BasicDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_BasicDetailTemps_AspNetUsers_Updatedby",
                table: "BasicDetailTemps");

            migrationBuilder.DropForeignKey(
                name: "FK_DocUploads_AspNetUsers_Updatedby",
                table: "DocUploads");

            migrationBuilder.DropForeignKey(
                name: "FK_MAppointment_AspNetUsers_Updatedby",
                table: "MAppointment");

            migrationBuilder.DropForeignKey(
                name: "FK_MapUnit_AspNetUsers_Updatedby",
                table: "MapUnit");

            migrationBuilder.DropForeignKey(
                name: "FK_MArmedCats_AspNetUsers_Updatedby",
                table: "MArmedCats");

            migrationBuilder.DropForeignKey(
                name: "FK_MArmedType_AspNetUsers_Updatedby",
                table: "MArmedType");

            migrationBuilder.DropForeignKey(
                name: "FK_MBde_AspNetUsers_Updatedby",
                table: "MBde");

            migrationBuilder.DropForeignKey(
                name: "FK_MComd_AspNetUsers_Updatedby",
                table: "MComd");

            migrationBuilder.DropForeignKey(
                name: "FK_MCorps_AspNetUsers_Updatedby",
                table: "MCorps");

            migrationBuilder.DropForeignKey(
                name: "FK_MDiv_AspNetUsers_Updatedby",
                table: "MDiv");

            migrationBuilder.DropForeignKey(
                name: "FK_MFmnBranches_AspNetUsers_Updatedby",
                table: "MFmnBranches");

            migrationBuilder.DropForeignKey(
                name: "FK_MFormation_AspNetUsers_Updatedby",
                table: "MFormation");

            migrationBuilder.DropForeignKey(
                name: "FK_MMappingProfile_AspNetUsers_Updatedby",
                table: "MMappingProfile");

            migrationBuilder.DropForeignKey(
                name: "FK_MPso_AspNetUsers_Updatedby",
                table: "MPso");

            migrationBuilder.DropForeignKey(
                name: "FK_MRank_AspNetUsers_Updatedby",
                table: "MRank");

            migrationBuilder.DropForeignKey(
                name: "FK_MRegimental_AspNetUsers_Updatedby",
                table: "MRegimental");

            migrationBuilder.DropForeignKey(
                name: "FK_MRegistration_AspNetUsers_Updatedby",
                table: "MRegistration");

            migrationBuilder.DropForeignKey(
                name: "FK_MSubDte_AspNetUsers_Updatedby",
                table: "MSubDte");

            migrationBuilder.DropForeignKey(
                name: "FK_MUnit_AspNetUsers_Updatedby",
                table: "MUnit");

            migrationBuilder.DropForeignKey(
                name: "FK_TrnFwds_AspNetUsers_Updatedby",
                table: "TrnFwds");

            migrationBuilder.DropForeignKey(
                name: "FK_TrnICardRequest_AspNetUsers_Updatedby",
                table: "TrnICardRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_TrnLogin_Log_AspNetUsers_Updatedby",
                table: "TrnLogin_Log");

            migrationBuilder.DropForeignKey(
                name: "FK_TrnStepCounter_AspNetUsers_Updatedby",
                table: "TrnStepCounter");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProfile_AspNetUsers_Updatedby",
                table: "UserProfile");

            migrationBuilder.DropIndex(
                name: "IX_UserProfile_Updatedby",
                table: "UserProfile");

            migrationBuilder.DropIndex(
                name: "IX_TrnStepCounter_Updatedby",
                table: "TrnStepCounter");

            migrationBuilder.DropIndex(
                name: "IX_TrnLogin_Log_Updatedby",
                table: "TrnLogin_Log");

            migrationBuilder.DropIndex(
                name: "IX_TrnICardRequest_Updatedby",
                table: "TrnICardRequest");

            migrationBuilder.DropIndex(
                name: "IX_TrnFwds_Updatedby",
                table: "TrnFwds");

            migrationBuilder.DropIndex(
                name: "IX_MUnit_Updatedby",
                table: "MUnit");

            migrationBuilder.DropIndex(
                name: "IX_MSubDte_Updatedby",
                table: "MSubDte");

            migrationBuilder.DropIndex(
                name: "IX_MRegistration_Updatedby",
                table: "MRegistration");

            migrationBuilder.DropIndex(
                name: "IX_MRegimental_Updatedby",
                table: "MRegimental");

            migrationBuilder.DropIndex(
                name: "IX_MRank_Updatedby",
                table: "MRank");

            migrationBuilder.DropIndex(
                name: "IX_MPso_Updatedby",
                table: "MPso");

            migrationBuilder.DropIndex(
                name: "IX_MMappingProfile_Updatedby",
                table: "MMappingProfile");

            migrationBuilder.DropIndex(
                name: "IX_MFormation_Updatedby",
                table: "MFormation");

            migrationBuilder.DropIndex(
                name: "IX_MFmnBranches_Updatedby",
                table: "MFmnBranches");

            migrationBuilder.DropIndex(
                name: "IX_MDiv_Updatedby",
                table: "MDiv");

            migrationBuilder.DropIndex(
                name: "IX_MCorps_Updatedby",
                table: "MCorps");

            migrationBuilder.DropIndex(
                name: "IX_MComd_Updatedby",
                table: "MComd");

            migrationBuilder.DropIndex(
                name: "IX_MBde_Updatedby",
                table: "MBde");

            migrationBuilder.DropIndex(
                name: "IX_MArmedType_Updatedby",
                table: "MArmedType");

            migrationBuilder.DropIndex(
                name: "IX_MArmedCats_Updatedby",
                table: "MArmedCats");

            migrationBuilder.DropIndex(
                name: "IX_MapUnit_Updatedby",
                table: "MapUnit");

            migrationBuilder.DropIndex(
                name: "IX_MAppointment_Updatedby",
                table: "MAppointment");

            migrationBuilder.DropIndex(
                name: "IX_DocUploads_Updatedby",
                table: "DocUploads");

            migrationBuilder.DropIndex(
                name: "IX_BasicDetailTemps_Updatedby",
                table: "BasicDetailTemps");

            migrationBuilder.DropIndex(
                name: "IX_BasicDetails_Updatedby",
                table: "BasicDetails");
        }
    }
}
