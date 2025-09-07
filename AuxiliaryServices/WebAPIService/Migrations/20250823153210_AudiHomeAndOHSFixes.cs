using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPIService.Migrations
{
    /// <inheritdoc />
    public partial class AudiHomeAndOHSFixes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InterGalacticScoreboardEntry",
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
                    table.PrimaryKey("PK_InterGalacticScoreboardEntry", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OHSScoreboardEntry",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PsnId = table.Column<string>(type: "TEXT", nullable: true),
                    WriteKey = table.Column<int>(type: "INTEGER", nullable: false),
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
                    table.PrimaryKey("PK_OHSScoreboardEntry", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SledMpScoreboardEntry",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    numOfRaces = table.Column<int>(type: "INTEGER", nullable: false),
                    time = table.Column<float>(type: "REAL", nullable: false),
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
                    table.PrimaryKey("PK_SledMpScoreboardEntry", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SledScoreboardEntry",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    numOfRaces = table.Column<int>(type: "INTEGER", nullable: false),
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
                    table.PrimaryKey("PK_SledScoreboardEntry", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VrunScoreboardEntry",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    numOfRaces = table.Column<int>(type: "INTEGER", nullable: false),
                    time = table.Column<float>(type: "REAL", nullable: false),
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
                    table.PrimaryKey("PK_VrunScoreboardEntry", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InterGalacticScoreboardEntry");

            migrationBuilder.DropTable(
                name: "OHSScoreboardEntry");

            migrationBuilder.DropTable(
                name: "SledMpScoreboardEntry");

            migrationBuilder.DropTable(
                name: "SledScoreboardEntry");

            migrationBuilder.DropTable(
                name: "VrunScoreboardEntry");
        }
    }
}
