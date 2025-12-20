using Microsoft.EntityFrameworkCore.Migrations;

namespace Alcatraz.Context.Migrations
{
    public partial class AddFriendships : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserRelationships",
                columns: table => new
                {
                    User1Id = table.Column<uint>(type: "INTEGER", nullable: false),
                    User2Id = table.Column<uint>(type: "INTEGER", nullable: false),
                    Status = table.Column<uint>(type: "INTEGER", nullable: false),
                    ByRelationShip = table.Column<uint>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRelationships", x => new { x.User1Id, x.User2Id });
                    table.ForeignKey(
                        name: "FK_UserRelationships_Users_User1Id",
                        column: x => x.User1Id,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRelationships_Users_User2Id",
                        column: x => x.User2Id,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserRelationships_User2Id",
                table: "UserRelationships",
                column: "User2Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserRelationships");
        }
    }
}
