using System;

namespace Clizer.Attributes
{
    public class CliArgumentAttribute : CliPropertyAttribute
    {
        public bool Required { get; set; }

        public CliArgumentAttribute(string name) : this(name, true) { }
        public CliArgumentAttribute(string name, bool required) : base(name) { Required = required; }

    }

    public class CliOptionAttribute : CliPropertyAttribute
    {
        public CliOptionAttribute(string name) : base(name) { }
    }

    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
    public abstract class CliPropertyAttribute : Attribute
    {
        public string Name { get; }
        public string Short { get; set; }
        public string Help { get; set; }

        public CliPropertyAttribute(string name) { Name = name; }
    }
}
