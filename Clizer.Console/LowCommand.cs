using Clizer.Attributes;
using System.Threading;
using System.Threading.Tasks;

namespace Clizer.Console
{
    [CliCmd("low", typeof(LowerCommand))]
    public class LowCommand
    {
        [CliOption("--force", Help = "Execute action forced.", Short = "-f")]
        public bool Force { get; set; }

        [CliArgument("value", Help = "Test value.", Short = "v")]
        public int Test { get; set; }

        public Task Execute(CancellationToken cancellationToken)
        {
            System.Console.WriteLine("Running! Value:" + Test + "; Force: " + Force);
            return Task.CompletedTask;
        }
    }
}
