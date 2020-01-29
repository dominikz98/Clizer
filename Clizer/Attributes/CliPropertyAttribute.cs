using System;

namespace Clizer.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
    public class CliPropertyAttribute : Attribute
    {
        public string Name { get; }
        public string Short { get; set; }
        public CommandPropertyType Type { get; set; } = CommandPropertyType.Option;
        public string Help { get; set; }
        public bool Required { get; set; }

        public CliPropertyAttribute(string name)
        {
            Name = name;
        }

    }

    public enum CommandPropertyType { Option, Argument }
}
