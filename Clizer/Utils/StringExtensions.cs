namespace Clizer.Utils
{
    public static class StringExtensions
    {
        public static string IgnoreCase(this string value, bool ignorecase)
            => ignorecase ? value.ToLower() : value;

        public static string GetArgName(this string value, bool ignorecase)
            => value.Contains(":") ? value.Split(':')[0].IgnoreCase(ignorecase) : value.IgnoreCase(ignorecase);
    }
}
