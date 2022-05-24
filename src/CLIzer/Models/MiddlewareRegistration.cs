namespace CLIzer.Models
{
    internal class MiddlewareRegistration
    {
        public Type Type { get; }
        public Func<IServiceProvider, object>? Factory { get; }

        public MiddlewareRegistration(Type middleware, Func<IServiceProvider, object>? factory = null)
        {
            Type = middleware;
            Factory = factory;
        }
    }
}
