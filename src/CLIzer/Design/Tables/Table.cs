using CLIzer.Contracts.Design;
using CLIzer.Design.Panel;
using CLIzer.Extensions;

namespace CLIzer.Design.Tables;

public abstract class Table<T> : ITableDefinition<T>
{
    public abstract ITableTitleDefinition? Title { get; }
    public abstract ITableColumnDefinition<T>[] ColumnDefinitions { get; }

    public IReadOnlyCollection<T> Values { get; private set; } = new List<T>();
    CanvasSize? IDesignComponent.Canvas { get; set; }
    public event EventHandler<int>? OnDrawed;

    internal ConsolePointer? _start;
    internal ConsolePointer? _end;

    public void Draw(IReadOnlyCollection<T> values)
    {
        Values = values;
        Draw();
    }

    public void Draw()
    {
        ConsolePointer? originalPosition = null;

        // store position (in case of redaw)
        if (_start is not null && _end is not null)
        {
            originalPosition = ConsolePointer.CreateByCurrent();
            Console.SetCursorPosition(_start.Left, _start.Top);
        }

        // draw table
        DrawInternal();

        // new table is larger or equal
        var currentPosition = ConsolePointer.CreateByCurrent();
        if (originalPosition is null || currentPosition.Top >= originalPosition!.Top)
            return;

        // remove original relics
        for (int i = currentPosition.Top; i < originalPosition.Top; i++)
        {
            ConsoleExtensions.Write(" ".PadLeft(Console.WindowWidth, ' '), ConsoleColor.Red);
            Console.Write(Environment.NewLine);
        }

        OnDrawed?.Invoke(this, _end!.Top - _start!.Top);
    }

    private void DrawInternal()
    {
        // store position
        _start ??= ConsolePointer.CreateByStart();

        if (!ColumnDefinitions.Any())
            return;

        // calculate relative column widths
        var columnWidths = TableWidthCalculator<T>.RelativeToWidth(Console.WindowWidth, ColumnDefinitions, Values);

        // draw table
        if (Title is not null && !string.IsNullOrWhiteSpace(Title.Name))
            DrawTitle(Title);

        DrawSeparator();
        DrawHeader(columnWidths, ColumnDefinitions);
        DrawSeparator();
        DrawRows(columnWidths, ColumnDefinitions);
        DrawSeparator();

        // store position
        _end = ConsolePointer.CreateByCurrent();
    }

    private static void DrawTitle(ITableTitleDefinition title)
    {
        var value = FormatCell(title.Name, Console.WindowWidth, title.Alignment, false, true);
        ConsoleExtensions.Write(value, title.Color);
        Console.WriteLine();
    }

    private static void DrawHeader(Dictionary<ITableColumnDefinition<T>, int> widths, ITableColumnDefinition<T>[] columns)
    {
        foreach (var column in columns)
        {
            var width = TableWidthCalculator.ExactToWidth(Console.WindowWidth, widths[column]) - 1;
            var value = FormatCell(column.Name ?? String.Empty, width, column.Alignment, false, true);
            DrawCell(value, column.Color, columns.First() == column);
        }
    }

    private static void DrawSeparator()
    {
        var value = "-".PadLeft(Console.WindowWidth, '-');
        Console.WriteLine(value);
    }

    private void DrawRows(Dictionary<ITableColumnDefinition<T>, int> widths, ITableColumnDefinition<T>[] columns)
    {
        foreach (var entry in Values)
        {
            foreach (var column in columns)
            {
                var value = column.ValueAccessor(entry);
                var style = column.StyleAccessor(entry, column);

                var width = TableWidthCalculator.ExactToWidth(Console.WindowWidth, widths[column]) - 1;
                var cell = FormatCell(value, width, style.Alignment, true, column.PadIfPossible);
                DrawCell(cell, style.Color, columns.First() == column);
            }

            Console.Write(Environment.NewLine);
        }
    }

    private static string FormatCell(string value, int width, Alignment alignment, bool truncateIfRequired, bool padIfPossible)
    {
        if (value.Length > width && truncateIfRequired)
            value = value[..(width - 3)] + "...";

        if (value.Length == width)
            return value;

        if (padIfPossible && value.Length <= width - 2)
            value = $" {value} ";

        if (alignment == Alignment.Start)
            return value.PadRight(width, ' ');

        if (alignment == Alignment.End)
            return value.PadLeft(width, ' ');

        var cpy = value;
        var spacePerSite = ((double)width - value.Length) / 2;

        var leftSpace = (int)Math.Round(spacePerSite, 0, MidpointRounding.ToZero);
        cpy = cpy.Insert(0, string.Empty.PadLeft(leftSpace, ' '));

        var rightSpace = (int)Math.Round(spacePerSite, 0, MidpointRounding.ToPositiveInfinity);
        cpy = cpy.Insert(cpy.Length, string.Empty.PadRight(rightSpace, ' '));

        return cpy;
    }

    private static void DrawCell(string value, ConsoleColor color, bool printStartBracket)
    {
        if (printStartBracket)
            Console.Write('|');

        ConsoleExtensions.Write(value, color);
        Console.Write('|');
    }
}
