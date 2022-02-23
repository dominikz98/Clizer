using CLIzer.Contracts;
using CLIzer.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace CLIzer.Help
{
    public class HelpTextPrinter
    {
        private readonly IServiceProvider _services;
        private readonly CommandResolver _resolver;

        public HelpTextPrinter(IServiceProvider services, CommandResolver resolver)
        {
            _services = services;
            _resolver = resolver;
        }

        public int Print()
        {
            if (_resolver.Called is null)
                return (int)ClizerExitCode.ERROR;

            var cmdinstance = _services.GetRequiredService(_resolver.Called.Type);

            Console.WriteLine(_resolver.Called.Name + (!string.IsNullOrEmpty(_resolver.Called.Type.GetHelptext()) ? ": " + _resolver.Called.Type.GetHelptext() : string.Empty));
            Console.WriteLine(string.Empty);

            var children = _resolver.Called.Commands;
            if (children.Any())
            {
                Console.WriteLine("[Commands]");
                Console.WriteLine(string.Join(Environment.NewLine, children.Select(x => $" {x.Name + (!string.IsNullOrEmpty(x.Type.GetHelptext()) ? ": " + x.Type.GetHelptext() : string.Empty)}").ToArray()));
                Console.WriteLine(string.Empty);
            }

            var arguments = _resolver.Called.Type.GetArguments();
            if (arguments.Any())
            {
                Console.WriteLine("[Arguments]");
                Console.WriteLine(string.Join(Environment.NewLine, arguments.Select(x => $" { (x.Name + (!string.IsNullOrEmpty(x.Shortcut) ? " | " + x.Shortcut : string.Empty) + (!string.IsNullOrEmpty(x.Helptext) ? ": " + x.Helptext : string.Empty))}").ToArray()));
                Console.WriteLine(string.Empty);
            }

            var options = _resolver.Called.Type.GetOptions();
            if (options.Any())
            {
                Console.WriteLine("[Options]");
                Console.WriteLine(string.Join(Environment.NewLine, options.Select(x => $" {(x.Name + (!string.IsNullOrEmpty(x.Shortcut) ? " | " + x.Shortcut : string.Empty) + (!string.IsNullOrEmpty(x.Helptext) ? ": " + x.Helptext : string.Empty))}").ToArray()));
            }

            return (int)ClizerExitCode.SUCCESS;
        }
    }
}
