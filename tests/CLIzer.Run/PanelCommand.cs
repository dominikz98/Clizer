using CLIzer.Attributes;
using CLIzer.Contracts;
using CLIzer.Design.Panel;
using CLIzer.Design.ProgressBar;

namespace CLIzer.Run;

[CliName("panel")]
public class PanelCommand : ICliCmd
{
    public async Task<ClizerExitCode> Execute(CancellationToken cancellationToken)
    {
        var data = new StudentEntity[]
        {
            new StudentEntity(1, "Smitty", new DateTime(2000,12,29)),
            new StudentEntity(2, "Spongebob", new DateTime(1998,1,5)),
            new StudentEntity(3, "Patrick", new DateTime(2000,11,3)),
            new StudentEntity(4, "Sandy", new DateTime(2000,3,28)),
            new StudentEntity(5, "Perla", new DateTime(1995,5,11)),
        };

        var table = new StudentTable();
  
        //var pb1 = new ProgressBar("downloading ...") { Color = ConsoleColor.Blue };
        var pb2 = new ProgressBar("extracting ...") { Color = ConsoleColor.Yellow };
        var pb3 = new ProgressBar("processing ...") { Color = ConsoleColor.Red };
        var pb4 = new ProgressBar("uploading ...") { Color = ConsoleColor.Green };
        var pb5 = new ProgressBar("finishing ...") { Color = ConsoleColor.Magenta };

        var panel = new Panel(new PanelRow[] { new PanelRow(table, pb2), new PanelRow(pb3, pb4), new PanelRow(pb5) });

        panel.Draw();

        //pb1.Step(33, 100);
        pb2.Step(50, 100);
        pb3.Step(12, 100);
        pb4.Step(75, 100);
        pb5.Step(64, 100);

        return ClizerExitCode.SUCCESS;
    }
}
