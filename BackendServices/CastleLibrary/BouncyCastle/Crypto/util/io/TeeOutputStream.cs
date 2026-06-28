using System;
using System.Diagnostics;
using System.IO;

namespace Org.BouncyCastle.Utilities.IO
{
    public class TeeOutputStream : BaseOutputStream
    {
        private readonly Stream output,
            tee;

        public TeeOutputStream(Stream output, Stream tee)
        {
            Debug.Assert(output.CanWrite);
            Debug.Assert(tee.CanWrite);

            this.output = output;
            this.tee = tee;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                output.Dispose();
                tee.Dispose();
            }
            base.Dispose(disposing);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            output.Write(buffer, offset, count);
            tee.Write(buffer, offset, count);
        }

        public override void Write(ReadOnlySpan<byte> buffer)
        {
            output.Write(buffer);
            tee.Write(buffer);
        }

        public override void WriteByte(byte value)
        {
            output.WriteByte(value);
            tee.WriteByte(value);
        }
    }
}
