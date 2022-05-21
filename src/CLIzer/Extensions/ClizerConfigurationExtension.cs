using CLIzer.Contracts;
using CLIzer.Middlewares;
using CLIzer.Models;
using CLIzer.Models.Mapper;
using CLIzer.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace CLIzer.Extensions
{
    public static class ClizerConfigurationExtension
    {
        public static ClizerConfiguration EnableMapping(this ClizerConfiguration configuration, string relativePath)
            => configuration.EnableMapping(new JsonFileByPathAccessor<ClizerDictionary>(relativePath));

        public static ClizerConfiguration EnableMapping(this ClizerConfiguration configuration, IClizerFileAccessor<ClizerDictionary> fileAccessor)
        {
            configuration.RegisterServices((services) =>
            {
                services.AddSingleton<IClizerMapper, ClizerMapper>();
                services.AddSingleton((_) => fileAccessor);
            });

            configuration.RegisterMiddleware<MapperMiddlware>();

            return configuration;
        }

        public static ClizerConfiguration EnableHelp(this ClizerConfiguration configuration)
        {
            configuration.RegisterMiddleware<HelpMiddleware>();
            configuration.RegisterServices(services => services.AddSingleton<HelpTextPrinter>());
            return configuration;
        }

        public static ClizerConfiguration RegisterConfig<TConfig>(this ClizerConfiguration configuration, string name, string relativePath) where TConfig : class, new()
            => configuration.RegisterConfig(name, new JsonFileByPathAccessor<TConfig>(relativePath));

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

            configuration.RegisterMiddleware<ConfigMiddleware<TConfig>>();

            return configuration;
        }

        public static ClizerConfiguration EnableAliases(this ClizerConfiguration configuration, string relativePath)
            => configuration.EnableAliases(new TextFileByPathAccessor(relativePath));

        public static ClizerConfiguration EnableAliases(this ClizerConfiguration configuration, IClizerFileAccessor<string> fileAccessor)
        {
            configuration.RegisterServices(services => services
                .AddSingleton((_) => new AliasesResolver(fileAccessor)));

            return configuration;
        }
    }
}
