namespace BestiaryArenaCracker.ApplicationCore.Services.Composition
{
    public class CompositionResult
    {
        public int CompositionId { get; set; }
        public int RemainingRuns { get; set; }
        public required Domain.Composition.Composition Composition { get; set; }
    }
}
