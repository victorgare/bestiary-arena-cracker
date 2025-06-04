namespace BestiaryArenaCracker.Domain.Entities
{
    public class CompositionEntity : IEntity
    {
        public required string CompositionHash { get; set; }
        public required string RoomId { get; set; }
    }
}
