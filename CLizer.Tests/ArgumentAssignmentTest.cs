using Clizer.Contracts;
using Clizer.Models;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CLizer.Tests
{
    public class ArgumentAssignmentTest : ICliCmd
    {

        [Fact]
        public async Task Run()
        {
            var args = new List<string>()
            {
                "test"
            };

            var clizer = new Clizer.Clizer();
            clizer.Configure().AddCommandContainer(new CommandContainer(typeof(ArgumentAssignmentTest)));
            var result = await clizer.Execute(args.ToArray(), default);
            Assert.True(result == (int)ClizerExitCodes.ERROR);
        }

        [SuppressMessage("Usage", "xUnit1013:Public method should be marked as test", Justification = "<Pending>")]
        public async Task<int> Execute(CancellationToken cancellationToken)
        => await Task.FromResult((int)ClizerExitCodes.SUCCESS);
    }
}

