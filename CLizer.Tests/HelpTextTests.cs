using Clizer.Contracts;
using Clizer.Models;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CLizer.Tests
{
    public class HelpTextTests : ICliCmd
    {
        public int First { get; set; }
        public int Second { get; set; }
        public bool Force { get; set; }

        [Fact]
        public async Task Run()
        {
            var args = new List<string>()
            {
                "help"
            };

            var clizer = new Clizer.Clizer();
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
