using System;
using Org.BouncyCastle.Crypto.Digests;

namespace Org.BouncyCastle.Crypto.Kems.MLKem
{
    internal abstract class Symmetric
    {
        internal readonly int XofBlockBytes;

        internal abstract void Hash_h(ReadOnlySpan<byte> input, Span<byte> output);

        internal abstract void Hash_g(ReadOnlySpan<byte> input, Span<byte> output);

        internal abstract void Kdf(ReadOnlySpan<byte> input, Span<byte> output);

        internal abstract void Prf(ReadOnlySpan<byte> seed, byte nonce, Span<byte> output);

        internal abstract void XofAbsorb(ReadOnlySpan<byte> seed, byte x, byte y);

        internal abstract void XofSqueezeBlocks(Span<byte> output);

        internal Symmetric(int xofBlockBytes)
        {
            this.XofBlockBytes = xofBlockBytes;
        }

        internal static void DoDigest(IDigest digest, ReadOnlySpan<byte> input, Span<byte> output)
        {
            digest.BlockUpdate(input);
            digest.DoFinal(output);
        }

        internal sealed class ShakeSymmetric : Symmetric
        {
            private readonly ShakeDigest xof;
            private readonly Sha3Digest sha3Digest512;
            private readonly Sha3Digest sha3Digest256;
            private readonly ShakeDigest shakeDigest;

            internal ShakeSymmetric()
                : base(164)
            {
                xof = new ShakeDigest(128);
                shakeDigest = new ShakeDigest(256);
                sha3Digest256 = new Sha3Digest(256);
                sha3Digest512 = new Sha3Digest(512);
            }

            internal override void Hash_h(ReadOnlySpan<byte> input, Span<byte> output) =>
                DoDigest(sha3Digest256, input, output);

            internal override void Hash_g(ReadOnlySpan<byte> input, Span<byte> output) =>
                DoDigest(sha3Digest512, input, output);

            internal override void Kdf(ReadOnlySpan<byte> input, Span<byte> output) =>
                DoDigest(shakeDigest, input, output);

            internal override void Prf(ReadOnlySpan<byte> seed, byte nonce, Span<byte> output)
            {
                shakeDigest.BlockUpdate(seed);
                shakeDigest.Update(nonce);
                shakeDigest.OutputFinal(output);
            }

            internal override void XofAbsorb(ReadOnlySpan<byte> seed, byte x, byte y)
            {
                Span<byte> xy = stackalloc byte[2] { x, y };

                xof.Reset();
                xof.BlockUpdate(seed);
                xof.BlockUpdate(xy);
            }

            internal override void XofSqueezeBlocks(Span<byte> output)
            {
                xof.Output(output);
            }
        }
    }
}
