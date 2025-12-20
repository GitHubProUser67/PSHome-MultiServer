// SPDX-FileCopyrightText: 2002-2025 PCSX2 Dev Team
// SPDX-License-Identifier: GPL-3.0+

namespace PS2FloatLibrary
{
    //****************************************************************
    // Radix Divisor
    // Algorithm reference: DOI 10.1109/ARITH.1995.465363
    // From the PCSX2 Team (TellowKrinkle)
    //****************************************************************
    public class RadixDivisor
    {
        private struct CSAResult
        {
            public uint Sum;
            public uint Carry;

            public CSAResult(uint sum, uint carry)
            {
                Sum = sum;
                Carry = carry;
            }
        }

        private static CSAResult CSA(uint a, uint b, uint c)
        {
            uint u = a ^ b;
            return new CSAResult(u ^ c, ((a & b) | (u & c)) << 1);
        }

        public bool dz = false;
        public bool iv = false;
        public bool of = false;
        public bool uf = false;

        public uint floatResult;

        public RadixDivisor(bool divMode, uint f1, uint f2)
        {
            if (divMode)
            {
                if (((f1 & 0x7F800000) == 0) && ((f2 & 0x7F800000) != 0))
                {
                    floatResult = 0;
                    floatResult &= PS2Float.MAX_FLOATING_POINT_VALUE;
                    floatResult |= (uint)(((int)(f2 >> 31) != (int)(f1 >> 31)) ? 1 : 0 & 1) << 31;
                    return;
                }
                if (((f1 & 0x7F800000) != 0) && ((f2 & 0x7F800000) == 0))
                {
                    dz = true;
                    floatResult = PS2Float.MAX_FLOATING_POINT_VALUE;
                    floatResult &= PS2Float.MAX_FLOATING_POINT_VALUE;
                    floatResult |= (uint)(((int)(f2 >> 31) != (int)(f1 >> 31)) ? 1 : 0 & 1) << 31;
                    return;
                }
                if (((f1 & 0x7F800000) == 0) && ((f2 & 0x7F800000) == 0))
                {
                    iv = true;
                    floatResult = PS2Float.MAX_FLOATING_POINT_VALUE;
                    floatResult &= PS2Float.MAX_FLOATING_POINT_VALUE;
                    floatResult |= (uint)(((int)(f2 >> 31) != (int)(f1 >> 31)) ? 1 : 0 & 1) << 31;
                    return;
                }

                floatResult = FastDiv(f1, f2);
            }
            else if ((f2 & 0x7F800000) == 0)
            {
                floatResult = 0;
                iv = ((f2 >> 31) & 1) != 0;
                return;
            }
            else
                floatResult = FastSqrt(f2);
        }

        private static int QuotientSelect(CSAResult current)
        {
	        // Note: Decimal point is between bits 24 and 25
	        uint mask = (1 << 24) - 1; // Bit 23 needs to be or'd in instead of added
            int test = (int)(((current.Sum & ~mask) + current.Carry) | (current.Sum & mask));
	        if (test >= 1 << 23)
                // test >= 0.25
                return 1;
            else if (test < unchecked((int)(~0u << 24)))
                // test < -0.5
                return -1;
            return 0;
        }

        private static uint Mantissa(uint x)
        {
            return (x & 0x7fffff) | 0x800000;
        }

        private static uint Exponent(uint x)
        {
            return (x >> 23) & byte.MaxValue;
        }

        private uint FastDiv(uint a, uint b)
        {
            uint am = Mantissa(a) << 2;
            uint bm = Mantissa(b) << 2;
            CSAResult current = new CSAResult(am, 0);
            uint quotient = 0;
            int quotientBit = 1;
	        for (int i = 0; i < 25; i++)
	        {
		        quotient = (quotient << 1) + (uint)quotientBit;
		        uint add = quotientBit > 0 ? ~bm : quotientBit < 0 ? bm : 0;
                current.Carry += (quotientBit > 0) ? 1u : 0u;
		        CSAResult csa = CSA(current.Sum, current.Carry, add);
                quotientBit = QuotientSelect(quotientBit != 0 ? csa : current);
                current.Sum = csa.Sum << 1;
		        current.Carry = csa.Carry << 1;
	        }
            uint sign = (a ^ b) & PS2Float.SIGNMASK;
            uint Dvdtexp = Exponent(a);
            uint Dvsrexp = Exponent(b);
            int cexp = (int)Dvdtexp - (int)Dvsrexp + 126;
            if (quotient >= (1 << 24))
            {
                cexp += 1;
                quotient >>= 1;
            }
            if (Dvdtexp == 0 && Dvsrexp == 0)
            {
                iv = true;
                return sign | PS2Float.MAX_FLOATING_POINT_VALUE;
            }
            else if (Dvdtexp == 0 || Dvsrexp != 0)
            {
                if (Dvdtexp == 0 && Dvsrexp != 0) 
                    return sign;
            }
            else
            {
                dz = true;
                return sign | PS2Float.MAX_FLOATING_POINT_VALUE;
            }
            if (cexp > byte.MaxValue)
            {
                of = true;
                return sign | PS2Float.MAX_FLOATING_POINT_VALUE;
            }
            else if (cexp < 1)
            {
                uf = true;
                return sign;
            }
            return (quotient & 0x7fffff) | ((uint)cexp << 23) | sign;
        }

        private uint FastSqrt(uint a)
        {
            uint m = Mantissa(a) << 1;
            if ((a & 0x800000) == 0) // If exponent is odd after subtracting bias of 127
                m <<= 1;
            CSAResult current = new CSAResult(m, 0);
            uint quotient = 0;
            int quotientBit = 1;
	        for (int i = 0; i < 25; i++)
	        {
                // Adding n to quotient adds n * (2*quotient + n) to quotient^2
                // (which is what we need to subtract from the remainder)
                uint adjust = quotient + ((uint)quotientBit << (24 - i));
                quotient += (uint)quotientBit << (25 - i);
                uint add = quotientBit > 0 ? ~adjust : quotientBit < 0 ? adjust : 0;
                current.Carry += (quotientBit > 0) ? 1u : 0u;
                CSAResult csa = CSA(current.Sum, current.Carry, add);
                quotientBit = QuotientSelect(quotientBit != 0 ? csa : current);
                current.Sum = csa.Sum << 1;
		        current.Carry = csa.Carry << 1;
	        }
            int Dvdtexp = (int)Exponent(a);
            if (Dvdtexp == 0)
                return 0;
            Dvdtexp = (Dvdtexp + 127) >> 1;
            uint result = ((quotient >> 2) & 0x7fffff) | ((uint)Dvdtexp << 23);
            bool sign = ((a >> 31) & 1) != 0;
            if (sign)
            {
                sign = ((result >> 31) & 1) != 0;
                if (sign)
                    result ^= PS2Float.SIGNMASK;
                iv = true;
            }
            return result;
        }
    }
}
