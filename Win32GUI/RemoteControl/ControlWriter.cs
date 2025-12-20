using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace RemoteControl
{
    public class ControlWriter : TextWriter
    {
        private const int maxLines = 46;

        private Control textbox;

        private readonly Queue<string> lines = new Queue<string>();
        private readonly StringBuilder currentLine = new StringBuilder();

        public ControlWriter(Control textbox)
        {
            this.textbox = textbox;
        }

        public override void Write(char value)
        {
            switch (value)
            { 
                case '\n':
                    FlushCurrentLine();
                    break;
                default:
                    if (value != '\r') // ignore carriage return
                        currentLine.Append(value);
                    break;
            }
        }

        public override void Write(string value)
        {
            foreach (char c in value)
                Write(c);
        }

        private void FlushCurrentLine()
        {
            string line = currentLine.ToString();
            currentLine.Clear();

            if (textbox.InvokeRequired)
                textbox.Invoke(new Action(() =>
                {
                    AddLineToTextbox(line);
                }));
            else
                AddLineToTextbox(line);
        }

        private void AddLineToTextbox(string line)
        {
            lines.Enqueue(line);
            if (lines.Count > maxLines)
                lines.Dequeue();

            textbox.Text = string.Join(Environment.NewLine, lines);
        }

        public override Encoding Encoding => Encoding.ASCII;
    }
}
