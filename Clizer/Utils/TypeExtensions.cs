using Clizer.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Clizer.Utils
{
    public static class TypeExtensions
    {
        public static List<CliIArgAttribute> GetArguments(this Type type)
            => type.GetProperties()
                    ?.Where(x => x.PropertyType != typeof(bool) && x.GetCustomAttribute<CliIArgAttribute>() != null)
                    ?.Select(x => x.GetCustomAttribute<CliIArgAttribute>()!)
                    ?.ToList() 
                    ?? new List<CliIArgAttribute>();

        public static List<CliIArgAttribute> GetOptions(this Type type)
            => type.GetProperties()
                    ?.Where(x => x.PropertyType == typeof(bool) && x.GetCustomAttribute<CliIArgAttribute>() != null)
                    ?.Select(x => x.GetCustomAttribute<CliIArgAttribute>()!)
                    ?.ToList() 
                    ?? new List<CliIArgAttribute>();

        public static List<PropertyInfo> GetCliProperties(this Type type)
            => type.GetProperties()
                    ?.Where(x => x.GetCustomAttribute<CliIArgAttribute>() != null)
                    ?.ToList() 
                    ?? new List<PropertyInfo>();

        public static string? GetHelptext(this Type type)
            => type.GetCustomAttribute<CliHelpAttribute>()?.Helptext;

        public static string? GetHelptext(this PropertyInfo property)
            => property.GetCustomAttribute<CliHelpAttribute>()?.Helptext;
    }
}
