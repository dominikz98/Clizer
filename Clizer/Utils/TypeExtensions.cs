using Clizer.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Clizer.Utils
{
    public static class TypeExtensions
    {
        public static List<PropertyInfo> GetArguments(this Type type)
            => type.GetProperties().Where(x => x.PropertyType != typeof(bool) && x.GetCustomAttribute<CliIgnoreAttribute>() == null)?.ToList();

        public static List<PropertyInfo> GetOptions(this Type type)
            => type.GetProperties().Where(x => x.PropertyType == typeof(bool) && x.GetCustomAttribute<CliIgnoreAttribute>() == null)?.ToList();
    }
}
