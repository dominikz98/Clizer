using Clizer.Attributes;
using Clizer.Helper;
using Clizer.Models;
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

        /// <summary>
        /// Entry point.
        /// </summary>
        public async Task Execute(string[] args, CancellationToken cancellationToken = default)
        {
            try
            {
                // Get command classes from entry assembly
                var clicmds = GetCliCmdsFromAssembly();

                // Get lowest matching command and 
                var lastclicmd = GetLowestMatchingCommand(args, clicmds);

                // Create command instance
                var clicmdinstance = CreateCliCmdInstance(lastclicmd.Cmd.Class);

                // Attach property values to command instance
                AttachCliPropertyValues(clicmdinstance, lastclicmd.Args);

            }
            catch (ClizerException cex)
            {
                Console.ForegroundColor = _configuration.ExceptionColor;
                Console.WriteLine(cex.Message);
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Configure Clizer
        /// </summary>
        public void Configure(ClizerConfiguration configuration) => _configuration = configuration;

        /// <summary>
        /// Verfies setup of cli commands, their method and properties
        /// </summary>
        public void Verify()
        {
            // TODO: implement
        }

        /// <summary>
        /// Returns classes with annotated CliCmdAttribute from calling assembly
        /// </summary>
        private IEnumerable<CliCmd> GetCliCmdsFromAssembly()
        {
            var cmds = Assembly.GetEntryAssembly().GetTypes().Where(x => x.GetCustomAttribute<CliCmdAttribute>() != null).Select(y => new CliCmd(y, y.GetCustomAttribute<CliCmdAttribute>()));
            if (cmds == null || cmds.Count() == 0)
                throw new ClizerException("No cli commands found!");
            return cmds;
        }

        /// <summary>
        /// Returns the last listed command from args. Throws error if called commands are hierarchic invalid or not found
        /// </summary>
        /// <param name="args">Called args</param>
        /// <param name="commands">All cli command classes</param>
        /// <returns>Last listed command with called options and arguments</returns>
        private (CliCmd Cmd, string[] Args) GetLowestMatchingCommand(string[] args, IEnumerable<CliCmd> commands)
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

            if (command == null)
                throw new ClizerException($"Command \"{args[0].ToLower()}\" not registered!");

            return (Cmd: command, Args: args.ToList().GetRange(level, args.Length - level).ToArray());
        }

        /// <summary>
        /// Creates an instance of given cli command type.
        /// </summary>
        private object CreateCliCmdInstance(Type clicmdtype)
        {
            var commandInstance = Activator.CreateInstance(clicmdtype);
            if (commandInstance == null)
                throw new ClizerException($"Unable to create instance of \"{clicmdtype.Name}\"!");
            return commandInstance;
        }

        /// <summary>
        /// Sets property values of cli command instance. Throws error is required properties are not setted or invalide value is passed.
        /// </summary>
        private void AttachCliPropertyValues(object cmdInstance, string[] args)
        {
            // TODO: Throw error if argument is not property of instance
            foreach (var property in cmdInstance.GetType().GetProperties().Where(x => x.GetCustomAttribute<CliPropertyAttribute>() != null))
            {
                var attribute = property.GetCustomAttribute<CliPropertyAttribute>();
                var value = args.Select(x => x.IgnoreCasing(_configuration.IgnoreStringCase)).FirstOrDefault(y => y == attribute.Name.IgnoreCasing(_configuration.IgnoreStringCase) || y == attribute.Short.IgnoreCasing(_configuration.IgnoreStringCase));

                if (attribute.Required && value == null)
                    throw new ClizerException($"Missing option \"{attribute.Name}\"!");

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
                    throw new ClizerException($"Invalid value for argument (Expected: {property.PropertyType.Name}, value: {value})");

                property.SetValue(cmdInstance, parsedvalue);
            }
        }

    }
}
