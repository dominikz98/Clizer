using CLIzer.Attributes;
using CLIzer.Contracts;
using CLIzer.Extensions;
using CLIzer.Middlewares;
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
        public async Task<ClizerExitCode> Execute(string[] args)
        {
            try
            {
                // remove empty args
                args = args.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

                // register cancellation source
                var cancellation = new CancellationTokenSource();
                Console.CancelKeyPress += new ConsoleCancelEventHandler((sender, args) => cancellation.Cancel());

                // register services, commands and middlewares
                var services = RegisterAllDependencies();

                // replace aliases with full command names
                var aliasesResolver = services.GetService<AliasesResolver>();
                if (aliasesResolver != null)
                {
                    await aliasesResolver.LoadAliases(cancellation.Token);
                    args = aliasesResolver.ReplaceAliasesWithCommands(args);
                }

                // resolve command to be executed
                args = SetCalledCommand(services, args);
                var resolver = services.GetRequiredService<CommandResolver>();
                var dynamicHelpCommandCalled = _configuration.Middlewares.Any(x => typeof(HelpMiddleware) != null)
                    && args.Any(x => x.Equals("help", StringComparison.OrdinalIgnoreCase));

                if (resolver.Called is null && !dynamicHelpCommandCalled)
                {
                    FindSimilarCommands(services, args);
                    return ClizerExitCode.ERROR;
                }

                // execute middlewares
                var exit = false;
                foreach (var type in _configuration.Middlewares)
                {
                    var middleware = (IClizerMiddleware)services.GetRequiredService(type);
                    var result = await middleware.Intercept(resolver, args, cancellation.Token);
                    if (result == ClizerPostAction.EXIT)
                        exit = true;
                }
                if (exit)
                    return ClizerExitCode.SUCCESS;

                // validate and attach passed arguments to command
                AttachAndValidateArguments(services, args);

                // execute called command
                var instance = services.GetRequiredService(resolver.Called.Type);
                return await ((ICliCmd)instance).Execute(cancellation.Token);
            }
            catch (Exception ex)
            {
                _configuration.ExceptionHandler(ex);
                return ClizerExitCode.ERROR;
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

            _configuration.Services.AddSingleton<CommandResolver>();

            return _configuration.Services.BuildServiceProvider();
        }

        private void InjectCommand(CommandRegistration command)
        {
            _configuration.Services!.AddSingleton(command.Type);
            foreach (var subcommand in command.Commands)
                InjectCommand(subcommand);
        }

        private string[] SetCalledCommand(IServiceProvider services, string[] args)
        {
            var parameter = new List<string>();
            var resolver = services.GetRequiredService<CommandResolver>();
            var parent = _configuration.Container!.RootCommand;
            var command = _configuration.Container!.RootCommand;
            command.Commands = _configuration.Container.Commands;

            // exclude options and arguments
            var commandArgs = args.ToList()
                .Where(x => !x.Contains('-'))
                .Where(x => !x.Contains(':'))
                .Select(x => x.ToLower())
                .ToArray();

            foreach (var arg in commandArgs)
            {
                var nextCommand = command.Commands.FirstOrDefault(x => x.Name.Equals(arg));
                if (nextCommand is null)
                {
                    parent = command;
                    command = null;
                    break;
                }

                args = args.Where(x => x != arg).ToArray();
                parent = command;
                command = nextCommand;
            }

            resolver.Parent = parent;
            resolver.Called = command;
            return args;
        }

        private void FindSimilarCommands(IServiceProvider services, string[] args)
        {
            var resolver = services.GetRequiredService<CommandResolver>();
            if (resolver.Parent is null)
                return;

            if (resolver.Parent == _configuration.Container!.RootCommand)
                return;

            Console.WriteLine($"Unknown command for {resolver.Parent.Name}. See '--help'");

            // list similar commands
            var parent = args.FirstOrDefault() ?? string.Empty;
            var similarCommands = resolver.Parent
                .GetAll()
                .Select(x => x.Name.ToLower())
                .Where(x => LevenshteinDistance.Compute(parent, x) <= 3);

            if (similarCommands.Any())
            {
                Console.WriteLine("Most similar commands:");
                foreach (var similar in similarCommands)
                    Console.WriteLine($" - {similar}");
            }

            // list similar aliases
            var aliasesResolver = services.GetRequiredService<AliasesResolver>();
            if (aliasesResolver is null)
                return;

            var similarAliases = aliasesResolver
                .Aliases
                .Select(x => x.Name.ToLower())
                .Where(x => LevenshteinDistance.Compute(parent, x) <= 2);

            if (!similarAliases.Any())
                return;

            Console.WriteLine(string.Empty);
            Console.WriteLine("Most similar aliases:");
            foreach (var similar in similarAliases)
                Console.WriteLine($" - {similar}");
        }

        private static void AttachAndValidateArguments(IServiceProvider services, string[] parameters)
        {
            var resolver = services.GetRequiredService<CommandResolver>();
            var cmdinstance = services.GetRequiredService(resolver.Called!.Type);

            foreach (var parameter in parameters)
            {
                var argname = parameter.Split(':')[0];
                var property = resolver.Called.Type
                    .GetCliProperties()
                    .Where(x => x.GetCustomAttribute<CliIArgAttribute>()?.Name?.ToLower() == argname
                        || x.GetCustomAttribute<CliIArgAttribute>()?.Shortcut.ToLower() == argname)
                    .FirstOrDefault();

                if (property == null)
                    throw new ClizerException($"{argname} is not a known property of {(!string.IsNullOrEmpty(resolver.Called.Name) ? resolver.Called.Name : resolver.Called.Type.Name)}!");

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
    }
}
