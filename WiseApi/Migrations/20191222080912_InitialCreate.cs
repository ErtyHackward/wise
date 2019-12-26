using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WiseApi.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DataProviderConfigurations",
                columns: table => new
                {
                    DataProviderConfigurationId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DataProviderType = table.Column<string>(nullable: true),
                    ConnectionString = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataProviderConfigurations", x => x.DataProviderConfigurationId);
                });

            migrationBuilder.CreateTable(
                name: "ReportConfigurations",
                columns: table => new
                {
                    ReportConfigurationId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DataProviderConfigurationId = table.Column<int>(nullable: true),
                    Query = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportConfigurations", x => x.ReportConfigurationId);
                    table.ForeignKey(
                        name: "FK_ReportConfigurations_DataProviderConfigurations_DataProvider~",
                        column: x => x.DataProviderConfigurationId,
                        principalTable: "DataProviderConfigurations",
                        principalColumn: "DataProviderConfigurationId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Executions",
                columns: table => new
                {
                    ReportExecutionId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ReportConfigurationId = table.Column<int>(nullable: true),
                    ExecutionStartedAt = table.Column<DateTime>(nullable: false),
                    ExecutionFinishedAt = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Executions", x => x.ReportExecutionId);
                    table.ForeignKey(
                        name: "FK_Executions_ReportConfigurations_ReportConfigurationId",
                        column: x => x.ReportConfigurationId,
                        principalTable: "ReportConfigurations",
                        principalColumn: "ReportConfigurationId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Executions_ReportConfigurationId",
                table: "Executions",
                column: "ReportConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportConfigurations_DataProviderConfigurationId",
                table: "ReportConfigurations",
                column: "DataProviderConfigurationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Executions");

            migrationBuilder.DropTable(
                name: "ReportConfigurations");

            migrationBuilder.DropTable(
                name: "DataProviderConfigurations");
        }
    }
}
