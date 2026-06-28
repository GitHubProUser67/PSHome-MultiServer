using System;
using System.Threading;
using System.Threading.Tasks;
using Org.BouncyCastle.Utilities.IO;

namespace Org.BouncyCastle.Crypto.IO
{
    public sealed class SignerSink : BaseOutputStream
    {
        private readonly ISigner m_signer;

        public SignerSink(ISigner signer)
        {
            m_signer = signer ?? throw new ArgumentNullException(nameof(signer));
        }

        public ISigner Signer => m_signer;

        public override void Write(byte[] buffer, int offset, int count)
        {
            Streams.ValidateBufferArguments(buffer, offset, count);

            if (count > 0)
            {
                m_signer.BlockUpdate(buffer, offset, count);
            }
        }

        public override Task WriteAsync(
            byte[] buffer,
            int offset,
            int count,
            CancellationToken cancellationToken
        )
        {
            return Streams.WriteAsyncDirect(this, buffer, offset, count, cancellationToken);
        }

        public override void Write(ReadOnlySpan<byte> buffer)
        {
            if (!buffer.IsEmpty)
            {
                m_signer.BlockUpdate(buffer);
            }
        }

        public override ValueTask WriteAsync(
            ReadOnlyMemory<byte> buffer,
            CancellationToken cancellationToken = default
        )
        {
            return Streams.WriteAsyncDirect(this, buffer, cancellationToken);
        }

        public override void WriteByte(byte value)
        {
            m_signer.Update(value);
        }
    }
}
