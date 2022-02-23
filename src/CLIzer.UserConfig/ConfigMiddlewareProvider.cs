using CLIzer.Contracts;
using CLIzer.Utils;

namespace CLIzer.UserConfig
{
    internal class ConfigMiddlewareProvider<T> : IClizerMiddleware where T : class, new()
    {
        private readonly IConfig<T> _config;
        private readonly IClizerFileAccessor<T> _accessor;

        public ConfigMiddlewareProvider(IConfig<T> config, IClizerFileAccessor<T> accessor)
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
