namespace CLIzer.Design.Panel;

internal class CanvasSize
{
    public ConsolePointer Start { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }

    public CanvasSize(ConsolePointer start, int width, int height)
    {
        Start = start;
        Width = width;
        Height = height;
    }
}
