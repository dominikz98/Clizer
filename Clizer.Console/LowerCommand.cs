using Clizer.Attributes;
using System.Threading;
using System.Threading.Tasks;

namespace Clizer.Console
{
    [CliCmd("lower", typeof(LowestCommand))]
    public class LowerCommand
    {
        public Task Execute(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
