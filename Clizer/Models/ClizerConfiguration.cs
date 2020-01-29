using SimpleInjector;
using System;

namespace Clizer.Models
{
    public class ClizerConfiguration
    {
        internal bool IgnoreStringCase { get; private set; } = true;
        internal Container DependencyContainer { get; private set; }
        internal Action<Exception> ExceptionHandler { get; private set; }
            = (ex) =>
            {
                if (ex is ClizerException)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Handler: " + ex.Message);
                    Console.ResetColor();
                }
                else
                    throw ex;
            };

        public ClizerConfiguration IgnoreLowerUpperCase(bool irgnore)
        {
            IgnoreStringCase = irgnore;
            return this;
        }

        public ClizerConfiguration AddDependencyContainer(Container container)
        {
            DependencyContainer = container;
            return this;
        }

        public ClizerConfiguration SetExceptionHandler(Action<Exception> handler)
        {
            ExceptionHandler = handler;
            return this;
        }
    }
}
