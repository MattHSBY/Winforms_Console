using MattUtils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Custom_Winforms_Library
{
    public abstract class Command
    {
        protected string NewLine = $"{Environment.NewLine}    ";
        public Command(Logger logger)
        {
            this.logger = logger;
        }

        public void Execute(string[] args)
        {
            bool subcommandfound = false;
            if (args.Length > 0)
            {
                foreach (KeyValuePair<string, SubCommand> kvp in SubCommands)
                {
                    if (kvp.Key.EqualsIgnoreCase(args[0]))
                    {
                        subcommandfound = true;
                        kvp.Value.Execute(args.SelectItems(1));
                    }
                }
            } 
            if (!subcommandfound)
                Run(args);

        }

        protected abstract void Run(string[] args);
        // this method should run whenever the command (or subcommand) currently being processed
        // has either : (1) no args at all, or (2) args[0] != the name of a registered sub-command.

        public abstract string GetHelp();

        public void RegisterSubCommand(string name, SubCommand command)
        {
            SubCommands[name] = command;
        }

        public Command getSubCommand(string name)
        {
            if (SubCommands.TryGetValue(name, out SubCommand cmd)) return cmd;
            return new CommandNotFound(logger);
        }

        protected void UnrecognisedCommand()
        {
            CommandNotFound commandNotFound = new CommandNotFound(logger, GetHelp());
            commandNotFound.Execute(Array.Empty<string>());
            
        }




        protected readonly Logger logger;

        private Dictionary<string, SubCommand> SubCommands = new Dictionary<string, SubCommand>(StringComparer.OrdinalIgnoreCase);

    }
    public abstract class SubCommand : Command
    {
        public Command Parent_Command;

        public override string GetHelp()
        {
            // This means that all sub-commands return their parent command's help message.
            return Parent_Command.GetHelp();
        }

        protected SubCommand(Logger logger, Command Parent_Command) : base(logger)
        {
            this.Parent_Command = Parent_Command;
        }
    }

    public class CommandNotFound : Command
    {
        private string output = $"Command not recognised. ";
        private string extended_message = "";

        public CommandNotFound(Logger logger) : base(logger) { }
        public CommandNotFound(Logger logger, string extended_message) : base(logger) 
        {
            this.extended_message = extended_message;
        }


        protected override void Run(string[] args)
        {
            logger.LogMessage(GetHelp() + (extended_message.Equals("") ? extended_message : $"Help message = {'"'}{extended_message}{'"'}"));
        }

        public override string GetHelp()
        {
            return output;
        }

        
    }
    public class EchoCommand : Command
    {
        public EchoCommand(Logger logger) : base(logger) { }

        public override string GetHelp()
        {
            return "Outputs your message. Usage : echo <message>";
        }

        protected override void Run(string[] args)
        {
            string message = args.Join(" ");
            logger.LogMessage(message);
        }
    }
    public class HelloCommand : Command
    {
        public HelloCommand(Logger logger) : base(logger) { }

        public override string GetHelp()
        {
            return "Simple command, outputs 'Hello.' Usage : hello";
        }

        protected override void Run(string[] args)
        {
            logger.LogMessage("Hello.");
        }
    }
    public class HelpCommand : Command
    {
        private Dictionary<string, Command> command_dictionary;

        private SubHelp_ListCommand List;

        public HelpCommand(Logger logger, Dictionary<string, Command> command_dictionary) : base(logger)
        {
            this.command_dictionary = command_dictionary;
            List = new SubHelp_ListCommand(logger, this);
            RegisterSubCommand("list", List);
        }
        
        public override string GetHelp()
        {
            return "Displays all registered commands and their descriptions. Usage : help <command_name>";
        }

        
        private class SubHelp_ListCommand : SubCommand
        {
            public SubHelp_ListCommand(Logger logger, HelpCommand Parent) : base(logger, Parent)
            {
                command_dictionary = Parent.command_dictionary;
            }


            protected override void Run(string[] args)
            {
                bool found = false;
                foreach (KeyValuePair<string, Command> kvp in command_dictionary.OrderBy(k => k.Key))
                {
                    string key = kvp.Key;
                    if (args.Length == 0 || key.EqualsIgnoreCase(args[0]))
                    {
                        found = true;
                        Command cmd = command_dictionary[key];
                        List<(string text, TextProperties properties)> msg = new List<(string text, TextProperties properties)>()
                        {
                            ($"{key}: ", new TextProperties(Color.Blue, fontStyle: FontStyle.Bold)),
                            ($"{cmd.GetHelp()}", new TextProperties())
                        };
                        logger.LogRawMessage(msg);
                    }

                }
                if (!found)
                {
                    UnrecognisedCommand();
                }
            }

            private Dictionary<string, Command> command_dictionary;
        }

        protected override void Run(string[] args)
        {
            if (args.Length == 1)
            {
                string cmd_name = args[0];
                if (command_dictionary.TryGetValue(cmd_name, out Command cmd))
                {
                    List<(string text, TextProperties properties)> msg = new()
                    {
                        ($"{cmd_name}: ", new TextProperties(Color.Blue, fontStyle: FontStyle.Bold)),
                        ($"{cmd.GetHelp()}",new TextProperties())
                    };
                    logger.LogRawMessage(msg);
                    return;
                }
            }
            List.Execute(args);

            
        }
    }
    public class ExitCommand : Command
    {
        public ExitCommand(Logger logger) : base(logger) { }

        public override string GetHelp()
        {
            return $"Exits the program. Usage : exit";
        }

        protected override void Run(string[] args)
        {
            logger.LogMessage("Exiting program.");
            Application.Exit();
        }
    }
    public class ClearCommand : Command
    {
        public ClearCommand(Logger logger) : base(logger)
        {
        }

        public override string GetHelp()
        {
            return $"Clears the console of all messages. Usage : clear";
        }

        protected override void Run(string[] args)
        {
            logger.ClearConsoleMessage();
        }
    }
    public class AliasCommand : Command
    {
        private CommandRegistry registry;
        private Dictionary<string, string> aliases;
        private Dictionary<string, Command> commands;
        private SubAlias_ListCommand List;
        private SubAlias_NewCommand New;
        private SubAlias_RemoveCommand Remove;

        public AliasCommand(Logger logger, CommandRegistry registry) : base(logger)
        {
            this.registry = registry;
            commands = registry._commands;
            aliases = registry._aliases;
            List = new SubAlias_ListCommand(logger, this);
            New = new SubAlias_NewCommand(logger, this);
            Remove = new SubAlias_RemoveCommand(logger, this);
            RegisterSubCommand("List", List);
            RegisterSubCommand("New", New);
            RegisterSubCommand("Remove", Remove);
        }

        public override string GetHelp()
        {
            return $"Controls the aliases for commands. Usage : " +
                $"{NewLine}(1) Alias list => lists all registered aliases. " +
                $"{NewLine}(2) Alias list <mapped command> => lists all aliases registered to the specified command. " +
                $"{NewLine}(3) Alias <new alias 1> <new alias 2> ... <mapped command> (new alias will overwrite any preexisting aliases with the same name)" +
                $"{NewLine}(4) Alias Remove <alias> => Removes the specified alias.";
        }

        private class SubAlias_NewCommand : SubCommand
        {
            private CommandRegistry registry;
            private Dictionary<string, Command> commands;

            public SubAlias_NewCommand(Logger logger, AliasCommand Parent) : base(logger, Parent)
            {
                this.registry = Parent.registry;
                commands = registry._commands;

            }


            protected override void Run(string[] args)
            {
                if (args.Length < 2)
                {
                    UnrecognisedCommand();
                    return;
                }
                string[] new_aliases = args.SelectItems(0, args.Length-2);
                string mapped_command = args.Last();
                if (commands.TryGetValue(mapped_command, out Command cmd))
                {
                    registry.RegisterAlias(mapped_command, new_aliases);
                    logger.LogMessage($"mapped aliases: [{new_aliases.Join(", ")}] => command: {mapped_command}");
                } else
                {
                    logger.LogMessage($"Could not find {mapped_command}, are you sure this is a registered command? Use {'"'}help{'"'} for a list of registered commands.");
                }
            }
        }

        private class SubAlias_ListCommand : SubCommand
        {
            private Dictionary<string, string> aliases;
            private Dictionary<string, Command> commands;

            public SubAlias_ListCommand(Logger logger, AliasCommand Parent) : base(logger,Parent)
            {
                aliases = Parent.aliases;
                commands = Parent.commands;
            }

            private void RawLogConfig(string command_name, string[] confirmed_aliases)
            {
                if (confirmed_aliases.Length == 0) return;
                logger.LogRawMessage(
                    new List<(string text, TextProperties properties)>
                    {
                        ($"[{command_name}]: ", new TextProperties(Color.Blue, fontStyle: FontStyle.Bold)),
                        (confirmed_aliases.Join(", "), new TextProperties())
                    }
                ); // do the bold blue thing done for help. TBC...
            }

            private string[] GetCommandAliases(string command_name)
            {
                string[] confirmed_aliases = Array.Empty<string>();
                if (commands.TryGetValue(command_name, out Command cmd))
                {
                    foreach (KeyValuePair<string, string> aliasPair in aliases)
                    {
                        if (aliasPair.Value.EqualsIgnoreCase(command_name))
                        {
                            confirmed_aliases = confirmed_aliases.App(aliasPair.Key);
                        }
                    }
                    return confirmed_aliases;


                } else
                {
                    return new string[0].App("No command found.");
                }
            }

            protected override void Run(string[] args)
            {
                if (args.Length == 0)
                {
                    foreach (KeyValuePair<string, Command> commandPair in commands)
                    {
                        string[] confirmed_aliases = GetCommandAliases(commandPair.Key);
                        RawLogConfig(commandPair.Key, confirmed_aliases);
                    }
                    return;
                }
                if (commands.TryGetValue(args[0], out Command cmd))
                {
                    string[] confirmed_aliases = GetCommandAliases(args[0]);
                    RawLogConfig(args[0], confirmed_aliases);
                } else
                {
                    UnrecognisedCommand();
                }

            }
        }

        private class SubAlias_RemoveCommand : SubCommand
        {
            private CommandRegistry registry;
            public SubAlias_RemoveCommand(Logger logger, AliasCommand Parent_Command) : base(logger, Parent_Command)
            {
                registry = Parent_Command.registry;
            }

            protected override void Run(string[] args)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    KeyValuePair<string, string> remove_status = registry.RemoveAlias(args[i]);
                    string str = "";
                    str = remove_status.Key.EqualsIgnoreCase("") ? 
                        $"Could not remove alias: {'"'}{args[i]}{'"'} (not a registered alias)." 
                        : $"Removed alias: {'"'}{args}{'"'} (mapped to: {remove_status.Value})";
                    logger.LogMessage(str);
                }


            }
        }

        protected override void Run(string[] args)
        {
            if (args.Length == 0)
            {
                List.Execute(args);
                return;
            }
            New.Execute(args);
        }
    }

    /*
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
                        ($"{command.GetHelp()}", new TextProperties())
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
                            ($"{command.GetHelp()}", new TextProperties())
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
    /*
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
        private Dictionary<string, ICommand> commands;
        public AliasCommand(Logger logger, CommandRegistry registry)
        {
            this.logger = logger;
            this.registry = registry;
            commands = registry._commands;
            aliases = registry._aliases;
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
                UnrecognisedCommand();
            }
            else if (args[0].EqualsIgnoreCase("remove"))
            {
                if (args.Length == 2)
                {
                    bool found = false;
                    string bound_command = "N/A";
                    foreach (string command in aliases.Keys)
                    {
                        if (command.EqualsIgnoreCase(args[1]))
                        {
                            bound_command = aliases[command];
                            registry._aliases.Remove(command);
                            found = true;

                        }
                    }
                    if (found)
                        logger.LogMessage($"Alias {'"'}{args[1]}{'"'} (bound to command: {bound_command}) removed. ");
                    else
                        logger.LogMessage($"Failed to remove alias: {'"'}{args[1]}{'"'}: alias not found. ");
                }
            }
            else if (args.Length > 1)
            {
                string[] aliases = args.SelectItems(0,args.Length-2);
                string mapped_command = args.Last();
                bool existing_command = false;
                foreach (string key in commands.Keys)
                {
                    if (key.EqualsIgnoreCase(mapped_command))
                    {
                        existing_command = true;
                        break;
                    }
                }
                if (existing_command)
                {
                    registry.RegisterAlias(mapped_command, aliases);
                    string aliases_string = "[";
                    for (int i = 0; i < aliases.Length; i++)
                    {
                        aliases_string += aliases[i] + ((i == aliases.Length - 1) ? "]" : ", ");
                    }
                    logger.LogMessage($"Registered aliases: {aliases_string} to command: {mapped_command}");
                }
                else
                {
                    logger.LogMessage("That is not an existing command. Please try again. Usage : Alias <new alias 1> <new alias 2> ... <mapped command>");
                }
            }
            else if (args[0].EqualsIgnoreCase("list"))
            {
                foreach (KeyValuePair<string, string> kvp in aliases)
                {
                    logger.LogMessage($"{kvp.Key} => {kvp.Value}");
                }
            }
            else
            {
                UnrecognisedCommand();
                return;
            }
            
        
            

        }

        public string GetHelp()
        {
            return "Controls the aliases for commands. Usage : (1) Alias list => lists all registered aliases. (2) Alias <new alias 1> <new alias 2> ... <mapped command> (3) Alias Remove <";
        }
    }
    */
}