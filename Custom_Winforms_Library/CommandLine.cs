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
    [ToolboxItem(true)]
    public partial class CommandLine : TextBox
    {
        private string[] sent_commands = new string[0];
        private string buffer_text = "";
        private int history_index = 0;

        private string current_message
        {
            get => (history_index == sent_commands.Length) ? buffer_text : sent_commands[history_index];
        }

        public event EventHandler<string> CommandEntered;

        public CommandLine()
        {
            InitializeComponent();
            Setup();
        }

        public void Setup()
        {
            KeyDown += new KeyEventHandler(OnKeyDown);
        }

        private void SetTextToCurrentMessage()
        {
            Text = current_message;
            SelectionStart = Text.Length;
        }
        
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            //Debug.WriteLine("KeyDown");
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                //Debug.WriteLine("Enter key down.");
                if (!Text.Equals(""))
                {
                    e.SuppressKeyPress = true;
                    string command = Text;
                    Clear();
                    EnterCommand(command);
                    
                    sent_commands = (
                        (sent_commands.Length>0) 
                        && command.EqualsIgnoreCase(sent_commands[sent_commands.Length-1])
                        ) 
                        ? sent_commands : sent_commands.App(command);
                    // add the command only if it's different from the last message added (so if the same command is entered 1000 times you don't
                    // have to go through every single one to get to the previous history.

                    history_index = sent_commands.Length;
                }
            } 
            else if (e.KeyCode == Keys.Up)
            {
                e.SuppressKeyPress = true;
                if (history_index == sent_commands.Length)
                {
                    buffer_text = Text;
                }

                if (sent_commands.Length == 0) return;

                if (history_index > 0)
                {
                    history_index--;
                }
                SetTextToCurrentMessage();

            }
            else if (e.KeyCode == Keys.Down)
            {
                e.SuppressKeyPress = true;
                if (history_index < sent_commands.Length)
                {
                    history_index++;
                } 
                SetTextToCurrentMessage();
            }
        }

        public void EnterCommand(string command)
        {
            CommandEntered?.Invoke(this, command);
            //Debug.WriteLine($"{command} entered.");
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }
    }
}
