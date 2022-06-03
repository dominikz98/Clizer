
using CLIzer;
using CLIzer.Contracts;
using CLIzer.Design.Tables;

await new Clizer()
    .Configure(config => config
    .RegisterCommands(typeof(Program).Assembly))
    .Execute(Array.Empty<string>());
