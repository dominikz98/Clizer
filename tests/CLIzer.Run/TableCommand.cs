using CLIzer.Attributes;
using CLIzer.Contracts;

namespace CLIzer.Run;

[CliName("table")]
public class TableCommand : ICliCmd
{
    public Task<ClizerExitCode> Execute(CancellationToken cancellationToken)
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
        table.Draw(data);

        var newData = data.ToList().GetRange(0, data.Length - 1);
        table.Draw(newData);

        return Task.FromResult(ClizerExitCode.SUCCESS);
    }
}
