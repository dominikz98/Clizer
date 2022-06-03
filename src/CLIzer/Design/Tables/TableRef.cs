using CLIzer.Contracts.Design.Tables;

namespace CLIzer.Design.Tables;

internal class TableRef<T> : ITableRef<T>
{
    public ITableDefinition<T> Definition { get; set; }
    public TablePointer Start { get; set; }
    public TablePointer End { get; set; }

    public TableRef(ITableDefinition<T> definition, int startLeft, int startTop) : this(definition, startLeft, startTop, startLeft, startTop) { }

    public TableRef(ITableDefinition<T> definition, int startLeft, int startTop, int endLeft, int endTop)
    {
        Definition = definition;
        Start = new TablePointer(startLeft, startTop);
        End = new TablePointer(endLeft, endTop);
    }
}
