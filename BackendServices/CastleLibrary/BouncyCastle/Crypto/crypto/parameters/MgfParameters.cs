using System;
using System.Buffers;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Parameters
{
    /// <remarks>Parameters for mask derivation functions.</remarks>
    public sealed class MgfParameters : IDerivationParameters
    {
        public static MgfParameters Create<TState>(
            int length,
            TState state,
            SpanAction<byte, TState> action
        )
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            if (length < 1)
                throw new ArgumentOutOfRangeException(nameof(length));

            MgfParameters result = new MgfParameters(length);
            action(result.m_seed, state);
            return result;
        }

        private readonly byte[] m_seed;

        public MgfParameters(byte[] seed)
        {
            m_seed = Arrays.CopyBuffer(seed);
        }

        public MgfParameters(byte[] seed, int off, int len)
        {
            m_seed = Arrays.CopySegment(seed, off, len);
        }

        private MgfParameters(int length)
        {
            if (length < 1)
                throw new ArgumentOutOfRangeException(nameof(length));

            m_seed = new byte[length];
        }

        public void CopySeedTo(byte[] buf, int off, int len) =>
            Arrays.CopyBufferToSegment(m_seed, buf, off, len);

        public byte[] GetSeed() => Arrays.InternalCopyBuffer(m_seed);

        [Obsolete("Use 'CopySeedTo' instead")]
        public void GetSeed(byte[] buffer, int offset) => m_seed.CopyTo(buffer, offset);

        public void GetSeed(Span<byte> output)
        {
            m_seed.CopyTo(output);
        }

        internal ReadOnlySpan<byte> InternalSeed => m_seed;

        public int SeedLength => m_seed.Length;
    }
}
