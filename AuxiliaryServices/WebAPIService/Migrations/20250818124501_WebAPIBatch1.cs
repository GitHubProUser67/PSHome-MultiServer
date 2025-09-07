using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPIService.Migrations
{
    /// <inheritdoc />
    public partial class WebAPIBatch1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClearasilScoreBoardEntry",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PsnId = table.Column<string>(type: "TEXT", nullable: true),
                    PlayerId = table.Column<string>(type: "TEXT", nullable: false),
                    Score = table.Column<float>(type: "REAL", nullable: false),
                    ExtraData1 = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    ExtraData2 = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    ExtraData3 = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    ExtraData4 = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    ExtraData5 = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClearasilScoreBoardEntry", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CogsScoreboardEntry",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PsnId = table.Column<string>(type: "TEXT", nullable: true),
                    PlayerId = table.Column<string>(type: "TEXT", nullable: false),
                    Score = table.Column<float>(type: "REAL", nullable: false),
                    ExtraData1 = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    ExtraData2 = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    ExtraData3 = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    ExtraData4 = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    ExtraData5 = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CogsScoreboardEntry", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FiringRangeScoreBoardEntry",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PsnId = table.Column<string>(type: "TEXT", nullable: true),
                    PlayerId = table.Column<string>(type: "TEXT", nullable: false),
                    Score = table.Column<float>(type: "REAL", nullable: false),
                    ExtraData1 = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    ExtraData2 = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    ExtraData3 = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    ExtraData4 = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    ExtraData5 = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FiringRangeScoreBoardEntry", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HexxScoreboardEntry",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PsnId = table.Column<string>(type: "TEXT", nullable: true),
                    PlayerId = table.Column<string>(type: "TEXT", nullable: false),
                    Score = table.Column<float>(type: "REAL", nullable: false),
                    ExtraData1 = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    ExtraData2 = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    ExtraData3 = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    ExtraData4 = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    ExtraData5 = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HexxScoreboardEntry", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HomeScoreboardEntry",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PsnId = table.Column<string>(type: "TEXT", nullable: true),
                    PlayerId = table.Column<string>(type: "TEXT", nullable: false),
                    Score = table.Column<float>(type: "REAL", nullable: false),
                    ExtraData1 = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    ExtraData2 = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    ExtraData3 = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    ExtraData4 = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    ExtraData5 = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomeScoreboardEntry", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrbrunnerScoreBoardEntry",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PsnId = table.Column<string>(type: "TEXT", nullable: true),
                    PlayerId = table.Column<string>(type: "TEXT", nullable: false),
                    Score = table.Column<float>(type: "REAL", nullable: false),
                    ExtraData1 = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    ExtraData2 = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    ExtraData3 = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    ExtraData4 = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    ExtraData5 = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrbrunnerScoreBoardEntry", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClearasilScoreBoardEntry");

            migrationBuilder.DropTable(
                name: "CogsScoreboardEntry");

            migrationBuilder.DropTable(
                name: "FiringRangeScoreBoardEntry");

            migrationBuilder.DropTable(
                name: "HexxScoreboardEntry");

            migrationBuilder.DropTable(
                name: "HomeScoreboardEntry");

            migrationBuilder.DropTable(
                name: "OrbrunnerScoreBoardEntry");
        }
    }
}
