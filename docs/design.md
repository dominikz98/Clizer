# UI Components

## Table

With this abstract base class you can define your own table.
It is important to override 2 properties:
- Title
- ColumnDefinitions

Both implement the 'ITableColumnDefinition' interface, which provides access to formatting, styling and sizing of a UI element.

Size ratios of the individual columns are determined automatically at runtime.

## Example

Calls:
> C:\Users\Spongebob> {{assembly}}

Command:
```csharp
await new Clizer()
    .Configure(config => config
    .RegisterCommands<TableCommand>())
    .Execute();

public class TableCommand : ICliCmd
{
    public Task<ClizerExitCode> Execute(CancellationToken cancellationToken)
    {

        var data = new StudentEntity[]
        {
            new StudentEntity(1, "Smitty", new DateTime(2000,12,29)),
            new StudentEntity(2, "Spongebob", new DateTime(1998,1,5)),
            new StudentEntity(3, "Patrick", new DateTime(2000,11,3)),
            new StudentEntity(4, "Sandy", new DateTime(2000,3,28)),
            new StudentEntity(5, "Perla", new DateTime(1995,5,11)),
        };

        var table = new StudentTable();
        table.Draw(data);

        var newData = data.ToList().GetRange(0, data.Length - 1);
        table.Draw(newData);

        return Task.FromResult(ClizerExitCode.SUCCESS);
    }
}

internal class StudentTable : Table<StudentEntity>
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
```


## Progressbar

The Progressbar element is kept quite simple.
Title and color can be configured.

To increase the progress the 'Step()' method must be called.

## Example

Calls:
> C:\Users\Spongebob> {{assembly}}

Command:
```csharp
await new Clizer()
    .Configure(config => config
    .RegisterCommands<ProgressbarCommand>())
    .Execute();

public class ProgressbarCommand : ICliCmd
{
    public async Task<ClizerExitCode> Execute(CancellationToken cancellationToken)
    {
        var pb = new ProgressBar("downloading ...")
        {
            Color = ConsoleColor.Blue
        };

        pb.Step(25, 100);
        await Task.Delay(2000, cancellationToken);

        pb.Step(50, 100);
        await Task.Delay(2000, cancellationToken);

        pb.Step(75, 100);
        await Task.Delay(2000, cancellationToken);

        pb.Step(100, 100);
        await Task.Delay(2000, cancellationToken);

        pb.Step(95, 100);

        return ClizerExitCode.SUCCESS;
    }
}
```