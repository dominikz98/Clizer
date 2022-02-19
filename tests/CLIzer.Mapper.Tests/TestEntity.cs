using System;

namespace CLIzer.Mapper.Tests
{
    internal class TestEntity
    {
        public Guid Id { get; set; }

        public TestEntity()
        {
            Id = Guid.NewGuid();
        }
    }
}
