using CLIzer.Contracts;
using CLIzer.Extensions;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace CLIzer.Tests.UserConfig
{
    public class General
    {
        [Fact]
        public async Task Create_and_Inject_Config()
        {
            var file = "smitty_werben_jagger_man_jensen.json";
            var exists = File.Exists(file);
            if (exists)
                File.Delete(file);

            var clizer = new Clizer()
                .Configure((config) => config
                    .RegisterConfig<TestUserConfiguration>("config", "smitty_werben_jagger_man_jensen.json")
                    .RegisterCommands((container) => container
                        .Root<UserConfigCommand>())
                );

            var result = await clizer.Execute(Array.Empty<string>());
            Assert.Equal(ClizerExitCode.SUCCESS, result);
        }

        [Fact]
        public async Task Inject_Config()
        {
            var fileAccessor = new InMemoryFileAccessor<TestUserConfiguration>(
                new TestUserConfiguration()
                {
                    Value = 99
                });

            var clizer = new Clizer()
                .Configure((config) => config
                    .RegisterConfig("config", fileAccessor)
                    .RegisterCommands((container) => container
                        .Root<UserConfigCommand>())
                );

            var result = await clizer.Execute(Array.Empty<string>());
            Assert.Equal(ClizerExitCode.SUCCESS, result);
        }
    }
}
