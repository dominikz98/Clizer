using CLIzer.Contracts.Design.Tables;
using CLIzer.Contracts.Tables;

namespace CLIzer.Design.Tables
{
    public class TablePrinter<T>
    {
        public static ITableRef Draw(ITableDefinition<T> table, IReadOnlyCollection<T> data)
        {
            if (!table.ColumnDefinitions.Any())
                return new TableRef();

            var columnWidths = TableWidthCalculator<T>.RelativeToFullWidth(table.ColumnDefinitions, data);

            return new TableRef();
        }

        public static ITableRef ReDraw(ITableRef reference)
            => reference;


    }
}
