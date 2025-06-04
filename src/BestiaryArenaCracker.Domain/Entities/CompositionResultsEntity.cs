namespace BestiaryArenaCracker.Domain.Entities
{
    public class CompositionResultsEntity : IEntity
    {
        public required int CompositionId { get; set; }
        public required int Ticks { get; set; }
        public required int Points { get; set; }
        public required string Grade { get; set; }
    }
}
