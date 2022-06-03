namespace CLIzer.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class CliHelpAttribute : Attribute
{
    internal string Helptext { get; set; }

    public CliHelpAttribute(string helptext)
    {
        Helptext = helptext;
    }
}
