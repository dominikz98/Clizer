namespace CLIzer.Contracts.Design.Tables;

public interface ITableTitleDefinition
{
    string Name { get; }
    ConsoleColor Color { get; }
    Alignment Alignment { get; }
}
