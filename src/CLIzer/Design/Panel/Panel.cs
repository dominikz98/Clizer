using CLIzer.Contracts.Design;

namespace CLIzer.Design.Panel;

public class Panel
{
    public IPanelRow[] Rows { get; }

    public Panel(IPanelRow[] rows)
    {
        Rows = rows;
    }

    public void Init()
    {
        var fullWidth = Console.WindowWidth;

        foreach (var row in Rows)
        {
            var counter = 0;
            var canvasWidth = fullWidth / row.Columns.Count();

            foreach (var column in row.Columns)
            {
                var pointer = new ConsolePointer(counter * canvasWidth, 0);
                column.Canvas = new CanvasSize(pointer, canvasWidth, 0);
                //column.OnHeightChanged
                counter++;
            }
        }
    }
}
