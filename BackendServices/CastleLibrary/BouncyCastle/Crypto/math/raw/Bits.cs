using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Org.BouncyCastle.Math.Raw
{
    internal static class Bits
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static uint BitPermuteStep(uint x, uint m, int s)
        {
            Debug.Assert((m & (m << s)) == 0U);
            Debug.Assert((m << s) >> s == m);

            uint t = (x ^ (x >> s)) & m;
            return t ^ (t << s) ^ x;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ulong BitPermuteStep(ulong x, ulong m, int s)
        {
            Debug.Assert((m & (m << s)) == 0UL);
            Debug.Assert((m << s) >> s == m);

            ulong t = (x ^ (x >> s)) & m;
            return t ^ (t << s) ^ x;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void BitPermuteStep2(ref uint hi, ref uint lo, uint m, int s)
        {
            Debug.Assert(!Unsafe.AreSame(ref hi, ref lo) || (m & (m << s)) == 0U);
            Debug.Assert((m << s) >> s == m);

            uint t = ((lo >> s) ^ hi) & m;
            lo ^= t << s;
            hi ^= t;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void BitPermuteStep2(ref ulong hi, ref ulong lo, ulong m, int s)
        {
            Debug.Assert(!Unsafe.AreSame(ref hi, ref lo) || (m & (m << s)) == 0UL);
            Debug.Assert((m << s) >> s == m);

            ulong t = ((lo >> s) ^ hi) & m;
            lo ^= t << s;
            hi ^= t;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static uint BitPermuteStepSimple(uint x, uint m, int s)
        {
            Debug.Assert((m << s) == ~m);
            Debug.Assert((m & ~m) == 0U);

            return ((x & m) << s) | ((x >> s) & m);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ulong BitPermuteStepSimple(ulong x, ulong m, int s)
        {
            Debug.Assert((m << s) == ~m);
            Debug.Assert((m & ~m) == 0UL);

            return ((x & m) << s) | ((x >> s) & m);
        }
    }
}
