using CLIzer.Contracts;

namespace CLIzer.Models
{
    public class CommandContainer
    {
        public List<CommandRegistration> Commands { get; }
        internal CommandRegistration RootCommand { get; private set; }

        public CommandContainer()
        {
            Commands = new List<CommandRegistration>();
            RootCommand = new CommandRegistration(this, typeof(EmptyCommand), string.Empty);
        }

        public CommandContainer Root<TCommand>() where TCommand : ICliCmd
        {
            var command = new CommandRegistration(this, typeof(TCommand), string.Empty);
            RootCommand = command;
            return this;
        }

        public CommandRegistration Command<TCommand>(string name) where TCommand : ICliCmd
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Command name must be passed!");

            var command = new CommandRegistration(this, typeof(TCommand), name);
            Commands.Add(command);

            return command;
        }
    }

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
    }
}
