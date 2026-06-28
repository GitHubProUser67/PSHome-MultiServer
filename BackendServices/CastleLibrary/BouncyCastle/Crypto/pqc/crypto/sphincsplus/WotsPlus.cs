using System;
using Org.BouncyCastle.Crypto.Utilities;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Pqc.Crypto.SphincsPlus
{
    internal class WotsPlus
    {
        private SphincsPlusEngine engine;
        private uint w;

        internal WotsPlus(SphincsPlusEngine engine)
        {
            this.engine = engine;
            this.w = this.engine.WOTS_W;
        }

        internal void PKGen(byte[] skSeed, byte[] pkSeed, Adrs paramAdrs, Span<byte> output)
        {
            Adrs wotspkAdrs = new Adrs(paramAdrs); // copy address to create OTS public key address

            byte[] tmpConcat = new byte[engine.WOTS_LEN * engine.N];
            for (uint i = 0; i < engine.WOTS_LEN; i++)
            {
                Adrs adrs = new Adrs(paramAdrs);
                adrs.SetTypeAndClear(Adrs.WOTS_PRF);
                adrs.SetKeyPairAddress(paramAdrs.GetKeyPairAddress());
                adrs.SetChainAddress(i);
                adrs.SetHashAddress(0);

                engine.PRF(pkSeed, skSeed, adrs, tmpConcat, engine.N * (int)i);

                adrs.SetTypeAndClear(Adrs.WOTS_HASH);
                adrs.SetKeyPairAddress(paramAdrs.GetKeyPairAddress());
                adrs.SetChainAddress(i);
                adrs.SetHashAddress(0);

                Chain(0, w - 1, pkSeed, adrs, tmpConcat.AsSpan(engine.N * (int)i, engine.N));
            }

            wotspkAdrs.SetTypeAndClear(Adrs.WOTS_PK);
            wotspkAdrs.SetKeyPairAddress(paramAdrs.GetKeyPairAddress());

            engine.T_l(pkSeed, wotspkAdrs, tmpConcat, output);
        }

        // #Input: Input string X, start index i, number of steps s, public seed PK.seed, address Adrs
        // #Output: value of F iterated s times on X
        private bool Chain(uint i, uint s, byte[] pkSeed, Adrs adrs, Span<byte> X)
        {
            if (s == 0)
                return true;

            // TODO Check this since the highest we use is i + s - 1
            if ((i + s) > (this.w - 1))
                return false;

            for (uint j = 0; j < s; ++j)
            {
                adrs.SetHashAddress(i + j);
                engine.F(pkSeed, adrs, X);
            }

            return true;
        }

        // #Input: Message M, secret seed SK.seed, public seed PK.seed, address Adrs
        // #Output: WOTS+ signature sig
        internal byte[] Sign(byte[] M, byte[] skSeed, byte[] pkSeed, Adrs paramAdrs)
        {
            Adrs adrs = new Adrs(paramAdrs);

            Span<uint> msg = stackalloc uint[engine.WOTS_LEN];

            // convert message to base w
            BaseW(M, w, msg[..engine.WOTS_LEN1]);

            // compute checksum
            uint csum = 0;
            for (int i = 0; i < engine.WOTS_LEN1; i++)
            {
                csum += w - 1 - msg[i];
            }

            // convert csum to base w
            if ((engine.WOTS_LOGW % 8) != 0)
            {
                csum <<= 8 - (engine.WOTS_LEN2 * engine.WOTS_LOGW % 8);
            }
            int len_2_bytes = (engine.WOTS_LEN2 * engine.WOTS_LOGW + 7) / 8;

            Span<byte> csum_bytes = stackalloc byte[4];
            Pack.UInt32_To_BE(csum, csum_bytes);
            BaseW(csum_bytes[^len_2_bytes..], w, msg[engine.WOTS_LEN1..]);

            byte[] sigConcat = new byte[engine.WOTS_LEN * engine.N];
            for (int i = 0; i < engine.WOTS_LEN; i++)
            {
                adrs.SetTypeAndClear(Adrs.WOTS_PRF);
                adrs.SetKeyPairAddress(paramAdrs.GetKeyPairAddress());
                adrs.SetChainAddress((uint)i);
                adrs.SetHashAddress(0);

                engine.PRF(pkSeed, skSeed, adrs, sigConcat, engine.N * i);

                adrs.SetTypeAndClear(Adrs.WOTS_HASH);
                adrs.SetKeyPairAddress(paramAdrs.GetKeyPairAddress());
                adrs.SetChainAddress((uint)i);
                adrs.SetHashAddress(0);

                Chain(0, msg[i], pkSeed, adrs, sigConcat.AsSpan(engine.N * i, engine.N));
            }

            return sigConcat;
        }

        //
        // Input: len_X-byte string X, int w, output length out_len
        // Output: outLen int array basew
        internal void BaseW(ReadOnlySpan<byte> X, uint w, Span<uint> output)
        {
            int total = 0;
            int bits = 0;
            int XOff = 0;
            int outOff = 0;

            for (int consumed = 0; consumed < output.Length; consumed++)
            {
                if (bits == 0)
                {
                    total = X[XOff++];
                    bits += 8;
                }

                bits -= engine.WOTS_LOGW;
                output[outOff++] = (uint)((total >> bits) & (w - 1));
            }
        }

        internal void PKFromSig(byte[] sig, byte[] M, byte[] pkSeed, Adrs adrs, Span<byte> output)
        {
            Adrs wotspkAdrs = new Adrs(adrs);

            Span<uint> msg = stackalloc uint[engine.WOTS_LEN];

            // convert message to base w
            BaseW(M, w, msg[..engine.WOTS_LEN1]);

            // compute checksum
            uint csum = 0;
            for (int i = 0; i < engine.WOTS_LEN1; i++)
            {
                csum += w - 1 - msg[i];
            }

            // convert csum to base w
            csum <<= 8 - (engine.WOTS_LEN2 * engine.WOTS_LOGW % 8);
            int len_2_bytes = (engine.WOTS_LEN2 * engine.WOTS_LOGW + 7) / 8;

            Span<byte> csum_bytes = stackalloc byte[4];
            Pack.UInt32_To_BE(csum, csum_bytes);
            BaseW(csum_bytes[^len_2_bytes..], w, msg[engine.WOTS_LEN1..]);

            byte[] tmpConcat = new byte[engine.WOTS_LEN * engine.N];
            for (int i = 0; i < engine.WOTS_LEN; i++)
            {
                adrs.SetChainAddress((uint)i);

                int sigPos = engine.N * i;
                Array.Copy(sig, sigPos, tmpConcat, sigPos, engine.N);
                Chain(msg[i], w - 1 - msg[i], pkSeed, adrs, tmpConcat.AsSpan(sigPos, engine.N));
            }

            wotspkAdrs.SetTypeAndClear(Adrs.WOTS_PK);
            wotspkAdrs.SetKeyPairAddress(adrs.GetKeyPairAddress());

            engine.T_l(pkSeed, wotspkAdrs, tmpConcat, output);
        }
    }
}
