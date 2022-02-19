using CLIzer.Attributes;
using System.Reflection;

namespace CLIzer.Utils
{
    public static class TypeExtensions
    {
        public static List<CliIArgAttribute> GetArguments(this Type type)
            => type.GetProperties()
                .Where(x => x.PropertyType != typeof(bool))
                .Where(x => x.GetCustomAttribute<CliIArgAttribute>() != null)
                .Select(x => x.GetCustomAttribute<CliIArgAttribute>()!)
                .ToList();

        public static List<CliIArgAttribute> GetOptions(this Type type)
            => type.GetProperties()
                .Where(x => x.PropertyType == typeof(bool))
                .Where(x => x.GetCustomAttribute<CliIArgAttribute>() != null)
                .Select(x => x.GetCustomAttribute<CliIArgAttribute>()!)
                .ToList();

        public static List<PropertyInfo> GetCliProperties(this Type type)
            => type.GetProperties()
                .Where(x => x.GetCustomAttribute<CliIArgAttribute>() != null)
                .ToList();

        public static string? GetHelptext(this Type type)
            => type.GetCustomAttribute<CliHelpAttribute>()?.Helptext;
    }
}
