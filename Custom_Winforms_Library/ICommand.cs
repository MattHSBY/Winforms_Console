using MattUtils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Custom_Winforms_Library
{
    public interface ICommand
    {
        void Execute(string[] args);
        string GetHelp();

    }
    public class EchoCommand : ICommand
    {
        private Logger logger;
        public EchoCommand(Logger logger)
        {
            this.logger = logger;
        }
        public void Execute(string[] args)
        {
            string message = args.Join(" ");
            logger.LogMessage(message);
        }
        public string GetHelp()
        {
            return $"Outputs your message. Usage : echo <message>";
        }
    }

    public class HelloCommand : ICommand
    {
        private Logger logger;

        public HelloCommand(Logger logger)
        {
            this.logger = logger;
        }

        public void Execute(string[] args)
        {
            logger.LogMessage($"hello.");
        }

        public string GetHelp()
        {
            return $"Simple command; outputs 'hello.' Usage : hello";
        }
    }

    public class HelpCommand : ICommand
    {
        private readonly Logger logger;
        private readonly Dictionary<string, ICommand> command_dictionary;
        public HelpCommand(Logger logger, Dictionary<string,ICommand> command_dictionary)
        {
            this.logger = logger;
            this.command_dictionary = command_dictionary; 
        }

        private void UnrecognisedCommand()
        {
            CommandNotFound commandNotFound = new CommandNotFound(logger);
            commandNotFound.Execute(Array.Empty<string>());
        }

        public void Execute(string[] args)
        {
            if (args.Length == 0)
            {
                foreach (var kvp in command_dictionary.OrderBy(k => k.Key))
                {
                    string key = kvp.Key;
                    ICommand command = command_dictionary[key];
                    List<(string text, TextProperties properties)> msg = new List<(string text, TextProperties properties)>
                    {
                        ($"{key}: ", new TextProperties(Color.Blue, fontStyle: FontStyle.Bold)),
                        ($"{command.GetHelp()}", new TextProperties(Color.Black))
                    };
                    logger.LogRawMessage(msg);

                }
            } 
            else if (args.Length == 1)
            {
                bool found = false;
                foreach (var kvp in command_dictionary.OrderBy(k => k.Key))
                {
                    string key = kvp.Key;
                    if (key.EqualsIgnoreCase(args[0]))
                    {
                        found = true;
                        ICommand command = command_dictionary[key];
                        List<(string text, TextProperties properties)> msg = new List<(string text, TextProperties properties)>
                        {
                            ($"{key}: ", new TextProperties(Color.Blue, fontStyle: FontStyle.Bold)),
                            ($"{command.GetHelp()}", new TextProperties(Color.Black))
                        };
                        logger.LogRawMessage(msg);
                    }
                }
                if (!found)
                {
                    UnrecognisedCommand();
                }
            } 
            else
            {
                UnrecognisedCommand();
            }

        }
        public string GetHelp()
        {
            return $"Displays all registered commands and their descriptions. Usage : help <command_name>";
        }
    }

    public class ExitCommand : ICommand
    {
        private readonly Logger? logger;
        public ExitCommand(Logger logger)
        {
            this.logger = logger;
        }
        public ExitCommand()
        {
            logger = null;
        }
        public void Execute(string[] args)
        {
            if (logger != null)
            {
                logger.LogMessage($"Exiting program.");
            }
            Application.Exit();
        }

        public string GetHelp()
        {
            return $"Exits the program. Usage : exit";
        }
    }

    public class ClearCommand : ICommand
    {
        private readonly Logger? logger;

        public ClearCommand(Logger logger)
        {
            this.logger = logger;


        }
        
        public void Execute(string[] args)
        {
            logger.ClearConsoleMessage();
        }

        public string GetHelp()
        {
            return $"Clears the console of all messages. Usage : clear";
        }
    }

    public class CommandNotFound : ICommand
    {
        private Logger logger;
        private string output = $"Command not found. Use {'"'}help{'"'}.";
        public CommandNotFound(Logger logger)
        {
            this.logger = logger;
        }

        public void Execute(string[] args)
        {
            logger.LogMessage(output);
        }
        public string GetHelp()
        {
            return output;
        }
    }

    public class AliasCommand : ICommand
    {
        private CommandRegistry registry;
        private readonly Logger? logger;
        private Dictionary<string, string> aliases;
        public AliasCommand(Logger logger, CommandRegistry registry)
        {
            this.registry = registry;
            this.aliases = registry._aliases;
        }

        public void Execute(string[] args)
        {
            if (args.Length > 0 && !args[0].EqualsIgnoreCase("list"))
            {


            } else
            {
                foreach (KeyValuePair<string, string> kvp in aliases)
                {
                    logger.LogMessage($"{kvp.Key} => {kvp.Value}");
                }
            }

        }

        public string GetHelp()
        {
            return "Controls the aliases for commands. Usage : (1) Alias list => lists all registered aliases. (2) Alias <new alias> <mapped command> (3) Alias Remove ";
        }
    }
}
