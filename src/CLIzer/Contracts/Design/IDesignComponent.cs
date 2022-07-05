using CLIzer.Design.Panel;

namespace CLIzer.Contracts.Design;

public interface IDesignComponent
{
    internal CanvasSize? Canvas { get; set; }
    internal void Draw();
    internal event EventHandler<int> OnDrawed;
}