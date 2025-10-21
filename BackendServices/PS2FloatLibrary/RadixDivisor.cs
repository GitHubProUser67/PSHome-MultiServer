// SPDX-FileCopyrightText: 2002-2025 PCSX2 Dev Team
// SPDX-License-Identifier: GPL-3.0+

using System;

namespace PS2FloatLibrary
{
    //****************************************************************
    // Radix Divisor
    // Algorithm reference: DOI 10.1109/ARITH.1995.465363
    // Fast version from the PCSX2 Team (TellowKrinkle)
    //****************************************************************
    public class RadixDivisor
    {
        private static bool _fastRadix = true; // Uses an accurate and faster radix divisor when set to true.

        public static bool FastRadix
        {
            get
            {
                return _fastRadix;
            }
            set
            {
                _fastRadix = value;
            }
        }

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

        private int[] Rest = new int[26];
        private int[] Quotient = new int[26];
        private int[] Product = new int[26];
        private int[] Sum = new int[26];
        private int[] Divisor = new int[26];
        private int[] Carry = new int[26];
        private int[] Mult = new int[26];

        private bool divMode;

        private int SubCarry;
        private int SubCarry0;
        private int SubSum;
        private int SubSum0;
        private int SubMult;

        public RadixDivisor(bool divMode, uint f1, uint f2)
        {
            this.divMode = divMode;

            if (divMode)
            {
                if (((f1 & 0x7F800000) == 0) && ((f2 & 0x7F800000) != 0))
                {
                    floatResult = 0;
                    floatResult &= ps2float.MAX_FLOATING_POINT_VALUE;
                    floatResult |= (uint)(((int)(f2 >> 31) != (int)(f1 >> 31)) ? 1 : 0 & 1) << 31;
                    return;
                }
                if (((f1 & 0x7F800000) != 0) && ((f2 & 0x7F800000) == 0))
                {
                    dz = true;
                    floatResult = ps2float.MAX_FLOATING_POINT_VALUE;
                    floatResult &= ps2float.MAX_FLOATING_POINT_VALUE;
                    floatResult |= (uint)(((int)(f2 >> 31) != (int)(f1 >> 31)) ? 1 : 0 & 1) << 31;
                    return;
                }
                if (((f1 & 0x7F800000) == 0) && ((f2 & 0x7F800000) == 0))
                {
                    iv = true;
                    floatResult = ps2float.MAX_FLOATING_POINT_VALUE;
                    floatResult &= ps2float.MAX_FLOATING_POINT_VALUE;
                    floatResult |= (uint)(((int)(f2 >> 31) != (int)(f1 >> 31)) ? 1 : 0 & 1) << 31;
                    return;
                }

                if (_fastRadix)
                {
                    floatResult = FastDiv(f1, f2);
                    return;
                }
            }
            else if ((f2 & 0x7F800000) == 0)
            {
                floatResult = 0;
                iv = ((f2 >> 31) & 1) != 0;
                return;
            }
            else if (_fastRadix)
            {
                floatResult = FastSqrt(f2);
                return;
            }

            uint floatDivisor, floatDividend;
            int i, j, csaRes;
            int man = 0;
            int QuotientValueDomain = 1;

            Product[0] = 1;
            Carry[25] = 1;

            if (divMode)
            {
                floatDividend = f1;
                floatDivisor = f2;
            }
            else
            {
                floatDividend = f2;
                floatDivisor = f1;
            }

            byte Dvdtexp = (byte)((floatDividend >> 23) & byte.MaxValue);
            byte Dvsrexp = (byte)((floatDivisor >> 23) & byte.MaxValue);
            int Dvdtsign = (int)(floatDividend >> 31);
            int Dvsrsign = (int)(floatDivisor >> 31);

            Sum[0] = 1;
            Sum[1] = ((floatDividend & 0x400000) != 0) ? 1 : 0;
            Sum[2] = ((floatDividend & 0x200000) != 0) ? 1 : 0;
            Sum[3] = ((floatDividend & 0x100000) != 0) ? 1 : 0;
            Sum[4] = ((floatDividend & 0x80000) != 0) ? 1 : 0;
            Sum[5] = ((floatDividend & 0x40000) != 0) ? 1 : 0;
            Sum[6] = ((floatDividend & 0x20000) != 0) ? 1 : 0;
            Sum[7] = (int)((floatDividend >> 16) & 1);
            Sum[8] = (int)((floatDividend >> 15) & 1);
            Sum[9] = ((floatDividend & 0x4000) != 0) ? 1 : 0;
            Sum[10] = ((floatDividend & 0x2000) != 0) ? 1 : 0;
            Sum[11] = ((floatDividend & 0x1000) != 0) ? 1 : 0;
            Sum[12] = ((floatDividend & 0x800) != 0) ? 1 : 0;
            Sum[13] = ((floatDividend & 0x400) != 0) ? 1 : 0;
            Sum[14] = ((floatDividend & 0x200) != 0) ? 1 : 0;
            Sum[15] = (int)((floatDividend >> 8) & 1);
            Sum[16] = (int)((floatDividend >> 7) & 1);
            Sum[17] = ((floatDividend & 0x40) != 0) ? 1 : 0;
            Sum[18] = ((floatDividend & 0x20) != 0) ? 1 : 0;
            Sum[19] = ((floatDividend & 0x10) != 0) ? 1 : 0;
            Sum[20] = ((floatDividend & 8) != 0) ? 1 : 0;
            Sum[21] = ((floatDividend & 4) != 0) ? 1 : 0;
            Sum[22] = ((floatDividend & 2) != 0) ? 1 : 0;
            Sum[23] = (int)(floatDividend & 1);
            Sum[24] = 0;
            Sum[25] = 0;

            Divisor[0] = 1;
            Divisor[1] = ((floatDivisor & 0x400000) != 0) ? 1 : 0;
            Divisor[2] = ((floatDivisor & 0x200000) != 0) ? 1 : 0;
            Divisor[3] = ((floatDivisor & 0x100000) != 0) ? 1 : 0;
            Divisor[4] = ((floatDivisor & 0x80000) != 0) ? 1 : 0;
            Divisor[5] = ((floatDivisor & 0x40000) != 0) ? 1 : 0;
            Divisor[6] = ((floatDivisor & 0x20000) != 0) ? 1 : 0;
            Divisor[7] = (int)((floatDivisor >> 16) & 1);
            Divisor[8] = (int)((floatDivisor >> 15) & 1);
            Divisor[9] = ((floatDivisor & 0x4000) != 0) ? 1 : 0;
            Divisor[10] = ((floatDivisor & 0x2000) != 0) ? 1 : 0;
            Divisor[11] = ((floatDivisor & 0x1000) != 0) ? 1 : 0;
            Divisor[12] = ((floatDivisor & 0x800) != 0) ? 1 : 0;
            Divisor[13] = ((floatDivisor & 0x400) != 0) ? 1 : 0;
            Divisor[14] = ((floatDivisor & 0x200) != 0) ? 1 : 0;
            Divisor[15] = (int)((floatDivisor >> 8) & 1);
            Divisor[16] = (int)((floatDivisor >> 7) & 1);
            Divisor[17] = ((floatDivisor & 0x40) != 0) ? 1 : 0;
            Divisor[18] = ((floatDivisor & 0x20) != 0) ? 1 : 0;
            Divisor[19] = ((floatDivisor & 0x10) != 0) ? 1 : 0;
            Divisor[20] = ((floatDivisor & 8) != 0) ? 1 : 0;
            Divisor[21] = ((floatDivisor & 4) != 0) ? 1 : 0;
            Divisor[22] = ((floatDivisor & 2) != 0) ? 1 : 0;
            Divisor[23] = (int)(floatDivisor & 1);
            Divisor[24] = 0;
            Divisor[25] = 0;

            if (!divMode && Dvdtexp % 2 == 1)
            {
                for (i = 0; i < 25; i++)
                {
                    Sum[25 - i] = Sum[24 - i];
                }
                Sum[0] = 0;
            }

            for (i = 0; i < 25; ++i)
            {
                MultipleFormation(QuotientValueDomain);
                csaRes = CSAQSLAdder(QuotientValueDomain);
                ProductQuotientRestTransformation(i, QuotientValueDomain);
                Carry[25] = csaRes > 0 ? 1 : 0;
                QuotientValueDomain = csaRes;
            }

            int sign = SignCalc(Dvdtsign, Dvsrsign) ? 1 : 0;
            int exp = ExpCalc(Dvdtexp, Dvsrexp);

            if (divMode && (Quotient[0] == 0))
                exp--;

            if (divMode)
            {
                if ((Dvdtexp == 0) && (Dvsrexp == 0))
                {
                    iv = true;
                    exp = byte.MaxValue;
                    for (i = 0; i < 25; i++)
                    {
                        Quotient[i] = 1;
                    }
                }
                else if ((Dvdtexp == 0) || (Dvsrexp != 0))
                {
                    if ((Dvdtexp == 0) && (Dvsrexp != 0))
                    {
                        exp = 0;
                        for (i = 0; i < 25; i++)
                        {
                            Quotient[i] = 0;
                        }
                    }
                }
                else
                {
                    dz = true;
                    exp = byte.MaxValue;
                    for (i = 0; i < 25; i++)
                    {
                        Quotient[i] = 1;
                    }
                }
            }
            else
            {
                if (Dvdtexp == 0)
                {
                    sign = 0;
                    exp = 0;
                    for (i = 0; i < 25; i++)
                    {
                        Quotient[i] = 0;
                    }
                }
                if (Dvdtsign == 1)
                {
                    iv = true;
                    sign = 0;
                }
            }

            if (divMode)
            {
                if (exp < 256)
                {
                    if (exp < 1)
                    {
                        uf = true;
                        exp = 0;
                        for (i = 0; i < 25; i++)
                        {
                            Quotient[i] = 0;
                        }
                    }
                }
                else
                {
                    of = true;
                    exp = byte.MaxValue;
                    for (i = 0; i < 25; i++)
                    {
                        Quotient[i] = 1;
                    }
                }
            }

            if (divMode)
                j = 2 - Quotient[0];
            else
                j = 1;

            for (i = j; i < j + 23; i++)
            {
                man = man * 2 + Quotient[i];
            }

            floatResult = 0;
            floatResult &= ps2float.MAX_FLOATING_POINT_VALUE;
            floatResult |= (uint)(sign & 1) << 31;
            floatResult &= 0x807FFFFF;
            floatResult |= (uint)(exp & byte.MaxValue) << 23;
            floatResult &= 0xFF800000;
            floatResult |= (uint)man & 0x7FFFFF;
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
            uint sign = (a ^ b) & 0x80000000;
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
                return sign | ps2float.MAX_FLOATING_POINT_VALUE;
            }
            else if (Dvdtexp == 0 || Dvsrexp != 0)
            {
                if (Dvdtexp == 0 && Dvsrexp != 0) 
                    return sign;
            }
            else
            {
                dz = true;
                return sign | ps2float.MAX_FLOATING_POINT_VALUE;
            }
            if (cexp > byte.MaxValue)
            {
                of = true;
                return sign | ps2float.MAX_FLOATING_POINT_VALUE;
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
                    result ^= 0x80000000;
                iv = true;
            }
            return result;
        }

        private bool SignCalc(int Dvdtsign, int Dvsrsign)
        {
            return divMode && Dvsrsign != Dvdtsign;
        }

        private int ExpCalc(int Dvdtexp, int Dvsrexp)
        {
            int result;

            if (divMode)
                return Dvdtexp - Dvsrexp + 127;
            if ((Dvdtexp & 1) != 0)
                result = (Dvdtexp - 127) / 2;
            else
                result = (Dvdtexp - 128) / 2;
            return result + 127;
        }

        private int CSAQSLAdder(int QuotientValueDomain)
        {
            int[] CarryArray = new int[4];
            int[] SumArray = new int[4];
            int i;

            if (QuotientValueDomain == 0)
            {
                SumArray[0] = SubSum;
                CarryArray[0] = SubCarry;
                for (i = 1; i < 4; i++)
                {
                    SumArray[i] = Sum[i - 1];
                    CarryArray[i] = Carry[i - 1];
                }
            }
            CSAAdder(SubSum, SubCarry, SubMult, out int tmpSum, out int tmpCarry);
            SubSum0 = tmpSum;
            CSAAdder(Sum[0], Carry[0], Mult[0], out tmpSum, out tmpCarry);
            SubSum = tmpSum;
            SubCarry0 = tmpCarry;
            CSAAdder(Sum[1], Carry[1], Mult[1], out tmpSum, out tmpCarry);
            Sum[0] = tmpSum;
            SubCarry = tmpCarry;
            for (i = 2; i < 26; i++)
            {
                CSAAdder(Sum[i], Carry[i], Mult[i], out tmpSum, out tmpCarry);
                Sum[i - 1] = tmpSum;
                Carry[i - 2] = tmpCarry;
            }
            Sum[i - 1] = 0;
            Carry[i - 2] = 0;
            Carry[i - 1] = ~QuotientValueDomain;
            Carry[i - 1] = (int)((uint)Carry[i - 1] >> 31);
            if (QuotientValueDomain != 0)
            {
                SumArray[0] = SubSum0;
                CarryArray[0] = SubCarry0;
                SumArray[1] = SubSum;
                CarryArray[1] = SubCarry;
                for (i = 2; i < 4; i++)
                {
                    SumArray[i] = Sum[i - 2];
                    CarryArray[i] = Carry[i - 2];
                }
            }
            return QSLAdder(SumArray, CarryArray);
        }

        private int QSLAdder(int[] SumArray, int[] CarryArray)
        {
            int specialCondition = 0;
            int result;
            int claResult = CLAAdder(SumArray, CarryArray);

            if (SumArray[3] == 1 || CarryArray[3] == 1 || (claResult % 2 != 0))
                specialCondition = 1;

            switch (claResult)
            {
                case 0:
                    result = specialCondition;
                    break;
                case 1:
                    result = specialCondition;
                    break;
                case 2:
                case 3:
                    result = 1;
                    break;
                case 4:
                case 5:
                case 6:
                    result = -1;
                    break;
                case 7:
                    result = 0;
                    break;
                default:
                    result = 0;
                    break;
            }

            return result;
        }

        private void ProductQuotientRestTransformation(int increment, int QuotientValueDomain)
        {
            int i;

            Product[increment] = 0;
            Product[increment + 1] = 1;
            if (QuotientValueDomain == 0)
                Rest[increment] = 1;
            else
            {
                if (QuotientValueDomain == -1)
                {
                    for (i = 0; i < 26; i++)
                        Quotient[i] = Rest[i];
                    Quotient[increment] = 1;
                    return;
                }
                else if (QuotientValueDomain == 1)
                {
                    for (i = 0; i < 26; ++i)
                        Rest[i] = Quotient[i];
                    Quotient[increment] = 1;
                    return;
                }
                throw new InvalidOperationException("[DivSqrt] - PQRTF: Quotient value domain error!");
            }
        }

        private void CSAAdder(int sum, int carry, int mult, out int resSum, out int resCarry)
        {
            int addResult = carry + sum + mult;
            resCarry = 0;
            resSum = 0;
            if (addResult == 1)
                resSum = 1;
            else if (addResult == 2)
                resCarry = 1;
            else if (addResult == 3)
            {
                resSum = 1;
                resCarry = 1;
            }
        }

        private int CLAAdder(int[] SumArray, int[] CarryArray)
        {
            return (2 * CarryArray[1] + 4 * CarryArray[0] + CarryArray[2] + 2 * SumArray[1] + 4 * SumArray[0] + SumArray[2]) % 8;
        }

        private bool BitInvert(int val)
        {
            return val < 1;
        }

        private void MultipleFormation(int QuotientValueDomain)
        {
            int i;

            if (QuotientValueDomain == 0)
            {
                SubMult = 0;
                for (i = 0; i < 26; i++)
                    Mult[i] = 0;
            }
            else if (divMode)
                DivideModeFormation(QuotientValueDomain);
            else
                RootModeFormation(QuotientValueDomain);
        }

        private void DivideModeFormation(int QuotientValueDomain)
        {
            int i;

            if (QuotientValueDomain < 1)
            {
                SubMult = 0;
                for (i = 0; i < 26; i++)
                    Mult[i] = Divisor[i];
            }
            else
            {
                SubMult = 1;
                for (i = 0; i < 26; i++)
                    Mult[i] = BitInvert(Divisor[i]) ? 1 : 0;
            }
        }

        private void RootModeFormation(int QuotientValueDomain)
        {
            int i;

            if (QuotientValueDomain < 1)
            {
                SubMult = 0;
                if (Product[0] == 1)
                    Mult[0] = 1;
                else
                    Mult[0] = Rest[0];
                for (i = 1; i < 26; i++)
                {
                    if (Product[i - 1] == 1 || Product[i] == 1)
                        Mult[i] = 1;
                    else
                        Mult[i] = Rest[i];
                }
            }
            else
            {
                SubMult = 1;
                Mult[0] = BitInvert(Quotient[0]) ? 1 : 0;
                for (i = 1; i < 26; i++)
                {
                    if (Product[i - 1] == 1)
                        Mult[i] = 0;
                    else
                        Mult[i] = BitInvert(Quotient[i]) ? 1 : 0;
                }
            }
        }
    }
}
