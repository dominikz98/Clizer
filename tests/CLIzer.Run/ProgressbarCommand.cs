using CLIzer.Attributes;
using CLIzer.Contracts;
using CLIzer.Design.ProgressBar;

namespace CLIzer.Run
{
    [CliName("pb")]
    public class ProgressbarCommand : ICliCmd
    {
        public async Task<ClizerExitCode> Execute(CancellationToken cancellationToken)
        {
            var pb = new ProgressBar("downloading ...")
            {
                Color = ConsoleColor.Blue
            };

            pb.Draw(25, 100);
            await Task.Delay(2000, cancellationToken);

            pb.Draw(50, 100);
            await Task.Delay(2000, cancellationToken);

            pb.Draw(75, 100);
            await Task.Delay(2000, cancellationToken);

            pb.Draw(100, 100);
            await Task.Delay(2000, cancellationToken);

            pb.Draw(95, 100);

            return ClizerExitCode.SUCCESS;
        }
    }
}
