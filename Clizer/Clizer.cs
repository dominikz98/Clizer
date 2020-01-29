using Clizer.Attributes;
using Clizer.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Clizer
{
    public class Clizer
    {
        private ClizerConfiguration _configuration;

        public async Task Execute(string[] args, CancellationToken cancellationToken = default)
        {
            var entryAssembly = Assembly.GetEntryAssembly();

            // Get subcommand classes
            var commands = entryAssembly.GetTypes()
                .Where(x => x.GetCustomAttribute<CliCmdAttribute>() != null)
                .Select(y => new CliCmdInstance(y, y.GetCustomAttribute<CliCmdAttribute>()));

            // Get lowest matching command and 
            var command = GetLowestMatchingCommand(args, commands);
            if (command == default)
            {
                ThrowError($"Command \"{args[0].ToLower()}\" not registered!");
                return;
            }

            // Create command instance
            var commandInstance = Activator.CreateInstance(command.Cmd.Class);
            if (commandInstance == null)
            {
                ThrowError($"Unable to create instance of \"{command.Cmd.Class.Name}\"!");
                return;
            }

            // Attach property values to command instance
            var cliprops = command.Cmd.Class.GetProperties().Where(x => x.GetCustomAttribute<CliPropertyAttribute>() != null);
            if (!AttachCliPropertyValues(cliprops, command.Args, commandInstance))
                return;

            foreach (var property in commandInstance.GetType().GetProperties())
            {
                Console.WriteLine($"{property.Name}: {property.GetValue(commandInstance)}");
            }
        }

        public void Configure(ClizerConfiguration configuration) => _configuration = configuration;

        public void Verify()
        {
            // TODO: implement
        }

        private bool AttachCliPropertyValues(IEnumerable<PropertyInfo> properties, string[] args, object cmdInstance)
        {
            foreach (var property in properties)
            {
                var attribute = property.GetCustomAttribute<CliPropertyAttribute>();
                var value = args.Select(x => x.IgnoreCasing(_configuration.IgnoreStringCase)).FirstOrDefault(y => y == attribute.Name.IgnoreCasing(_configuration.IgnoreStringCase) || y == attribute.Short.IgnoreCasing(_configuration.IgnoreStringCase));

                if (attribute.Required && value == null)
                {
                    ThrowError($"Missing option \"{attribute.Name}\"!");
                    return false;
                }

                if (value == null)
                    continue;

                object parsedvalue = null;
                switch (attribute.Type)
                {
                    case CommandPropertyType.Option:
                        parsedvalue = true;
                        break;
                    case CommandPropertyType.Argument:
                        parsedvalue = Convert.ChangeType(value.Split(':')[1], property.PropertyType);
                        break;
                }

                if (parsedvalue == null)
                {
                    ThrowError($"Invalid value for argument (Expected: {property.PropertyType.Name}, value: {value})");
                    return false;
                }
                Console.WriteLine(parsedvalue);
                property.SetValue(cmdInstance, parsedvalue);
            }
            return true;
        }

        private (CliCmdInstance Cmd, string[] Args) GetLowestMatchingCommand(string[] args, IEnumerable<CliCmdInstance> commands)
        {
            // First argument
            var command = commands.FindCommand(args[0], _configuration.IgnoreStringCase, string.Empty);
            if (command == null) return default;

            var level = 1;
            while (command.Attribute.SubCommands?.Length > 0 && level < args.Length)
            {
                var subcommand = commands.FindCommand(args[level], _configuration.IgnoreStringCase, args[level - 1]);
                if (subcommand == null)
                    break;

                command = subcommand;
                level++;
            };
            return (Cmd: command, Args: args.ToList().GetRange(level, args.Length - level).ToArray());
        }

        private void ThrowError(string message)
        {
            Console.ForegroundColor = _configuration.ExceptionColor;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }

    public class ClizerConfiguration
    {
        public ConsoleColor ExceptionColor { get; private set; } = ConsoleColor.Red;
        public bool IgnoreStringCase { get; private set; } = true;

        public ClizerConfiguration SetExceptionColor(ConsoleColor color)
        {
            ExceptionColor = color;
            return this;
        }

        public ClizerConfiguration IgnoreLowerUpperCase(bool irgnore)
        {
            IgnoreStringCase = irgnore;
            return this;
        }
    }
}
