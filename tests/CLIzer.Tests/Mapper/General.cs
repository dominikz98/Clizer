using CLIzer.Contracts;
using CLIzer.Extensions;
using CLIzer.Models.Mapper;
using System;
using System.Threading.Tasks;
using Xunit;

namespace CLIzer.Tests.Mapper
{
    public class General
    {
        [Fact]
        public async Task Map_To_Short_Id()
        {
            var pathResolver = new InMemoryFileAccessor<ClizerDictionary>(new ClizerDictionary());
            var mapper = new ClizerMapper(pathResolver);

            var entity1 = new TestEntity();
            var entity2 = new TestEntity();

            var mapping1 = await mapper.MapId(entity1, x => x.Id, -10, default);
            var mapping2 = await mapper.MapId(entity2, x => x.Id, 10, default);

            var reverse1 = mapper.GetByShortId<TestEntity>(mapping1.ShortId);
            var reverse2 = mapper.GetByShortId<TestEntity>(mapping2.ShortId);

            Assert.Null(reverse1);
            Assert.Equal(entity2.Id, reverse2);
        }

        [Fact]
        public async Task Injection()
        {
            var pathResolver = new InMemoryFileAccessor<ClizerDictionary>(new ClizerDictionary());

            var clizer = new Clizer()
                .Configure((config) => config
                    .EnableMapping(pathResolver)
                    .RegisterCommands((container) => container
                        .Root<MappingCommand>())
                );

            var result = await clizer.Execute(Array.Empty<string>());
            Assert.Equal(ClizerExitCode.SUCCESS, result);
        }
    }
}