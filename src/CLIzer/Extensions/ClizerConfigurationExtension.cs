using CLIzer.Accessors;
using CLIzer.Contracts;
using CLIzer.Middlewares;
using CLIzer.Models;
using CLIzer.Models.Mapper;
using CLIzer.Resolver;
using Microsoft.Extensions.DependencyInjection;

namespace CLIzer.Extensions
{
    public static class ClizerConfigurationExtension
    {
        public static ClizerConfiguration EnableMapping(this ClizerConfiguration configuration, string relativePath)
            => configuration.EnableMapping(new JsonFileByPathAccessor<ClizerDictionary>(relativePath));

        public static ClizerConfiguration EnableMapping(this ClizerConfiguration configuration, IClizerDataAccessor<ClizerDictionary> fileAccessor)
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
            return configuration;
        }

        public static ClizerConfiguration EnableConfig<TConfig>(this ClizerConfiguration configuration, string name, string relativePath) where TConfig : class, new()
            => configuration.EnableConfig(name, new JsonFileByPathAccessor<TConfig>(relativePath));

        public static ClizerConfiguration EnableConfig<TConfig>(this ClizerConfiguration configuration, string name, IClizerDataAccessor<TConfig> fileAccessor) where TConfig : class, new()
        {
            configuration.RegisterServices((services) =>
            {
                services.AddSingleton((_) => fileAccessor);
                services.AddSingleton<IConfig<TConfig>, ConfigProvider<TConfig>>();
            });

            configuration.RegisterMiddleware((services) =>
            {
                var accessor = services.GetRequiredService<IClizerDataAccessor<TConfig>>();
                var config = services.GetRequiredService<IConfig<TConfig>>();
                return new ConfigMiddleware<TConfig>(config, name, accessor);
            });

            return configuration;
        }

        public static ClizerConfiguration EnableAliases(this ClizerConfiguration configuration, string relativePath)
            => configuration.EnableAliases(new TextFileByPathAccessor(relativePath));

        public static ClizerConfiguration EnableAliases(this ClizerConfiguration configuration, IClizerDataAccessor<string> fileAccessor)
        {
            configuration.RegisterServices(services => services
                .AddSingleton((_) => new AliasesResolver(fileAccessor)));

            return configuration;
        }
    }
}
