using Clizer.Attributes;
using System.Threading;
using System.Threading.Tasks;

namespace Clizer.Console
{
    [CliCmd("lowest")]
    public class LowestCommand
    {
        public Task Execute(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
