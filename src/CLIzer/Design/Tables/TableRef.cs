using CLIzer.Contracts.Design;

namespace CLIzer.Design.Tables;

internal class TableRef<T> : IComponentRef<T>
{
    public ITableDefinition<T> Definition { get; set; }
    public ConsolePointer Start { get; set; }
    public ConsolePointer End { get; set; }

    public TableRef(ITableDefinition<T> definition, ConsolePointer pointer) : this(definition, pointer, pointer) { }

    public TableRef(ITableDefinition<T> definition, ConsolePointer startPointer, ConsolePointer endPointer)
    {
        Definition = definition;
        Start = startPointer;
        End = endPointer;
    }
}
