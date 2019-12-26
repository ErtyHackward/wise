using Microsoft.EntityFrameworkCore.Migrations;

namespace WiseApi.Migrations
{
    public partial class AddProviderTitle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "DataProviderConfigurations",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "DataProviderConfigurations");
        }
    }
}
