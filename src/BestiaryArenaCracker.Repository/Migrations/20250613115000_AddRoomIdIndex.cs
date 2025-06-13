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
        }
    }
}
