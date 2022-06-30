namespace CLIzer.Design;

public class ConsolePointer
{
    public int Left { get; set; }
    public int Top { get; set; }

    public ConsolePointer(int left, int top)
    {
        Left = left;
        Top = top;
    }

    internal static ConsolePointer CreateByStart()
    {
        var (left, top) = Console.GetCursorPosition();

        // needs to start in first position of a line
        if (left != 0)
        {
            Console.Write(Environment.NewLine);
            left = 0;
        }
        return new ConsolePointer(left, top);
    }

    internal static ConsolePointer CreateByCurrent()
    {
        var (left, top) = Console.GetCursorPosition();
        return new ConsolePointer(left, top);
    }
}
