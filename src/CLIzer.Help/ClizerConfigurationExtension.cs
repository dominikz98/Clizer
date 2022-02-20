using CLIzer.Models;
using Microsoft.Extensions.DependencyInjection;

namespace CLIzer.Help
{
    public static class ClizerConfigurationExtension
    {
        public static ClizerConfiguration EnableHelp(this ClizerConfiguration configuration)
        {
            configuration.RegisterMiddleware<HelpMiddleware>();
            configuration.RegisterServices(services => services.AddSingleton<HelpTextPrinter>());
            return configuration;
        }
    }
}
