using CLIzer.Contracts.Design;

namespace CLIzer.Contracts.Tables
{
    public interface ITableColumnDefinition<T> 
    {
        public string? Name { get; }
        ConsoleColor Color { get; }
        Alignment Alignment { get; }
        public Func<T, string> ValueAccessor { get; }
        public bool CanShrink { get; }
        public bool PadIfPossible { get; }
    }
}
