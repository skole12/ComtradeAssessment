using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComtradeAssessment.Migrations
{
    /// <inheritdoc />
    public partial class addCreatedByForCampaign : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Campaigns",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "Campaigns",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Campaigns_CreatedBy",
                table: "Campaigns",
                column: "CreatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_Campaigns_Users_CreatedBy",
                table: "Campaigns",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Campaigns_Users_CreatedBy",
                table: "Campaigns");

            migrationBuilder.DropIndex(
                name: "IX_Campaigns_CreatedBy",
                table: "Campaigns");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Campaigns");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Campaigns");
        }
    }
}
