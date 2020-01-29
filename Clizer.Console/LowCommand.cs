using Clizer.Attributes;
using Clizer.Console.Services;
using System.Threading;
using System.Threading.Tasks;

namespace Clizer.Console
{
    [CliCmd("low", typeof(LowerCommand), Help = "First command to test clizer.")]
    public class LowCommand
    {
        [CliOption("--force", Help = "Execute action forced.", Short = "-f")]
        public bool Force { get; set; }

        [CliArgument("value", Help = "Test value.", Short = "v")]
        public int Test { get; set; }

        private readonly ITestService _testService;

        public LowCommand(ITestService testService)
        {
            _testService = testService;
        }

        public Task Execute(CancellationToken cancellationToken)
        {
            System.Console.WriteLine(_testService.GetMessage(Test, Force));
            return Task.CompletedTask;
        }
    }
}
