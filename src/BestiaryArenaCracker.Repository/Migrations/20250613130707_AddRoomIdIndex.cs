using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BestiaryArenaCracker.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddRoomIdIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "RoomId",
                table: "Compositions",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Compositions_RoomId",
                table: "Compositions",
                column: "RoomId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
               name: "IX_Compositions_RoomId",
               table: "Compositions");

            migrationBuilder.AlterColumn<string>(
                name: "RoomId",
                table: "Compositions",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

        }
    }
}
