using System;
using System.IO;

namespace Org.BouncyCastle.Tls
{
    public interface DatagramSender
    {
        /// <exception cref="IOException"/>
        int GetSendLimit();

        /// <exception cref="IOException"/>
        void Send(byte[] buf, int off, int len);

        /// <exception cref="IOException"/>
        void Send(ReadOnlySpan<byte> buffer);
    }
}
