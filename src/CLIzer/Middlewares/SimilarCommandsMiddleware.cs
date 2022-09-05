using CLIzer.Contracts;
using CLIzer.Models;
using CLIzer.Resolver;
using CLIzer.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace CLIzer.Middlewares;

internal class SimilarCommandsMiddleware : IClizerMiddleware
{
    private readonly IServiceProvider _services;

    public SimilarCommandsMiddleware(IServiceProvider services)
    {
        _services = services;
    }

    public Task<ClizerPostAction> Intercept(CommandContext context, CancellationToken cancellationToken)
    {
        if (context.Command is not null)
            return Task.FromResult(ClizerPostAction.CONTINUE);

        Console.WriteLine($"Unknown command for {context.Execute}. See '--help'");

        // list similar commands
        if (context.RootCommand is null)
            return Task.FromResult(ClizerPostAction.CONTINUE);

        var similarCommands = context.RootCommand
            .GetAll()
            .Select(x => x.Name.ToLower())
            .Where(x => LevenshteinDistance.Compute(x, context.Execute) <= 3);

        if (similarCommands.Any())
        {
            Console.WriteLine("Most similar commands:");
            foreach (var similar in similarCommands)
                Console.WriteLine($" - {similar}");
        }

        // list similar aliases
        var aliasesResolver = _services.GetService<AliasesResolver>();
        if (aliasesResolver is null)
            return Task.FromResult(ClizerPostAction.EXIT);

        var similarAliases = aliasesResolver
            .Aliases
            .Select(x => x.Name.ToLower())
            .Where(x => LevenshteinDistance.Compute(context.Execute, x) <= 2);

        if (!similarAliases.Any())
            return Task.FromResult(ClizerPostAction.EXIT);

        Console.WriteLine(string.Empty);
        Console.WriteLine("Most similar aliases:");
        foreach (var similar in similarAliases)
            Console.WriteLine($" - {similar}");

        return Task.FromResult(ClizerPostAction.EXIT);
    }
}
