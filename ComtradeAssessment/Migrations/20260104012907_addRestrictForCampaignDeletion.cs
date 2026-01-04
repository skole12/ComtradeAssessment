using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComtradeAssessment.Migrations
{
    /// <inheritdoc />
    public partial class addRestrictForCampaignDeletion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CampaignOffers_Campaigns_CampaignId",
                schema: "TestZadatak",
                table: "CampaignOffers");

            migrationBuilder.AddForeignKey(
                name: "FK_CampaignOffers_Campaigns_CampaignId",
                schema: "TestZadatak",
                table: "CampaignOffers",
                column: "CampaignId",
                principalSchema: "TestZadatak",
                principalTable: "Campaigns",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CampaignOffers_Campaigns_CampaignId",
                schema: "TestZadatak",
                table: "CampaignOffers");

            migrationBuilder.AddForeignKey(
                name: "FK_CampaignOffers_Campaigns_CampaignId",
                schema: "TestZadatak",
                table: "CampaignOffers",
                column: "CampaignId",
                principalSchema: "TestZadatak",
                principalTable: "Campaigns",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
