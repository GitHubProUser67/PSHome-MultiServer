using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPIService.Migrations
{
    /// <inheritdoc />
    public partial class AnsadaPhoneAPI : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnsadaScoreBoardEntry",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PsnId = table.Column<string>(type: "TEXT", nullable: true),
                    Time = table.Column<string>(type: "TEXT", nullable: true),
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
                    table.PrimaryKey("PK_AnsadaScoreBoardEntry", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnsadaScoreBoardEntry");
        }
    }
}
