using CLIzer;
using CLIzer.Contracts;

var clizer = new Clizer()
    .Configure((config) => config
        .RegisterCommands(container => container
            .Root<MainCommand>()));

await clizer.Execute(args.ToArray());

class MainCommand : ICliCmd
{
    public async Task<ClizerExitCode> Execute(CancellationToken cancellationToken)
    {

        do
        {
            Console.WriteLine(cancellationToken.IsCancellationRequested);
            await Task.Delay(2000);
        } while (true);


        return ClizerExitCode.SUCCESS;
    }
}