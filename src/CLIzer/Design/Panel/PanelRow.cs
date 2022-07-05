using CLIzer.Contracts.Design;

namespace CLIzer.Design.Panel;

public class PanelRow : IPanelRow
{
    public IDesignComponent[] Columns { get; set; }

    public PanelRow(params IDesignComponent[] columns)
    {
        Columns = columns;
    }
}
