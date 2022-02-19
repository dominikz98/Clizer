using CLIzer.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace CLIzer.Models
{
    public class ClizerConfiguration
    {
        internal List<Action<IServiceCollection>> Dependencies { get; }
        internal IServiceCollection? Services { get; set; }
        internal List<Action<CommandContainer>> Commands { get; }
        internal CommandContainer? Container { get; set; }
        internal Action<Exception> ExceptionHandler { get; private set; } = (ex) => throw ex;
        internal List<Type> Middlewares { get; }

        public ClizerConfiguration()
        {
            Commands = new List<Action<CommandContainer>>();
            Dependencies = new List<Action<IServiceCollection>>();
            Middlewares = new List<Type>();
        }

        public ClizerConfiguration RegisterMiddleware<T>() where T : class, IClizerMiddleware
        {
            Middlewares.Add(typeof(T));
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

        public ClizerConfiguration RegisterCommands(CommandContainer commands)
        {
            Container = commands;
            return this;
        }

        public ClizerConfiguration RegisterCommands(Action<CommandContainer> commands)
        {
            Commands.Add(commands);
            return this;
        }

        public ClizerConfiguration HandleException(Action<Exception> handler)
        {
            ExceptionHandler = handler;
            return this;
        }
    }
}
