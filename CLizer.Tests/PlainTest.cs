using Clizer.Contracts;
using Clizer.Models;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CLizer.Tests
{
    public class PlainTest : ICliCmd
    {

        [Fact]
        public async Task Run()
        {
            var clizer = new Clizer.Clizer();
            clizer.Configure().AddCommandContainer(new CommandContainer(typeof(PlainTest)));
            var result = await clizer.Execute(new string[0], default);
            Assert.Equal((int)ClizerExitCodes.SUCCESS, result);
        }

        [SuppressMessage("Usage", "xUnit1013:Public method should be marked as test", Justification = "<Pending>")]
        public async Task<int> Execute(CancellationToken cancellationToken)
            => await Task.FromResult((int)ClizerExitCodes.SUCCESS);
    }
}
