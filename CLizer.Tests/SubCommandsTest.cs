using Clizer.Contracts;
using Clizer.Models;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CLizer.Tests
{
    public class SubCommandsTest : ICliCmd
    {

        [Fact]
        public async Task Run()
        {
            var args = new List<string>()
            {
                "parent",
                "child"
            };

            var clizer = new Clizer.Clizer();
            clizer.Configure().AddCommandContainer(new CommandContainer(typeof(SubCommandsTest))
                                .Register<SubCommandsTest, ParentCmd>("parent")
                                    .Register<ParentCmd, ChildCmd>("child"));
            var result = await clizer.Execute(args.ToArray(), default);
            Assert.Equal(999, result);
        }

        [SuppressMessage("Usage", "xUnit1013:Public method should be marked as test", Justification = "<Pending>")]
        public async Task<int> Execute(CancellationToken cancellationToken)
            => await Task.FromResult((int)ClizerExitCodes.SUCCESS);
    }

    public class ChildCmd : ICliCmd
    {
        public async Task<int> Execute(CancellationToken cancellationToken)
            => await Task.FromResult(999);

    }
    public class ParentCmd : ICliCmd
    {
        public async Task<int> Execute(CancellationToken cancellationToken)
            => await Task.FromResult((int)ClizerExitCodes.SUCCESS);
    }
}
