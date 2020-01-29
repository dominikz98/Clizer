using Clizer.Console.Services;
using Clizer.Models;
using System.Threading.Tasks;

namespace Clizer.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var dependencycontainer = new SimpleInjector.Container();
            dependencycontainer.Register<ITestService, TestService>();

            var clizer = new Clizer();
            clizer.Configure(new ClizerConfiguration()
                //.SetExceptionHandler((ex) => System.Console.WriteLine("FEHLER!"))
                .IgnoreLowerUpperCase(true)
                .AddDependencyContainer(dependencycontainer));
            clizer.Verify();
            await clizer.Execute(args);
        }
    }
}
