using Clizer.Contracts;
using Clizer.Models;
using Newtonsoft.Json;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Clizer.Tests
{
    public class UserConfigurationTest
    {
        [Fact]
        public async Task RunConfig()
        {
            var clizer = new Clizer();
            clizer.Configure().AddCommandContainer(new CommandContainer(typeof(TestCommand)))
                                .EnableUserConfiguration<TestUserConfiguration>();
            Assert.Equal((int)ClizerExitCodes.SUCCESS, await clizer.Execute(new string[] { "config" }, default));
        }

        [Fact]
        public async Task RunInjectConfig()
        {
            var config = new TestUserConfiguration()
            {
                Test = 99
            };
            File.WriteAllText("config.json", JsonConvert.SerializeObject(config));

            var clizer = new Clizer();
            clizer.Configure().AddCommandContainer(new CommandContainer(typeof(TestCommand)))
                                .EnableUserConfiguration<TestUserConfiguration>();
            Assert.Equal(config.Test, await clizer.Execute(new string[0], default));
        }
    }

    public class TestCommand : ICliCmd
    {
        private readonly TestUserConfiguration _configuration;

        public TestCommand(TestUserConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task<int> Execute(CancellationToken cancellationToken)
            => Task.FromResult(_configuration.Test);
    }

    public class TestUserConfiguration
    {
        public int Test { get; set; }
    }
}
