using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WiseApi.Migrations
{
    public partial class UsersAndGroups : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Runs",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AccessMode",
                table: "Reports",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AuthorId",
                table: "Reports",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(nullable: true),
                    IsAdmin = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReportGroup",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportGroup", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DisplayName = table.Column<string>(nullable: true),
                    Login = table.Column<string>(nullable: true),
                    PasswordHash = table.Column<string>(nullable: true),
                    Ip = table.Column<string>(nullable: true),
                    RegisteredAt = table.Column<DateTime>(nullable: false),
                    LastVisitedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReportGroupJoin",
                columns: table => new
                {
                    ReportId = table.Column<int>(nullable: false),
                    GroupId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportGroupJoin", x => new { x.ReportId, x.GroupId });
                    table.ForeignKey(
                        name: "FK_ReportGroupJoin_ReportGroup_GroupId",
                        column: x => x.GroupId,
                        principalTable: "ReportGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReportGroupJoin_Reports_ReportId",
                        column: x => x.ReportId,
                        principalTable: "Reports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserGroupJoin",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    GroupId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGroupJoin", x => new { x.UserId, x.GroupId });
                    table.ForeignKey(
                        name: "FK_UserGroupJoin_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserGroupJoin_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Runs_UserId",
                table: "Runs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_AuthorId",
                table: "Reports",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportGroupJoin_GroupId",
                table: "ReportGroupJoin",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_UserGroupJoin_GroupId",
                table: "UserGroupJoin",
                column: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Users_AuthorId",
                table: "Reports",
                column: "AuthorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Runs_Users_UserId",
                table: "Runs",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Users_AuthorId",
                table: "Reports");

            migrationBuilder.DropForeignKey(
                name: "FK_Runs_Users_UserId",
                table: "Runs");

            migrationBuilder.DropTable(
                name: "ReportGroupJoin");

            migrationBuilder.DropTable(
                name: "UserGroupJoin");

            migrationBuilder.DropTable(
                name: "ReportGroup");

            migrationBuilder.DropTable(
                name: "Groups");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Runs_UserId",
                table: "Runs");

            migrationBuilder.DropIndex(
                name: "IX_Reports_AuthorId",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Runs");

            migrationBuilder.DropColumn(
                name: "AccessMode",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "Reports");
        }
    }
}
