namespace BestiaryArenaCracker.Domain
{
    public class BoardItem
    {
        public required int Tile { get; set; }
        public required Monster Monster { get; set; }
        public required Item Equipment { get; set; }
    }
}
