using CLIzer.Models;

namespace CLIzer.Utils
{
    public class CommandResolver
    {
        public CommandRegistration? Parent { get; set; }
        public CommandRegistration? Called { get; internal set; }
    }
}
