
using CLIzer;
using CLIzer.Run;

await new Clizer()
    .Configure(config => config
    .RegisterCommands<TableCommand>())
    .Execute();
