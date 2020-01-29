using Clizer.Attributes;
using System;

namespace Clizer.Models
{
    internal class CliCmd
    {
        public Type Class { get; set; }
        public CliCmdAttribute Attribute { get; set; }

        public CliCmd(Type cClass, CliCmdAttribute attribute)
        {
            Class = cClass;
            Attribute = attribute;
        }

    }
}
