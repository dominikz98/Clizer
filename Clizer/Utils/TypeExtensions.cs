using Clizer.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Clizer.Utils
{
    public static class TypeExtensions
    {
        public static List<PropertyInfo> GetArguments(this Type type)
            => type.GetProperties().Where(x => x.PropertyType != typeof(bool) && x.GetCustomAttribute<CliIgnoreAttribute>() == null)?.ToList();

        public static List<PropertyInfo> GetOptions(this Type type)
            => type.GetProperties().Where(x => x.PropertyType == typeof(bool) && x.GetCustomAttribute<CliIgnoreAttribute>() == null)?.ToList();

        public static string GetHelptext(this Type type)
            => type.GetCustomAttribute<CliHelpAttribute>()?.Helptext;

        public static string GetHelptext(this PropertyInfo property)
            => property.GetCustomAttribute<CliHelpAttribute>()?.Helptext;
    }
}
