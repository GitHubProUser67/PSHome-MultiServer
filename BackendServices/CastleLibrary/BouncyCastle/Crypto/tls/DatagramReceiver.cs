using System;
using System.IO;

namespace Org.BouncyCastle.Tls
{
    public interface DatagramReceiver
    {
        /// <exception cref="IOException"/>
        int GetReceiveLimit();

        /// <remarks>
        /// A <paramref name="waitMillis"/> of zero is interpreted as an infinite timeout.
        /// </remarks>
        /// <exception cref="IOException"/>
        int Receive(byte[] buf, int off, int len, int waitMillis);

        /// <remarks>
        /// A <paramref name="waitMillis"/> of zero is interpreted as an infinite timeout.
        /// </remarks>
        /// <exception cref="IOException"/>
        int Receive(Span<byte> buffer, int waitMillis);
    }
}
