using System.Text;

namespace RemoteControl
{
    public class ControlWriter(Control textbox) : TextWriter
    {
        private const int maxLines = 46;

        private readonly Control textbox = textbox;

        private readonly Queue<string> lines = new();
        private readonly StringBuilder currentLine = new();

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
            foreach (var c in value)
                Write(c);
        }

        private void FlushCurrentLine()
        {
            var line = currentLine.ToString();
            currentLine.Clear();

            if (textbox.InvokeRequired)
                textbox.Invoke(
                    new Action(() =>
                    {
                        AddLineToTextbox(line);
                    })
                );
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
