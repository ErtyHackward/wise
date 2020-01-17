using Microsoft.EntityFrameworkCore.Migrations;

namespace WiseApi.Migrations
{
    public partial class GroupSecurity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AccessMode",
                table: "ReportGroup",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ReportGroupUserGroupJoin",
                columns: table => new
                {
                    ReportGroupId = table.Column<int>(nullable: false),
                    UserGroupId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportGroupUserGroupJoin", x => new { x.ReportGroupId, x.UserGroupId });
                    table.ForeignKey(
                        name: "FK_ReportGroupUserGroupJoin_ReportGroup_ReportGroupId",
                        column: x => x.ReportGroupId,
                        principalTable: "ReportGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReportGroupUserGroupJoin_Groups_UserGroupId",
                        column: x => x.UserGroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Groups",
                columns: new[] { "Id", "IsAdmin", "Title" },
                values: new object[] { 1, false, "Пользователь" });

            migrationBuilder.InsertData(
                table: "Groups",
                columns: new[] { "Id", "IsAdmin", "Title" },
                values: new object[] { 2, true, "Администратор" });

            migrationBuilder.CreateIndex(
                name: "IX_ReportGroupUserGroupJoin_UserGroupId",
                table: "ReportGroupUserGroupJoin",
                column: "UserGroupId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReportGroupUserGroupJoin");

            migrationBuilder.DeleteData(
                table: "Groups",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Groups",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DropColumn(
                name: "AccessMode",
                table: "ReportGroup");
        }
    }
}
