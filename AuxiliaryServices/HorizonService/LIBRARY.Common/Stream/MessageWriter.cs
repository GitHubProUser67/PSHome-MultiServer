using System.Text;

namespace Horizon.LIBRARY.Common.Stream
{
    public class MessageWriter : System.IO.BinaryWriter
    {
        public int MediusVersion { get; set; }
        public int AppId { get; set; }
        public MessageWriter() : base() { }
        public MessageWriter(System.IO.Stream output) : base(output) { }
        public MessageWriter(System.IO.Stream output, Encoding encoding) : base(output, encoding) { }
        public MessageWriter(System.IO.Stream output, Encoding encoding, bool leaveOpen) : base(output, encoding, leaveOpen) { }
    }
}