#region Copyright
/*============================================================================

This is a C# port of the SoftFloat library release 3e by Thomas Kaiser (2022).
The copyright from the original source code is listed below.

This C source file is part of the SoftFloat IEEE Floating-Point Arithmetic
Package, Release 3e, by John R. Hauser.

Copyright 2011, 2012, 2013, 2014, 2015, 2016, 2017, 2018 The Regents of the
University of California.  All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

 1. Redistributions of source code must retain the above copyright notice,
    this list of conditions, and the following disclaimer.

 2. Redistributions in binary form must reproduce the above copyright notice,
    this list of conditions, and the following disclaimer in the documentation
    and/or other materials provided with the distribution.

 3. Neither the name of the University nor the names of its contributors may
    be used to endorse or promote products derived from this software without
    specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE REGENTS AND CONTRIBUTORS "AS IS", AND ANY
EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE, ARE
DISCLAIMED.  IN NO EVENT SHALL THE REGENTS OR CONTRIBUTORS BE LIABLE FOR ANY
DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

=============================================================================*/
#endregion

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Tommunism.SoftFloat
{
    using static Internals;
    using static Primitives;

    [StructLayout(LayoutKind.Sequential, Pack = sizeof(ulong), Size = sizeof(ulong))]
    public readonly struct Float64
    {
        #region Fields

        // WARNING: DO NOT ADD OR CHANGE ANY OF THESE FIELDS!!!
        private readonly ulong _v;

        private const int MantissaBits = 52;

        #endregion

        #region Constructors

        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Used to avoid accidentally calling other overloads.")]
        private Float64(ulong v, bool dummy)
        {
            _v = v;
        }

        public Float64(double value)
        {
            _v = BitConverter.DoubleToUInt64Bits(value);
        }

        #endregion

        #region Methods

        public static explicit operator Float64(double value) => new(value);
        public static implicit operator double(Float64 value) => BitConverter.UInt64BitsToDouble(value._v);

        public static Float64 FromUIntBits(ulong value) => FromBitsUI64(value);

        public ulong ToUInt64Bits() => _v;

        public ulong RawMantissa => _v & 0xFFFFFFFFFFFFF;
        public long Mantissa
        {
            get
            {
                if (((ushort)(_v >> MantissaBits & 0x7FF)) != 0)
                {
                    ulong sign = (ulong)((long)_v >> 63);
                    return (long)(((RawMantissa | 0x10000000000000) ^ sign) - sign);
                }
                else
                {
                    ulong sign = (ulong)((long)_v >> 63);
                    return (long)(((RawMantissa) ^ sign) - sign);
                }
            }
        }

        // THIS IS THE INTERNAL CONSTRUCTOR FOR RAW BITS.
        internal static Float64 FromBitsUI64(ulong v) => new(v, dummy: false);

        #region Integer-to-floating-point Conversions

        // ui32_to_f64
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "API consistency and possible future use.")]
        public static Float64 FromUInt32(SoftFloatContext context, uint a)
        {
            if (a == 0)
                return FromBitsUI64(0);

            var shiftDist = CountLeadingZeroes32(a) + 21;
            return PackToF64(false, 0x432 - shiftDist, (ulong)a << shiftDist);
        }

        // ui64_to_f64
        public static Float64 FromUInt64(SoftFloatContext context, ulong a)
        {
            if (a == 0)
                return FromBitsUI64(0);

            return (a & 0x8000000000000000) != 0
                ? RoundPackToF64(context, false, 0x43D, ShortShiftRightJam64(a, 1))
                : NormRoundPackToF64(context, false, 0x43C, a);
        }

        // i32_to_f64
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "API consistency and possible future use.")]
        public static Float64 FromInt32(SoftFloatContext context, int a)
        {
            if (a == 0)
                return FromBitsUI64(0);

            var sign = a < 0;
            var absA = (uint)(sign ? -a : a);
            var shiftDist = CountLeadingZeroes32(absA) + 21;
            return PackToF64(sign, 0x432 - shiftDist, (ulong)absA << shiftDist);
        }

        // i64_to_f64
        public static Float64 FromInt64(SoftFloatContext context, long a)
        {
            var sign = a < 0;
            if ((a & 0x7FFFFFFFFFFFFFFF) == 0)
                return FromBitsUI64(sign ? PackToF64UI(true, 0x43E, 0UL) : 0UL);

            var absA = (ulong)(sign ? -a : a);
            return NormRoundPackToF64(context, sign, 0x43C, absA);
        }

        #endregion

        #region Floating-point-to-integer Conversions

        public uint ToUInt32(SoftFloatContext context, bool exact) => ToUInt32(context, context.Rounding, exact);

        public ulong ToUInt64(SoftFloatContext context, bool exact) => ToUInt64(context, context.Rounding, exact);

        public int ToInt32(SoftFloatContext context, bool exact) => ToInt32(context, context.Rounding, exact);

        public long ToInt64(SoftFloatContext context, bool exact) => ToInt64(context, context.Rounding, exact);

        // f64_to_ui32
        public uint ToUInt32(SoftFloatContext context, RoundingMode roundingMode, bool exact)
        {
            ulong sig;
            int exp, shiftDist;
            bool sign;

            sign = SignF64UI(_v);
            exp = ExpF64UI(_v);
            sig = FracF64UI(_v);

            if (exp == 0x7FF && sig != 0)
            {
                switch (context.Specialize.UInt32NaNKind)
                {
                    case SpecializeNaNIntegerKind.NaNIsPosOverflow:
                        sign = false;
                        break;

                    case SpecializeNaNIntegerKind.NaNIsNegOverflow:
                        sign = true;
                        break;

                    case SpecializeNaNIntegerKind.NaNIsUnique:
                        context.RaiseFlags(ExceptionFlags.Invalid);
                        return context.UInt32FromNaN;
                }
            }

            if (exp != 0)
                sig |= 0x0010000000000000;

            shiftDist = 0x427 - exp;
            if (0 < shiftDist)
                sig = ShiftRightJam64(sig, shiftDist);

            return RoundToUI32(context, sign, sig, roundingMode, exact);
        }

        // f64_to_ui64
        public ulong ToUInt64(SoftFloatContext context, RoundingMode roundingMode, bool exact)
        {
            ulong sig;
            int exp, shiftDist;
            UInt64Extra sigExtra;
            bool sign;

            sign = SignF64UI(_v);
            exp = ExpF64UI(_v);
            sig = FracF64UI(_v);

            if (exp != 0)
                sig |= 0x0010000000000000;

            shiftDist = 0x433 - exp;
            if (shiftDist <= 0)
            {
                if (shiftDist < -11)
                {
                    context.RaiseFlags(ExceptionFlags.Invalid);
                    return (exp == 0x7FF && FracF64UI(_v) != 0)
                        ? context.UInt64FromNaN
                        : context.UInt64FromOverflow(sign);
                }

                sigExtra.V = sig << (-shiftDist);
                sigExtra.Extra = 0;
            }
            else
            {
                sigExtra = ShiftRightJam64Extra(sig, 0, shiftDist);
            }

            return RoundToUI64(context, sign, sigExtra.V, sigExtra.Extra, roundingMode, exact);
        }

        // f64_to_i32
        public int ToInt32(SoftFloatContext context, RoundingMode roundingMode, bool exact)
        {
            ulong sig;
            int exp, shiftDist;
            bool sign;

            sign = SignF64UI(_v);
            exp = ExpF64UI(_v);
            sig = FracF64UI(_v);

            if (exp == 0x7FF && sig != 0)
            {
                switch (context.Specialize.Int32NaNKind)
                {
                    case SpecializeNaNIntegerKind.NaNIsPosOverflow:
                        sign = false;
                        break;

                    case SpecializeNaNIntegerKind.NaNIsNegOverflow:
                        sign = true;
                        break;

                    case SpecializeNaNIntegerKind.NaNIsUnique:
                        context.RaiseFlags(ExceptionFlags.Invalid);
                        return context.Int32FromNaN;
                }
            }

            if (exp != 0)
                sig |= 0x0010000000000000;

            shiftDist = 0x427 - exp;
            if (0 < shiftDist)
                sig = ShiftRightJam64(sig, shiftDist);

            return RoundToI32(context, sign, sig, roundingMode, exact);
        }

        // f64_to_i64
        public long ToInt64(SoftFloatContext context, RoundingMode roundingMode, bool exact)
        {
            ulong sig;
            int exp, shiftDist;
            UInt64Extra sigExtra;
            bool sign;

            sign = SignF64UI(_v);
            exp = ExpF64UI(_v);
            sig = FracF64UI(_v);

            if (exp != 0)
                sig |= 0x0010000000000000;

            shiftDist = 0x433 - exp;
            if (shiftDist <= 0)
            {
                if (shiftDist < -11)
                {
                    context.RaiseFlags(ExceptionFlags.Invalid);
                    return (exp == 0x7FF && FracF64UI(_v) != 0)
                        ? context.Int64FromNaN
                        : context.Int64FromOverflow(sign);
                }

                sigExtra.V = sig << (-shiftDist);
                sigExtra.Extra = 0;
            }
            else
            {
                sigExtra = ShiftRightJam64Extra(sig, 0, shiftDist);
            }

            return RoundToI64(context, sign, sigExtra.V, sigExtra.Extra, roundingMode, exact);
        }

        // f64_to_ui32_r_minMag
        public uint ToUInt32RoundMinMag(SoftFloatContext context, bool exact)
        {
            ulong sig;
            int exp, shiftDist;
            uint z;
            bool sign;

            exp = ExpF64UI(_v);
            sig = FracF64UI(_v);

            shiftDist = 0x433 - exp;
            if (53 <= shiftDist)
            {
                if (exact && ((uint)exp | sig) != 0)
                    context.ExceptionFlags |= ExceptionFlags.Inexact;

                return 0;
            }

            sign = SignF64UI(_v);
            if (sign || shiftDist < 21)
            {
                context.RaiseFlags(ExceptionFlags.Invalid);
                return (exp == 0x7FF && sig != 0)
                    ? context.UInt32FromNaN
                    : context.UInt32FromOverflow(sign);
            }

            sig |= 0x0010000000000000;
            z = (uint)(sig >> shiftDist);
            if (exact && ((ulong)z << shiftDist) != sig)
                context.ExceptionFlags |= ExceptionFlags.Inexact;

            return z;
        }

        // f64_to_ui64_r_minMag
        public ulong ToUInt64RoundMinMag(SoftFloatContext context, bool exact)
        {
            ulong sig, z;
            int exp, shiftDist;
            bool sign;

            exp = ExpF64UI(_v);
            sig = FracF64UI(_v);

            shiftDist = 0x433 - exp;
            if (53 <= shiftDist)
            {
                if (exact && ((uint)exp | sig) != 0)
                    context.ExceptionFlags |= ExceptionFlags.Inexact;

                return 0;
            }

            sign = SignF64UI(_v);
            if (sign)
            {
                context.RaiseFlags(ExceptionFlags.Invalid);
                return (exp == 0x7FF && sig != 0)
                    ? context.UInt64FromNaN
                    : context.UInt64FromOverflow(sign);
            }

            if (shiftDist <= 0)
            {
                if (shiftDist < -11)
                {
                    context.RaiseFlags(ExceptionFlags.Invalid);
                    return (exp == 0x7FF && sig != 0)
                        ? context.UInt64FromNaN
                        : context.UInt64FromOverflow(sign);
                }

                z = (sig | 0x0010000000000000) << (-shiftDist);
            }
            else
            {
                sig |= 0x0010000000000000;
                z = sig >> shiftDist;
                if (exact && (sig << (-shiftDist)) != 0)
                    context.ExceptionFlags |= ExceptionFlags.Inexact;
            }

            return z;
        }

        // f64_to_i32_r_minMag
        public int ToInt32RoundMinMag(SoftFloatContext context, bool exact)
        {
            ulong sig;
            int exp, shiftDist;
            int absZ;
            bool sign;

            exp = ExpF64UI(_v);
            sig = FracF64UI(_v);

            shiftDist = 0x433 - exp;
            if (53 <= shiftDist)
            {
                if (exact && ((uint)exp | sig) != 0)
                    context.ExceptionFlags |= ExceptionFlags.Inexact;

                return 0;
            }

            sign = SignF64UI(_v);
            if (shiftDist < 22)
            {
                if (sign && exp == 0x41E && sig < 0x0000000000200000)
                {
                    if (exact && sig != 0)
                        context.ExceptionFlags |= ExceptionFlags.Inexact;

                    return -0x7FFFFFFF - 1;
                }

                context.RaiseFlags(ExceptionFlags.Invalid);
                return (exp == 0x7FF && sig != 0)
                    ? context.Int32FromNaN
                    : context.Int32FromOverflow(sign);
            }

            sig |= 0x0010000000000000;
            absZ = (int)(sig >> shiftDist);
            if (exact && ((ulong)(uint)absZ << shiftDist) != sig)
                context.ExceptionFlags |= ExceptionFlags.Inexact;

            return sign ? -absZ : absZ;
        }

        // f64_to_i64_r_minMag
        public long ToInt64RoundMinMag(SoftFloatContext context, bool exact)
        {
            ulong sig;
            int exp, shiftDist;
            long absZ;
            bool sign;

            sign = SignF64UI(_v);
            exp = ExpF64UI(_v);
            sig = FracF64UI(_v);

            shiftDist = 0x433 - exp;
            if (shiftDist <= 0)
            {
                if (shiftDist < -10)
                {
                    if (_v == PackToF64UI(true, 0x43E, 0))
                        return -0x7FFFFFFFFFFFFFFF - 1;

                    context.RaiseFlags(ExceptionFlags.Invalid);
                    return (exp == 0x7FF && sig != 0)
                        ? context.Int64FromNaN
                        : context.Int64FromOverflow(sign);
                }

                sig |= 0x0010000000000000;
                absZ = (long)(sig << -shiftDist);
            }
            else
            {
                if (53 <= shiftDist)
                {
                    if (exact && ((uint)exp | sig) != 0)
                        context.ExceptionFlags |= ExceptionFlags.Inexact;

                    return 0;
                }

                sig |= 0x0010000000000000;
                absZ = (long)(sig >> shiftDist);
                if (exact && ((ulong)absZ << shiftDist) != sig)
                    context.ExceptionFlags |= ExceptionFlags.Inexact;
            }

            return sign ? -absZ : absZ;
        }

        #endregion

        #region Floating-point-to-floating-point Conversions

        // f64_to_f16
        public Float16 ToFloat16(SoftFloatContext context)
        {
            ulong frac;
            int exp;
            uint frac16;
            bool sign;

            sign = SignF64UI(_v);
            exp = ExpF64UI(_v);
            frac = FracF64UI(_v);

            if (exp == 0x7FF)
            {
                if (frac != 0)
                {
                    context.Float64BitsToCommonNaN(_v, out var commonNaN);
                    return context.CommonNaNToFloat16(in commonNaN);
                }

                return PackToF16(sign, 0x1F, 0);
            }

            frac16 = (uint)ShortShiftRightJam64(frac, 38);
            if (((uint)exp | frac16) == 0)
                return PackToF16(sign, 0, 0);

            return RoundPackToF16(context, sign, exp - 0x3F1, frac16 | 0x4000);
        }

        // f64_to_f32
        public Float32 ToFloat32(SoftFloatContext context)
        {
            ulong frac;
            int exp;
            uint frac32;
            bool sign;

            sign = SignF64UI(_v);
            exp = ExpF64UI(_v);
            frac = FracF64UI(_v);

            if (exp == 0x7FF)
            {
                if (frac != 0)
                {
                    context.Float64BitsToCommonNaN(_v, out var commonNaN);
                    return context.CommonNaNToFloat32(in commonNaN);
                }

                return PackToF32(sign, 0xFF, 0);
            }

            frac32 = (uint)ShortShiftRightJam64(frac, 22);
            if (((uint)exp | frac32) == 0)
                return PackToF32(sign, 0, 0);

            return RoundPackToF32(context, sign, exp - 0x381, frac32 | 0x40000000);
        }

        // f64_to_extF80
        public ExtFloat80 ToExtFloat80(SoftFloatContext context)
        {
            ulong frac;
            int exp;
            bool sign;

            sign = SignF64UI(_v);
            exp = ExpF64UI(_v);
            frac = FracF64UI(_v);

            if (exp == 0x7FF)
            {
                if (frac != 0)
                {
                    context.Float64BitsToCommonNaN(_v, out var commonNaN);
                    return context.CommonNaNToExtFloat80(in commonNaN);
                }

                return PackToExtF80(sign, 0x7FFF, 0x8000000000000000);
            }

            if (exp == 0)
            {
                if (frac == 0)
                    return PackToExtF80(sign, 0, 0);

                (exp, frac) = NormSubnormalF64Sig(frac);
            }

            return PackToExtF80(sign, exp + 0x3C00, (frac | 0x0010000000000000) << 11);
        }

        // f64_to_f128
        public Float128 ToFloat128(SoftFloatContext context)
        {
            ulong frac;
            int exp;
            SFUInt128 frac128;
            bool sign;

            sign = SignF64UI(_v);
            exp = ExpF64UI(_v);
            frac = FracF64UI(_v);

            if (exp == 0x7FF)
            {
                if (frac != 0)
                {
                    context.Float64BitsToCommonNaN(_v, out var commonNaN);
                    return context.CommonNaNToFloat128(in commonNaN);
                }

                return PackToF128(sign, 0x7FFF, 0, 0);
            }

            if (exp == 0)
            {
                if (frac == 0)
                    return PackToF128(sign, 0, 0, 0);

                (exp, frac) = NormSubnormalF64Sig(frac);
                exp--;
            }

            frac128 = ShortShiftLeft128(0, frac, 60);
            return PackToF128(sign, exp + 0x3C00, frac128.V64, frac128.V00);
        }

        #endregion

        #region Arithmetic Operations

        public Float64 RoundToInt(SoftFloatContext context, bool exact) => RoundToInt(context, context.Rounding, exact);

        // f64_roundToInt
        public Float64 RoundToInt(SoftFloatContext context, RoundingMode roundingMode, bool exact)
        {
            ulong uiZ, lastBitMask, roundBitsMask;
            int exp;

            exp = ExpF64UI(_v);
            if (exp <= 0x3FE)
            {
                if ((_v & 0x7FFFFFFFFFFFFFFF) == 0)
                    return this;

                if (exact)
                    context.ExceptionFlags |= ExceptionFlags.Inexact;

                uiZ = _v & PackToF64UI(true, 0, 0);
                switch (roundingMode)
                {
                    case RoundingMode.NearEven:
                        {
                            if (FracF64UI(_v) == 0)
                                break;

                            goto case RoundingMode.NearMaxMag;
                        }
                    case RoundingMode.NearMaxMag:
                        {
                            if (exp == 0x3FE)
                                uiZ |= PackToF64UI(false, 0x3FF, 0);

                            break;
                        }
                    case RoundingMode.Min:
                        {
                            if (uiZ != 0)
                                uiZ = PackToF64UI(true, 0x3FF, 0);

                            break;
                        }
                    case RoundingMode.Max:
                        {
                            if (uiZ == 0)
                                uiZ = PackToF64UI(false, 0x3FF, 0);

                            break;
                        }
                    case RoundingMode.Odd:
                        {
                            uiZ |= PackToF64UI(false, 0x3FF, 0);
                            break;
                        }
                }

                return Float64.FromBitsUI64(uiZ);
            }

            if (0x433 <= exp)
            {
                if (exp == 0x7FF && FracF64UI(_v) != 0)
                    return context.PropagateNaNFloat64Bits(_v, 0);

                return this;
            }

            uiZ = _v;
            lastBitMask = (ulong)1 << (0x433 - exp);
            roundBitsMask = lastBitMask - 1;
            if (roundingMode == RoundingMode.NearMaxMag)
            {
                uiZ += lastBitMask >> 1;
            }
            else if (roundingMode == RoundingMode.NearEven)
            {
                uiZ += lastBitMask >> 1;
                if ((uiZ & roundBitsMask) == 0)
                    uiZ &= ~lastBitMask;
            }
            else if (roundingMode == (SignF64UI(_v) ? RoundingMode.Min : RoundingMode.Max))
            {
                uiZ += roundBitsMask;
            }

            uiZ &= ~roundBitsMask;
            if (uiZ != _v)
            {
                if (roundingMode == RoundingMode.Odd)
                    uiZ |= lastBitMask;

                if (exact)
                    context.ExceptionFlags |= ExceptionFlags.Inexact;
            }

            return Float64.FromBitsUI64(uiZ);
        }

        // f64_add
        public static Float64 Add(SoftFloatContext context, Float64 a, Float64 b)
        {
            var signA = SignF64UI(a._v);
            var signB = SignF64UI(b._v);

            return (signA == signB)
                ? AddMagsF64(context, a._v, b._v, signA)
                : SubMagsF64(context, a._v, b._v, signA);
        }

        // f64_sub
        public static Float64 Subtract(SoftFloatContext context, Float64 a, Float64 b)
        {
            var signA = SignF64UI(a._v);
            var signB = SignF64UI(b._v);

            return (signA == signB)
                ? SubMagsF64(context, a._v, b._v, signA)
                : AddMagsF64(context, a._v, b._v, signA);
        }

        // f64_mul
        public static Float64 Multiply(SoftFloatContext context, Float64 a, Float64 b)
        {
            ulong uiA, sigA, uiB, sigB, magBits, sigZ;
            int expA, expB, expZ;
            bool signA, signB, signZ;
            SFUInt128 sig128Z;

            uiA = a._v;
            signA = SignF64UI(uiA);
            expA = ExpF64UI(uiA);
            sigA = FracF64UI(uiA);
            uiB = b._v;
            signB = SignF64UI(uiB);
            expB = ExpF64UI(uiB);
            sigB = FracF64UI(uiB);
            signZ = signA ^ signB;

            if (expA == 0x7FF)
            {
                if (sigA != 0 || (expB == 0x7FF && sigB != 0))
                    return context.PropagateNaNFloat64Bits(uiA, uiB);

                magBits = (uint)expB | sigB;
                if (magBits == 0)
                {
                    context.RaiseFlags(ExceptionFlags.Invalid);
                    return context.DefaultNaNFloat64;
                }
                else
                {
                    return PackToF64(signZ, 0x7FF, 0);
                }
            }

            if (expB == 0x7FF)
            {
                if (sigB != 0)
                    return context.PropagateNaNFloat64Bits(uiA, uiB);

                magBits = (uint)expA | sigA;
                if (magBits == 0)
                {
                    context.RaiseFlags(ExceptionFlags.Invalid);
                    return context.DefaultNaNFloat64;
                }
                else
                {
                    return PackToF64(signZ, 0x7FF, 0);
                }
            }

            if (expA == 0)
            {
                if (sigA == 0)
                    return PackToF64(signZ, 0, 0);

                (expA, sigA) = NormSubnormalF64Sig(sigA);
            }

            if (expB == 0)
            {
                if (sigB == 0)
                    return PackToF64(signZ, 0, 0);

                (expB, sigB) = NormSubnormalF64Sig(sigB);
            }

            expZ = expA + expB - 0x3FF;
            sigA = (sigA | 0x0010000000000000) << 10;
            sigB = (sigB | 0x0010000000000000) << 11;
            sig128Z = Mul64To128(sigA, sigB);
            sigZ = sig128Z.V64 | (sig128Z.V00 != 0 ? 1U : 0);

            if (sigZ < 0x4000000000000000)
            {
                --expZ;
                sigZ <<= 1;
            }

            return RoundPackToF64(context, signZ, expZ, sigZ);
        }

        // f64_mulAdd
        public static Float64 MultiplyAndAdd(SoftFloatContext context, Float64 a, Float64 b, Float64 c) =>
            MulAddF64(context, a._v, b._v, c._v, MulAddOperation.None);

        // f64_div
        public static Float64 Divide(SoftFloatContext context, Float64 a, Float64 b)
        {
            ulong uiA, sigA, uiB, sigB, rem, sigZ;
            int expA, expB, expZ;
            uint recip32, sig32Z, doubleTerm, q;
            bool signA, signB, signZ;

            uiA = a._v;
            signA = SignF64UI(uiA);
            expA = ExpF64UI(uiA);
            sigA = FracF64UI(uiA);
            uiB = b._v;
            signB = SignF64UI(uiB);
            expB = ExpF64UI(uiB);
            sigB = FracF64UI(uiB);
            signZ = signA ^ signB;

            if (expA == 0x7FF)
            {
                if (sigA != 0)
                    return context.PropagateNaNFloat64Bits(uiA, uiB);

                if (expB == 0x7FF)
                {
                    if (sigB != 0)
                        return context.PropagateNaNFloat64Bits(uiA, uiB);

                    context.RaiseFlags(ExceptionFlags.Invalid);
                    return context.DefaultNaNFloat64;
                }

                return PackToF64(signZ, 0x7FF, 0);
            }

            if (expB == 0x7FF)
            {
                if (sigB != 0)
                    return context.PropagateNaNFloat64Bits(uiA, uiB);

                return PackToF64(signZ, 0, 0);
            }

            if (expB == 0)
            {
                if (sigB == 0)
                {
                    if (((uint)expA | sigA) == 0)
                    {
                        context.RaiseFlags(ExceptionFlags.Invalid);
                        return context.DefaultNaNFloat64;
                    }

                    context.RaiseFlags(ExceptionFlags.Infinite);
                    return PackToF64(signZ, 0x7FF, 0);
                }

                (expB, sigB) = NormSubnormalF64Sig(sigB);
            }

            if (expA == 0)
            {
                if (sigA == 0)
                    return PackToF64(signZ, 0, 0);

                (expA, sigA) = NormSubnormalF64Sig(sigA);
            }

            expZ = expA - expB + 0x3FE;
            sigA |= 0x0010000000000000;
            sigB |= 0x0010000000000000;
            if (sigA < sigB)
            {
                --expZ;
                sigA <<= 11;
            }
            else
            {
                sigA <<= 10;
            }

            sigB <<= 11;
            recip32 = ApproxRecip32_1((uint)(sigB >> 32)) - 2;
            sig32Z = (uint)(((uint)(sigA >> 32) * (ulong)recip32) >> 32);
            doubleTerm = sig32Z << 1;
            rem = ((sigA - (ulong)doubleTerm * (uint)(sigB >> 32)) << 28)
                - (ulong)doubleTerm * ((uint)sigB >> 4);
            q = (uint)((((uint)(rem >> 32) * (ulong)recip32) >> 32) + 4);
            sigZ = ((ulong)sig32Z << 32) + ((ulong)q << 4);

            if ((sigZ & 0x1FF) < (4 << 4))
            {
                q &= ~7U;
                sigZ &= ~(ulong)0x7F;
                doubleTerm = q << 1;
                rem = ((rem - (ulong)doubleTerm * (uint)(sigB >> 32)) << 28)
                    - (ulong)doubleTerm * ((uint)sigB >> 4);
                if ((rem & 0x8000000000000000) != 0)
                    sigZ -= 1 << 7;
                else if (rem != 0)
                    sigZ |= 1;
            }

            return RoundPackToF64(context, signZ, expZ, sigZ);
        }

        // f64_rem
        public static Float64 Modulus(SoftFloatContext context, Float64 a, Float64 b)
        {
            ulong uiA, sigA, uiB, sigB, q64;
            int expA, expB, expDiff;
            ulong rem, altRem, meanRem;
            uint q, recip32;
            bool signA, signRem;

            uiA = a._v;
            signA = SignF64UI(uiA);
            expA = ExpF64UI(uiA);
            sigA = FracF64UI(uiA);
            uiB = b._v;
            expB = ExpF64UI(uiB);
            sigB = FracF64UI(uiB);

            if (expA == 0x7FF)
            {
                if (sigA != 0 || (expB == 0x7FF && sigB != 0))
                    return context.PropagateNaNFloat64Bits(uiA, uiB);

                context.RaiseFlags(ExceptionFlags.Invalid);
                return context.DefaultNaNFloat64;
            }

            if (expB == 0x7FF)
            {
                if (sigB != 0)
                    return context.PropagateNaNFloat64Bits(uiA, uiB);

                return a;
            }

            if (expA < expB - 1)
                return a;

            if (expB == 0)
            {
                if (sigB == 0)
                {
                    context.RaiseFlags(ExceptionFlags.Invalid);
                    return context.DefaultNaNFloat64;
                }

                (expB, sigB) = NormSubnormalF64Sig(sigB);
            }

            if (expA == 0)
            {
                if (sigA == 0)
                    return a;

                (expA, sigA) = NormSubnormalF64Sig(sigA);
            }

            rem = sigA | 0x0010000000000000;
            sigB |= 0x0010000000000000;
            expDiff = expA - expB;
            if (expDiff < 1)
            {
                if (expDiff < -1)
                    return a;

                sigB <<= 9;
                if (expDiff != 0)
                {
                    rem <<= 8;
                    q = 0;
                }
                else
                {
                    rem <<= 9;
                    q = sigB <= rem ? 1U : 0;
                    if (q != 0)
                        rem -= sigB;
                }
            }
            else
            {
                recip32 = ApproxRecip32_1((uint)(sigB >> 21));

                // Changing the shift of 'rem' here requires also changing the initial subtraction from 'expDiff'.
                rem <<= 9;
                expDiff -= 30;

                // The scale of 'sigB' affects how many bits are obtained during each cycle of the loop. Currently this is 29 bits per loop
                // iteration, the maximum possible.
                sigB <<= 9;
                while (true)
                {
                    q64 = (uint)(rem >> 32) * (ulong)recip32;
                    if (expDiff < 0)
                        break;

                    q = (uint)((q64 + 0x80000000) >> 32);
                    rem <<= 29;
                    rem -= q * sigB;
                    if ((rem & 0x8000000000000000) != 0)
                        rem += sigB;

                    expDiff -= 29;
                }

                // ('expDiff' cannot be less than -29 here.)
                q = (uint)(q64 >> 32) >> (~expDiff & 31);
                rem = (rem << (expDiff + 30)) - q * sigB;
                if ((rem & 0x8000000000000000) != 0)
                {
                    altRem = rem + sigB;
                    goto selectRem;
                }
            }

            do
            {
                altRem = rem;
                ++q;
                rem -= sigB;
            }
            while ((rem & 0x8000000000000000) == 0);

        selectRem:
            meanRem = rem + altRem;
            if ((meanRem & 0x8000000000000000) != 0 || (meanRem == 0 && (q & 1) != 0))
                rem = altRem;

            signRem = signA;
            if ((rem & 0x8000000000000000) != 0)
            {
                signRem = !signRem;
                rem = (ulong)(-(long)rem);
            }

            return NormRoundPackToF64(context, signRem, expB, rem);
        }

        // f64_sqrt
        public Float64 SquareRoot(SoftFloatContext context)
        {
            ulong uiA, sigA, rem, sigZ, shiftedSigZ;
            int expA, expZ;
            uint sig32A, recipSqrt32, sig32Z, q;
            bool signA;

            uiA = _v;
            signA = SignF64UI(uiA);
            expA = ExpF64UI(uiA);
            sigA = FracF64UI(uiA);

            if (expA == 0x7FF)
            {
                if (sigA != 0)
                    return context.PropagateNaNFloat64Bits(uiA, 0);

                if (!signA)
                    return this;

                context.RaiseFlags(ExceptionFlags.Invalid);
                return context.DefaultNaNFloat64;
            }

            if (signA)
            {
                if (((uint)expA | sigA) == 0)
                    return this;

                context.RaiseFlags(ExceptionFlags.Invalid);
                return context.DefaultNaNFloat64;
            }

            if (expA == 0)
            {
                if (sigA == 0)
                    return this;

                (expA, sigA) = NormSubnormalF64Sig(sigA);
            }

            // ('sig32Z' is guaranteed to be a lower bound on the square root of 'sig32A', which makes 'sig32Z' also a lower bound on the
            // square root of 'sigA'.)
            expZ = ((expA - 0x3FF) >> 1) + 0x3FE;
            expA &= 1;
            sigA |= 0x0010000000000000;
            sig32A = (uint)(sigA >> 21);
            recipSqrt32 = ApproxRecipSqrt32_1((uint)expA, sig32A);
            sig32Z = (uint)(((ulong)sig32A * recipSqrt32) >> 32);
            if (expA != 0)
            {
                sigA <<= 8;
                sig32Z >>= 1;
            }
            else
            {
                sigA <<= 9;
            }

            rem = sigA - (ulong)sig32Z * sig32Z;
            q = (uint)(((uint)(rem >> 2) * (ulong)recipSqrt32) >> 32);
            sigZ = ((ulong)sig32Z << 32 | 1 << 5) + ((ulong)q << 3);

            if ((sigZ & 0x1FF) < 0x22)
            {
                sigZ &= ~(ulong)0x3F;
                shiftedSigZ = sigZ >> 6;
                rem = (sigA << 52) - shiftedSigZ * shiftedSigZ;
                if ((rem & 0x8000000000000000) != 0)
                    --sigZ;
                else if (rem != 0)
                    sigZ |= 1;
            }

            return RoundPackToF64(context, false, expZ, sigZ);
        }

        #endregion

        #region Comparison Operations

        // f64_eq (signaling=false) & f64_eq_signaling (signaling=true)
        public static bool CompareEqual(SoftFloatContext context, Float64 a, Float64 b, bool signaling)
        {
            ulong uiA, uiB;

            uiA = a._v;
            uiB = b._v;

            if (IsNaNF64UI(uiA) || IsNaNF64UI(uiB))
            {
                if (signaling || context.IsSignalingNaNFloat64Bits(uiA) || context.IsSignalingNaNFloat64Bits(uiB))
                    context.RaiseFlags(ExceptionFlags.Invalid);

                return false;
            }

            return uiA == uiB || ((uiA | uiB) & 0x7FFFFFFFFFFFFFFF) == 0;
        }

        // f64_le (signaling=true) & f64_le_quiet (signaling=false)
        public static bool CompareLessThanOrEqual(SoftFloatContext context, Float64 a, Float64 b, bool signaling)
        {
            ulong uiA, uiB;
            bool signA, signB;

            uiA = a._v;
            uiB = b._v;

            if (IsNaNF64UI(uiA) || IsNaNF64UI(uiB))
            {
                if (signaling || context.IsSignalingNaNFloat64Bits(uiA) || context.IsSignalingNaNFloat64Bits(uiB))
                    context.RaiseFlags(ExceptionFlags.Invalid);

                return false;
            }

            signA = SignF64UI(uiA);
            signB = SignF64UI(uiB);

            return (signA != signB)
                ? (signA || ((uiA | uiB) & 0x7FFFFFFFFFFFFFFF) == 0)
                : (uiA == uiB || (signA ^ (uiA < uiB)));
        }

        // f64_lt (signaling=true) & f64_lt_quiet (signaling=false)
        public static bool CompareLessThan(SoftFloatContext context, Float64 a, Float64 b, bool signaling)
        {
            ulong uiA, uiB;
            bool signA, signB;

            uiA = a._v;
            uiB = b._v;

            if (IsNaNF64UI(uiA) || IsNaNF64UI(uiB))
            {
                if (signaling || context.IsSignalingNaNFloat64Bits(uiA) || context.IsSignalingNaNFloat64Bits(uiB))
                    context.RaiseFlags(ExceptionFlags.Invalid);

                return false;
            }

            signA = SignF64UI(uiA);
            signB = SignF64UI(uiB);

            return (signA != signB)
                ? (signA && ((uiA | uiB) & 0x7FFFFFFFFFFFFFFF) != 0)
                : (uiA != uiB && (signA ^ (uiA < uiB)));
        }

        #endregion

        #endregion
    }

}