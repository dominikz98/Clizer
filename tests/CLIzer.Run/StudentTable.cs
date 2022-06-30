using CLIzer.Contracts.Design;
using CLIzer.Design.Tables;

namespace CLIzer.Run;

internal class StudentTable : CliTable<StudentEntity>
{
    public override ITableTitleDefinition? Title { get; }
        = new TableTitleDefinition("Test-Entities")
        {
            Color = ConsoleColor.Blue,
            Alignment = Alignment.Center
        };

    public override ITableColumnDefinition<StudentEntity>[] ColumnDefinitions { get; }
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
                Alignment = Alignment.End,
                StyleAccessor = (x, style) => x.Birthday.Year < 2000 ? new TableCellStyle(ConsoleColor.Red, style.Alignment) : style
            }
        };
}
