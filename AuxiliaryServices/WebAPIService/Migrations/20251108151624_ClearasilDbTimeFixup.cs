using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPIService.Migrations
{
    /// <inheritdoc />
    public partial class ClearasilDbTimeFixup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Time",
                table: "ClearasilScoreBoardEntry",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Time",
                table: "ClearasilScoreBoardEntry");
        }
    }
}
