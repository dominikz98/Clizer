using System;
using System.Threading.Tasks;

namespace Clizer.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var clizer = new Clizer();
            clizer.Configure(new ClizerConfiguration().SetExceptionColor(ConsoleColor.Red));
            await clizer.Execute(args);
        }
    }
}
