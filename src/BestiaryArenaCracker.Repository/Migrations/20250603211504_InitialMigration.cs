using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BestiaryArenaCracker.Repository.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Compositions table
            migrationBuilder.CreateTable(
                name: "Compositions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                    CompositionHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoomId = table.Column<string>(type: "varchar(50)", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Compositions", x => x.Id);
                });

            // Composition Monsters
            migrationBuilder.CreateTable(
                name: "CompositionMonsters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                    CompositionId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    Hitpoints = table.Column<int>(type: "int", nullable: false),
                    Attack = table.Column<int>(type: "int", nullable: false),
                    AbilityPower = table.Column<int>(type: "int", nullable: false),
                    Armor = table.Column<int>(type: "int", nullable: false),
                    MagicResistance = table.Column<int>(type: "int", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    TileLocation = table.Column<int>(type: "int", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompositionMonsters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompositionMonsters_Compositions_CompositionId",
                        column: x => x.CompositionId,
                        principalTable: "Compositions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Composition result
            migrationBuilder.CreateTable(
                name: "CompositionResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                    CompositionId = table.Column<int>(type: "int", nullable: false),
                    Ticks = table.Column<int>(type: "int", nullable: false),
                    Points = table.Column<int>(type: "int", nullable: false),
                    Grade = table.Column<string>(type: "varchar(10)", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompositionResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompositionResults_Compositions_CompositionId",
                        column: x => x.CompositionId,
                        principalTable: "Compositions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable("Composition");
            migrationBuilder.DropTable("CompositionMonsters");
            migrationBuilder.DropTable("CompositionResult");
        }
    }
}
