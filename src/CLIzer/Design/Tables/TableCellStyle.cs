using CLIzer.Contracts.Design;
using CLIzer.Contracts.Design.Tables;

namespace CLIzer.Design.Tables
{
    public class TableCellStyle : ITableCellStyle
    {
        public ConsoleColor Color { get; set; }
        public Alignment Alignment { get; set; }

        public TableCellStyle(ConsoleColor color, Alignment alignment)
        {
            Color = color;
            Alignment = alignment;
        }
    }
}
