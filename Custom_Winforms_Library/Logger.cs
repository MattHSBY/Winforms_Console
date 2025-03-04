using MattUtils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Custom_Winforms_Library
{
    public class Logger
    {
        private readonly List<Action<List<(string text, TextProperties properties)>>> LogListeners = new();

        public Logger()
        {

        }

        public void RegisterLogListener(Action<List<(string text, TextProperties properties)>> logListener)
        {
            LogListeners.Add(logListener);
        }

        public void LogException(Exception e)
        {
            string message = e.Message;
            LogMessage(message, new TextProperties(Color.Red));
        }

        public void ClearConsoleMessage()
        {
            string str = "/clear_console";
            LogRawMessage(str);
        }
        
        public void UnregisterLogListener(Action<List<(string text, TextProperties properties)>> logListener)
        {
            LogListeners.Remove(logListener);
        }
        
        public void LogMessage(List<(string text, TextProperties properties)> message)
        {
            TextProperties properties = new TextProperties(fontStyle:FontStyle.Bold);
            if (message[0].properties.TextColor != null)
            {
                properties.TextColor = message[0].properties.TextColor;
            }

            List<(string text, TextProperties properties)> new_message = new List<(string text, TextProperties properties)>
            {
                ($"[{DateTime.UtcNow}] ", properties)
            };
            for (int i = 0; i < message.Count; i++)
            {
                (string text, TextProperties properties) msg = message[i];
                new_message.Add(msg);
            }
            string str = "(";
            for (int i = 0; i < new_message.Count; i++)
            {
                string color = "";
                string fontStyle = fontStyle = $"{new_message[i].properties.fontStyle}";

                if (new_message[i].properties.TextColor.HasValue)
                {
                    color = $"{new_message[i].properties.TextColor}";
                }
                str = $"{str}{new_message[i].text}:{color = ((color == "") ? "null" : color)},{fontStyle}";
                str = (i == new_message.Count - 1) ? str + ")" : str + "), ";
            }



            LogRawMessage(new_message);

        }
        public void LogMessage(string message, TextProperties properties)
        {
            List<(string text, TextProperties properties)> msg = new List<(string text, TextProperties properties)>()
            {
                (message, properties)
            };
            LogMessage(msg);
        }
        public void LogMessage(string message)
        {
            List<(string text, TextProperties properties)> msg = new List<(string text, TextProperties properties)>()
            {
                (message, new TextProperties())
            };
            LogMessage(msg);
        }

        public void LogRawMessage(List<(string text, TextProperties properties)> message)
        {
            foreach (var listener in LogListeners)
            {
                listener.Invoke(message);
            }
        }
        public void LogRawMessage(string message, TextProperties properties)
        {
            List<(string text, TextProperties properties)> msg = new List<(string text, TextProperties properties)>()
            {
                (message, properties)
            };
            LogRawMessage(msg);
        }
        public void LogRawMessage(string message)
        {
            List<(string text, TextProperties properties)> msg = new List<(string text, TextProperties properties)>()
            {
                (message, new())
            };
            LogRawMessage(msg);
        }
    }
}
