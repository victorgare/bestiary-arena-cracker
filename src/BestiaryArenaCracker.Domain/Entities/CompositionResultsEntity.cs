namespace BestiaryArenaCracker.Domain.Entities
{
    public class CompositionResultsEntity : IEntity
    {
        public int CompositionId { get; set; }
        public int Ticks { get; set; }
        public int Points { get; set; }
        public string Grade { get; set; } = null!;
    }
}
