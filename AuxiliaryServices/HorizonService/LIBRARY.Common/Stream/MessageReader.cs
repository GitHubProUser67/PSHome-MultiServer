using System.Text;

namespace Horizon.LIBRARY.Common.Stream
{
    public class MessageReader : BinaryReader
    {
        public int MediusVersion { get; set; }
        public int AppId { get; set; }

        public long RemainingBytes
        {
            get
            {
                if (!BaseStream.CanSeek)
                    throw new NotSupportedException("[MessageReader] - Stream does not support seeking.");

                return BaseStream.Length - BaseStream.Position;
            }
        }

        public MessageReader(System.IO.Stream input)
            : base(input) { }

        public MessageReader(System.IO.Stream input, Encoding encoding)
            : base(input, encoding) { }

        public MessageReader(System.IO.Stream input, Encoding encoding, bool leaveOpen)
            : base(input, encoding, leaveOpen) { }
    }
}
