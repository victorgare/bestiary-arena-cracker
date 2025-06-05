using BestiaryArenaCracker.ApplicationCore.Interfaces.Providers;
using BestiaryArenaCracker.ApplicationCore.Interfaces.Repositories;
using BestiaryArenaCracker.ApplicationCore.Interfaces.Services;
using BestiaryArenaCracker.Domain;

namespace BestiaryArenaCracker.ApplicationCore.Services
{
    public class CompositionService(IRoomConfigProvider roomConfigProvider, IApplicationDbContext dbContext) : ICompositionService
    {
        public Task<CompositionResult> FindCompositionAsync()
        {
            var firstRoom = roomConfigProvider.Rooms.FirstOrDefault();

            var hitboxes = firstRoom?.File.Data.HitboxesDictionary ?? [];
            var actors = firstRoom?.File.Data.ActorsDictionary ?? [];

            Composition? composition = null;
            foreach (var tile in hitboxes)
            {
                // if there is no actor at this tile we'll use it
                if (!actors.ContainsKey(tile.Key))
                {
                    foreach (var creature in Enum.GetValues<Creatures>())
                    {
                        var monster = new Monster(creature);

                        foreach (var equipment in Enum.GetValues<Equipments>())
                        {
                            foreach (var stat in Enum.GetValues<EquipmentStat>())
                            {
                                var item = new Item(equipment, stat);

                                if (composition == null)
                                {
                                    return Task.FromResult(new CompositionResult
                                    {
                                        Composition = new Composition
                                        {
                                            Board = [
                                               new ()
                                               {
                                                   Equipment = item,
                                                   Monster = monster,
                                                   Tile = tile.Key
                                               }
                                       ]
                                        }
                                    });
                                }
                            }
                        }
                    }
                }

            }

            throw new Exception("No valid composition found. This should not happen.");
        }
    }
}
