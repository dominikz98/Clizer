using Clizer.Contracts;
using Clizer.Models;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Clizer.Tests
{
    public class HelpTextTests : ICliCmd
    {
        [Theory]
        [InlineData("help")]
        [InlineData("--help")]
        [InlineData("-h")]
        public async Task TestHelp(string arg)
        {
            var args = new List<string>()
            {
                arg
            };

            var clizer = new Clizer();
            clizer.Configure().AddCommandContainer(new CommandContainer(typeof(HelpTextTests), "Simple helptext test!"));
            var result = await clizer.Execute(args.ToArray(), default);
            Assert.NotEqual(999, result);
            Assert.NotEqual((int)ClizerExitCodes.ERROR, result);
        }

        [SuppressMessage("Usage", "xUnit1013:Public method should be marked as test", Justification = "<Pending>")]
        public async Task<int> Execute(CancellationToken cancellationToken)
            => await Task.FromResult(999);


    }
}
