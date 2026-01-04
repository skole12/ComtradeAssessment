using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComtradeAssessment.Migrations
{
    /// <inheritdoc />
    public partial class schema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "TestZadatak");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "Users",
                newSchema: "TestZadatak");

            migrationBuilder.RenameTable(
                name: "Roles",
                newName: "Roles",
                newSchema: "TestZadatak");

            migrationBuilder.RenameTable(
                name: "Campaigns",
                newName: "Campaigns",
                newSchema: "TestZadatak");

            migrationBuilder.RenameTable(
                name: "CampaignPurchases",
                newName: "CampaignPurchases",
                newSchema: "TestZadatak");

            migrationBuilder.RenameTable(
                name: "CampaignOffers",
                newName: "CampaignOffers",
                newSchema: "TestZadatak");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Users",
                schema: "TestZadatak",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "Roles",
                schema: "TestZadatak",
                newName: "Roles");

            migrationBuilder.RenameTable(
                name: "Campaigns",
                schema: "TestZadatak",
                newName: "Campaigns");

            migrationBuilder.RenameTable(
                name: "CampaignPurchases",
                schema: "TestZadatak",
                newName: "CampaignPurchases");

            migrationBuilder.RenameTable(
                name: "CampaignOffers",
                schema: "TestZadatak",
                newName: "CampaignOffers");
        }
    }
}
