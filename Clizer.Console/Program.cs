using Clizer.Console.Services;
using Clizer.Models;
using System;
using System.Threading.Tasks;

namespace Clizer.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var dependencycontainer = new SimpleInjector.Container();
            //dependencycontainer.Register<ITestService, TestService>();

            var clizer = new Clizer();
            clizer.Configure(new ClizerConfiguration()
                .SetExceptionColor(ConsoleColor.Red)
                .IgnoreLowerUpperCase(true)
                .AddDependencyContainer(dependencycontainer));
            await clizer.Execute(args);
        }
    }
}
