using Microsoft.EntityFrameworkCore.Migrations;

namespace Alcatraz.Context.Migrations
{
    public partial class AddPlayerUnlocks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RewardFlags",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RewardFlags",
                table: "Users");
        }
    }
}
