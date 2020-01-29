using Clizer.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Clizer.Helper
{
    internal class CliCmdInstance
    {
        public Type Class { get; set; }
        public CliCmdAttribute Attribute { get; set; }

        public CliCmdInstance(Type cClass, CliCmdAttribute attribute)
        {
            Class = cClass;
            Attribute = attribute;
        }

    }

    internal static class CliCmdInstanceExtensions
    {
        public static CliCmdInstance FindCommand(this IEnumerable<CliCmdInstance> commands, string name, bool ignorecase, string parent)
        {
            var command = commands.FindCommand(name, ignorecase);
            if (command == null) return null;

            if (string.IsNullOrEmpty(parent) && commands.Where(x => x.Attribute.SubCommands?.Contains(command.Class) ?? false)?.Count() > 0)
                return null;

            if (!string.IsNullOrEmpty(parent) && (!commands.FindCommand(parent, ignorecase)?.Attribute.SubCommands.Contains(command.Class) ?? true))
                return null;

            return command;
        }
        private static CliCmdInstance FindCommand(this IEnumerable<CliCmdInstance> commands, string name, bool ignorecase)
            => commands.FirstOrDefault(x => (ignorecase ? x.Attribute.Name.ToLower() : x.Attribute.Name) == (ignorecase ? name.ToLower() : name));
    }
}
