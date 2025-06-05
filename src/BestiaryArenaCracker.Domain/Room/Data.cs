namespace BestiaryArenaCracker.Domain.Room
{
    public class Data
    {
        public Actor[]? Actors { get; set; }
        public bool[]? Hitboxes { get; set; }
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

        public int[] GetFreeTiles()
        {
            var hitboxIndices = ToDictionary(Hitboxes).Keys;
            var actorIndices = ToDictionary(Actors).Keys;
            return [.. hitboxIndices.Except(actorIndices)];
        }
    }
}
