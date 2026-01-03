using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComtradeAssessment.Migrations
{
    /// <inheritdoc />
    public partial class changePKOfCampaignOffer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CampaignOffers",
                table: "CampaignOffers");

            migrationBuilder.DropIndex(
                name: "IX_CampaignOffers_CampaignId",
                table: "CampaignOffers");

            migrationBuilder.DropColumn(
                name: "CampaignOfferId",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "CampaignOffers");

            migrationBuilder.AddColumn<int>(
                name: "CampaignId",
                table: "Purchases",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CampaignOffers",
                table: "CampaignOffers",
                columns: new[] { "CampaignId", "CustomerId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CampaignOffers",
                table: "CampaignOffers");

            migrationBuilder.DropColumn(
                name: "CampaignId",
                table: "Purchases");

            migrationBuilder.AddColumn<long>(
                name: "CampaignOfferId",
                table: "Purchases",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Id",
                table: "CampaignOffers",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CampaignOffers",
                table: "CampaignOffers",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignOffers_CampaignId",
                table: "CampaignOffers",
                column: "CampaignId");
        }
    }
}
