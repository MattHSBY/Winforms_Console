using MattUtils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Custom_Winforms_Library
{
    public class CommandRegistry
    {
        private Dictionary<string, ICommand> _commands = new Dictionary<string, ICommand>(StringComparer.OrdinalIgnoreCase);
        public Dictionary<string, string> _aliases = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private ICommand commandNotFound;
        private Logger logger;
        public void RegisterCommand(ICommand command, string key)
        {
            _commands[key] = command;
            Debug.WriteLine($"{command} registered");
        }
        
        public CommandRegistry(Logger logger)
        {
            this.logger = logger;
            RegisterCommand(new HelloCommand(logger), "hello");
            RegisterCommand(new EchoCommand(logger), "echo");
            RegisterCommand(new ExitCommand(), "exit");
            RegisterCommand(new ClearCommand(logger), "clear");

            RegisterAlias("clear","clr");
            RegisterAlias("help", "commands", "cmds");


            RegisterHelpCommand();
            commandNotFound = new CommandNotFound(logger);
        }
        public void RegisterHelpCommand()
        {
            _commands["help"] = new HelpCommand(logger,_commands);

        }
        public void RegisterAlias(string command, params string[] aliases)
        {
            for (int i = 0; i < aliases.Length; i++)
            {
                _aliases[aliases[i]] = command;
            }

        }
        

        public ICommand GetCommand(string commandName)
        {
            if (_commands.TryGetValue(commandName, out ICommand command))
            {
                return command;
            }
            if (_aliases.TryGetValue(commandName, out string alias))
            {
                if (_commands.TryGetValue(alias, out command))
                {
                    return command;
                }
            }
            return commandNotFound;


        }

        public string GetHelp(string commandName)
        {
            ICommand command = GetCommand(commandName);
            return command?.GetHelp() ?? commandNotFound.GetHelp();
        }

    }
}
