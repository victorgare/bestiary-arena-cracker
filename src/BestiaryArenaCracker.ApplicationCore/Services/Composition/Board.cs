namespace BestiaryArenaCracker.ApplicationCore.Services.Composition
{
    public class Board
    {
        public int Tile { get; set; }
        public Monster Monster { get; set; } = null!;
        public Equipment Equipment { get; set; } = null!;
    }
}
