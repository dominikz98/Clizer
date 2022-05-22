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

        internal List<CommandRegistration> GetAll()
        {
            var result = new List<CommandRegistration>();
            if (RootCommand is not null)
                result.Add(RootCommand);

            foreach (var command in Commands)
            {
                var subcommands = command.GetAll();
                result.AddRange(subcommands);
            }

            return result;
        }
    }
}
