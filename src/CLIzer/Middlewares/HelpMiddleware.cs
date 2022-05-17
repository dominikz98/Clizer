using CLIzer.Contracts;
using CLIzer.Utils;

namespace CLIzer.Middlewares
{
    public class HelpMiddleware : IClizerMiddleware
    {
        private readonly HelpTextPrinter _printer;

        public const string HelpCommand = "help";
        public const string HelpOption = "--help";
        public const string HelpOptionShortcut = "-h";
        private static readonly IReadOnlyCollection<string> _helpAll = new List<string>() { HelpCommand, HelpOption, HelpOptionShortcut };

        public HelpMiddleware(HelpTextPrinter printer)
        {
            _printer = printer;
        }

        public Task<ClizerPostAction> Intercept(CommandResolver commandResolver, string[] args, CancellationToken cancellationToken)
        {
            if (args.Intersect(_helpAll).Any())
            {
                _printer.Print();
                return Task.FromResult(ClizerPostAction.EXIT);
            }

            return Task.FromResult(ClizerPostAction.CONTINUE);
        }
    }
}
