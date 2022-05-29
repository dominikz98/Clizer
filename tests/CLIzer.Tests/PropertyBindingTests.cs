using CLIzer.Attributes;
using CLIzer.Contracts;
using CLIzer.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CLIzer.Tests
{
    public class PropertyBindingTests
    {
        [Fact]
        public async Task Wrong_Property()
        {
            var args = new List<string>()
            {
                "--test:1"
            };

            var clizer = new Clizer()
                .Configure((config) => config
                    .RegisterCommands<PropertyBindingCmd>()
                );

            await Assert.ThrowsAnyAsync<ClizerException>(() => clizer.Execute(args.ToArray()));
        }

        [Fact]
        public async Task Correct_Binding_Property()
        {
            var args = new List<string>()
            {
                "--number:1",
                "--force"
            };

            var clizer = new Clizer();
            clizer.Configure((config) => config.RegisterCommands<PropertyBindingCmd>());
            var result = await clizer.Execute(args.ToArray());
            Assert.Equal(ClizerExitCode.SUCCESS, result);
        }

        [Fact]
        public async Task Ignored_Property()
        {
            var args = new List<string>()
            {
                "--ignore"
            };

            var clizer = new Clizer();
            clizer.Configure((config) => config.RegisterCommands<PropertyBindingCmd>());
            await Assert.ThrowsAnyAsync<ClizerException>(() => clizer.Execute(args.ToArray()));
        }

        [Fact]
        public async Task Path_Property()
        {
            var args = new List<string>()
            {
                "--number:1",
                "--force",
                @"--path:E:\Bibliotheken\Projects"
            };

            var clizer = new Clizer();
            clizer.Configure((config) => config.RegisterCommands<PropertyBindingCmd>());
            var result = await clizer.Execute(args.ToArray());
            Assert.Equal(ClizerExitCode.SUCCESS, result);
        }

        [Fact]
        public async Task With_Shortcuts()
        {
            var args = new List<string>()
            {
                "-n:1",
                "-f"
            };

            var clizer = new Clizer();
            clizer.Configure((config) => config.RegisterCommands<PropertyBindingCmd>());
            var result = await clizer.Execute(args.ToArray());
            Assert.Equal(ClizerExitCode.SUCCESS, result);
        }
    }

    class PropertyBindingCmd : ICliCmd
    {
        [CliIArg("number", "n")]
        [Range(1, 100)]
        public int Number { get; set; }
        [CliIArg("force", "f")]
        public bool Force { get; set; }

        [CliIArg("path")]
        public string? Path { get; set; }
        public bool Ignore { get; set; }

        public Task<ClizerExitCode> Execute(CancellationToken cancellationToken)
        {
            if (Force && Number > 0)
                return Task.FromResult(ClizerExitCode.SUCCESS);

            return Task.FromResult(ClizerExitCode.ERROR);
        }
    }
}