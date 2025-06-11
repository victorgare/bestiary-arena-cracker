namespace BestiaryArenaCracker.Domain.Composition
{
    public class Composition
    {
        public required string Map { get; set; }

        public List<Board> Board { get; set; } = [];
    }
}
