using CLIzer.Contracts.Design;
using CLIzer.Extensions;

namespace CLIzer.Design.Tables;

public class TablePrinter<T>
{
    public static IComponentRef<T> Draw(ITableDefinition<T> table, IReadOnlyCollection<T> data)
    {
        // store position
        var startPointer = ConsolePointer.CreateByStart();

        if (!table.ColumnDefinitions.Any())
            return new TableRef<T>(table, startPointer);

        // calculate relative column widths
        var columnWidths = TableWidthCalculator<T>.RelativeToWidth(Console.WindowWidth, table.ColumnDefinitions, data);

        // draw table
        if (table.Title is not null && !string.IsNullOrWhiteSpace(table.Title.Name))
            DrawTitle(table.Title);

        DrawSeparator();
        DrawHeader(columnWidths, table.ColumnDefinitions);
        DrawSeparator();
        DrawRows(columnWidths, table.ColumnDefinitions, data);
        DrawSeparator();

        // store position
        var endPointer = ConsolePointer.CreateByCurrent();

        return new TableRef<T>(table, startPointer, endPointer);
    }

    public static IComponentRef<T> ReDraw(IComponentRef<T> originalRef, IReadOnlyCollection<T> data)
    {
        // store position
        var (currentLeft, currentTop) = Console.GetCursorPosition();
        Console.SetCursorPosition(originalRef.Start.Left, originalRef.Start.Top);

        // redraw table
        var currentRef = Draw(originalRef.Definition, data);

        // new table is larger or equal
        if (currentRef.End.Top >= originalRef.End.Top)
            return currentRef;

        // remove original relics
        for (int i = currentRef.End.Top; i < originalRef.End.Top; i++)
        {
            ConsoleExtensions.Write(" ".PadLeft(Console.WindowWidth, ' '), ConsoleColor.Red);
            Console.Write(Environment.NewLine);
        }

        Console.SetCursorPosition(currentLeft, currentTop);
        return currentRef;
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

    private static void DrawRows(Dictionary<ITableColumnDefinition<T>, int> widths, ITableColumnDefinition<T>[] columns, IReadOnlyCollection<T> data)
    {
        foreach (var entry in data)
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
