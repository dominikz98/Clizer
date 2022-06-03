﻿using CLIzer.Contracts.Tables;
using CLIzer.Models;

namespace CLIzer.Design.Tables;

internal static class TableWidthCalculator<T>
{
    public static Dictionary<ITableColumnDefinition<T>, int> RelativeToFullWidth(ITableColumnDefinition<T>[] columnDefinitions, IReadOnlyCollection<T> data)
    {
        var maxWidth = Console.WindowWidth;
        var relativeWidths = new Dictionary<ITableColumnDefinition<T>, int>();

        // get required chars per column
        foreach (var column in columnDefinitions)
        {
            var requiredChars = data.Select(x => column.ValueAccessor(x))
                .Select(x => x.Length + 1)
                .Max();

            if (columnDefinitions.First() == column)
                requiredChars += 1;

            var calculatedWidth = (double)maxWidth / 100 * requiredChars;
            var rounded = (int)Math.Round(calculatedWidth, 0, MidpointRounding.ToPositiveInfinity);

            relativeWidths.Add(column, rounded);
        }

        // check if width fits
        var requiredWidth = relativeWidths.Sum(x => x.Value);

        if (requiredWidth < 99)
            return ExtendColumns(requiredWidth, relativeWidths, columnDefinitions);

        var truncatedColumns = TruncateColumns(requiredWidth, relativeWidths, columnDefinitions);
        if (!truncatedColumns.Any())
            throw new ClizerException("Table columns allocate to much width to be drawed. Try remove a column.");

        return truncatedColumns;
    }

    private static Dictionary<ITableColumnDefinition<T>, int> ExtendColumns(
        int requiredWidth,
        Dictionary<ITableColumnDefinition<T>, int> relativeWidths,
        IReadOnlyCollection<ITableColumnDefinition<T>> columnDefinitions)
    {
        // attach remaining width
        var extendWidthPerColumn = (100 - (double)requiredWidth) / columnDefinitions.Count();
        var roundedExtendWidthPerColumn = (int)Math.Round(extendWidthPerColumn, 0, MidpointRounding.ToZero);

        foreach (var column in relativeWidths.Keys)
            relativeWidths[column] += roundedExtendWidthPerColumn;

        return relativeWidths;
    }

    private static Dictionary<ITableColumnDefinition<T>, int> TruncateColumns(
        int requiredWidth,
        Dictionary<ITableColumnDefinition<T>, int> relativeWidths,
        IReadOnlyCollection<ITableColumnDefinition<T>> columnDefinitions)
    {
        // truncate columns shrinkable columns
        if (!columnDefinitions.Any())
            return new Dictionary<ITableColumnDefinition<T>, int>();

        var shrinkableColumns = columnDefinitions
        .Where(x => x.CanShrink)
        .ToList();

        if (!shrinkableColumns.Any())
            shrinkableColumns = columnDefinitions.ToList();

        var shrinkWidthPerColumn = ((double)requiredWidth - 100) / shrinkableColumns.Count();
        var roundedShrinkWidthPerColumn = (int)Math.Round(shrinkWidthPerColumn, 0, MidpointRounding.ToZero);

        var almostShrinkableColums = shrinkableColumns
            .Where(x => relativeWidths[x] - roundedShrinkWidthPerColumn > 1)
            .ToList();

        if (almostShrinkableColums.Count != shrinkableColumns.Count)
            return TruncateColumns(requiredWidth, relativeWidths, almostShrinkableColums);

        foreach (var column in relativeWidths.Keys)
            if (!shrinkableColumns.Contains(column))
                continue;
            else
                relativeWidths[column] -= roundedShrinkWidthPerColumn;

        return relativeWidths;
    }
}
