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

        /*
        private void TrimLines(int maxLines)
        {
            if (Lines.Length > maxLines)
            {
                Select(0,Lines.Length-maxLines);
                SelectedText = "";
                messages = maxLines;
                
            }
        }
        */
        private int trimCounter = 0;

        private const int trimThreshold = 50;

        public void TrimLines(int maxLines)
        {
            if (++trimCounter < trimThreshold) return;
            trimCounter = 0;

            string[] lines = Lines;
            if (lines.Length > maxLines)
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
                try
                {
                    TrimLines(500);
                    SuspendLayout();  // <-- Stop UI layout updates

                    SelectionStart = Text.Length;
                    SelectionLength = 0;

                    if (messages != 0)
                    {
                        AppendText($"{Environment.NewLine}");
                    }

                    foreach (var part in message)
                    {
                        SelectionColor = part.properties.TextColor ?? Default_text_color;
                        SelectionFont = part.properties.TextFont ?? Font;
                        AppendText(part.text);
                    }

                    ScrollToCaret();
                    messages++;
                }
                finally
                {
                    ResumeLayout();  // <-- Resume UI layout updates
                }
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
