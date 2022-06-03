
using CLIzer;
using CLIzer.Contracts;
using CLIzer.Design.Tables;

await new Clizer()
    .Configure(config => config
        .RegisterCommands(typeof(Program).Assembly))
    .Execute(Array.Empty<string>());


public class MainCommand : ICliCmd
{
    public Task<ClizerExitCode> Execute(CancellationToken cancellationToken)
    {

        var data = new TestEntity[]
        {
            new TestEntity(1, "Smitty", new DateTime(2000,12,29)),
            new TestEntity(2, "Spongebob", new DateTime(1998,1,5)),
            new TestEntity(3, "Patrick", new DateTime(2000,11,3)),
            new TestEntity(4, "Sandy", new DateTime(2000,3,28)),
            new TestEntity(5, "Perla", new DateTime(1995,5,11)),
        };

        var table = new TableDefinition<TestEntity>()
        {
            Name = "Test-Entities",
            ColumnDefinitions = new TableColumnDefinition<TestEntity>[]
            {
                new TableColumnDefinition<TestEntity>("Id", (x) => x.Id.ToString()),
                new TableColumnDefinition<TestEntity>("Name", (x) => x.Name),
                new TableColumnDefinition<TestEntity>("Birthday", (x) => x.Birthday.ToString("yyyy.MM.dd")),
            }
        };

        TablePrinter.Print(table);

        return Task.FromResult(ClizerExitCode.SUCCESS);
    }
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