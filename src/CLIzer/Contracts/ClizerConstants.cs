namespace CLIzer.Contracts
{
    public class ClizerConstants
    {
        public const string ConfigCommand = "config";

        public const string HelpCommand = "help";
        public const string HelpOption = "--help";
        public const string HelpOptionShortcut = "-h";
        public static IReadOnlyCollection<string> Help = new List<string>() { HelpCommand, HelpOption, HelpOptionShortcut };

        public const string MappingFile = "mappings.json";
    }
}
