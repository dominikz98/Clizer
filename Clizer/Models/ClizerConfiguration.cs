using SimpleInjector;
using System;

namespace Clizer.Models
{
    public class ClizerConfiguration
    {
        internal ConsoleColor ExceptionColor { get; private set; } = ConsoleColor.Red;
        internal bool IgnoreStringCase { get; private set; } = true;
        internal Container DependencyContainer { get; private set; }

        public ClizerConfiguration SetExceptionColor(ConsoleColor color)
        {
            ExceptionColor = color;
            return this;
        }

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
    }
}
