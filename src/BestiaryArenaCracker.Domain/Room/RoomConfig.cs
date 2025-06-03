namespace BestiaryArenaCracker.Domain.Room
{
    public class RoomConfig
    {
        public required string Id { get; set; }
        public required File File { get; set; }
        public int Difficulty { get; set; }
        public int MaxTeamSize { get; set; }
        public int StaminaCost { get; set; }
    }
}
