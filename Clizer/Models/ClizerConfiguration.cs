using Clizer.Contracts;
using SimpleInjector;
using System;
using System.Collections.Generic;

namespace Clizer.Models
{
    public class ClizerConfiguration
    {
        internal Container _DependencyContainer { get; private set; } = new Container();
        internal Action<Exception> _ExceptionHandler { get; private set; }
            = (ex) =>
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
            };
        internal CommandContainer _CommandContainer { get; private set; }

        public ClizerConfiguration AddDependencyContainer(Container dependecycontainer)
        {
            _DependencyContainer = dependecycontainer;
            return this;
        }

        public ClizerConfiguration SetExceptionHandler(Action<Exception> handler)
        {
            _ExceptionHandler = handler;
            return this;
        }

        public ClizerConfiguration AddCommandContainer(CommandContainer commandContainer)
        {
            _CommandContainer = commandContainer;
            return this;
        }
    }

    public class CommandContainer
    {
        internal CommandRegistration _RootCommand;

        public CommandContainer(Type rootcommand) : this(rootcommand, string.Empty) { }
        public CommandContainer(Type rootcommand, string helptext)
            => _RootCommand = new CommandRegistration(rootcommand, string.Empty, helptext);

        public CommandContainer Register<TParent, TChild>(string childname) where TParent : ICliCmd where TChild : ICliCmd
            => Register<TParent, TChild>(childname, string.Empty);

        public CommandContainer Register<TParent, TChild>(string childname, string childhelptext) where TParent : ICliCmd where TChild : ICliCmd
        {
            var parent = _RootCommand.Find(typeof(TParent));
            if (parent == null)
                throw new Exception("Parent not registered (at this moment)");
            parent.AddChild(new CommandRegistration(typeof(TChild), childname, childhelptext));
            return this;
        }

    }

    internal class CommandRegistration
    {
        public Type CmdType { get; private set; }
        public string Name { get; private set; }
        public string Help { get; private set; }
        public List<CommandRegistration> Childrens { get; private set; } = new List<CommandRegistration>();

        public CommandRegistration(Type type, string name, string helptext)
        {
            CmdType = type;
            Name = name;
            Help = helptext;
        }

        public CommandRegistration(Type type, string name) : this(type, name, string.Empty) { }

        public void AddChild(CommandRegistration child)
            => Childrens.Add(child);

        public CommandRegistration Find(Type type)
        {
            if (type == CmdType)
                return this;

            foreach (var child in Childrens)
            {
                var result = child.Find(type);
                if (result != null)
                    return result;
            }
            return null;
        }
    }
}
