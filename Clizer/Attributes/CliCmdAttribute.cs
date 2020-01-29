using System;

namespace Clizer.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class CliCmdAttribute : Attribute
    {
        public string Name { get; }
        public bool DisableHelp { get; set; } = false;
        public Type[] SubCommands { get; set; }

        public CliCmdAttribute(string name)
        {
            Name = name;
        }

        public CliCmdAttribute(string name, params Type[] subcommands)
        {
            Name = name;
            SubCommands = subcommands;
        }
    }
}
