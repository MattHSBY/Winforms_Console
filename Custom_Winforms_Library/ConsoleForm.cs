using MattUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Custom_Winforms_Library
{
    public partial class ConsoleForm : Form
    {
        private CommandProcessor commandProcessor;
        public CommandRegistry commandRegistry;
        public Logger logger;
        public ConsoleForm()
        {
            InitializeComponent();
            Setup();
        }

        

        private void Setup()
        {
            consoleBox.ReadOnly = true;
            
            logger = new Logger();
            logger.RegisterLogListener(LogToConsole);
            commandRegistry = new CommandRegistry(logger);
            commandProcessor = new CommandProcessor(commandRegistry);
            commandLine.CommandEntered += CommandEntered;
            //consoleBox.Enter += ConsoleBox_Enter;
        }

        private void LogToConsole(List<(string text,TextProperties properties)> message)
        {
            if (message.Count == 1 && message[0].text.EqualsIgnoreCase("/clear_console"))
            {
                consoleBox.Clear_Console();
                return;
            }
            try
            {
                consoleBox.WriteLine(message);
            }
            catch
            {
                Debug.WriteLine("Couldn't log message.");
            }
        }

        private void CommandEntered(object? sender, string command)
        {
            Debug.WriteLine("Command entered");
            commandProcessor.ProcessCommand(command);
        }
    }
}
