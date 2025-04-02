using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;
using System.Windows.Forms;
using System.Runtime.ExceptionServices;
using System.Security;

namespace Custom_Winforms_Library
{
    

    [ToolboxItem(true)]
    public partial class ConsoleBox : RichTextBox
    {
        private ConcurrentQueue<List<(string text, TextProperties properties)>> messageQueue = new();
        private CancellationTokenSource? cancellationTokenSource;

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
            StartProcessingQueue();
        }

        private void StartProcessingQueue()
        {
            cancellationTokenSource = new CancellationTokenSource();
            Task.Run(() => ProcessQueue(cancellationTokenSource.Token), cancellationTokenSource.Token);
        }

        private void ProcessQueue(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (messageQueue.TryDequeue(out var message))
                {
                    if (!IsDisposed && IsHandleCreated)
                    {
                        Invoke(new Action(() => SafeWriteLine(message)));
                    }
                }
                else
                {
                    Thread.Sleep(1); // Pause to avoid busy-waiting
                }
            }
        }
        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        private void SafeWriteLine(List<(string text, TextProperties properties)> message)
        {
            try
            {
                TrimLines(500);
                SuspendLayout();

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
            catch (System.AccessViolationException)
            {
                SafeWriteLine(new List<(string text, TextProperties properties)>
                {
                    ("----------------------------------------- catch block", new TextProperties(Color.Green))
                }
                );
                SafeWriteLine(message);
            }
            finally
            {
                ResumeLayout();
            }
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
            messageQueue.Enqueue(message);
        }

        public void WriteLine(string message)
        {
            List<(string text, TextProperties properties)> messageParts =
                new List<(string text, TextProperties properties)> { (message, new TextProperties(Default_text_color)) };
            WriteLine(messageParts);
        }

        public void WriteLine(string message, TextProperties properties)
        {
            List<(string text, TextProperties properties)> messageParts =
                new List<(string text, TextProperties properties)> { (message, properties) };
            WriteLine(messageParts);
        }


        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }
    }
}
