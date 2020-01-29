using System;

namespace Clizer.Models
{
    public class ClizerException : Exception
    {
        public ClizerException(string message) : base(message) { }
    }
}
