using System;

namespace CLIzer.Tests.Mapper
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
