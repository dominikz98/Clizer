using CLIzer.Contracts;
using CLIzer.Extensions;
using CLIzer.Models;

namespace CLIzer.Middlewares;

public class HelpMiddleware : IClizerMiddleware
{
    private const string HelpCommand = "help";
    private const string HelpOption = "--help";
    private const string HelpOptionShortcut = "-h";
    private static readonly IReadOnlyCollection<string> HelpAll = new List<string>() { HelpCommand, HelpOption, HelpOptionShortcut };

    public Task<ClizerPostAction> Intercept(CommandContext context, CancellationToken cancellationToken)
    {
        if (!context.Args.Intersect(HelpAll).Any()
            && !context.Execute.Equals(HelpCommand, StringComparison.OrdinalIgnoreCase))
            return Task.FromResult(ClizerPostAction.CONTINUE);

        if (context.Command is null &&
            context.CallChain.Count < 1
            && context.RootCommand is null)
            return Task.FromResult(ClizerPostAction.EXIT);

        var command = context.Command ?? (context.CallChain.Count < 1 ? context.RootCommand! : context.CallChain.Last());
        Console.WriteLine(command.Name + (!string.IsNullOrEmpty(command.Type.GetHelptext()) ? ": " + command.Type.GetHelptext() : string.Empty));
        Console.WriteLine(string.Empty);

        var children = command.Commands;
        if (children.Any())
        {
            Console.WriteLine("[Commands]");
            Console.WriteLine(string.Join(Environment.NewLine, children.Select(x => $" {x.Name + (!string.IsNullOrEmpty(x.Type.GetHelptext()) ? ": " + x.Type.GetHelptext() : string.Empty)}").ToArray()));
            Console.WriteLine(string.Empty);
        }

        var arguments = command.Type.GetArguments();
        if (arguments.Any())
        {
            Console.WriteLine("[Arguments]");
            Console.WriteLine(string.Join(Environment.NewLine, arguments.Select(x => $" {(x.Name + (!string.IsNullOrEmpty(x.Shortcut) ? " | " + x.Shortcut : string.Empty) + (!string.IsNullOrEmpty(x.Helptext) ? ": " + x.Helptext : string.Empty))}").ToArray()));
            Console.WriteLine(string.Empty);
        }

        var options = command.Type.GetOptions();
        if (options.Any())
        {
            Console.WriteLine("[Options]");
            Console.WriteLine(string.Join(Environment.NewLine, options.Select(x => $" {(x.Name + (!string.IsNullOrEmpty(x.Shortcut) ? " | " + x.Shortcut : string.Empty) + (!string.IsNullOrEmpty(x.Helptext) ? ": " + x.Helptext : string.Empty))}").ToArray()));
        }

        return Task.FromResult(ClizerPostAction.EXIT);
    }
}
