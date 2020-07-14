using Clizer.Utils;
using System;
using Xunit;

namespace Clizer.Tests
{
    public class MappingTests
    {
        [Fact]
        public void Run()
        {
            var entity1 = new TestEntity();
            var entity2 = new TestEntity();

            var mapping1 = ClizerMapper.MapId(entity1, x => x.Id, -10);
            var mapping2 = ClizerMapper.MapId(entity2, x => x.Id, 10);
            
            Assert.Null(ClizerMapper.GetByShortId<TestEntity>(mapping1.ShortId));
            Assert.Equal(ClizerMapper.GetByShortId<TestEntity>(mapping2.ShortId), entity2.Id);
        }
    }

    class TestEntity
    {
        public Guid Id { get; set; }

        public TestEntity()
        {
            Id = Guid.NewGuid();
        }
    }
}
