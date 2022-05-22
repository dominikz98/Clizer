using CLIzer.Contracts;
using CLIzer.Models;
using CLIzer.Utils;

namespace CLIzer.Middlewares
{
    internal class ConfigMiddleware<T> : IClizerMiddleware where T : class, new()
    {
        private readonly IConfig<T> _config;
        private readonly IClizerDataAccessor<T> _accessor;

        public ConfigMiddleware(IConfig<T> config, IClizerDataAccessor<T> accessor)
        {
            _config = config;
            _accessor = accessor;
        }

        public async Task<ClizerPostAction> Intercept(CommandResolver commandResolver, string[] args, CancellationToken cancellationToken)
        {
            var data = await _accessor.Load(cancellationToken);
            if (_config is ConfigProvider<T> config)
                config.Value = data ?? new();

            return ClizerPostAction.CONTINUE;
        }
    }
}
