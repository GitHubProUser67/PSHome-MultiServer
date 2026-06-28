using System;
using Org.BouncyCastle.Utilities.IO;

namespace Org.BouncyCastle.Crypto.Digests
{
    public sealed class Prehash : IDigest
    {
        public static Prehash ForDigest(IDigest digest) =>
            ForParameters(digest.AlgorithmName, digest.GetDigestSize());

        public static Prehash ForParameters(string digestName, int digestSize) =>
            new Prehash(digestName, digestSize);

        private readonly string m_algorithmName;
        private readonly LimitedBuffer m_buf;

        private Prehash(string algorithmName, int digestSize)
        {
            m_algorithmName = algorithmName;
            m_buf = new LimitedBuffer(digestSize);
        }

        public string AlgorithmName => m_algorithmName;

        public int GetByteLength() => throw new NotSupportedException();

        public int GetDigestSize() => m_buf.Limit;

        public void Update(byte input) => m_buf.WriteByte(input);

        public void BlockUpdate(byte[] input, int inOff, int inLen) =>
            m_buf.Write(input, inOff, inLen);

        public void BlockUpdate(ReadOnlySpan<byte> input) => m_buf.Write(input);

        public int DoFinal(byte[] output, int outOff)
        {
            try
            {
                if (GetDigestSize() != m_buf.Count)
                    throw new InvalidOperationException("Incorrect prehash size");

                return m_buf.CopyTo(output, outOff);
            }
            finally
            {
                Reset();
            }
        }

        public int DoFinal(Span<byte> output)
        {
            try
            {
                if (GetDigestSize() != m_buf.Count)
                    throw new InvalidOperationException("Incorrect prehash size");

                return m_buf.CopyTo(output);
            }
            finally
            {
                Reset();
            }
        }

        public void Reset() => m_buf.Reset();
    }
}
