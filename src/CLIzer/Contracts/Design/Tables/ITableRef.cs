using CLIzer.Design.Tables;

namespace CLIzer.Contracts.Design.Tables;

public interface ITableRef<T>
{
    ITableDefinition<T> Definition { get; set; }
    TablePointer Start { get; set; }
    TablePointer End { get; set; }
}
