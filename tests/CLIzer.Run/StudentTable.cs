using CLIzer.Contracts.Design;
using CLIzer.Contracts.Design.Tables;
using CLIzer.Design.Tables;

namespace CLIzer.Run;

internal class StudentTable : ITableDefinition<StudentEntity>
{
    public ITableTitleDefinition? Title { get; }
        = new TableTitleDefinition("Test-Entities")
        {
            Color = ConsoleColor.Blue,
            Alignment = Alignment.Center
        };

    public ITableColumnDefinition<StudentEntity>[] ColumnDefinitions { get; }
        = new TableColumnDefinition<StudentEntity>[]
        {
            new TableColumnDefinition<StudentEntity>("Id", (x) => $"#{x.Id}")
            {
                Color = ConsoleColor.Magenta,
                CanShrink = false
            },
            new TableColumnDefinition<StudentEntity>("Name", (x) => x.Name)
            {
                Alignment = Alignment.Center
            },
            new TableColumnDefinition<StudentEntity>("Birthday", (x) => $"{x.Birthday:yyyy.MM.dd}")
            {
                Alignment = Alignment.End
            }
        };
}
