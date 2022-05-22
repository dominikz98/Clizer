using CLIzer.Contracts;

namespace CLIzer.Models
{
    public class CommandRegistration
    {
        public Type Type { get; }
        public string Name { get; }
        public List<CommandRegistration> Commands { get; internal set; }

        private readonly CommandContainer _container;

        internal CommandRegistration(CommandContainer parent, Type type, string name)
        {
            Type = type;
            Name = name.ToLower();
            Commands = new List<CommandRegistration>();
            _container = parent;
        }

        public CommandRegistration Command<TCommand>(string name) where TCommand : ICliCmd
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Command name must be passed!");

            var command = new CommandRegistration(_container, typeof(TCommand), name);
            Commands.Add(command);

            return this;
        }

        public CommandRegistration SubCommand<TCommand>(string name) where TCommand : ICliCmd
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Command name must be passed!");

            var command = new CommandRegistration(_container, typeof(TCommand), name);
            Commands.Add(command);

            return command;
        }

        public CommandContainer Return()
            => _container;

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
