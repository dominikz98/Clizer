using Clizer.Attributes;
using System.Collections.Generic;
using System.Linq;

namespace Clizer.Helper
{
    internal static class FormatExtensions
    {
        public static string IgnoreCasing(this string input, bool ignoreCase)
            => ignoreCase ? input.ToLower() : input;

        public static string FindArg(this string[] args, string propname, string propshort, bool ignorecase)
        {
            propname = propname.IgnoreCasing(ignorecase);
            propshort = propshort.IgnoreCasing(ignorecase);
            return args.Select(x => x.IgnoreCasing(ignorecase))
                        .FirstOrDefault(y => (y.Contains(":") ? y.Split(':')[0] : y) == propname || (y.Contains(":") ? y.Split(':')[0] : y) == propshort);
        }

        public static string[] GetUnkownArguments(this string[] args, IEnumerable<CliPropertyAttribute> attributes, bool ignorecase)
        {
            var unknown = args.Select(x => x.Contains(":") ? x.Split(':')[0] : x).Select(z => z.IgnoreCasing(ignorecase)).Except(attributes.Select(y => y.Name.IgnoreCasing(ignorecase)));
            return unknown.Except(attributes.Select(y => y.Short.IgnoreCasing(ignorecase))).ToArray();
        }
    }
}
