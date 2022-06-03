namespace CLIzer.Extensions;

public static class ConsoleExtensions
{
    public static void AppendToLine(ConsoleColor color, int position, string text)
    {
        var left = Console.CursorLeft;
        Console.CursorTop--;
        Console.CursorLeft = position;
        WriteColored(color, text);
        Console.CursorTop++;
        Console.CursorLeft = left;
    }

    public static void WriteColoredLine(ConsoleColor color, string text)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(text);
        Console.ResetColor();
    }

    public static void WriteColored(ConsoleColor color, string text)
    {
        Console.ForegroundColor = color;
        Console.Write(text);
        Console.ResetColor();
    }
}
