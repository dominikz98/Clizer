using System;
using System.Collections.Generic;
using System.Text;

namespace Clizer.Helper
{
    internal static class FormatExtensions
    {
        public static string IgnoreCasing(this string input, bool ignoreCase)
            => ignoreCase ? input.ToLower() : input;
    }
}
