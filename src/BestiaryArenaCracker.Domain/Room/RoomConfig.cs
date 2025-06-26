namespace BestiaryArenaCracker.Domain.Room
{
    public class RoomConfig
    {
        public required string Id { get; set; }
        public string Name => RoomIdToName[Id];
        public required File File { get; set; }
        public int Difficulty { get; set; }
        public int MaxTeamSize { get; set; }
        public int StaminaCost { get; set; }

        private static readonly Dictionary<string, string> RoomIdToName = new()
        {
            ["rkswrs"] = "Sewers",
            ["rkwht"] = "Wheat Field",
            ["rkevgr"] = "Evergreen Fields",
            ["rkwlf"] = "Wolf's Den",
            ["rkhny"] = "Honeyflower Tower",
            ["rkspdr"] = "Spider Lair",
            ["rkgblnb"] = "Goblin Bridge",
            ["rkgblnt"] = "Goblin Temple",
            ["rkminom"] = "Minotaur Mage Room",
            ["rksklt"] = "Rotten Graveyard",
            ["rkktn"] = "Katana Quest",
            ["rkbear"] = "Bear Room",
            ["rkmino"] = "Minotaur Hell",
            ["rkboat"] = "Amber's Raft",
            ["rkpsn"] = "Swampy Path",
            ["rkshi"] = "Lonesome Dragon",
            ["crbrd"] = "City Boardgames",
            ["crswrs"] = "Carlin Sewers",
            ["crmnks"] = "Isle of Kings",
            ["crghst1"] = "Ghostlands Surface",
            ["crghst2"] = "Ghostlands Library",
            ["crghst3"] = "Ghostlands Ritual Site",
            ["crdhll"] = "Demon Skeleton Hell",
            ["crzth"] = "Zathroth's Throne",
            ["crdmn"] = "Demonrage Seal",
            ["crbnsh"] = "Banshee's Last Room",
            ["molse"] = "Maze Gates",
            ["molsl"] = "Labyrinth Depths",
            ["molsd"] = "Hidden City of Demona",
            ["molst"] = "Teleporter Trap",
            ["fboat"] = "Folda Boat",
            ["fentr"] = "Cave Entrance",
            ["fwater"] = "Frozen Aquifer",
            ["fawar"] = "Alawar's Vault",
            ["fvega"] = "Vega Mountain",
            ["fsant"] = "Santa Claus Home",
            ["abmaze"] = "Hedge Maze",
            ["abwasp"] = "Ab'Dendriel Hive",
            ["abbane"] = "Elvenbane",
            ["abgobl"] = "Femor Hills",
            ["ofbed"] = "Orcish Barracks",
            ["offire"] = "The Orc King Hall",
            ["ofsha"] = "A Shamanic Ritual",
            ["ofsho"] = "Shore Camp",
            ["ofsmi"] = "Orcsmith Orcshop",
            ["kof"] = "Orc Fortress Outskirts",
            ["kfarm"] = "The Farms",
            ["kpub"] = "Dwarven Brewery",
            ["khub"] = "Mine Hub",
            ["kking"] = "Emperor Kruzak's Treasure Room",
            ["klab"] = "Mad Technomancer's Lab",
            ["roboat"] = "Awash Steamship",
            ["robwal"] = "Robson's Isle Ruins",
            ["vbridge"] = "Dwarven Bridge"
        };
    }
}
