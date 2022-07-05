using CLIzer.Contracts.Design;

namespace CLIzer.Design.Panel;

public class Panel
{
    public IPanelRow[] Rows { get; }
    public ConsolePointer? Start { get; private set; }
    public ConsolePointer? End { get; private set; }

    private bool _onRedraw = false;

    public Panel(IPanelRow[] rows)
    {
        Rows = rows;
        RegisterComponents();
    }

    public void Draw()
        => Draw(null);

    private void Draw(IDesignComponent? sender)
    {
        _onRedraw = true;
        Start ??= ConsolePointer.CreateByCurrent(true);

        var rowIndex = 0;
        foreach (var row in Rows)
        {
            foreach (var column in row.Columns)
            {
                // calculate relative start pointer position
                column.Canvas!.Pointer.Top = rowIndex < 0
                    ? Start.Top
                    : Rows.ToList()
                        .GetRange(0, rowIndex)
                        .Select(x => x.Columns.Max(y => y.Canvas?.Height ?? 0))
                        .Sum();

                if (sender is not null && column == sender)
                    continue;

                column.Draw();
            }

            rowIndex++;
        }

        if (End is null)
        {
            End = ConsolePointer.CreateByCurrent(true);
            End.Top += 1;
        }

        _onRedraw = false;
    }

    private void RegisterComponents()
    {
        var fullWidth = Console.WindowWidth;

        foreach (var row in Rows)
        {
            var counter = 0;
            var canvasWidth = fullWidth / row.Columns.Length;

            foreach (var column in row.Columns)
            {
                var pointer = new ConsolePointer(counter * canvasWidth, 0);
                column.Canvas = new CanvasSize(pointer, canvasWidth, 0);
                column.OnDrawed += OnReDrawRequired;
                counter++;
            }
        }
    }

    private void OnReDrawRequired(object? sender, int height)
    {
        if (sender is not IDesignComponent component)
            return;

        var canvas = Rows.SelectMany(x => x.Columns).First(x => x == component).Canvas;
        canvas!.Height = height;

        if (!_onRedraw)
        {
            Draw(component);
            return;
        }

        if (End is not null)
            Console.SetCursorPosition(End.Left, End.Top);
    }
}
