using Clizer.Models;
using System.Collections.Generic;
using System.Linq;

namespace Clizer.Helper
{
    internal static class CliCmdExtensions
    {
        public static CliCmd FindCommand(this IEnumerable<CliCmd> commands, string name, bool ignorecase, string parent)
        {
            var command = commands.FindCommand(name, ignorecase);
            if (command == null) return null;

            if (string.IsNullOrEmpty(parent) && commands.Where(x => x.Attribute.SubCommands?.Contains(command.Class) ?? false)?.Count() > 0)
                return null;

            if (!string.IsNullOrEmpty(parent) && (!commands.FindCommand(parent, ignorecase)?.Attribute.SubCommands.Contains(command.Class) ?? true))
                return null;

            return command;
        }
        private static CliCmd FindCommand(this IEnumerable<CliCmd> commands, string name, bool ignorecase)
            => commands.FirstOrDefault(x => (ignorecase ? x.Attribute.Name.ToLower() : x.Attribute.Name) == (ignorecase ? name.ToLower() : name));
    }
}
