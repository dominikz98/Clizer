using CLIzer.Contracts;
using CLIzer.Help;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace CLIzer.Tests
{
    public class HelpTextTests
    {
        [Theory]
        [InlineData("help")]
        [InlineData("--help")]
        [InlineData("-h")]
        public async Task Call_Help(string arg)
        {
            var args = new List<string>()
            {
                arg
            };

            var clizer = new Clizer().Configure(config => config.EnableHelp());

            var result = await clizer.Execute(args.ToArray(), default);
            Assert.NotEqual(ClizerExitCode.ERROR, result);
        }
    }
}
