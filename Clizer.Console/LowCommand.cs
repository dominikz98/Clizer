using Clizer.Attributes;

namespace Clizer.Console
{
    [CliCmd("low", typeof(LowerCommand))]
    public class LowCommand
    {
        [CliProperty("--force", Help = "Execute action forced.", Short = "-f", Type = CommandPropertyType.Option, Required = true)]
        public bool Force { get; set; }

        [CliProperty("value", Help = "Test value.", Short = "v", Type = CommandPropertyType.Argument)]
        public string Test { get; set; }
    }
}
