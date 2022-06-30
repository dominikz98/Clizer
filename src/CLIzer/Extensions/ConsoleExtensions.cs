namespace CLIzer.Extensions;

public static class ConsoleExtensions
{
    public static void AppendToLine(string text, int position, ConsoleColor color)
    {
        var left = Console.CursorLeft;
        Console.CursorTop--;
        Console.CursorLeft = position;
        Write(text, color);
        Console.CursorTop++;
        Console.CursorLeft = left;
    }

    public static void WriteLine(string text, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(text);
        Console.ResetColor();
    }

    public static void Write(string text, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.Write(text);
        Console.ResetColor();
    }
}
