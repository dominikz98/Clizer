using System;

namespace Clizer.Models
{
    public class ClizerConfiguration
    {
        public ConsoleColor ExceptionColor { get; private set; } = ConsoleColor.Red;
        public bool IgnoreStringCase { get; private set; } = true;

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
    }
}
