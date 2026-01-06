using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComtradeAssessment.Migrations
{
    /// <inheritdoc />
    public partial class normalisationOfCampaignResults : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DiscountsOffered",
                table: "Campaigns",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PurchasesMade",
                table: "Campaigns",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountsOffered",
                table: "Campaigns");

            migrationBuilder.DropColumn(
                name: "PurchasesMade",
                table: "Campaigns");
        }
    }
}
