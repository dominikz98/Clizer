namespace CLIzer.Design.Panel;

internal class CanvasSize
{
    public ConsolePointer Pointer { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }

    public CanvasSize(ConsolePointer pointer, int width, int height)
    {
        Pointer = pointer;
        Width = width;
        Height = height;
    }
}
