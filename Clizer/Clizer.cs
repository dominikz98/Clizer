﻿using Clizer.Attributes;
using Clizer.Helper;
using Clizer.Models;
using SimpleInjector;
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

        public Clizer()
        {
            _configuration = new ClizerConfiguration();
        }

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

                // Show help information text in case of --help, -h
                if (CheckIfHelpIsRequired(Cmd, Args))
                    return;

                // Create command instance
                var clicmdinstance = CreateCliCmdInstance(Cmd.Class);

                // Attach property values to command instance
                AttachCliPropertyValues(clicmdinstance, Args);

                // Execute 'Execute' method from command instance
                await ExecuteCliCommandMethod(clicmdinstance, cancellationToken);
            }
            catch (Exception ex)
            {
                _configuration.ExceptionHandler(ex);
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
            try
            {
                // CliCommand:
                var clicmds = GetCliCmdsFromAssembly();

                // - min 1 command
                if ((clicmds?.Count() ?? 0) == 0)
                    throw new ClizerException($"At leaset one public class must be a cli command with an {nameof(CliCmdAttribute)}!");

                foreach (var clicmd in clicmds)
                {
                    // Sub commands have attributes
                    if (clicmd.Attribute.SubCommands != null)
                        foreach (var subcmd in clicmd.Attribute.SubCommands)
                            if (subcmd.GetCustomAttribute<CliCmdAttribute>() == null)
                                throw new ClizerException($"\"{clicmd.Class.Name}\": Subcommand is missing {nameof(CliCmdAttribute)}!");

                    // - 1 constructor
                    if ((clicmd.Class.GetConstructors()?.Count() ?? 2) > 1)
                        throw new ClizerException($"\"{clicmd.Class.Name}\": Cli commands only allowed to have 1 constructor!");

                    // - 1 public Task Execute(CancellationToken cancellationToken) method
                    var execmethod = clicmd.Class.GetMethod("Execute");
                    if (execmethod == null || execmethod.GetParameters()?.Count() != 1 || execmethod.GetParameters().FirstOrDefault().ParameterType != typeof(CancellationToken))
                        throw new ClizerException($"\"{clicmd.Class.Name}\": Cli commands must have a method \"Task Execute(CancellationToken cancellationToken)\"!");

                    // CliOption:
                    foreach (var cliprop in clicmd.Class.GetProperties().Where(x => x.GetCustomAttribute<CliOptionAttribute>() != null))
                    {
                        // - type == bool
                        if (cliprop.PropertyType != typeof(bool))
                            throw new ClizerException($"\"{clicmd.Class.Name}.{cliprop.Name}\": Cli options must be type bool, actual: {cliprop.PropertyType.Name}!");

                        var attr = cliprop.GetCustomAttribute<CliOptionAttribute>();

                        // - name == -h || name == --help (override)
                        if (attr.Name.ToLower() == "--help" || attr.Short == "-h")
                            throw new ClizerException($"\"{clicmd.Class.Name}.{cliprop.Name}\": Help option will be automatically generated, to define help messages use properties in {nameof(CliOptionAttribute)} and {nameof(CliArgumentAttribute)}!");

                        // - name == -ed || name == --editor (override)
                        if (attr.Name.ToLower() == "--editor" || attr.Short == "-ed")
                            throw new ClizerException($"\"{clicmd.Class.Name}.{cliprop.Name}\": Editor option will be automatically generated!");
                    }
                }

                // SimpleInjector
                _configuration.DependencyContainer?.Verify();
            }
            catch (Exception ex)
            {
                _configuration.ExceptionHandler(ex);
            }
        }

        /// <summary>
        /// Lists all recognized commands, their subcommands and properties
        /// </summary>
        public void Dump()
        {
            foreach (var clicmd in GetCliCmdsFromAssembly())
            {
                Console.WriteLine(clicmd.Attribute.Name);
                Console.WriteLine(string.Empty.PadLeft(clicmd.Attribute.Name.Length, '-'));
                if (clicmd.Attribute.SubCommands?.Count() > 0) Console.WriteLine("Sub: " + string.Join(";", clicmd.Attribute.SubCommands.Select(x => x.GetCustomAttribute<CliCmdAttribute>().Name)));
                foreach (var cliprop in clicmd.Class.GetProperties().Where(x => x.GetCustomAttribute<CliPropertyAttribute>() != null))
                    Console.WriteLine(" - " + cliprop.GetCustomAttribute<CliPropertyAttribute>().Name);

                Console.WriteLine(string.Empty);
            }
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
        /// Generates and displays help text if required.
        /// </summary>
        private bool CheckIfHelpIsRequired(CliCmd cmd, string[] args)
        {
            if (args.Select(x => x.IgnoreCasing(_configuration.IgnoreStringCase)).FirstOrDefault(y => y == "--help" || y == "-h") == null)
                return false;

            // Write command help text
            var cmdhelp = $"{cmd.Attribute.Name}{(string.IsNullOrEmpty(cmd.Attribute.Help) ? string.Empty : ": " + cmd.Attribute.Help)}";
            Console.WriteLine(cmdhelp);
            Console.WriteLine(string.Empty.PadLeft(cmdhelp.Length, '-'));

            // Write properties help text
            var props = cmd.Class.GetProperties().Where(x => x.GetCustomAttribute<CliPropertyAttribute>() != null).Select(y => y.GetCustomAttribute<CliPropertyAttribute>());
            foreach (var prop in props)
                Console.WriteLine($"{prop.Name}: {prop.Help}");

            return true;
        }

        /// <summary>
        /// Creates an instance of given cli command type.
        /// </summary>
        private object CreateCliCmdInstance(Type clicmdtype)
        {
            if (clicmdtype.GetConstructors()?.Count() > 1)
                throw new ClizerException($"\"{clicmdtype.Name}\" contains more than 1 constructor!");

            var constructor = clicmdtype.GetConstructors().FirstOrDefault();
            var parameters = new List<object>();
            foreach (var parameter in constructor.GetParameters())
            {
                object injection = null;
                try
                {
                    injection = _configuration.DependencyContainer.GetInstance(parameter.ParameterType);
                }
                catch (ActivationException)
                {

                    throw new ClizerException($"\"{clicmdtype.Name}\" needs an instance of \"{parameter.ParameterType}\" which is not registered!");
                }

                parameters.Add(injection);
            }

            var commandInstance = Activator.CreateInstance(clicmdtype, parameters.ToArray());
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
