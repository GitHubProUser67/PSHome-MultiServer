using System;

namespace Org.BouncyCastle.Crypto
{
    /// <summary>Base interface for general purpose byte derivation functions.</summary>
    public interface IDerivationFunction
    {
        void Init(IDerivationParameters parameters);

        /// <summary>The message digest used as the basis for the function.</summary>
        IDigest Digest { get; }

        int GenerateBytes(byte[] output, int outOff, int length);

        int GenerateBytes(Span<byte> output);
    }
}
