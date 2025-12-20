using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Alcatraz.Context.Migrations
{
    public partial class AddPlayerStatistics_UpdateUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "GameNickName",
                table: "Users",
                newName: "PlayerNickName");

            migrationBuilder.CreateTable(
                name: "PlayerStatisticBoards",
                columns: table => new
                {
                    Id = table.Column<uint>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PlayerId = table.Column<uint>(type: "INTEGER", nullable: false),
                    BoardId = table.Column<int>(type: "INTEGER", nullable: false),
                    Rank = table.Column<int>(type: "INTEGER", nullable: false),
                    Score = table.Column<float>(type: "REAL", nullable: false),
                    LastUpdate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerStatisticBoards", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlayerStatisticBoardValues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PlayerBoardId = table.Column<int>(type: "INTEGER", nullable: false),
                    PropertyId = table.Column<int>(type: "INTEGER", nullable: false),
                    ValueJSON = table.Column<string>(type: "TEXT", nullable: true),
                    RankingCriterionIndex = table.Column<int>(type: "INTEGER", nullable: false),
                    SliceScoreJSON = table.Column<string>(type: "TEXT", nullable: true),
                    ScoreLostForNextSliceJSON = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerStatisticBoardValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerStatisticBoardValues_PlayerStatisticBoards_PlayerBoardId",
                        column: x => x.PlayerBoardId,
                        principalTable: "PlayerStatisticBoards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerStatisticBoardValues_PlayerBoardId",
                table: "PlayerStatisticBoardValues",
                column: "PlayerBoardId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerStatisticBoardValues");

            migrationBuilder.DropTable(
                name: "PlayerStatisticBoards");

            migrationBuilder.RenameColumn(
                name: "PlayerNickName",
                table: "Users",
                newName: "GameNickName");
        }
    }
}
