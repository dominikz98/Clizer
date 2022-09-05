using CLIzer.Contracts.Design;
using CLIzer.Design.Panel;
using CLIzer.Extensions;

namespace CLIzer.Design.ProgressBar;

public class ProgressBar : IDesignComponent
{
    public string? Title { get; }
    public ConsoleColor Color { get; set; } = Console.ForegroundColor;
    public int Count { get; private set; }
    public int Max { get; private set; } = 100;

    CanvasSize? IDesignComponent.Canvas { get; set; }
    public event EventHandler<int>? OnDrawed;

    private ConsolePointer? _internalPointer;

    public ProgressBar() { }

    public ProgressBar(string title)
    {
        Title = title;
    }

    public void Step(int count, int max)
    {
        Count = count;
        Max = max;
        ((IDesignComponent)this).Draw();
    }

    void IDesignComponent.Draw()
    {
        // set and store cursor position 
        _internalPointer ??= ConsolePointer.CreateByCurrent();
        var canvasPointer = ((IDesignComponent)this).Canvas?.Pointer ?? _internalPointer;
        var canvasWidth = ((IDesignComponent)this).Canvas?.Width ?? Console.WindowWidth;
        Console.SetCursorPosition(canvasPointer.Left, canvasPointer.Top);

        // draw prefix
        var valueInPercent = Max / 100 * Count;
        var prefix = string.Empty;
        if (!string.IsNullOrWhiteSpace(Title))
            prefix += $" {Title}";

        prefix = $"{prefix} {valueInPercent,3}%";
        Console.Write(prefix);

        // get filled length in percent
        var valueLength = canvasWidth - (canvasPointer.Left - canvasPointer.Left + prefix.Length) - 4;
        var filled = (int)((double)valueLength / 100 * valueInPercent);

        // draw bar
        Console.Write(" |");

        var filledValue = "".PadLeft(filled, '=');
        ConsoleExtensions.Write(filledValue, Color);

        var emptyValue = "".PadLeft(valueLength - filledValue.Length, '-');
        Console.Write(emptyValue);

        Console.Write("| ");

        OnDrawed?.Invoke(this, 1);
    }
}