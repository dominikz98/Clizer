using CLIzer.Contracts;
using System.Diagnostics;

namespace CLIzer.UserConfig
{
    internal class ConfigCommandProvider<T> : ICliCmd where T : class, new()
    {
        private readonly IClizerFileAccessor<T> _configAccessor;

        public ConfigCommandProvider(IClizerFileAccessor<T> configAccessor)
        {
            _configAccessor = configAccessor;
        }

        public Task<int> Execute(CancellationToken cancellationToken)
        {
            if (_configAccessor.Path is null)
                return Task.FromResult((int)ClizerExitCodes.ERROR);

            Process.Start(new ProcessStartInfo(_configAccessor.Path) { UseShellExecute = true });
            return Task.FromResult((int)ClizerExitCodes.SUCCESS);
        }
    }
}
