using CLIzer.Design.Panel;

namespace CLIzer.Contracts.Design;

public interface IDesignComponent
{
    internal CanvasSize? Canvas { get; set; }
    internal void Draw(bool suppressReDraw);
    internal event EventHandler<int> OnDrawed;
}

public interface IDesignComponent<T> : IDesignComponent
{
    T? Value { get; }
}