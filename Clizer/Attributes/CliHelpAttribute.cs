﻿using System;

namespace Clizer.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
    public class CliHelpAttribute : Attribute
    {
        internal string Helptext { get; set; }

        public CliHelpAttribute(string helptext)
        {
            Helptext = helptext;
        }
    }
}
