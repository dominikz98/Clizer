namespace CLIzer.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class CliNameAttribute : Attribute
{
    public string Value { get; set; }

    public CliNameAttribute(string name)
    {
        Value = name;
    }
}
