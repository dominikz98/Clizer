﻿using System;

namespace Clizer.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CliIArgAttribute : Attribute
    {
        internal string? Helptext { get; private set; }

        internal string? _name;
        internal string? Name
        {
            get { return !(_name ?? string.Empty).StartsWith("--") ? "--" + _name : _name; }
            private set { _name = value; }
        }

        internal string? _shortcut;
        internal string Shortcut
        {
            get { return !string.IsNullOrEmpty(_shortcut) && !_shortcut.StartsWith("-") ? "-" + _shortcut : (_shortcut ?? string.Empty); }
            private set { _shortcut = value; }
        }


        public CliIArgAttribute(string name, string shortcut, string helptext)
        {
            Helptext = helptext;
            Name = name;
            Shortcut = shortcut;
        }

        public CliIArgAttribute(string name, string shortcut) : this(name, shortcut, string.Empty) { }

        public CliIArgAttribute(string name) : this(name, string.Empty, string.Empty) { }
    }
}
