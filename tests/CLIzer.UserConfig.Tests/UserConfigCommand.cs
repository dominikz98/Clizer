using CLIzer.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace CLIzer.UserConfig.Tests
{
    public class UserConfigCommand : ICliCmd
    {
        private readonly TestUserConfiguration? _configuration;

        public UserConfigCommand(IConfig<TestUserConfiguration> configuration)
        {
            _configuration = configuration.Value;
        }

        public Task<int> Execute(CancellationToken cancellationToken)
        {
            if (_configuration is null)
                return Task.FromResult((int)ClizerExitCodes.ERROR);

            return Task.FromResult(_configuration.Value);
        }
    }
}
