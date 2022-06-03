using CLIzer.Contracts;
using CLIzer.Design.Tables;

namespace CLIzer.Run;

public class TableCommand : ICliCmd
{
    public Task<ClizerExitCode> Execute(CancellationToken cancellationToken)
    {

        var data = new TestEntity[]
        {
        new TestEntity(12345667, "Smitty", new DateTime(2000,12,29)),
        new TestEntity(12345668, "Spongebob", new DateTime(1998,1,5)),
        new TestEntity(12345669, "Patrick", new DateTime(2000,11,3)),
        new TestEntity(12345670, "Sandy", new DateTime(2000,3,28)),
        new TestEntity(12345671, "Perla", new DateTime(1995,5,11)),
        };

        var table = new TableDefinition<TestEntity>()
        {
            Name = "Test-Entities",
            ColumnDefinitions = new TableColumnDefinition<TestEntity>[]
            {
            new TableColumnDefinition<TestEntity>("Id", (x) => $"#{x.Id}")
            {
                Color = ConsoleColor.Magenta,
                CanShrink = false
            },
            new TableColumnDefinition<TestEntity>("Name", (x) => x.Name),
            new TableColumnDefinition<TestEntity>("Birthday", (x) => $"{x.Birthday:yyyy.MM.dd}"),
            }
        };

        TablePrinter<TestEntity>.Draw(table, data);

        return Task.FromResult(ClizerExitCode.SUCCESS);
    }

    class TestEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Birthday { get; set; }

        public TestEntity(int id, string name, DateTime birthday)
        {
            Id = id;
            Name = name;
            Birthday = birthday;
        }
    }
}
