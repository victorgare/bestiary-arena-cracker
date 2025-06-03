namespace BestiaryArenaCracker.Domain.Room
{
    public class Data
    {
        public Actor[]? Actors { get; set; }
        public bool[]? Hitboxes { get; set; }
        public Dictionary<int, bool> HitboxesDictionary => ToDictionary(Hitboxes).Where(c => !c.Value).ToDictionary();

        public Dictionary<int, Actor> ActorsDictionary => ToDictionary(Actors);

        private static Dictionary<int, TSource> ToDictionary<TSource>(TSource[]? source)
        {
            var dict = new Dictionary<int, TSource>();
            if (source != null)
            {
                for (int i = 0; i < source.Length; i++)
                {
                    var item = source[i];
                    if (item != null)
                    {
                        dict[i] = item;
                    }
                }
            }
            return dict;
        }
    }
}
