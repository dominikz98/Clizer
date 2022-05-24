using CLIzer.Attributes;
using CLIzer.Contracts;
using CLIzer.Extensions;
using CLIzer.Models;
using CLIzer.Resolver;
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
                var context = CreateCommandContext(services, args);

                // execute middlewares
                var exit = false;
                var middlewares = services.GetServices<IClizerMiddleware>();
                foreach (var middleware in middlewares)
                {
                    var result = await middleware.Intercept(context, cancellation.Token);
                    if (result == ClizerPostAction.EXIT)
                        exit = true;
                }
                if (exit)
                    return ClizerExitCode.SUCCESS;

                if (context.Command is null)
                    return ClizerExitCode.ERROR;

                // validate and attach passed arguments to command
                AttachAndValidateArguments(services);

                // execute called command
                var instance = services.GetRequiredService(context.Command.Type);
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
                if (middleware.Factory is not null)
                    _configuration.Services.AddSingleton(middleware.Factory);
                else
                    _configuration.Services.AddSingleton(typeof(IClizerMiddleware), middleware.Type);

            // inject commands
            _configuration.RootCommand ??= new CommandRegistration(typeof(EmptyCommand), string.Empty);
            foreach (var command in _configuration.RootCommand!.GetAll().Distinct())
                _configuration.Services!.AddSingleton(command.Type);

            // internal services
            _configuration.Services.AddSingleton<CommandContext>();

            return _configuration.Services.BuildServiceProvider();
        }

        private CommandContext CreateCommandContext(IServiceProvider services, string[] args)
        {
            var callChain = new List<CommandRegistration>();
            var context = services.GetRequiredService<CommandContext>();
            context.RootCommand = _configuration.RootCommand!;
            var command = _configuration.RootCommand!;

            // exclude options and arguments
            var commandArgs = args
                .Where(x => !x.Contains('-'))
                .Where(x => !x.Contains(':'))
                .ToList() ?? new List<string>();

            foreach (var arg in commandArgs)
            {
                context.Execute = arg;
                var nextCommand = command.Commands.FirstOrDefault(x => x.Name.Equals(arg, StringComparison.OrdinalIgnoreCase));
                if (nextCommand is null)
                {
                    command = null;
                    break;
                }

                callChain.Add(command);
                command = nextCommand;
            }

            context.CallChain = callChain;
            context.Args = args.Except(commandArgs).ToList();
            context.Command = command;
            return context;
        }

        private static void AttachAndValidateArguments(IServiceProvider services)
        {
            var context = services.GetRequiredService<CommandContext>();
            var cmdinstance = services.GetRequiredService(context.Command!.Type);

            foreach (var parameter in context.Args)
            {
                var argname = parameter.Split(':')[0];
                var property = context.Command.Type
                    .GetCliProperties()
                    .Where(x => x.GetCustomAttribute<CliIArgAttribute>()?.Name?.ToLower() == argname
                        || x.GetCustomAttribute<CliIArgAttribute>()?.Shortcut.ToLower() == argname)
                    .FirstOrDefault();

                if (property == null)
                    throw new ClizerException($"{argname} is not a known property of {(!string.IsNullOrEmpty(context.Command.Name) ? context.Command.Name : context.Command.Type.Name)}!");

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
