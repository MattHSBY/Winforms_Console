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
        public Dictionary<string, Command> _commands = new Dictionary<string, Command>(StringComparer.OrdinalIgnoreCase);
        public Dictionary<string, string> _aliases = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase); // <alias, command>
        private Command commandNotFound;
        private Logger logger;
        public void RegisterCommand(Command command, string key)
        {
            _commands[key] = command;
            Debug.WriteLine($"{command} registered");
        }
        
        public CommandRegistry(Logger logger)
        {
            this.logger = logger;
            RegisterCommand(new HelloCommand(logger), "hello");
            RegisterCommand(new EchoCommand(logger), "echo");
            RegisterCommand(new ExitCommand(logger), "exit");
            RegisterCommand(new ClearCommand(logger), "clear");
            RegisterCommand(new AliasCommand(logger,this), "alias");
            RegisterCommand(new SpamCommand(logger), "Spam");

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
                RemoveAlias(aliases[i]);
                _aliases[aliases[i]] = command;
            }

        }
        public KeyValuePair<string, string> RemoveAlias(string alias)
        {
            if (_aliases.TryGetValue(alias, out string value))
            {
                _aliases.Remove(alias);
                return new KeyValuePair<string, string>(alias, value);
            }
            return new KeyValuePair<string, string>();
            

        }


        public Command GetCommand(string commandName)
        {
            if (_commands.TryGetValue(commandName, out Command command))
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
            Command command = GetCommand(commandName);
            return command?.GetHelp() ?? commandNotFound.GetHelp();
        }

    }
}
