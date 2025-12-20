using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Alcatraz.Context.Migrations
{
    /// <inheritdoc />
    public partial class UbiAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UbiData",
                table: "Users",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UbiData",
                table: "Users");
        }
    }
}
