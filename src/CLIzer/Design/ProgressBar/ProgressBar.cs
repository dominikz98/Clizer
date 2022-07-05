using CLIzer.Contracts.Design;
using CLIzer.Design.Panel;
using CLIzer.Extensions;

namespace CLIzer.Design.ProgressBar;

public class ProgressBar : IDesignComponent<ProgressBarValue>
{
    public string? Title { get; }
    public ConsoleColor Color { get; set; } = Console.ForegroundColor;

    public ProgressBarValue? Value { get; private set; }
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
        Value = new ProgressBarValue(count, max);
        ((IDesignComponent<ProgressBarValue>)this).Draw(false);
    }

    void IDesignComponent.Draw(bool suppressReDraw)
    {
        // set and store cursor position 
        _internalPointer ??= ConsolePointer.CreateByCurrent();
        var canvasPointer = ((IDesignComponent<ProgressBarValue>)this).Canvas?.Pointer ?? _internalPointer;
        var canvasWidth = ((IDesignComponent<ProgressBarValue>)this).Canvas?.Width ?? Console.WindowWidth;
        Console.SetCursorPosition(canvasPointer.Left, canvasPointer.Top);

        // draw prefix
        var valueInPercent = (Value?.Max ?? 100) / 100 * (Value?.Count ?? 0);
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