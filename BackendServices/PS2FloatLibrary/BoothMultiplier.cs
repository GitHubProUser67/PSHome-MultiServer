using Tommunism.SoftFloat;

namespace PS2FloatLibrary
{
    //****************************************************************
    // Booth Multiplier
    // From the PCSX2 Team (TellowKrinkle)
    //****************************************************************
    public class BoothMultiplier
    {
        // Uses hardware multiply for most of the multiplication (accurate).
        public static bool fastMul = true;

        private struct BoothRecode
        {
            public uint data;
            public uint negate;
        };

        private struct AddResult
        {
            public uint lo;
            public uint hi;
        };

        private static BoothRecode Booth(uint a, uint b, uint bit)
        {
            uint test = (bit != 0 ? b >> (int)(bit * 2 - 1) : b << 1) & 7;
            a <<= (int)(bit * 2);
            a += (test == 3 || test == 4) ? a : 0;
            uint neg = (test >= 4 && test <= 6) ? ~0u : 0;
            uint pos = 1u << (int)(bit * 2);
            a ^= neg & (uint)-pos;
            a &= (test >= 1 && test <= 6) ? ~0u : 0;
            return new BoothRecode { data = a, negate = neg & pos };
        }

        // Add 3 rows of bits in parallel
        private static AddResult Add3(uint a, uint b, uint c)
        {
            uint u = a ^ b;
            return new AddResult { lo = u ^ c, hi = ((u & c) | (a & b)) << 1 };
        }

        public static ulong MulMantissa(uint a, uint b)
        {
            ulong full;
            if (fastMul)
                full = (ulong)a * (ulong)b;
            else
            {
                Wallace wallace = new Wallace(a, b);
                full = Float64.FromBitsUI64(wallace.fs_multiplier).RawMantissa + Float64.FromBitsUI64(wallace.ft_multiplier).RawMantissa;
            }
            BoothRecode b0 = Booth(a, b, 0);
            BoothRecode b1 = Booth(a, b, 1);
            BoothRecode b2 = Booth(a, b, 2);
            BoothRecode b3 = Booth(a, b, 3);
            BoothRecode b4 = Booth(a, b, 4);
            BoothRecode b5 = Booth(a, b, 5);
            BoothRecode b6 = Booth(a, b, 6);
            BoothRecode b7 = Booth(a, b, 7);

            // First cycle
            AddResult t0 = Add3(b1.data, b2.data, b3.data);
            AddResult t1 = Add3(b4.data & ~0x7ffu, b5.data & ~0xfffu, b6.data);
            // A few adds get skipped, squeeze them back in
            t1.hi |= b6.negate | (b5.data & 0x800);
            b7.data |= (b5.data & 0x400) + b5.negate;

            // Second cycle
            AddResult t2 = Add3(b0.data, t0.lo, t0.hi);
            AddResult t3 = Add3(b7.data, t1.lo, t1.hi);

            // Third cycle
            AddResult t4 = Add3(t2.hi, t3.lo, t3.hi);

            // Fourth cycle
            AddResult t5 = Add3(t2.lo, t4.lo, t4.hi);

            // Discard bits and sum
            t5.hi += b7.negate;
            t5.lo &= ~0x7fffu;
            t5.hi &= ~0x7fffu;
            return full - (((t5.lo + t5.hi) ^ full) & 0x8000);
        }
    }
}
