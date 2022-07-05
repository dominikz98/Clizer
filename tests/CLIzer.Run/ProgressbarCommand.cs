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

            pb.Step(25, 100);
            //await Task.Delay(2000, cancellationToken);

            //pb.Step(50, 100);
            //await Task.Delay(2000, cancellationToken);

            //pb.Step(75, 100);
            //await Task.Delay(2000, cancellationToken);

            //pb.Step(100, 100);
            //await Task.Delay(2000, cancellationToken);

            //pb.Step(95, 100);

            return ClizerExitCode.SUCCESS;
        }
    }
}
