using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Alcatraz.Context.Migrations
{
    /// <inheritdoc />
    public partial class NET8_NULLABLE_FIXUP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UbiData",
                table: "Users",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT"
            );

            migrationBuilder.AlterColumn<byte[]>(
                name: "PublicData",
                table: "Users",
                type: "BLOB",
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "BLOB"
            );

            migrationBuilder.AlterColumn<byte[]>(
                name: "PrivateData",
                table: "Users",
                type: "BLOB",
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "BLOB"
            );

            migrationBuilder.AlterColumn<string>(
                name: "PlayerNickName",
                table: "Users",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT"
            );

            migrationBuilder.AlterColumn<string>(
                name: "MACAddress",
                table: "Users",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UbiData",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<byte[]>(
                name: "PublicData",
                table: "Users",
                type: "BLOB",
                nullable: false,
                defaultValue: new byte[0],
                oldClrType: typeof(byte[]),
                oldType: "BLOB",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<byte[]>(
                name: "PrivateData",
                table: "Users",
                type: "BLOB",
                nullable: false,
                defaultValue: new byte[0],
                oldClrType: typeof(byte[]),
                oldType: "BLOB",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<string>(
                name: "PlayerNickName",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<string>(
                name: "MACAddress",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true
            );
        }
    }
}
