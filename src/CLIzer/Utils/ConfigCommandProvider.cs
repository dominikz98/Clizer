using CLIzer.Contracts;
using System.Diagnostics;

namespace CLIzer.Utils
{
    internal class ConfigCommandProvider<T> : ICliCmd where T : class, new()
    {
        private readonly IClizerDataAccessor<T> _configAccessor;

        public ConfigCommandProvider(IClizerDataAccessor<T> configAccessor)
        {
            _configAccessor = configAccessor;
        }

        public Task<ClizerExitCode> Execute(CancellationToken cancellationToken)
        {
            if (_configAccessor.Source is null)
                return Task.FromResult(ClizerExitCode.ERROR);

            Process.Start(new ProcessStartInfo(_configAccessor.Source) { UseShellExecute = true });
            return Task.FromResult(ClizerExitCode.SUCCESS);
        }
    }
}
