using System;

namespace Org.BouncyCastle.Crypto
{
    public interface IKemDecapsulator
    {
        void Init(ICipherParameters parameters);

        int EncapsulationLength { get; }

        int SecretLength { get; }

        void Decapsulate(
            byte[] encBuf,
            int encOff,
            int encLen,
            byte[] secBuf,
            int secOff,
            int secLen
        );

        void Decapsulate(ReadOnlySpan<byte> encapsulation, Span<byte> secret);
    }
}
