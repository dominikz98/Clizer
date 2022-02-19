using CLIzer.Contracts;
using CLIzer.Mapper.Contracts;
using CLIzer.Mapper.Models;
using CLIzer.Models;
using CLIzer.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace CLIzer.Mapper
{
    public static class ClizerConfigurationExtension
    {
        public static ClizerConfiguration EnableMapping(this ClizerConfiguration configuration, string relativePath)
            => EnableMapping(configuration, new JsonFileByRelativePathAccessor<ClizerDictionary>(relativePath));

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
    }
}
