using CLIzer.Design;

namespace CLIzer.Contracts.Design;

public interface IComponentRef<T>
{
    ITableDefinition<T> Definition { get; set; }
    ConsolePointer Start { get; set; }
    ConsolePointer End { get; set; }
}
