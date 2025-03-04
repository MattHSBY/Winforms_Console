using MattUtils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Custom_Winforms_Library
{
    public class CommandProcessor
    {
        private CommandRegistry registry;
        public CommandProcessor(CommandRegistry registry)
        {
            this.registry = registry;
        }

        private string[] SeperateArguments(string input)
        {
            string[] args = Array.Empty<string>();
            StringBuilder currentArg = new StringBuilder();
            bool inQuotes = false;
            char quoteChar = '\0';

            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];

                if ((c == '"' || c == '\'') && (!inQuotes || c == quoteChar))
                {
                    if (!inQuotes)
                    {
                        inQuotes = true;
                        quoteChar = c;
                    }
                    else
                    {
                        inQuotes = false;
                    }
                }
                else if (char.IsWhiteSpace(c) && !inQuotes)
                {
                    if (currentArg.Length > 0)
                    {
                        args = args.App(currentArg.ToString());
                        currentArg.Clear();
                    }
                }
                else
                {
                    currentArg.Append(c);
                }
            }



            if (currentArg.Length > 0)
            {
                args = args.App(currentArg.ToString());
            }


            //args.Print(1);

            return args;


        }


        public void ProcessCommand(string input)
        {
            var parts = SeperateArguments(input);
            var commandName = parts[0];
            var arguments = parts.Skip(1).ToArray();
            if (commandName.EqualsIgnoreCase("help")) 
                registry.RegisterHelpCommand();
            
            ICommand command = registry.GetCommand(commandName);
            
            command.Execute(arguments);
        }


    }

    
}
