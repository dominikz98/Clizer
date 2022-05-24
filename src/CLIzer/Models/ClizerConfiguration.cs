using CLIzer.Contracts;
using CLIzer.Resolver;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace CLIzer.Models
{
    public class ClizerConfiguration
    {
        internal List<Action<IServiceCollection>> Dependencies { get; }
        internal IServiceCollection? Services { get; set; }
        internal CommandRegistration? RootCommand { get; set; }
        internal Action<Exception> ExceptionHandler { get; private set; } = (ex) => throw ex;
        internal List<MiddlewareRegistration> Middlewares { get; }


        public ClizerConfiguration()
        {
            Dependencies = new List<Action<IServiceCollection>>();
            Middlewares = new List<MiddlewareRegistration>();
        }

        public ClizerConfiguration RegisterMiddleware<T>(Func<IServiceProvider, T>? factory = null) where T : class, IClizerMiddleware
        {
            if (factory is null)
                Middlewares.Add(new MiddlewareRegistration(typeof(T), null));
            else
                Middlewares.Add(new MiddlewareRegistration(typeof(T), (sp) => factory(sp)));
            return this;
        }

        public ClizerConfiguration RegisterServices(IServiceCollection dependencies)
        {
            Services = dependencies;
            return this;
        }

        public ClizerConfiguration RegisterServices(Action<IServiceCollection> dependencies)
        {
            Dependencies.Add(dependencies);
            return this;
        }

        public ClizerConfiguration RegisterCommands(params Assembly[] assemblies)
            => RegisterCommands<EmptyCommand>(assemblies, new NameByAttributeOrDefaultResolver());
        public ClizerConfiguration RegisterCommands(Assembly[] assemblies, ICommandNameResolver nameResolver)
            => RegisterCommands<EmptyCommand>(assemblies, nameResolver);
        public ClizerConfiguration RegisterCommands<RootCmd>(params Assembly[] asemblies) where RootCmd : ICliCmd
            => RegisterCommands<RootCmd>(asemblies, new NameByAttributeOrDefaultResolver());

        public ClizerConfiguration RegisterCommands<RootCmd>(Assembly[] assemblies, ICommandNameResolver nameResolver) where RootCmd : ICliCmd
        {
            if (!assemblies.Contains(GetType().Assembly))
                assemblies = assemblies.Append(GetType().Assembly).ToArray();

            if (!assemblies.Contains(typeof(RootCmd).Assembly))
                assemblies = assemblies.Append(typeof(RootCmd).Assembly).ToArray();

            // get all commands
            var commands = new List<Type>();
            foreach (var assembly in assemblies)
                commands.AddRange(assembly
                    .GetTypes()
                    .Where(x => x.GetTypeInfo()
                        .ImplementedInterfaces
                        .Contains(typeof(ICliCmd)))
                    .Where(x => !x.IsInterface)
                    .Where(x => !x.IsAbstract)
                    .Where(x => x != typeof(RootCmd))
                    .ToList());

            // get entry commands
            var commandsWithoutParent = commands
                .Where(x => !x
                    .GetInterfaces()
                    .Where(y => y != typeof(ICliCmd))
                    .Where(y => y.GetInterface(typeof(ICliCmd).Name) != null)
                    .Any())
                .ToList();

            // register root command and resolve hierarchical
            var rootNameResolver = new PersistNameResolver<RootCmd>(string.Empty, nameResolver);
            RootCommand = AttachSubCommands(typeof(RootCmd), commands, rootNameResolver);

            // resolve subcommands hierarchical from custom entry point
            var remainingCommands = commands.Except(commandsWithoutParent).ToList();
            foreach (var command in commandsWithoutParent)
            {
                var registration = AttachSubCommands(command, remainingCommands, nameResolver);
                RootCommand.Commands.Add(registration);
            }

            return this;
        }

        private static CommandRegistration AttachSubCommands(Type command, List<Type> allCommandTypes, ICommandNameResolver nameResolver)
        {
            var subcommands = allCommandTypes
                .Where(x => x.GetTypeInfo()
                    .ImplementedInterfaces
                    .Contains(typeof(ICliCmd<>)
                        .MakeGenericType(command)))
                .ToList();

            var name = nameResolver.Resolve(command);
            var registration = new CommandRegistration(command, name);
            foreach (var subcommand in subcommands)
            {
                var subregistration = AttachSubCommands(subcommand, allCommandTypes, nameResolver);
                registration.Commands.Add(subregistration);
            }

            return registration;
        }

        public ClizerConfiguration HandleException(Action<Exception> handler)
        {
            ExceptionHandler = handler;
            return this;
        }
    }
}
