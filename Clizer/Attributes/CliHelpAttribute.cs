using System;

namespace Clizer.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CliHelpAttribute : Attribute
    {
        internal string _Helptext { get; set; }

        public CliHelpAttribute(string helptext)
        {
            _Helptext = helptext;
        }
    }
}
