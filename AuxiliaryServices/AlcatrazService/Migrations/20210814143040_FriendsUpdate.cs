using Microsoft.EntityFrameworkCore.Migrations;

namespace Alcatraz.Context.Migrations
{
    public partial class FriendsUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Status",
                table: "UserRelationships",
                newName: "Details");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Details",
                table: "UserRelationships",
                newName: "Status");
        }
    }
}
