using CLIzer.Attributes;
using CLIzer.Contracts;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CLIzer.Tests
{
    public class ValidationAnnotationTest
    {
        [Fact]
        public async Task Invalid_Range()
        {
            var args = new List<string>()
            {
                "add",
                "--second:0"
            };

            var clizer = new Clizer();
            clizer.Configure((config) => config

                .HandleException((ex) => throw ex)
                .RegisterCommands((container) => container
                    .Command<AddCmd>("add")
                )
            );

            await Assert.ThrowsAsync<ValidationException>(() => clizer.Execute(args.ToArray(), default));
        }
    }

    public class AddCmd : ICliCmd
    {
        [Range(1, 100)]
        [CliIArg("first")]
        public int First { get; set; }
        [CliIArg("second")]
        public int Second { get; set; }

        public Task<int> Execute(CancellationToken cancellationToken)
            => Task.FromResult((int)ClizerExitCodes.SUCCESS);
    }
}
