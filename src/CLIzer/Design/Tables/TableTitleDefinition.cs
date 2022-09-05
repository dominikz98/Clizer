using CLIzer.Contracts.Design;

namespace CLIzer.Design.Tables;

public class TableTitleDefinition : ITableTitleDefinition
{
    public string Name { get; set; }
    public ConsoleColor Color { get; set; } = Console.ForegroundColor;
    public Alignment Alignment { get; set; } = Alignment.Start;

    public TableTitleDefinition(string name)
    {
        Name = name;
    }
}
