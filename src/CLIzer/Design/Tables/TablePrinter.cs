using CLIzer.Contracts.Design;
using CLIzer.Contracts.Design.Tables;

namespace CLIzer.Design.Tables;

public class TablePrinter<T>
{
    public static ITableRef<T> Draw(ITableDefinition<T> table, IReadOnlyCollection<T> data)
    {
        // store position
        var (startLeft, startTop) = Console.GetCursorPosition();

        if (!table.ColumnDefinitions.Any())
            return new TableRef<T>(table, startLeft, startTop);

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
        Console.Write(Environment.NewLine);

        // store position
        var (endLeft, endTop) = Console.GetCursorPosition();

        return new TableRef<T>(table, startLeft, startTop, endLeft, endTop);
    }

    public static ITableRef<T> ReDraw(ITableRef<T> reference)
        => reference;

    private static void DrawTitle(ITableTitleDefinition title)
    {
        var value = FormatCell(title.Name, Console.WindowWidth, title.Alignment, false, true);
        Draw(value, title.Color);
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
            foreach (var column in columns)
            {
                var width = TableWidthCalculator.ExactToWidth(Console.WindowWidth, widths[column]) - 1;
                var value = FormatCell(column.ValueAccessor(entry), width, column.Alignment, true, column.PadIfPossible);
                DrawCell(value, column.Color, columns.First() == column);
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

        Draw(value, color);
        Console.Write('|');
    }

    private static void Draw(string value, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.Write(value);
        Console.ResetColor();
    }
}
