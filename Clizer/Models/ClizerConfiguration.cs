using Clizer.Constants;
using Clizer.Contracts;
using Clizer.Utils;
using Newtonsoft.Json;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.IO;

namespace Clizer.Models
{
    public class ClizerConfiguration
    {
        internal Container DependencyContainer { get; private set; } = new Container();
        internal Action<Exception> ExceptionHandler { get; private set; }
            = (ex) => ConsoleExtensions.WriteColoredLine(ConsoleColor.Red, ex.Message);
        internal CommandContainer? CommandContainer { get; private set; }


        public ClizerConfiguration EnableUserConfiguration<TConfig>() where TConfig : class, new()
        {
            try
            {
                var content = File.ReadAllText(ClizerConstants.ConfigFile);
                var instance = JsonConvert.DeserializeObject<TConfig>(content);
                DependencyContainer.RegisterSingleton(() => JsonConvert.DeserializeObject<TConfig>(content));
            }
            catch (Exception)
            {
                File.WriteAllText("config.json", JsonConvert.SerializeObject(new TConfig()));
            }
            return this;
        }

        public ClizerConfiguration AddDependencyContainer(Container dependecycontainer)
        {
            DependencyContainer = dependecycontainer;
            return this;
        }

        public ClizerConfiguration SetExceptionHandler(Action<Exception> handler)
        {
            ExceptionHandler = handler;
            return this;
        }

        public ClizerConfiguration AddCommandContainer(CommandContainer commandContainer)
        {
            CommandContainer = commandContainer;
            return this;
        }
    }

    public class CommandContainer
    {
        internal CommandRegistration _rootCommand;

        public CommandContainer(Type rootcommand)
            => _rootCommand = new CommandRegistration(rootcommand, string.Empty);

        public CommandContainer Register<TParent, TChild>(string childname) where TParent : ICliCmd where TChild : ICliCmd
        {
            var parent = _rootCommand.Find(typeof(TParent));
            if (parent == null)
                throw new Exception("Parent not registered (at this moment)");
            parent.AddChild(new CommandRegistration(typeof(TChild), childname));
            return this;
        }

    }

    internal class CommandRegistration
    {
        public Type CmdType { get; private set; }
        public string Name { get; private set; }
        public List<CommandRegistration> Childrens { get; private set; } = new List<CommandRegistration>();

        public CommandRegistration(Type type, string name)
        {
            CmdType = type;
            Name = name;
        }

        public void AddChild(CommandRegistration child)
            => Childrens.Add(child);

        public CommandRegistration? Find(Type type)
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
