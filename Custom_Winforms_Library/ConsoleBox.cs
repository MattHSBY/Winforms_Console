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
    public partial class ConsoleBox : RichTextBox
    {
        int messages = 0;
        private static Color bgColour = Color.Black;

        public Color Default_text_color 
        { 
            get => Color.FromArgb(bgColour.A,255 - bgColour.R, 255- bgColour.G, 255 - bgColour.B);
        }

        public ConsoleBox()
        {
            InitializeComponent();
            Setup();
        }

        private void Setup()
        {
            ReadOnly = true;
        }

        public void Clear_Console()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => Clear_Console()));
            } else
            {
                Clear();
                messages = 0;

            }
        }
        private void TrimLines(int maxLines)
        {
            if (Lines.Length > maxLines)
            {
                Select(0,Lines.Length-maxLines);
                SelectedText = "";
                messages = maxLines;
            }
        }


        public void WriteLine(List<(string text, TextProperties properties)> message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => WriteLine(message)));
            } 
            else
            {
                TrimLines(500);
                SelectionStart = Text.Length;
                SelectionLength = 0;
                if (messages != 0)
                {
                    AppendText($"{Environment.NewLine}");

                }
                
                //SelectionColor = colour;
                foreach (var part in message)
                {

                    SelectionColor = Default_text_color;
                    SelectionColor = part.properties.TextColor ?? SelectionColor;
                    SelectionFont = part.properties.TextFont;
                    
                    AppendText(part.text);
                }
                

                ScrollToCaret();
                messages++;
            }
        }

        public void WriteLine(string message)
        {
            List<(string text, TextProperties properties)> messageParts = new List<(string text, TextProperties properties)> { (message, new TextProperties(Default_text_color))};


            WriteLine(messageParts);
        }

        public void WriteLine(string message, TextProperties properties)
        {
            List<(string text, TextProperties properties)> messageParts = new List<(string text, TextProperties properties)> { (message, properties)};


            WriteLine(messageParts);
        }



        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }
    }
}
