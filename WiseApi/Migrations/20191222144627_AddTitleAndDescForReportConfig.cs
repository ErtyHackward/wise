using Microsoft.EntityFrameworkCore.Migrations;

namespace WiseApi.Migrations
{
    public partial class AddTitleAndDescForReportConfig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "ReportConfigurations",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "ReportConfigurations",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "ReportConfigurations");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "ReportConfigurations");
        }
    }
}
