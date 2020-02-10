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
    public class ValidationAnnotationTest : ICliCmd
    {

        [Fact]
        public async Task Run()
        {
            var args = new List<string>()
            {
                "add",
                "second:0"
            };

            var clizer = new Clizer.Clizer();
            clizer.Configure().SetExceptionHandler((ex) => throw ex)
                              .AddCommandContainer(new CommandContainer(typeof(ValidationAnnotationTest))
                                .Register<ValidationAnnotationTest, AddCmd>("add"));
            await Assert.ThrowsAsync<ValidationException>(() => clizer.Execute(args.ToArray(), default));
        }

        [SuppressMessage("Usage", "xUnit1013:Public method should be marked as test", Justification = "<Pending>")]
        public async Task<int> Execute(CancellationToken cancellationToken)
        => await Task.FromResult((int)ClizerExitCodes.SUCCESS);
    }

    public class AddCmd : ICliCmd
    {
        [Range(1, 100)]
        public int First { get; set; }
        public int Second { get; set; }

        public async Task<int> Execute(CancellationToken cancellationToken)
            => await Task.FromResult((int)ClizerExitCodes.SUCCESS);
    }
}

