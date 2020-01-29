using Clizer.Attributes;

namespace Clizer.Console
{
    [CliCmd("low", typeof(LowerCommand))]
    public class LowCommand
    {
        [CliOption("--force", Help = "Execute action forced.", Short = "-f")]
        public bool Force { get; set; }

        [CliArgument("value", Help = "Test value.", Short = "v")]
        public string Test { get; set; }
    }
}
