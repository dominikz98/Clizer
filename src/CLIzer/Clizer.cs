using CLIzer.Attributes;
using CLIzer.Contracts;
using CLIzer.Models;
using CLIzer.Utils;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace CLIzer
{
    public class Clizer
    {
        private ClizerConfiguration _configuration;

        public Clizer(ClizerConfiguration config)
        {
            _configuration = config;
        }

        public Clizer() : this(new()) { }

        /// <summary>
        /// Entry point.
        /// </summary>
        public async Task<int> Execute(string[] args, CancellationToken cancellationToken)
        {
            try
            {
                var services = RegisterAllDependencies();

                var (command, parameters) = GetCalledCommand(args);

                if (command is null)
                    return (int)ClizerExitCodes.ERROR;

                if (parameters.Intersect(ClizerConstants.Help).Any())
                    return ShowHelptext(services, command);

                foreach (var type in _configuration.Middlewares)
                {
                    var middleware = (IClizerMiddleware)services.GetRequiredService(type);
                    await middleware.Intercept(parameters.ToArray(), cancellationToken);
                }


                AttachAndValidateArguments(services, command, parameters);

                var instance = services.GetRequiredService(command.Type);
                return await ((ICliCmd)instance).Execute(cancellationToken);
            }
            catch (Exception ex)
            {
                _configuration.ExceptionHandler(ex);
                return (int)ClizerExitCodes.ERROR;
            }
        }

        public Clizer Configure(Action<ClizerConfiguration> config)
        {
            config(_configuration);
            return this;
        }

        public Clizer Configure(ClizerConfiguration config)
        {
            _configuration = config;
            return this;
        }

        private IServiceProvider RegisterAllDependencies()
        {
            // register user and extension services
            _configuration.Services ??= new ServiceCollection();
            foreach (var dependency in _configuration.Dependencies)
                dependency(_configuration.Services);

            // register middlewares
            foreach (var middleware in _configuration.Middlewares)
                _configuration.Services.AddSingleton(middleware);

            // register commands
            _configuration.Container ??= new CommandContainer();
            foreach (var command in _configuration.Commands)
                command(_configuration.Container);

            // inject commands
            InjectCommand(_configuration.Container.RootCommand);
            foreach (var command in _configuration.Container.Commands)
                InjectCommand(command);

            return _configuration.Services.BuildServiceProvider();
        }

        private void InjectCommand(CommandRegistration command)
        {
            _configuration.Services!.AddSingleton(command.Type);
            foreach (var subcommand in command.Commands)
                InjectCommand(subcommand);
        }

        private (CommandRegistration? command, string[] parameters) GetCalledCommand(string[] args)
        {
            var parameter = new List<string>();
            var command = _configuration.Container!.RootCommand;
            command.Commands = _configuration.Container.Commands;
            args = args.ToList()
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.ToLower())
                .ToArray();

            for (int i = 0; i < args.Length; i++)
            {
                var nextCommand = command.Commands.FirstOrDefault(x => x.Name.Equals(args[i]));
                if (nextCommand is null)
                {
                    args = args.ToList().GetRange(i, args.Length - i).ToArray();
                    return (command, args);
                }

                command = nextCommand;
            }

            return (command, Array.Empty<string>());
        }

        private static void AttachAndValidateArguments(IServiceProvider services, CommandRegistration command, string[] parameters)
        {
            if (command == null)
                return;

            var cmdinstance = services.GetRequiredService(command.Type);

            foreach (var parameter in parameters)
            {
                var argname = parameter.Split(':')[0];
                var property = command.Type
                    .GetCliProperties()
                    .Where(x => x.GetCustomAttribute<CliIArgAttribute>()?.Name?.ToLower() == argname
                        || x.GetCustomAttribute<CliIArgAttribute>()?.Shortcut.ToLower() == argname)
                    .FirstOrDefault();

                if (property == null)
                    throw new ClizerException($"{argname} is not a known property of {(!string.IsNullOrEmpty(command.Name) ? command.Name : command.Type.Name)}!");

                // Argument
                if (parameter.Contains(':'))
                {
                    var firstindex = parameter.IndexOf(':') + 1;
                    if (parameter.Length - firstindex <= 0)
                        throw new ClizerException($"{parameter} has an invalid argument format!");

                    var argvalue = parameter[firstindex..];
                    try
                    {
                        property.SetValue(cmdinstance, Convert.ChangeType(argvalue, property.PropertyType));
                        continue;
                    }
                    catch (Exception)
                    {
                        throw new ClizerException($"Invalid value for argument (Expected: {property.PropertyType.Name}, value: {parameter})");
                    }
                }

                // Option
                property.SetValue(cmdinstance, true);
            }

            // Validate
            var properties = cmdinstance.GetType()
                .GetProperties()
                .Where(x => x.GetCustomAttribute<CliIArgAttribute>() != null);

            foreach (var property in properties)
            {
                var validations = property.GetCustomAttributes(false)
                    .Where(x => x is ValidationAttribute);

                foreach (var validateattr in validations)
                    ((ValidationAttribute)validateattr).Validate(property.GetValue(cmdinstance), property.Name);
            }
        }

        private static int ShowHelptext(IServiceProvider services, CommandRegistration command)
        {
            var cmdinstance = services.GetRequiredService(command.Type);

            Console.WriteLine(command.Name + (!string.IsNullOrEmpty(command.Type.GetHelptext()) ? ": " + command.Type.GetHelptext() : string.Empty));
            Console.WriteLine(string.Empty);

            var children = command.Commands;
            if (children.Any())
            {
                Console.WriteLine("[Commands]");
                Console.WriteLine(string.Join(Environment.NewLine, children.Select(x => $" {x.Name + (!string.IsNullOrEmpty(x.Type.GetHelptext()) ? ": " + x.Type.GetHelptext() : string.Empty)}").ToArray()));
                Console.WriteLine(string.Empty);
            }

            var arguments = command.Type.GetArguments();
            if (arguments.Any())
            {
                Console.WriteLine("[Arguments]");
                Console.WriteLine(string.Join(Environment.NewLine, arguments.Select(x => $" { (x.Name + (!string.IsNullOrEmpty(x.Shortcut) ? " | " + x.Shortcut : string.Empty) + (!string.IsNullOrEmpty(x.Helptext) ? ": " + x.Helptext : string.Empty))}").ToArray()));
                Console.WriteLine(string.Empty);
            }

            var options = command.Type.GetOptions();
            if (options.Any())
            {
                Console.WriteLine("[Options]");
                Console.WriteLine(string.Join(Environment.NewLine, options.Select(x => $" {(x.Name + (!string.IsNullOrEmpty(x.Shortcut) ? " | " + x.Shortcut : string.Empty) + (!string.IsNullOrEmpty(x.Helptext) ? ": " + x.Helptext : string.Empty))}").ToArray()));
            }

            return (int)ClizerExitCodes.SUCCESS;
        }
    }
}
