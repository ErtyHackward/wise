using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WiseApi.Migrations
{
    public partial class ReportStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QueryTimeLast",
                table: "Runs");

            migrationBuilder.DropColumn(
                name: "TimeType",
                table: "Reports");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Runs",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Runs");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "QueryTimeLast",
                table: "Runs",
                type: "time(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TimeType",
                table: "Reports",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
