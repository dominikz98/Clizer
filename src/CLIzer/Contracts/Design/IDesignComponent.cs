using CLIzer.Design.Panel;

namespace CLIzer.Contracts.Design;

public interface IDesignComponent
{
    internal CanvasSize? Canvas { get; set; }
    internal event EventHandler<int> OnHeightChanged;
}