using Clizer.Contracts;
using Clizer.Models;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Clizer.Tests
{
    public class MissingImplementationTest : ICliCmd
    {

        [Fact]
        public async Task Run()
        {
            var clizer = new Clizer();
            clizer.Configure().SetExceptionHandler((ex) => throw ex); 
            await Assert.ThrowsAsync<ClizerException>(() => clizer.Execute(new string[0], default));
        }

        [SuppressMessage("Usage", "xUnit1013:Public method should be marked as test", Justification = "<Pending>")]
        public async Task<int> Execute(CancellationToken cancellationToken)
            => await Task.FromResult((int)ClizerExitCodes.SUCCESS);
    }
}
