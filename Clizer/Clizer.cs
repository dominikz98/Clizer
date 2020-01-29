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
                var (Cmd, Args) = GetLowestMatchingCommand(args, clicmds);

                // Create command instance
                var clicmdinstance = CreateCliCmdInstance(Cmd.Class);

                // Attach property values to command instance
                AttachCliPropertyValues(clicmdinstance, Args);

                // Execute 'Execute' method from command instance
                await ExecuteCliCommandMethod(clicmdinstance, cancellationToken);

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
            // Check args
            var properties = cmdInstance.GetType().GetProperties().Where(x => x.GetCustomAttribute<CliPropertyAttribute>() != null);
            foreach (var unknown in args.GetUnkownArguments(properties.Select(x => x.GetCustomAttribute<CliPropertyAttribute>()), _configuration.IgnoreStringCase))
                throw new ClizerException($"Unknown argument/option \"{unknown}\"");

            // Set property values
            foreach (var property in properties)
            {
                var optionatr = property.GetCustomAttribute<CliOptionAttribute>();
                var argumentatr = property.GetCustomAttribute<CliArgumentAttribute>();
                var arg = args.FindArg((optionatr?.Name ?? argumentatr.Name), (optionatr?.Short ?? argumentatr.Short), _configuration.IgnoreStringCase);
                object value = null;

                if (optionatr != null)
                    value = !string.IsNullOrEmpty(arg);

                else if (argumentatr != null)
                {
                    if (argumentatr.Required && string.IsNullOrEmpty(arg))
                        throw new ClizerException($"Missing value for required argument \"{argumentatr.Name}\"!");

                    if (!arg.Contains(":") || (arg.Split(':')?[1].Length ?? 0) <= 0)
                        throw new ClizerException($"Value of called argument \"{argumentatr.Name}\" cannot be empty!");

                    try
                    {
                        value = Convert.ChangeType(arg.Split(':')?[1], property.PropertyType);
                    }
                    catch (Exception)
                    {
                        throw new ClizerException($"Invalid value for argument (Expected: {property.PropertyType.Name}, value: {arg})");
                    }
                }

                property.SetValue(cmdInstance, value);
            }
        }

        /// <summary>
        /// Executes the entry point method in cli command instance
        /// </summary>
        private async Task ExecuteCliCommandMethod(object cmdInstance, CancellationToken cancellationToken)
            => await (Task)cmdInstance.GetType().GetMethod("Execute")?.Invoke(cmdInstance, new object[] { cancellationToken });

    }
}
