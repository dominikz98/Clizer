
using CLIzer;

await new Clizer()
    .Configure(config => config
    .RegisterCommands(typeof(Program).Assembly))
    .Execute(new string[] { "table" });
