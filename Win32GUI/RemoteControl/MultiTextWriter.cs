using System.Text;

namespace RemoteControl
{
    public class MultiTextWriter : TextWriter
    {
        private readonly IEnumerable<TextWriter> writers;

        public MultiTextWriter(IEnumerable<TextWriter> writers)
        {
            this.writers = [.. writers];
        }

        public MultiTextWriter(params TextWriter[] writers)
        {
            this.writers = writers;
        }

        public override void Write(char value)
        {
            foreach (var writer in writers)
                writer.Write(value);

            Thread.Sleep(1); // Avoids GUI stalling issues if spammed.
        }

        public override void Write(string value)
        {
            if (string.IsNullOrEmpty(value))
                return;

            foreach (var writer in writers)
                writer.Write(value);

            Thread.Sleep(1); // Avoids GUI stalling issues if spammed.
        }

        public override void Flush()
        {
            foreach (var writer in writers)
                writer.Flush();
        }

        public override void Close()
        {
            foreach (var writer in writers)
                writer.Close();
        }

        public override Encoding Encoding
        {
            get { return Encoding.ASCII; }
        }
    }
}
