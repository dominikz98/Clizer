using Clizer.Contracts;
using Clizer.Models;
using Clizer.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Clizer
{
    public class Clizer
    {
        private ClizerConfiguration _configuration;
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
                AttachAndValidateArguments(parameter);
                return await ExecuteCommand(cancellationToken);
            }
            catch (Exception ex)
            {
                _configuration._ExceptionHandler(ex);
                return (int)ClizerExitCodes.ERROR;
            }
        }

        /// <summary>
        /// Configure Clizer
        /// </summary>
        public ClizerConfiguration Configure() => _configuration;

        private void RegisterCommandsAsDependencies()
        {
            if (_configuration._CommandContainer?._RootCommand == null)
                throw new ClizerException($"Root command must be registered! (Call {nameof(ClizerConfiguration.AddCommandContainer)} in configuration)");

            RegisterCommandAsDependencies(_configuration._CommandContainer._RootCommand);
        }

        private void RegisterCommandAsDependencies(CommandRegistration command)
        {
            _configuration._DependencyContainer.Register(command.CmdType);
            if (command.Childrens.Any())
                foreach (var children in command.Childrens)
                    RegisterCommandAsDependencies(children);
        }

        private string[] GetCalledCommand(string[] args)
        {
            _calledCommand = _configuration._CommandContainer._RootCommand;
            for (int i = 0; i < args.Length; i++)
            {
                var nextCommand = _calledCommand.Childrens?.FirstOrDefault(x => x.Name == args[i]);
                if (nextCommand == null)
                    return args.ToList().GetRange(i, args.Length - i).ToArray();
                _calledCommand = nextCommand;
            }
            return new string[0];
        }

        private void AttachAndValidateArguments(string[] args)
        {
            var cmdinstance = _configuration._DependencyContainer.GetInstance(_calledCommand.CmdType);

            foreach (var arg in args)
            {
                var property = cmdinstance.GetType().GetProperties().FirstOrDefault(x => x.Name.IgnoreCase(true) == arg.Split(':')[0].IgnoreCase(true));
                if (property == null)
                    throw new ClizerException($"{arg} is not a known property of {_calledCommand.Name}!");

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
            foreach (var property in cmdinstance.GetType().GetProperties())
                foreach (var validateattr in property.GetCustomAttributes(false).Where(x => x is ValidationAttribute))
                    ((ValidationAttribute)validateattr).Validate(property.GetValue(cmdinstance), property.Name);
        }

        private async Task<int> ExecuteCommand(CancellationToken cancellationToken)
            => await ((ICliCmd)_configuration._DependencyContainer.GetInstance(_calledCommand.CmdType)).Execute(cancellationToken);
    }
}
