using Clizer.Attributes;
using Clizer.Contracts;
using Clizer.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CLizer.Tests
{
    public class PropertyBindingTests : ICliCmd
    {
        [CliIArg("number")]
        [Range(1, 100)]
        public int Number { get; set; }
        [CliIArg("force")]
        public bool Force { get; set; }

        public bool Ignore { get; set; }

        [Fact]
        public async Task TestWrongProperty()
        {
            var args = new List<string>()
            {
                "test"
            };

            var clizer = new Clizer.Clizer();
            clizer.Configure().AddCommandContainer(new CommandContainer(typeof(PropertyBindingTests)));
            var result = await clizer.Execute(args.ToArray(), default);
            Assert.Equal((int)ClizerExitCodes.ERROR, result);
        }

        [Fact]
        public async Task TestCorrectBindingProperty()
        {
            var args = new List<string>()
            {
                "--number:1",
                "--force"
            };

            var clizer = new Clizer.Clizer();
            clizer.Configure().AddCommandContainer(new CommandContainer(typeof(PropertyBindingTests)));
            var result = await clizer.Execute(args.ToArray(), default);
            Assert.Equal((int)ClizerExitCodes.SUCCESS, result);
        }

        [Fact]
        public async Task TestIgnoredProperty()
        {
            var args = new List<string>()
            {
                "ignore"
            };

            var clizer = new Clizer.Clizer();
            clizer.Configure().AddCommandContainer(new CommandContainer(typeof(PropertyBindingTests)));
            var result = await clizer.Execute(args.ToArray(), default);
            Assert.Equal((int)ClizerExitCodes.ERROR, result);
        }

        [SuppressMessage("Usage", "xUnit1013:Public method should be marked as test", Justification = "<Pending>")]
        public async Task<int> Execute(CancellationToken cancellationToken)
            => await Task.FromResult(Force && Number > 0 ? (int)ClizerExitCodes.SUCCESS : (int)ClizerExitCodes.ERROR);
    }
}

