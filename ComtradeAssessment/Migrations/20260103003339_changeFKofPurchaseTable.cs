using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComtradeAssessment.Migrations
{
    /// <inheritdoc />
    public partial class changeFKofPurchaseTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "CampaignId",
                table: "Purchases",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_CampaignId",
                table: "Purchases",
                column: "CampaignId");

            migrationBuilder.AddForeignKey(
                name: "FK_Purchases_Campaigns_CampaignId",
                table: "Purchases",
                column: "CampaignId",
                principalTable: "Campaigns",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Purchases_Campaigns_CampaignId",
                table: "Purchases");

            migrationBuilder.DropIndex(
                name: "IX_Purchases_CampaignId",
                table: "Purchases");

            migrationBuilder.AlterColumn<int>(
                name: "CampaignId",
                table: "Purchases",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
