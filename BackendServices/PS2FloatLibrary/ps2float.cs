using System;
using System.Runtime.CompilerServices;

namespace PS2FloatLibrary
{
    public struct PS2Float : IEquatable<PS2Float>, IComparable<PS2Float>, IComparable
    {
        private const byte BIAS = 127;
        private const byte MANTISSA_BITS = 23;

        public const uint SIGNMASK = 0x80000000;
        public const uint MAX_FLOATING_POINT_VALUE = 0x7FFFFFFF;
        public const uint MIN_FLOATING_POINT_VALUE = uint.MaxValue;
        public const uint ONE = 0x3F800000;
        public const uint MIN_ONE = 0xBF800000;

        public uint raw { get; private set; }

        public readonly uint Mantissa => raw & 0x7FFFFF;
        public readonly byte Exponent => (byte)((raw >> MANTISSA_BITS) & byte.MaxValue);
        public readonly bool Sign => ((raw >> 31) & 1) != 0;

        public bool dz;
        public bool iv;
        public bool of;
        public bool uf;

        public PS2Float(uint raw)
        {
            dz = false;
            iv = false;
            of = false;
            uf = false;
            this.raw = raw;
        }

        public unsafe PS2Float(float value)
        {
            dz = false;
            iv = false;
            of = false;
            uf = false;
            raw = *(uint*)&value;
        }

        public PS2Float(bool sign, byte exponent, uint mantissa)
        {
            dz = false;
            iv = false;
            of = false;
            uf = false;
            raw = 0;
            raw |= (sign ? 1u : 0u) << 31;
            raw |= (uint)(exponent << MANTISSA_BITS);
            raw |= mantissa & 0x7FFFFF;
        }

        public static PS2Float Zero => new PS2Float(byte.MinValue);
        public static PS2Float MaxValue => new PS2Float(MAX_FLOATING_POINT_VALUE);
        public static PS2Float MinValue => new PS2Float(MIN_FLOATING_POINT_VALUE);
        public static PS2Float One => new PS2Float(ONE);
        public static PS2Float MinOne => new PS2Float(MIN_ONE);

        public static PS2Float operator +(PS2Float f1, PS2Float f2)
        {
            return f1.Add(f2);
        }

        public PS2Float Add(PS2Float addend)
        {
            if (IsDenormalized() || addend.IsDenormalized())
                return SolveAddSubDenormalizedOperation(this, addend, true);

            uint a = raw;
            uint b = addend.raw;

            // Exponent difference
            int expDiff = Exponent - addend.Exponent;

            // diff = 1 .. 24, expt < expd
            if (expDiff > 0 && expDiff < 25)
            {
                expDiff--;
                b = (uint)((unchecked((int)MIN_FLOATING_POINT_VALUE) << expDiff) & b);
            }
            // diff = -24 .. -1 , expd < expt
            else if (expDiff < 0 && expDiff > -25)
            {
                expDiff = -expDiff;
                expDiff--;
                a = (uint)(a & (unchecked((int)MIN_FLOATING_POINT_VALUE) << expDiff));
            }

            return new PS2Float(a).DoAdd(new PS2Float(b));
        }

        public static PS2Float operator -(PS2Float f1, PS2Float f2)
        {
            return f1.Sub(f2);
        }

        public PS2Float Sub(PS2Float subtrahend)
        {
            if (IsDenormalized() || subtrahend.IsDenormalized())
                return SolveAddSubDenormalizedOperation(this, subtrahend, false);

            uint a = raw;
            uint b = subtrahend.raw;

            // Exponent difference
            int expDiff = Exponent - subtrahend.Exponent;

            // diff = 1 .. 24, expt < expd
            if (expDiff > 0 && expDiff < 25)
            {
                expDiff--;
                b = (uint)((unchecked((int)MIN_FLOATING_POINT_VALUE) << expDiff) & b);
            }
            // diff = -24 .. -1 , expd < expt
            else if (expDiff < 0 && expDiff > -25)
            {
                expDiff = -expDiff;
                expDiff--;
                a = (uint)(a & (unchecked((int)MIN_FLOATING_POINT_VALUE) << expDiff));
            }

            return new PS2Float(a).DoAdd(-new PS2Float(b));
        }

        public static PS2Float operator *(PS2Float f1, PS2Float f2)
        {
            return f1.Mul(f2);
        }

        public PS2Float Mul(PS2Float mulend)
        {
            if (IsDenormalized() || mulend.IsDenormalized() || IsZero() || mulend.IsZero())
                return new PS2Float(DetermineMultiplicationDivisionOperationSign(this, mulend), 0, 0);

            return DoMul(mulend);
        }

        public PS2Float MulAdd(PS2Float opsend, PS2Float optend)
        {
            PS2Float mulres = opsend.Mul(optend);
            PS2Float addres = Add(mulres);
            uint rawres = addres.raw;
            bool oflw = addres.of;
            bool uflw = addres.uf;
            DetermineMacException(3, raw, of, mulres.of, mulres.Sign ? 1 : 0, ref rawres, ref oflw, ref uflw);
            return new PS2Float(rawres) { of = oflw, uf = uflw };
        }

        public PS2Float MulAddAcc(PS2Float opsend, PS2Float optend)
        {
            PS2Float mulres = opsend.Mul(optend);
            PS2Float addres = Add(mulres);
            uint rawres = addres.raw;
            bool oflw = addres.of;
            bool uflw = addres.uf;
            DetermineMacException(8, raw, of, mulres.of, mulres.Sign ? 1 : 0, ref rawres, ref oflw, ref uflw);
            raw = rawres;
            of = oflw;
            return new PS2Float(rawres) { of = oflw, uf = uflw };
        }

        public PS2Float MulSub(PS2Float opsend, PS2Float optend)
        {
            PS2Float mulres = opsend.Mul(optend);
            PS2Float subres = Sub(mulres);
            uint rawres = subres.raw;
            bool oflw = subres.of;
            bool uflw = subres.uf;
            DetermineMacException(4, raw, of, mulres.of, mulres.Sign ? 1 : 0, ref rawres, ref oflw, ref uflw);
            return new PS2Float(rawres) { of = oflw, uf = uflw };
        }

        public PS2Float MulSubAcc(PS2Float opsend, PS2Float optend)
        {
            PS2Float mulres = opsend.Mul(optend);
            PS2Float subres = Sub(mulres);
            uint rawres = subres.raw;
            bool oflw = subres.of;
            bool uflw = subres.uf;
            DetermineMacException(9, raw, of, mulres.of, mulres.Sign ? 1 : 0, ref rawres, ref oflw, ref uflw);
            raw = rawres;
            of = oflw;
            return new PS2Float(rawres) { of = oflw, uf = uflw };
        }

        public static PS2Float operator /(PS2Float f1, PS2Float f2)
        {
            return f1.Div(f2);
        }

        public PS2Float Div(PS2Float divend)
        {
            RadixDivisor radix = new RadixDivisor(true, raw, divend.raw);
            return new PS2Float(radix.floatResult) {
                dz = radix.dz,
                iv = radix.iv,
                of = radix.of,
                uf = radix.uf
            };
        }

        private PS2Float DoAdd(PS2Float other)
        {
            byte selfExponent = Exponent;
            int resExponent = selfExponent - other.Exponent;

            if (resExponent < 0)
                return other.DoAdd(this);
            else if (resExponent >= 25)
                return this;

            const byte roundingMultiplier = 6;

            // http://graphics.stanford.edu/~seander/bithacks.html#ConditionalNegate
            uint sign1 = (uint)((int)raw >> 31);
            int selfMantissa = (int)(((Mantissa | 0x800000) ^ sign1) - sign1);
            uint sign2 = (uint)((int)other.raw >> 31);
            int otherMantissa = (int)(((other.Mantissa | 0x800000) ^ sign2) - sign2);

            int man = (selfMantissa << roundingMultiplier) + ((otherMantissa << roundingMultiplier) >> resExponent);
            int absMan = Math.Abs(man);
            if (absMan == 0)
                return Zero;

            int rawExp = selfExponent - roundingMultiplier;

            int amount = BitUtils.normalizeAmounts[BitUtils.CountLeadingSignBits(absMan)];
            rawExp -= amount;
            absMan <<= amount;

            int msbIndex = BitUtils.BitScanReverse8(absMan >> MANTISSA_BITS);
            rawExp += msbIndex;
            absMan >>= msbIndex;

            if (rawExp > byte.MaxValue)
            {
                PS2Float result = man < 0 ? MinValue : MaxValue;
                result.of = true;
                return result;
            }
            else if (rawExp < 1)
                return new PS2Float(man < 0, 0, 0)
                {
                    uf = true
                };

            return new PS2Float((uint)man & SIGNMASK | (uint)rawExp << MANTISSA_BITS | ((uint)absMan & 0x7FFFFF));
        }

        private PS2Float DoMul(PS2Float other)
        {
            byte selfExponent = Exponent;
            byte otherExponent = other.Exponent;
            uint selfMantissa = Mantissa | 0x800000;
            uint otherMantissa = other.Mantissa | 0x800000;
            uint sign = (raw ^ other.raw) & SIGNMASK;

            int resExponent = selfExponent + otherExponent - BIAS;
            uint resMantissa = (uint)(BoothMultiplier.MulMantissa(selfMantissa, otherMantissa) >> MANTISSA_BITS);

            if (resMantissa > 0xFFFFFF)
            {
                resMantissa >>= 1;
                resExponent++;
            }

            if (resExponent > byte.MaxValue)
                return new PS2Float(sign | MAX_FLOATING_POINT_VALUE) 
                { 
                    of = true
                };
            else if (resExponent < 1)
                return new PS2Float(sign)
                {
                    uf = true
                };

            return new PS2Float(sign | (uint)(resExponent << MANTISSA_BITS) | (resMantissa & 0x7FFFFF));
        }

        /// <summary>
        /// Returns the square root of x
        /// </summary>
        public PS2Float Sqrt()
        {
            RadixDivisor radix = new RadixDivisor(false, 0, new PS2Float(false, Exponent, Mantissa).raw);
            return new PS2Float(radix.floatResult)
            {
                dz = radix.dz,
                iv = radix.iv,
            };
        }

        public PS2Float Rsqrt(PS2Float other)
        {
            RadixDivisor radixSqrt = new RadixDivisor(false, 0, new PS2Float(false, other.Exponent, other.Mantissa).raw);
            RadixDivisor radixDiv = new RadixDivisor(true, raw, radixSqrt.floatResult);
            return new PS2Float(radixDiv.floatResult)
            {
                dz = radixSqrt.dz || radixDiv.dz,
                iv = radixSqrt.iv || radixDiv.iv,
                of = radixDiv.of,
                uf = radixDiv.uf
            };
        }

        public PS2Float ELENG(PS2Float y, PS2Float z)
        {
            PS2Float ACC = Mul(this);
            ACC.MulAddAcc(y, y);
            PS2Float p = ACC.MulAdd(z, z);
            return p.Sqrt();
        }

        public PS2Float ERCPR()
        {
            return One.Div(this);
        }

        public PS2Float ERLENG(PS2Float y, PS2Float z)
        {
            PS2Float ACC = Mul(this);
            ACC.MulAddAcc(y, y);
            PS2Float p = ACC.MulAdd(z, z);
            p = One.Rsqrt(p);
            return p;
        }

        public PS2Float ERSADD(PS2Float y, PS2Float z)
        {
            PS2Float ACC = Mul(this);
            ACC.MulAddAcc(y, y);
            PS2Float p = ACC.MulAdd(z, z);
            p = One.Div(p);
            return p;
        }

        public PS2Float ESQRT()
        {
            return Sqrt();
        }

        public PS2Float ESQUR()
        {
            return Mul(this);
        }

        public PS2Float ESUM(PS2Float y, PS2Float z, PS2Float w)
        {
            PS2Float ACC = Mul(One);
            ACC.MulAddAcc(y, One);
            ACC.MulAddAcc(z, One);
            return ACC.MulAdd(w, One);
        }

        public PS2Float ERSQRT()
        {
            return One.Rsqrt(this);
        }

        public PS2Float ESADD(PS2Float y, PS2Float z)
        {
            PS2Float ACC = Mul(this);
            ACC.MulAddAcc(y, y);
            return ACC.MulAdd(z, z);
        }

        public PS2Float EEXP()
        {
            float[] consts = new float[6] {0.249998688697815f, 0.031257584691048f, 0.002591371303424f,
                        0.000171562001924f, 0.000005430199963f, 0.000000690600018f};

            PS2Float tmp1 = Mul(this);
            PS2Float ACC = Mul(new PS2Float(consts[0]));
            PS2Float tmp2 = tmp1.Mul(this);
            ACC.MulAddAcc(tmp1, new PS2Float(consts[1]));
            tmp1 = tmp2.Mul(this);
            ACC.MulAddAcc(tmp2, new PS2Float(consts[2]));
            tmp2 = tmp1.Mul(this);
            ACC.MulAddAcc(tmp1, new PS2Float(consts[3]));
            tmp1 = tmp2.Mul(this);
            ACC.MulAddAcc(tmp2, new PS2Float(consts[4]));
            ACC.MulAddAcc(One, One);
            PS2Float p = ACC.MulAdd(tmp1, new PS2Float(consts[5]));
            p = p.Mul(p);
            p = p.Mul(p);
            p = One.Div(p);

            return p;
        }

        public PS2Float EATAN()
        {
            float[] eatanconst = new float[9] { 0.999999344348907f, -0.333298563957214f, 0.199465364217758f, -0.13085337519646f,
                            0.096420042216778f, -0.055909886956215f, 0.021861229091883f, -0.004054057877511f,
                            0.785398185253143f };

            PS2Float tmp1 = Add(One);
            PS2Float tmp2 = Sub(One);
            this = tmp2.Div(tmp1);
            PS2Float tmp3 = Mul(this);
            PS2Float ACC = new PS2Float(eatanconst[0]).Mul(this);
            tmp1 = tmp3.Mul(this);
            tmp2 = tmp1.Mul(tmp3);
            ACC.MulAddAcc(tmp1, new PS2Float(eatanconst[1]));
            tmp1 = tmp2.Mul(tmp3);
            ACC.MulAddAcc(tmp2, new PS2Float(eatanconst[2]));
            tmp2 = tmp1.Mul(tmp3);
            ACC.MulAddAcc(tmp1, new PS2Float(eatanconst[3]));
            tmp1 = tmp2.Mul(tmp3);
            ACC.MulAddAcc(tmp2, new PS2Float(eatanconst[4]));
            tmp2 = tmp1.Mul(tmp3);
            ACC.MulAddAcc(tmp1, new PS2Float(eatanconst[5]));
            tmp1 = tmp2.Mul(tmp3);
            ACC.MulAddAcc(tmp2, new PS2Float(eatanconst[6]));
            ACC.MulAddAcc(One, new PS2Float(eatanconst[8]));

            return ACC.MulAdd(tmp1, new PS2Float(eatanconst[7]));
        }

        public PS2Float ESIN()
        {
            float[] sinconsts = new float[5] { 1.0f, -0.166666567325592f, 0.008333025500178f, -0.000198074136279f, 0.000002601886990f };

            PS2Float tmp3 = Mul(this);
            PS2Float ACC = Mul(new PS2Float(sinconsts[0]));
            PS2Float tmp1 = tmp3.Mul(this);
            PS2Float tmp2 = tmp1.Mul(tmp3);
            ACC.MulAddAcc(tmp1, new PS2Float(sinconsts[1]));
            tmp1 = tmp2.Mul(tmp3);
            ACC.MulAddAcc(tmp2, new PS2Float(sinconsts[2]));
            tmp2 = tmp1.Mul(tmp3);
            ACC.MulAddAcc(tmp1, new PS2Float(sinconsts[3]));

            return ACC.MulAdd(tmp2, new PS2Float(sinconsts[4]));
        }

        public bool IsDenormalized()
        {
            return Exponent == 0;
        }

        public bool IsZero()
        {
            return Abs() == 0;
        }

        public static bool operator ==(PS2Float f1, PS2Float f2)
        {
            return f1.Equals(f2);
        }

        public override bool Equals(object obj) => obj != null && GetType() == obj.GetType() && Equals((PS2Float)obj);

        public bool Equals(PS2Float other)
        {
            return raw == other.raw;
        }

        public override int GetHashCode()
        {
            return (int)raw;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(PS2Float f1, PS2Float f2) => !(f1 == f2);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(PS2Float f1, PS2Float f2) => f1.CompareTo(f2) < 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(PS2Float f1, PS2Float f2) => f1.CompareTo(f2) > 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(PS2Float f1, PS2Float f2) => f1.CompareTo(f2) <= 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(PS2Float f1, PS2Float f2) => f1.CompareTo(f2) >= 0;

        public int CompareTo(PS2Float other)
        {
            int selfTwoComplementVal = (int)Abs();
            if (Sign) selfTwoComplementVal = -selfTwoComplementVal;

            int otherTwoComplementVal = (int)other.Abs();
            if (other.Sign) otherTwoComplementVal = -otherTwoComplementVal;

            return selfTwoComplementVal.CompareTo(otherTwoComplementVal);
        }

        public int CompareTo(object obj) => obj is PS2Float f ? CompareTo(f) : throw new ArgumentException("obj");

        public int CompareOperands(PS2Float other)
        {
            return Abs().CompareTo(other.Abs());
        }

        /// <summary>
        /// Returns the maximum of the two given PS2Float values.
        /// </summary>
        public static PS2Float Max(PS2Float f1, PS2Float f2)
        {
            int a = (int)f1.raw;
            int b = (int)f2.raw;

            return new PS2Float((a < 0 && b < 0) ? (uint)Math.Min(a, b) : (uint)Math.Max(a, b));
        }

        /// <summary>
        /// Returns the minimum of the two given PS2Float values.
        /// </summary>
        public static PS2Float Min(PS2Float f1, PS2Float f2)
        {
            int a = (int)f1.raw;
            int b = (int)f2.raw;

            return new PS2Float((a < 0 && b < 0) ? (uint)Math.Max(a, b) : (uint)Math.Min(a, b));
        }

        public static void Clip(uint f1, uint f2, out bool cplus, out bool cminus)
        {
            bool resultPlus = false;
            bool resultMinus = false;
            uint a;

            if ((f1 & 0x7F800000) == 0)
            {
                f1 &= 0xFF800000;
            }

            a = f1;

            if ((f2 & 0x7F800000) == 0)
            {
                f2 &= 0xFF800000;
            }

            f1 = f1 & MAX_FLOATING_POINT_VALUE;
            f2 = f2 & MAX_FLOATING_POINT_VALUE;

            if ((-1 < (int)a) && (f2 < f1))
                resultPlus = true;

            cplus = resultPlus;

            if (((int)a < 0) && (f2 < f1))
                resultMinus = true;

            cminus = resultMinus;
        }

        public uint Abs()
        {
            return raw & MAX_FLOATING_POINT_VALUE;
        }

        private PS2Float Negate()
        {
            return new PS2Float(raw ^ SIGNMASK);
        }

        private static PS2Float SolveAddSubDenormalizedOperation(PS2Float a, PS2Float b, bool add)
        {
            bool sign = add ? DetermineAdditionOperationSign(a, b) : DetermineSubtractionOperationSign(a, b);

            if (a.IsDenormalized() && !b.IsDenormalized())
                return new PS2Float(sign, b.Exponent, b.Mantissa);
            else if (!a.IsDenormalized() && b.IsDenormalized())
                return new PS2Float(sign, a.Exponent, a.Mantissa);
            else if (a.IsDenormalized() && b.IsDenormalized())
            {
                if (add)
                {
                    if (!a.Sign || !b.Sign)
                        return new PS2Float(false, 0, 0);
                    else if (a.Sign && b.Sign)
                        return new PS2Float(true, 0, 0);
                    else
                        throw new InvalidOperationException("Unhandled addition operation flags");
                }
                else
                {
                    if (!a.Sign || b.Sign)
                        return new PS2Float(false, 0, 0);
                    else if (a.Sign && !b.Sign)
                        return new PS2Float(true, 0, 0);
                    else
                        throw new InvalidOperationException("Unhandled subtraction operation flags");
                }
            }
            else
                throw new InvalidOperationException("Both numbers are not denormalized");
        }

        private static bool DetermineMultiplicationDivisionOperationSign(PS2Float a, PS2Float b)
        {
            return a.Sign ^ b.Sign;
        }

        private static bool DetermineAdditionOperationSign(PS2Float a, PS2Float b)
        {
            return a.CompareOperands(b) >= 0 ? a.Sign : b.Sign;
        }

        private static bool DetermineSubtractionOperationSign(PS2Float a, PS2Float b)
        {
            return a.CompareOperands(b) >= 0 ? a.Sign : !b.Sign;
        }

        private static void DetermineMacException(byte mode, uint acc, bool acc_oflw, bool moflw, int msign, ref uint addsubres, ref bool oflw, ref bool uflw)
        {
            bool roundToMax;

            if ((mode == 3) || (mode == 8))
                roundToMax = msign == 0;
            else
            {
                if ((mode != 4) && (mode != 9))
                    throw new InvalidOperationException("Unhandled MacFlag operation flags");

                roundToMax = msign != 0;
            }

            if (!acc_oflw)
            {
                if (moflw)
                {
                    if (roundToMax)
                    {
                        addsubres = MAX_FLOATING_POINT_VALUE;
                        uflw = false;
                        oflw = true;
                    }
                    else
                    {
                        addsubres = MIN_FLOATING_POINT_VALUE;
                        uflw = false;
                        oflw = true;
                    }
                }
            }
            else if (!moflw)
            {
                addsubres = acc;
                uflw = false;
                oflw = true;
            }
            else if (roundToMax)
            {
                addsubres = MAX_FLOATING_POINT_VALUE;
                uflw = false;
                oflw = true;
            }
            else
            {
                addsubres = MIN_FLOATING_POINT_VALUE;
                uflw = false;
                oflw = true;
            }
        }

        /// <summary>
        /// Converts a float number to a ps2float value
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator PS2Float(float f)
        {
            return new PS2Float(f);
        }

        /// <summary>
        /// Creates a double number from a ps2float value
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator double(PS2Float f)
        {
            return f.ToDouble();
        }

        /// <summary>
        /// Creates a int number from a ps2float value
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator int(PS2Float f)
        {
            return Ftoi(0, f.raw);
        }

        /// <summary>
        /// Creates a ps2float number from a int value
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator PS2Float(int value)
        {
            return Itof(0, value);
        }

        public static PS2Float operator -(PS2Float f) => f.Negate();

        public static PS2Float Itof(int complement, int value)
        {
            if (value == 0)
                return Zero;

            int resExponent;

            bool negative = value < 0;

            if (value == int.MinValue)
            {
                if (complement <= 0)
                    // special case
                    return new PS2Float(0xcf000000);
                else
                    value = int.MaxValue;
            }

            int u = Math.Abs(value);

            int shifts;

            int lzcnt = BitUtils.CountLeadingSignBits(u);
            if (lzcnt < 8)
            {
                int count = 8 - lzcnt;
                u >>= count;
                shifts = -count;
            }
            else
            {
                int count = lzcnt - 8;
                u <<= count;
                shifts = count;
            }

            resExponent = BIAS + MANTISSA_BITS - shifts - complement;

            if (resExponent >= 158)
                return negative ? new PS2Float(0xcf000000) : new PS2Float(0x4f000000);
            else if (resExponent >= 0)
                return new PS2Float(negative, (byte)resExponent, (uint)u);

            return Zero;
        }

        public static int Ftoi(int complement, uint f)
        {
            uint a, result;

            a = f;
            if ((f & 0x7F800000) == 0)
                result = 0;
            else
            {
                complement = (int)(f >> MANTISSA_BITS & byte.MaxValue) + complement;
                f &= 0x7FFFFF;
                f |= 0x800000;
                if (complement < 158)
                {
                    if (complement > 126)
                    {
                        f = (f << 7) >> (31 - ((byte)complement - 126));
                        if ((int)a < 0)
                            f = ~f + 1;
                        result = f;
                    }
                    else
                        result = 0;
                }
                else if ((int)a < 0)
                    result = SIGNMASK;
                else
                    result = MAX_FLOATING_POINT_VALUE;
            }

            return (int)result;
        }

        public double ToDouble()
        {
            double res = (Mantissa / Math.Pow(2, MANTISSA_BITS) + 1.0) * Math.Pow(2, Exponent - 127.0);
            if (Sign)
                res *= -1.0;

            return res;
        }

        public override string ToString()
        {
            double res = ToDouble();

            uint value = raw;
            if (IsDenormalized())
                return $"Denormalized({res:F6})";
            else if (value == MAX_FLOATING_POINT_VALUE)
                return $"Fmax({res:F6})";
            else if (value == MIN_FLOATING_POINT_VALUE)
                return $"-Fmax({res:F6})";

            return $"PS2Float({res:F6})";
        }
    }
}
