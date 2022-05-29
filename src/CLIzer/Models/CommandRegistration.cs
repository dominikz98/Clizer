namespace CLIzer.Models
{
    public class CommandRegistration
    {
        public Type Type { get; }
        public string Name { get; }
        public List<CommandRegistration> Commands { get; internal set; }

        internal CommandRegistration(Type type, string name)
        {
            Type = type;
            Name = name.ToLower();
            Commands = new List<CommandRegistration>();
        }

        internal List<CommandRegistration> GetAll()
        {
            var result = new List<CommandRegistration>
            {
                this
            };

            foreach (var command in Commands)
            {
                var subcommands = command.GetAll();
                result.AddRange(subcommands);
            }

            return result;
        }
    }
}
