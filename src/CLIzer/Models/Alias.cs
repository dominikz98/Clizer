namespace CLIzer.Models;

internal class Alias
{
    public string Name { get; set; }
    public List<string> Commands { get; set; }

    public Alias(string name, List<string> commands)
    {
        Name = name;
        Commands = commands;
    }
}
