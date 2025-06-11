namespace BestiaryArenaCracker.Domain.Extensions
{
    public static class StringExtensions
    {
        public static string ToDisplayName(this string name)
        {
            if (string.IsNullOrEmpty(name))
                return name;

            var sb = new System.Text.StringBuilder();
            sb.Append(char.ToLowerInvariant(name[0]));
            for (int i = 1; i < name.Length; i++)
            {
                if (char.IsUpper(name[i]))
                {
                    sb.Append(' ');
                    sb.Append(char.ToLowerInvariant(name[i]));
                }
                else
                {
                    sb.Append(name[i]);
                }
            }
            return sb.ToString();
        }
    }
}
