using CLIzer.Contracts;
using CLIzer.Models;

namespace CLIzer.Utils
{
    internal class AliasesResolver
    {
        private readonly List<Alias> _aliases = new();
        private readonly IClizerFileAccessor<string> _file;

        public AliasesResolver(IClizerFileAccessor<string> file)
        {
            _file = file;
        }

        public async Task LoadAliases(CancellationToken cancellationToken)
        {
            var entries = (await _file.Load(cancellationToken))
                ?.Split(Environment.NewLine)
                .ToList()
                ?? new List<string>();

            foreach (var entry in entries)
            {
                if (!entry.Contains('='))
                    continue;

                var parts = entry.Split('=');
                if (parts.Length != 2)
                    continue;

                var name = parts[0].Trim().ToLower();
                var commands = parts[1].Split(" ")
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => x.Trim().ToLower())
                    .ToList();

                _aliases.Add(new Alias(name, commands));
            }
        }

        public string[] ReplaceAliasesWithCommands(string[] args)
        {
            var result = new List<string>();
            foreach (var arg in args)
            {
                var commands = ReplaceAliasesWithCommands(arg);
                result.AddRange(commands);
            }
            return result.ToArray();
        }

        private string[] ReplaceAliasesWithCommands(string arg)
        {
            var alias = _aliases.FirstOrDefault(x => x.Name.Equals(arg, StringComparison.OrdinalIgnoreCase));
            if (alias is null)
                return new string[] { arg };

            var result = new List<string>();
            foreach (var command in alias.Commands)
            {
                var subcommands = ReplaceAliasesWithCommands(command);
                result.AddRange(subcommands);
            }

            return result.ToArray();
        }
    }
}
