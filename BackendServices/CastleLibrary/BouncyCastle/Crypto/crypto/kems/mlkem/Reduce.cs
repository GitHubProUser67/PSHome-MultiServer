using System.Runtime.CompilerServices;

namespace Org.BouncyCastle.Crypto.Kems.MLKem
{
    internal static class Reduce
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static short MontgomeryReduce(int a)
        {
            short u = (short)(a * MLKemEngine.QInv);
            int t = u * MLKemEngine.Q;
            t = a - t;
            t >>= 16;
            return (short)t;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static short BarrettReduce(short a)
        {
            short v = (short)(((1U << 26) + (MLKemEngine.Q / 2)) / MLKemEngine.Q);
            short t = (short)((v * a) >> 26);
            t = (short)(t * MLKemEngine.Q);
            return (short)(a - t);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static short CondSubQ(short a)
        {
            a -= MLKemEngine.Q;
            a += (short)((a >> 15) & MLKemEngine.Q);
            return a;
        }

        // NB: We only care about the sign bit fof the result: it will be 1 iff the argument was in range
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int CheckModulus(short a) => a - MLKemEngine.Q;
    }
}
