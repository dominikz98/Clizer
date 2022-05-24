namespace CLIzer.Models
{
    public class CommandContext
    {
        public CommandRegistration? RootCommand { get; set; }
        public CommandRegistration? Command { get; internal set; }
        public string Execute { get; set; }
        public IReadOnlyCollection<CommandRegistration> CallChain { get; internal set; }
        public IReadOnlyCollection<string> Args { get; internal set; }

        public CommandContext()
        {
            Execute = string.Empty;
            CallChain = new List<CommandRegistration>();
            Args = new List<string>();
        }
    }
}
