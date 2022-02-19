using CLIzer.Contracts;
using CLIzer.Models;
using CLIzer.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace CLIzer.UserConfig
{
    public static class ClizerConfigurationExtension
    {
        public static ClizerConfiguration RegisterConfig<TConfig>(this ClizerConfiguration configuration, string name, string relativePath) where TConfig : class, new()
            => RegisterConfig(configuration, name, new JsonFileByRelativePathAccessor<TConfig>(relativePath));

        public static ClizerConfiguration RegisterConfig<TConfig>(this ClizerConfiguration configuration, string name, IClizerFileAccessor<TConfig> fileAccessor) where TConfig : class, new()
        {
            configuration.RegisterServices((services) =>
            {
                services.AddSingleton((_) => fileAccessor);
                services.AddSingleton<IConfig<TConfig>, ConfigProvider<TConfig>>();
            });

            configuration.RegisterCommands((container) =>
            {
                container.Command<ConfigCommandProvider<TConfig>>(name);
            });

            configuration.RegisterMiddleware<ConfigMiddlewareProvider<TConfig>>();

            return configuration;
        }
    }
}
