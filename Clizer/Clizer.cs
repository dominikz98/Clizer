﻿using Clizer.Attributes;
using Clizer.Contracts;
using Clizer.Models;
using Clizer.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Clizer
{
    public class Clizer
    {
        private readonly ClizerConfiguration _configuration;
        private CommandRegistration _calledCommand;

        public Clizer()
            => _configuration = new ClizerConfiguration();

        /// <summary>
        /// Entry point.
        /// </summary>
        public async Task<int> Execute(string[] args, CancellationToken cancellationToken)
        {
            try
            {
                RegisterCommandsAsDependencies();
                var parameter = GetCalledCommand(args);
                if (parameter.Contains("help") || parameter.Contains("-h") || parameter.Contains("--help"))
                    return ShowHelptext();

                AttachAndValidateArguments(parameter);
                return await ExecuteCommand(cancellationToken);
            }
            catch (Exception ex)
            {
                _configuration.ExceptionHandler(ex);
                return (int)ClizerExitCodes.ERROR;
            }
        }

        /// <summary>
        /// Configure Clizer.
        /// </summary>
        public ClizerConfiguration Configure() => _configuration;

        /// <summary>
        /// Register and validate commands from CommandContainer in SimpleInjector DependencyContainer.
        /// </summary>
        private void RegisterCommandsAsDependencies()
        {
            if (_configuration.CommandContainer?._RootCommand == null)
                throw new ClizerException($"Root command must be registered! (Call {nameof(ClizerConfiguration.AddCommandContainer)} in configuration)");

            if (_configuration.CommandContainer._RootCommand.CmdType.GetInterface(typeof(ICliCmd).Name) == null)
                throw new ClizerException($"Class '{_configuration.CommandContainer?._RootCommand.CmdType.Name}' needs to implement '{typeof(ICliCmd).Name}'!");

            RegisterCommandAsDependencies(_configuration.CommandContainer._RootCommand);
        }

        /// <summary>
        /// Registers command and subcommands recursive.
        /// </summary>
        /// <param name="command">actual command</param>
        private void RegisterCommandAsDependencies(CommandRegistration command)
        {
            _configuration.DependencyContainer.RegisterSingleton(command.CmdType, command.CmdType);
            if (command.Childrens.Any())
                foreach (var children in command.Childrens)
                    RegisterCommandAsDependencies(children);
        }

        /// <summary>
        /// Sets called command.
        /// </summary>
        /// <param name="args">command arguments</param>
        /// <returns></returns>
        private string[] GetCalledCommand(string[] args)
        {
            _calledCommand = _configuration.CommandContainer._RootCommand;
            for (int i = 0; i < args.Length; i++)
            {
                var nextCommand = _calledCommand.Childrens?.FirstOrDefault(x => x.Name == args[i]);
                if (nextCommand == null)
                    return args.ToList().GetRange(i, args.Length - i).ToArray();
                _calledCommand = nextCommand;
            }
            return new string[0];
        }

        /// <summary>
        /// Attaches and validates command arguments.
        /// </summary>
        /// <param name="args">command arguments</param>
        private void AttachAndValidateArguments(string[] args)
        {
            var cmdinstance = _configuration.DependencyContainer.GetInstance(_calledCommand.CmdType);

            foreach (var arg in args)
            {
                var argname = arg.Split(':')[0].IgnoreCase(true);
                var property = cmdinstance.GetType().GetCliProperties().FirstOrDefault(x => x.GetCustomAttribute<CliIArgAttribute>().Name.IgnoreCase(true) == argname || x.GetCustomAttribute<CliIArgAttribute>().Shortcut.IgnoreCase(true) == argname);
                if (property == null)
                    throw new ClizerException($"{argname} is not a known property of {(!string.IsNullOrEmpty(_calledCommand.Name) ? _calledCommand.Name : _calledCommand.CmdType.Name)}!");

                // Argument
                if (arg.Contains(":"))
                {
                    if (arg.Split(':').Length != 2)
                        throw new ClizerException($"{arg} has an invalid argument format!");

                    var argvalue = arg.Split(':')[1];
                    try
                    {
                        property.SetValue(cmdinstance, Convert.ChangeType(arg.Split(':')?[1], property.PropertyType));
                        continue;
                    }
                    catch (Exception)
                    {
                        throw new ClizerException($"Invalid value for argument (Expected: {property.PropertyType.Name}, value: {arg})");
                    }
                }

                // Option
                property.SetValue(cmdinstance, true);
            }

            // Validate
            foreach (var property in cmdinstance.GetType().GetProperties().Where(x => x.GetCustomAttribute<CliIArgAttribute>() != null))
                foreach (var validateattr in property.GetCustomAttributes(false).Where(x => x is ValidationAttribute))
                    ((ValidationAttribute)validateattr).Validate(property.GetValue(cmdinstance), property.Name);
        }

        /// <summary>
        ///  Executes called command 'execute' method
        /// </summary>
        /// <returns>command result</returns>
        private async Task<int> ExecuteCommand(CancellationToken cancellationToken)
            => await ((ICliCmd)_configuration.DependencyContainer.GetInstance(_calledCommand.CmdType)).Execute(cancellationToken);

        /// <summary>
        /// Generates helptext for called command
        /// </summary>
        private int ShowHelptext()
        {
            var cmdinstance = _configuration.DependencyContainer.GetInstance(_calledCommand.CmdType);

            Console.WriteLine(_calledCommand.Name + (!string.IsNullOrEmpty(_calledCommand.CmdType.GetHelptext()) ? ": " + _calledCommand.CmdType.GetHelptext() : string.Empty));
            Console.WriteLine(string.Empty);

            var children = _calledCommand.Childrens;
            if (children.Any())
            {
                Console.WriteLine("[Commands]");
                Console.WriteLine(string.Join(Environment.NewLine, children.Select(x => $" {x.Name + (!string.IsNullOrEmpty(x.CmdType.GetHelptext()) ? ": " + x.CmdType.GetHelptext() : string.Empty)}").ToArray()));
                Console.WriteLine(string.Empty);
            }

            var arguments = _calledCommand.CmdType.GetArguments();
            if (arguments.Any())
            {
                Console.WriteLine("[Arguments]");
                Console.WriteLine(string.Join(Environment.NewLine, arguments.Select(x => $" { (x.Name + (!string.IsNullOrEmpty(x.Shortcut) ? " | " + x.Shortcut : string.Empty) + (!string.IsNullOrEmpty(x.Helptext) ? ": " + x.Helptext : string.Empty))}").ToArray()));
                Console.WriteLine(string.Empty);
            }

            var options = _calledCommand.CmdType.GetOptions();
            if (options.Any())
            {
                Console.WriteLine("[Options]");
                Console.WriteLine(string.Join(Environment.NewLine, options.Select(x => $" {(x.Name + (!string.IsNullOrEmpty(x.Shortcut) ? " | " + x.Shortcut : string.Empty) + (!string.IsNullOrEmpty(x.Helptext) ? ": " + x.Helptext : string.Empty))}").ToArray()));
            }

            return (int)ClizerExitCodes.SUCCESS;
        }
    }
}
