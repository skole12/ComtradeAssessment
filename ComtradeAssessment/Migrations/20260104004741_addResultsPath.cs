using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComtradeAssessment.Migrations
{
    /// <inheritdoc />
    public partial class addResultsPath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CsvResultsPath",
                schema: "TestZadatak",
                table: "Campaigns",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CsvResultsPath",
                schema: "TestZadatak",
                table: "Campaigns");
        }
    }
}
