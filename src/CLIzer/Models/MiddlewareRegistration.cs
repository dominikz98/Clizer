using CLIzer.Contracts;

namespace CLIzer.Models
{
    internal class MiddlewareRegistration
    {
        public Type Type { get; }
        public Func<IServiceProvider, IClizerMiddleware>? Factory { get; }

        public MiddlewareRegistration(Type middleware, Func<IServiceProvider, IClizerMiddleware>? factory = null)
        {
            Type = middleware;
            Factory = factory;
        }
    }
}
