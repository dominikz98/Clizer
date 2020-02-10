using System.Threading;
using System.Threading.Tasks;

namespace Clizer.Contracts
{
    public interface ICliCmd
    {
        Task<int> Execute(CancellationToken cancellationToken);
    }
}
