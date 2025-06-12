using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BestiaryArenaCracker.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddOnDeleteCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_CompositionResults_CompositionId",
                table: "CompositionResults",
                column: "CompositionId");

            migrationBuilder.CreateIndex(
                name: "IX_CompositionMonsters_CompositionId",
                table: "CompositionMonsters",
                column: "CompositionId");

            migrationBuilder.AddForeignKey(
                name: "FK_CompositionMonsters_Compositions_CompositionId",
                table: "CompositionMonsters",
                column: "CompositionId",
                principalTable: "Compositions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompositionResults_Compositions_CompositionId",
                table: "CompositionResults",
                column: "CompositionId",
                principalTable: "Compositions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompositionMonsters_Compositions_CompositionId",
                table: "CompositionMonsters");

            migrationBuilder.DropForeignKey(
                name: "FK_CompositionResults_Compositions_CompositionId",
                table: "CompositionResults");

            migrationBuilder.DropIndex(
                name: "IX_CompositionResults_CompositionId",
                table: "CompositionResults");

            migrationBuilder.DropIndex(
                name: "IX_CompositionMonsters_CompositionId",
                table: "CompositionMonsters");
        }
    }
}
