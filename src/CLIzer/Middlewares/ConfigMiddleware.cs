using CLIzer.Contracts;
using CLIzer.Models;
using System.Diagnostics;

namespace CLIzer.Middlewares;

internal class ConfigMiddleware<T> : IClizerMiddleware where T : class, new()
{
    private readonly string _keyword;
    private readonly IConfig<T> _config;
    private readonly IClizerDataAccessor<T> _accessor;

    public ConfigMiddleware(IConfig<T> config, string keyword, IClizerDataAccessor<T> accessor)
    {
        _config = config;
        _keyword = keyword;
        _accessor = accessor;
    }

    public async Task<ClizerPostAction> Intercept(CommandContext context, CancellationToken cancellationToken)
    {
        var data = await _accessor.Load(cancellationToken);
        if (_config is ConfigProvider<T> config)
            config.Value = data ?? new();

        if (context.Command is not null 
            || !context.Execute.Equals(_keyword, StringComparison.OrdinalIgnoreCase))
            return ClizerPostAction.CONTINUE;

        if (_accessor.Source is null)
            return ClizerPostAction.EXIT;

        Process.Start(new ProcessStartInfo(_accessor.Source) { UseShellExecute = true });
        return ClizerPostAction.EXIT;
    }
}
