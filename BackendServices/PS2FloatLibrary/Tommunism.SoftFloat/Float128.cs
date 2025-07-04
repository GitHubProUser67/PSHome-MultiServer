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

    [StructLayout(LayoutKind.Sequential, Pack = sizeof(ulong), Size = sizeof(ulong) * 2)]
    public readonly struct Float128
    {
        #region Fields

        // WARNING: DO NOT ADD OR CHANGE ANY OF THESE FIELDS!!!
        private readonly ulong _v0;
        private readonly ulong _v64;

        #endregion

        #region Constructors

        private Float128(SFUInt128 v)
        {
            _v0 = v.V00;
            _v64 = v.V64;
        }

        private Float128(ulong v64, ulong v0)
        {
            _v0 = v0;
            _v64 = v64;
        }

        #endregion

        #region Methods

        public static Float128 FromUIntBits(ulong upper, ulong lower) => new(v64: upper, v0: lower);

        public (ulong hi, ulong lo) ToUInt64x2Bits() => (_v64, _v0);

#if NET7_0_OR_GREATER
    public static Float128 FromUIntBits(UInt128 value) => new(v64: value.GetUpperUI64(), v0: value.GetLowerUI64());

    public UInt128 ToUInt128Bits() => new(_v64, _v0);
#endif

        // THIS IS THE INTERNAL CONSTRUCTOR FOR RAW BITS.
        internal static Float128 FromBitsUI128(SFUInt128 v) => new(v);

        // THIS IS THE INTERNAL CONSTRUCTOR FOR RAW BITS.
        internal static Float128 FromBitsUI128(ulong v64, ulong v0) => new(v64, v0);

        #region Integer-to-floating-point Conversions

        // ui32_to_f128
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "API consistency and possible future use.")]
        public static Float128 FromUInt32(SoftFloatContext context, uint a)
        {
            if (a != 0)
            {
                var shiftDist = CountLeadingZeroes32(a) + 17;
                return PackToF128(false, 0x402E - shiftDist, (ulong)a << shiftDist, 0);
            }

            return FromBitsUI128(v64: 0, v0: 0);
        }

        // ui64_to_f128
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "API consistency and possible future use.")]
        public static Float128 FromUInt64(SoftFloatContext context, ulong a)
        {
            if (a != 0)
            {
                var shiftDist = CountLeadingZeroes64(a) + 49;
                var zSig = (64 <= shiftDist)
                    ? new SFUInt128(upper: a << (shiftDist - 64), lower: 0)
                    : ShortShiftLeft128(0, a, shiftDist);
                return PackToF128(false, 0x406E - shiftDist, zSig.V64, zSig.V00);
            }

            return FromBitsUI128(v64: 0, v0: 0);
        }

        // i32_to_f128
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "API consistency and possible future use.")]
        public static Float128 FromInt32(SoftFloatContext context, int a)
        {
            if (a != 0)
            {
                var sign = a < 0;
                var absA = (uint)(sign ? -a : a);
                var shiftDist = CountLeadingZeroes32(absA) + 17;
                return PackToF128(sign, 0x402E - shiftDist, (ulong)absA << shiftDist, 0);
            }

            return FromBitsUI128(v64: 0, v0: 0);
        }

        // i64_to_f128
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "API consistency and possible future use.")]
        public static Float128 FromInt64(SoftFloatContext context, long a)
        {
            if (a != 0)
            {
                var sign = a < 0;
                var absA = (ulong)(sign ? -a : a);
                var shiftDist = CountLeadingZeroes64(absA) + 49;
                var zSig = (64 <= shiftDist)
                    ? new SFUInt128(upper: absA << (shiftDist - 64), lower: 0)
                    : ShortShiftLeft128(0, absA, shiftDist);
                return PackToF128(sign, 0x406E - shiftDist, zSig.V64, zSig.V00);
            }

            return FromBitsUI128(v64: 0, v0: 0);
        }

        #endregion

        #region Floating-point-to-integer Conversions

        public uint ToUInt32(SoftFloatContext context, bool exact) => ToUInt32(context, context.Rounding, exact);

        public ulong ToUInt64(SoftFloatContext context, bool exact) => ToUInt64(context, context.Rounding, exact);

        public int ToInt32(SoftFloatContext context, bool exact) => ToInt32(context, context.Rounding, exact);

        public long ToInt64(SoftFloatContext context, bool exact) => ToInt64(context, context.Rounding, exact);

        // f128_to_ui32
        public uint ToUInt32(SoftFloatContext context, RoundingMode roundingMode, bool exact)
        {
            ulong uiA64, uiA0, sig64;
            int exp, shiftDist;
            bool sign;

            uiA64 = _v64;
            uiA0 = _v0;
            sign = SignF128UI64(uiA64);
            exp = ExpF128UI64(uiA64);
            sig64 = FracF128UI64(uiA64) | (uiA0 != 0 ? 1U : 0);

            if (exp == 0x7FFF && sig64 != 0)
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
                sig64 |= 0x0001000000000000;

            shiftDist = 0x4023 - exp;
            if (0 < shiftDist)
                sig64 = ShiftRightJam64(sig64, shiftDist);

            return RoundToUI32(context, sign, sig64, roundingMode, exact);
        }

        // f128_to_ui64
        public ulong ToUInt64(SoftFloatContext context, RoundingMode roundingMode, bool exact)
        {
            ulong uiA64, uiA0, sig64, sig0;
            int exp, shiftDist;
            bool sign;

            uiA64 = _v64;
            uiA0 = _v0;
            sign = SignF128UI64(uiA64);
            exp = ExpF128UI64(uiA64);
            sig64 = FracF128UI64(uiA64);
            sig0 = uiA0;

            shiftDist = 0x402F - exp;
            if (shiftDist <= 0)
            {
                if (shiftDist < -15)
                {
                    context.RaiseFlags(ExceptionFlags.Invalid);
                    return (exp == 0x7FFF && (sig64 | sig0) != 0)
                        ? context.UInt64FromNaN
                        : context.UInt64FromOverflow(sign);
                }

                sig64 |= 0x0001000000000000;
                if (shiftDist != 0)
                    (sig64, sig0) = ShortShiftLeft128(sig64, sig0, -shiftDist);
            }
            else
            {
                if (exp != 0)
                    sig64 |= 0x0001000000000000;

                (sig0, sig64) = ShiftRightJam64Extra(sig64, sig0, shiftDist);
            }

            return RoundToUI64(context, sign, sig64, sig0, roundingMode, exact);
        }

        // f128_to_i32
        public int ToInt32(SoftFloatContext context, RoundingMode roundingMode, bool exact)
        {
            ulong uiA64, uiA0, sig64, sig0;
            int exp, shiftDist;
            bool sign;

            uiA64 = _v64;
            uiA0 = _v0;
            sign = SignF128UI64(uiA64);
            exp = ExpF128UI64(uiA64);
            sig64 = FracF128UI64(uiA64);
            sig0 = uiA0;

            if (exp == 0x7FFF && (sig64 | sig0) != 0)
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
                sig64 |= 0x0001000000000000;

            sig64 |= sig0 != 0 ? 1U : 0;
            shiftDist = 0x4023 - exp;
            if (0 < shiftDist)
                sig64 = ShiftRightJam64(sig64, shiftDist);

            return RoundToI32(context, sign, sig64, roundingMode, exact);
        }

        // f128_to_i64
        public long ToInt64(SoftFloatContext context, RoundingMode roundingMode, bool exact)
        {
            ulong uiA64, uiA0, sig64, sig0;
            int exp, shiftDist;
            bool sign;

            uiA64 = _v64;
            uiA0 = _v0;
            sign = SignF128UI64(uiA64);
            exp = ExpF128UI64(uiA64);
            sig64 = FracF128UI64(uiA64);
            sig0 = uiA0;

            shiftDist = 0x402F - exp;
            if (shiftDist <= 0)
            {
                if (shiftDist < -15)
                {
                    context.RaiseFlags(ExceptionFlags.Invalid);
                    return (exp == 0x7FFF && (sig64 | sig0) != 0)
                        ? context.Int64FromNaN
                        : context.Int64FromOverflow(sign);
                }

                sig64 |= 0x0001000000000000;
                if (shiftDist != 0)
                    (sig64, sig0) = ShortShiftLeft128(sig64, sig0, -shiftDist);
            }
            else
            {
                if (exp != 0)
                    sig64 |= 0x0001000000000000;

                (sig0, sig64) = ShiftRightJam64Extra(sig64, sig0, shiftDist);
            }

            return RoundToI64(context, sign, sig64, sig0, roundingMode, exact);
        }

        // f128_to_ui32_r_minMag
        public uint ToUInt32RoundMinMag(SoftFloatContext context, bool exact)
        {
            ulong uiA64, uiA0, sig64;
            int exp, shiftDist;
            uint z;
            bool sign;

            uiA64 = _v64;
            uiA0 = _v0;
            exp = ExpF128UI64(uiA64);
            sig64 = FracF128UI64(uiA64) | (uiA0 != 0 ? 1U : 0);

            shiftDist = 0x402F - exp;
            if (49 <= shiftDist)
            {
                if (exact && ((uint)exp | sig64) != 0)
                    context.ExceptionFlags |= ExceptionFlags.Inexact;

                return 0;
            }

            sign = SignF128UI64(uiA64);
            if (sign || shiftDist < 17)
            {
                context.RaiseFlags(ExceptionFlags.Invalid);
                return (exp == 0x7FFF && sig64 != 0)
                    ? context.UInt32FromNaN
                    : context.UInt32FromOverflow(sign);
            }

            sig64 |= 0x0001000000000000;
            z = (uint)(sig64 >> shiftDist);
            if (exact && ((ulong)z << shiftDist) != sig64)
                context.ExceptionFlags |= ExceptionFlags.Inexact;

            return z;
        }

        // f128_to_ui64_r_minMag
        public ulong ToUInt64RoundMinMag(SoftFloatContext context, bool exact)
        {
            ulong uiA64, uiA0, sig64, sig0, z;
            int exp, shiftDist;
            int negShiftDist;
            bool sign;

            uiA64 = _v64;
            uiA0 = _v0;
            sign = SignF128UI64(uiA64);
            exp = ExpF128UI64(uiA64);
            sig64 = FracF128UI64(uiA64);
            sig0 = uiA0;

            shiftDist = 0x402F - exp;
            if (shiftDist < 0)
            {
                if (sign || shiftDist < -15)
                {
                    context.RaiseFlags(ExceptionFlags.Invalid);
                    return (exp == 0x7FFF && (sig64 | sig0) != 0)
                        ? context.UInt64FromNaN
                        : context.UInt64FromOverflow(sign);
                }

                sig64 |= 0x0001000000000000;
                negShiftDist = -shiftDist;
                z = (sig64 << negShiftDist) | (sig0 >> shiftDist);
                if (exact && (sig0 << negShiftDist) != 0)
                    context.ExceptionFlags |= ExceptionFlags.Inexact;
            }
            else
            {
                if (49 <= shiftDist)
                {
                    if (exact && ((uint)exp | sig64 | sig0) != 0)
                        context.ExceptionFlags |= ExceptionFlags.Inexact;

                    return 0;
                }

                if (sign)
                {
                    context.RaiseFlags(ExceptionFlags.Invalid);
                    return (exp == 0x7FFF && (sig64 | sig0) != 0)
                        ? context.UInt64FromNaN
                        : context.UInt64FromOverflow(sign);
                }

                sig64 |= 0x0001000000000000;
                z = sig64 >> shiftDist;
                if (exact && (sig0 != 0 || (z << shiftDist) != sig64))
                    context.ExceptionFlags |= ExceptionFlags.Inexact;
            }

            return z;
        }

        // f128_to_i32_r_minMag
        public int ToInt32RoundMinMag(SoftFloatContext context, bool exact)
        {
            ulong uiA64, uiA0, sig64;
            int exp, shiftDist, absZ;
            bool sign;

            uiA64 = _v64;
            uiA0 = _v0;
            exp = ExpF128UI64(uiA64);
            sig64 = FracF128UI64(uiA64) | (uiA0 != 0 ? 1U : 0);

            shiftDist = 0x402F - exp;
            if (49 <= shiftDist)
            {
                if (exact && ((uint)exp | sig64) != 0)
                    context.ExceptionFlags |= ExceptionFlags.Inexact;

                return 0;
            }

            sign = SignF128UI64(uiA64);
            if (shiftDist < 18)
            {
                if (sign && shiftDist == 17 && sig64 < 0x0000000000020000)
                {
                    if (exact && sig64 != 0)
                        context.ExceptionFlags |= ExceptionFlags.Inexact;

                    return -0x7FFFFFFF - 1;
                }

                context.RaiseFlags(ExceptionFlags.Invalid);
                return (exp == 0x7FFF && sig64 != 0)
                    ? context.Int32FromNaN
                    : context.Int32FromOverflow(sign);
            }

            sig64 |= 0x0001000000000000;
            absZ = (int)(uint)(sig64 >> shiftDist);
            if (exact && ((ulong)(uint)absZ << shiftDist) != sig64)
                context.ExceptionFlags |= ExceptionFlags.Inexact;

            return sign ? -absZ : absZ;
        }

        // f128_to_i64_r_minMag
        public long ToInt64RoundMinMag(SoftFloatContext context, bool exact)
        {
            ulong uiA64, uiA0, sig64, sig0;
            int exp, shiftDist;
            int negShiftDist;
            long absZ;
            bool sign;

            uiA64 = _v64;
            uiA0 = _v0;
            sign = SignF128UI64(uiA64);
            exp = ExpF128UI64(uiA64);
            sig64 = FracF128UI64(uiA64);
            sig0 = uiA0;

            shiftDist = 0x402F - exp;
            if (shiftDist < 0)
            {
                if (shiftDist < -14)
                {
                    if (uiA64 == 0xC03E000000000000 && sig0 < 0x0002000000000000)
                    {
                        if (exact && sig0 != 0)
                            context.ExceptionFlags |= ExceptionFlags.Inexact;

                        return -0x7FFFFFFFFFFFFFFF - 1;
                    }

                    context.RaiseFlags(ExceptionFlags.Invalid);
                    return (exp == 0x7FFF && (sig64 | sig0) != 0)
                        ? context.Int64FromNaN
                        : context.Int64FromOverflow(sign);
                }

                sig64 |= 0x0001000000000000;
                negShiftDist = -shiftDist;
                absZ = (long)((sig64 << negShiftDist) | (sig0 >> shiftDist));
                if (exact && (sig0 << negShiftDist) != 0)
                    context.ExceptionFlags |= ExceptionFlags.Inexact;
            }
            else
            {
                if (49 <= shiftDist)
                {
                    if (exact && ((uint)exp | sig64 | sig0) != 0)
                        context.ExceptionFlags |= ExceptionFlags.Inexact;

                    return 0;
                }

                sig64 |= 0x0001000000000000;
                absZ = (long)(sig64 >> shiftDist);
                if (exact && (sig0 != 0 || (ulong)(absZ << shiftDist) != sig64))
                    context.ExceptionFlags |= ExceptionFlags.Inexact;
            }

            return sign ? -absZ : absZ;
        }

        #endregion

        #region Floating-point-to-floating-point Conversions

        // f128_to_f16
        public Float16 ToFloat16(SoftFloatContext context)
        {
            ulong uiA64, uiA0, frac64;
            int exp;
            uint frac16;
            bool sign;

            uiA64 = _v64;
            uiA0 = _v0;
            sign = SignF128UI64(uiA64);
            exp = ExpF128UI64(uiA64);
            frac64 = FracF128UI64(uiA64) | (uiA0 != 0 ? 1U : 0);

            if (exp == 0x7FFF)
            {
                if (frac64 != 0)
                {
                    context.Float128BitsToCommonNaN(uiA64, uiA0, out var commonNaN);
                    return context.CommonNaNToFloat16(in commonNaN);
                }

                return PackToF16(sign, 0x1F, 0);
            }

            frac16 = (uint)ShortShiftRightJam64(frac64, 34);
            if (((uint)exp | frac16) == 0)
                return PackToF16(sign, 0, 0);

            exp -= 0x3FF1;
            if (sizeof(int) < sizeof(int) && exp < -0x40)
                exp = -0x40;

            return RoundPackToF16(context, sign, exp, frac16 | 0x4000);
        }

        // f128_to_f32
        public Float32 ToFloat32(SoftFloatContext context)
        {
            ulong uiA64, uiA0, frac64;
            int exp;
            uint frac32;
            bool sign;

            uiA64 = _v64;
            uiA0 = _v0;
            sign = SignF128UI64(uiA64);
            exp = ExpF128UI64(uiA64);
            frac64 = FracF128UI64(uiA64) | (uiA0 != 0 ? 1U : 0);

            if (exp == 0x7FFF)
            {
                if (frac64 != 0)
                {
                    context.Float128BitsToCommonNaN(uiA64, uiA0, out var commonNaN);
                    return context.CommonNaNToFloat32(in commonNaN);
                }

                return PackToF32(sign, 0xFF, 0);
            }

            frac32 = (uint)ShortShiftRightJam64(frac64, 18);
            if (((uint)exp | frac32) == 0)
                return PackToF32(sign, 0, 0);

            exp -= 0x3F81;
            if (sizeof(int) < sizeof(int) && exp < -0x1000)
                exp = -0x1000;

            return RoundPackToF32(context, sign, exp, frac32 | 0x40000000);
        }

        // f128_to_f64
        public Float64 ToFloat64(SoftFloatContext context)
        {
            ulong uiA64, uiA0, frac64, frac0;
            int exp;
            SFUInt128 frac128;
            bool sign;

            uiA64 = _v64;
            uiA0 = _v0;
            sign = SignF128UI64(uiA64);
            exp = ExpF128UI64(uiA64);
            frac64 = FracF128UI64(uiA64);
            frac0 = uiA0;

            if (exp == 0x7FFF)
            {
                if ((frac64 | frac0) != 0)
                {
                    context.Float128BitsToCommonNaN(uiA64, uiA0, out var commonNaN);
                    return context.CommonNaNToFloat64(in commonNaN);
                }

                return PackToF64(sign, 0x7FF, 0);
            }

            frac128 = ShortShiftLeft128(frac64, frac0, 14);
            frac64 = frac128.V64 | (frac128.V00 != 0 ? 1U : 0);
            if (((uint)exp | frac64) == 0)
                return PackToF64(sign, 0, 0);

            exp -= 0x3C01;
            if (sizeof(int) < sizeof(int) && exp < -0x1000)
                exp = -0x1000;

            return RoundPackToF64(context, sign, exp, frac64 | 0x4000000000000000);
        }

        // f128_to_extF80
        public ExtFloat80 ToExtFloat80(SoftFloatContext context)
        {
            ulong uiA64, uiA0, frac64, frac0;
            int exp;
            SFUInt128 sig128;
            bool sign;

            uiA64 = _v64;
            uiA0 = _v0;
            sign = SignF128UI64(uiA64);
            exp = ExpF128UI64(uiA64);
            frac64 = FracF128UI64(uiA64);
            frac0 = uiA0;

            if (exp == 0x7FFF)
            {
                if ((frac64 | frac0) != 0)
                {
                    context.Float128BitsToCommonNaN(uiA64, uiA0, out var commonNaN);
                    return context.CommonNaNToExtFloat80(in commonNaN);
                }

                return PackToExtF80(sign, 0x7FFF, 0x8000000000000000);
            }

            if (exp == 0)
            {
                if ((frac64 | frac0) == 0)
                    return PackToExtF80(sign, 0, 0);

                (exp, (frac64, frac0)) = NormSubnormalF128Sig(frac64, frac0);
            }

            sig128 = ShortShiftLeft128(frac64 | 0x0001000000000000, frac0, 15);
            return RoundPackToExtF80(context, sign, exp, sig128.V64, sig128.V00, ExtFloat80RoundingPrecision._80);
        }

        #endregion

        #region Arithmetic Operations

        public Float128 RoundToInt(SoftFloatContext context, bool exact) => RoundToInt(context, context.Rounding, exact);

        // f128_roundToInt
        public Float128 RoundToInt(SoftFloatContext context, RoundingMode roundingMode, bool exact)
        {
            ulong uiA64, uiA0, lastBitMask0, roundBitsMask, lastBitMask64;
            int exp;
            SFUInt128 uiZ;
            bool roundNearEven;

            uiA64 = _v64;
            uiA0 = _v0;
            exp = ExpF128UI64(uiA64);

            if (0x402F <= exp)
            {
                if (0x406F <= exp)
                {
                    if (exp == 0x7FFF && (FracF128UI64(uiA64) | uiA0) != 0)
                        return context.PropagateNaNFloat128Bits(uiA64, uiA0, 0, 0);

                    return this;
                }

                lastBitMask0 = (ulong)2 << (0x406E - exp);
                roundBitsMask = lastBitMask0 - 1;
                uiZ = new SFUInt128(upper: uiA64, lower: uiA0);
                roundNearEven = roundingMode == RoundingMode.NearEven;
                if (roundNearEven || roundingMode == RoundingMode.NearMaxMag)
                {
                    if (exp == 0x402F)
                    {
                        if (0x8000000000000000 <= uiZ.V00)
                        {
                            ++uiZ.V64;
                            if (roundNearEven && (uiZ.V00 == 0x8000000000000000))
                                uiZ.V64 &= ~1U;
                        }
                    }
                    else
                    {
                        uiZ = Add128(uiZ.V64, uiZ.V00, 0, lastBitMask0 >> 1);
                        if (roundNearEven && (uiZ.V00 & roundBitsMask) == 0)
                            uiZ.V00 &= ~lastBitMask0;
                    }
                }
                else if (roundingMode == (SignF128UI64(uiZ.V64) ? RoundingMode.Min : RoundingMode.Max))
                {
                    uiZ = Add128(uiZ.V64, uiZ.V00, 0, roundBitsMask);
                }

                uiZ.V00 &= ~roundBitsMask;
                lastBitMask64 = lastBitMask0 == 0 ? 1U : 0;
            }
            else
            {
                if (exp < 0x3FFF)
                {
                    if (((uiA64 & 0x7FFFFFFFFFFFFFFF) | uiA0) == 0)
                        return this;

                    if (exact)
                        context.ExceptionFlags |= ExceptionFlags.Inexact;

                    uiZ = new SFUInt128(upper: uiA64 & PackToF128UI64(true, 0, 0), lower: 0);
                    switch (roundingMode)
                    {
                        case RoundingMode.NearEven:
                            {
                                if ((FracF128UI64(uiA64) | uiA0) == 0)
                                    break;

                                goto case RoundingMode.NearMaxMag;
                            }
                        case RoundingMode.NearMaxMag:
                            {
                                if (exp == 0x3FFE)
                                    uiZ.V64 |= PackToF128UI64(false, 0x3FFF, 0);

                                break;
                            }
                        case RoundingMode.Min:
                            {
                                if (uiZ.V64 != 0)
                                    uiZ.V64 = PackToF128UI64(true, 0x3FFF, 0);

                                break;
                            }
                        case RoundingMode.Max:
                            {
                                if (uiZ.V64 == 0)
                                    uiZ.V64 = PackToF128UI64(false, 0x3FFF, 0);

                                break;
                            }
                        case RoundingMode.Odd:
                            {
                                uiZ.V64 |= PackToF128UI64(false, 0x3FFF, 0);
                                break;
                            }
                    }

                    return Float128.FromBitsUI128(uiZ);
                }

                uiZ = new SFUInt128(upper: uiA64, lower: 0);
                lastBitMask64 = (ulong)1 << (0x402F - exp);
                roundBitsMask = lastBitMask64 - 1;
                if (roundingMode == RoundingMode.NearMaxMag)
                {
                    uiZ.V64 += lastBitMask64 >> 1;
                }
                else if (roundingMode == RoundingMode.NearEven)
                {
                    uiZ.V64 += lastBitMask64 >> 1;
                    if (((uiZ.V64 & roundBitsMask) | uiA0) == 0)
                        uiZ.V64 &= ~lastBitMask64;
                }
                else if (roundingMode == (SignF128UI64(uiZ.V64) ? RoundingMode.Min : RoundingMode.Max))
                {
                    uiZ.V64 = (uiZ.V64 | (uiA0 != 0 ? 1U : 0)) + roundBitsMask;
                }

                uiZ.V64 &= ~roundBitsMask;
                lastBitMask0 = 0;
            }

            if (uiZ.V64 != uiA64 || uiZ.V00 != uiA0)
            {
                if (roundingMode == RoundingMode.Odd)
                    uiZ = new SFUInt128(upper: uiZ.V64 | lastBitMask64, lower: uiZ.V00 | lastBitMask0);

                if (exact)
                    context.ExceptionFlags |= ExceptionFlags.Inexact;
            }

            return Float128.FromBitsUI128(uiZ);
        }

        // f128_add
        public static Float128 Add(SoftFloatContext context, Float128 a, Float128 b)
        {
            var signA = SignF128UI64(a._v64);
            var signB = SignF128UI64(b._v64);

            return (signA == signB)
                ? AddMagsF128(context, a._v64, a._v0, b._v64, b._v0, signA)
                : SubMagsF128(context, a._v64, a._v0, b._v64, b._v0, signA);
        }

        // f128_sub
        public static Float128 Subtract(SoftFloatContext context, Float128 a, Float128 b)
        {
            var signA = SignF128UI64(a._v64);
            var signB = SignF128UI64(b._v64);

            return (signA == signB)
                ? SubMagsF128(context, a._v64, a._v0, b._v64, b._v0, signA)
                : AddMagsF128(context, a._v64, a._v0, b._v64, b._v0, signA);
        }

        // f128_mul
        public static Float128 Multiply(SoftFloatContext context, Float128 a, Float128 b)
        {
            ulong uiA64, uiA0, uiB64, uiB0, magBits, sigZExtra;
            int expA, expB, expZ;
            SFUInt128 sigA, sigB, sigZ;
            SFUInt256 sig256Z;
            bool signA, signB, signZ;

            uiA64 = a._v64;
            uiA0 = a._v0;
            signA = SignF128UI64(uiA64);
            expA = ExpF128UI64(uiA64);
            sigA = new SFUInt128(upper: FracF128UI64(uiA64), lower: uiA0);
            uiB64 = b._v64;
            uiB0 = b._v0;
            signB = SignF128UI64(uiB64);
            expB = ExpF128UI64(uiB64);
            sigB = new SFUInt128(upper: FracF128UI64(uiB64), lower: uiB0);
            signZ = signA ^ signB;

            if (expA == 0x7FFF)
            {
                if (!sigA.IsZero || (expB == 0x7FFF && !sigB.IsZero))
                    return context.PropagateNaNFloat128Bits(uiA64, uiA0, uiB64, uiB0);

                magBits = (uint)expB | sigB.V64 | sigB.V00;
                if (magBits == 0)
                {
                    context.RaiseFlags(ExceptionFlags.Invalid);
                    return context.DefaultNaNFloat128;
                }

                return PackToF128(signZ, 0x7FFF, 0, 0);
            }

            if (expB == 0x7FFF)
            {
                if (!sigB.IsZero)
                    return context.PropagateNaNFloat128Bits(uiA64, uiA0, uiB64, uiB0);

                magBits = (uint)expA | sigA.V64 | sigA.V00;
                if (magBits == 0)
                {
                    context.RaiseFlags(ExceptionFlags.Invalid);
                    return context.DefaultNaNFloat128;
                }

                return PackToF128(signZ, 0x7FFF, 0, 0);
            }

            if (expA == 0)
            {
                if (sigA.IsZero)
                    return PackToF128(signZ, 0, 0, 0);

                (expA, sigA) = NormSubnormalF128Sig(sigA);
            }

            if (expB == 0)
            {
                if (sigB.IsZero)
                    return PackToF128(signZ, 0, 0, 0);

                (expB, sigB) = NormSubnormalF128Sig(sigB);
            }

            expZ = expA + expB - 0x4000;
            sigA.V64 |= 0x0001000000000000;
            sigB <<= 16;
            sig256Z = Mul128To256M(sigA, sigB);
            sigZExtra = sig256Z[IndexWord(4, 1)] | (sig256Z[IndexWord(4, 0)] != 0 ? 1U : 0);
            sigZ = Add128(
                sig256Z[IndexWord(4, 3)], sig256Z[IndexWord(4, 2)],
                sigA.V64, sigA.V00
            );

            if (0x0002000000000000 <= sigZ.V64)
            {
                ++expZ;
                (sigZExtra, sigZ) = ShortShiftRightJam128Extra(sigZ, sigZExtra, 1);
            }

            return RoundPackToF128(context, signZ, expZ, sigZ, sigZExtra);
        }

        // f128_mulAdd
        public static Float128 MultiplyAndAdd(SoftFloatContext context, Float128 a, Float128 b, Float128 c) =>
            MulAddF128(context, a._v64, a._v0, b._v64, b._v0, c._v64, c._v0, MulAddOperation.None);

        // f128_div
        public static Float128 Divide(SoftFloatContext context, Float128 a, Float128 b)
        {
            Span<uint> qs = stackalloc uint[3];
            ulong uiA64, uiA0, uiB64, uiB0, q64, sigZExtra;
            int expA, expB, expZ;
            SFUInt128 sigA, sigB, rem, term, sigZ;
            uint recip32, q;
            bool signA, signB, signZ;

            uiA64 = a._v64;
            uiA0 = a._v0;
            signA = SignF128UI64(uiA64);
            expA = ExpF128UI64(uiA64);
            sigA = new SFUInt128(upper: FracF128UI64(uiA64), lower: uiA0);
            uiB64 = b._v64;
            uiB0 = b._v0;
            signB = SignF128UI64(uiB64);
            expB = ExpF128UI64(uiB64);
            sigB = new SFUInt128(upper: FracF128UI64(uiB64), lower: uiB0);
            signZ = signA ^ signB;

            if (expA == 0x7FFF)
            {
                if (!sigA.IsZero)
                    return context.PropagateNaNFloat128Bits(uiA64, uiA0, uiB64, uiB0);

                if (expB == 0x7FFF)
                {
                    if (!sigB.IsZero)
                        return context.PropagateNaNFloat128Bits(uiA64, uiA0, uiB64, uiB0);

                    context.RaiseFlags(ExceptionFlags.Invalid);
                    return context.DefaultNaNFloat128;
                }

                return PackToF128(signZ, 0x7FFF, 0, 0);
            }

            if (expB == 0x7FFF)
            {
                if (!sigB.IsZero)
                    return context.PropagateNaNFloat128Bits(uiA64, uiA0, uiB64, uiB0);

                return PackToF128(signZ, 0, 0, 0);
            }

            if (expB == 0)
            {
                if (sigB.IsZero)
                {
                    if (((uint)expA | sigA.V64 | sigA.V00) == 0)
                    {
                        context.RaiseFlags(ExceptionFlags.Invalid);
                        return context.DefaultNaNFloat128;
                    }

                    context.RaiseFlags(ExceptionFlags.Infinite);
                    return PackToF128(signZ, 0x7FFF, 0, 0);
                }

                (expB, sigB) = NormSubnormalF128Sig(sigB);
            }

            if (expA == 0)
            {
                if (sigA.IsZero)
                    return PackToF128(signZ, 0, 0, 0);

                (expA, sigA) = NormSubnormalF128Sig(sigA);
            }

            expZ = expA - expB + 0x3FFE;
            sigA.V64 |= 0x0001000000000000;
            sigB.V64 |= 0x0001000000000000;
            rem = sigA;
            if (sigA < sigB)
            {
                --expZ;
                rem = sigA + sigA;
            }

            recip32 = ApproxRecip32_1((uint)(sigB.V64 >> 17));
            for (var ix = 3; ;)
            {
                q64 = (ulong)(uint)(rem.V64 >> 19) * recip32;
                q = (uint)((q64 + 0x80000000) >> 32);
                if (--ix < 0)
                    break;

                rem <<= 29;
                term = sigB * q;
                rem -= term;
                if ((rem.V64 & 0x8000000000000000) != 0)
                {
                    --q;
                    rem += sigB;
                }

                qs[ix] = q;
            }

            if (((q + 1) & 7) < 2)
            {
                rem <<= 29;
                term = sigB * q;
                rem -= term;
                if ((rem.V64 & 0x8000000000000000) != 0)
                {
                    --q;
                    rem += sigB;
                }
                else if (sigB <= rem)
                {
                    ++q;
                    rem -= sigB;
                }

                if (!rem.IsZero)
                    q |= 1;
            }

            sigZExtra = (ulong)q << 60;
            term = ShortShiftLeft128(0, qs[1], 54);
            sigZ = Add128(
                (ulong)qs[2] << 19, ((ulong)qs[0] << 25) + (q >> 4),
                term.V64, term.V00
            );

            return RoundPackToF128(context, signZ, expZ, sigZ, sigZExtra);
        }

        // f128_rem
        public static Float128 Modulus(SoftFloatContext context, Float128 a, Float128 b)
        {
            ulong uiA64, uiA0, uiB64, uiB0, q64;
            int expA, expB, expDiff;
            SFUInt128 sigA, sigB, rem, term, altRem, meanRem;
            uint q, recip32;
            bool signA, signRem;

            uiA64 = a._v64;
            uiA0 = a._v0;
            signA = SignF128UI64(uiA64);
            expA = ExpF128UI64(uiA64);
            sigA = new SFUInt128(upper: FracF128UI64(uiA64), lower: uiA0);
            uiB64 = b._v64;
            uiB0 = b._v0;
            expB = ExpF128UI64(uiB64);
            sigB = new SFUInt128(upper: FracF128UI64(uiB64), lower: uiB0);

            if (expA == 0x7FFF)
            {
                if (!sigA.IsZero || (expB == 0x7FFF && !sigB.IsZero))
                    return context.PropagateNaNFloat128Bits(uiA64, uiA0, uiB64, uiB0);

                context.RaiseFlags(ExceptionFlags.Invalid);
                return context.DefaultNaNFloat128;
            }

            if (expB == 0x7FFF)
            {
                if (!sigB.IsZero)
                    return context.PropagateNaNFloat128Bits(uiA64, uiA0, uiB64, uiB0);

                return a;
            }

            if (expB == 0)
            {
                if (sigB.IsZero)
                {
                    context.RaiseFlags(ExceptionFlags.Invalid);
                    return context.DefaultNaNFloat128;
                }

                (expB, sigB) = NormSubnormalF128Sig(sigB);
            }

            if (expA == 0)
            {
                if (sigA.IsZero)
                    return a;

                (expA, sigA) = NormSubnormalF128Sig(sigA);
            }

            sigA.V64 |= 0x0001000000000000;
            sigB.V64 |= 0x0001000000000000;
            rem = sigA;
            expDiff = expA - expB;
            if (expDiff < 1)
            {
                if (expDiff < -1)
                    return a;

                if (expDiff != 0)
                {
                    --expB;
                    sigB += sigB;
                    q = 0;
                }
                else
                {
                    q = sigB <= rem ? 1U : 0;
                    if (q != 0)
                        rem -= sigB;
                }
            }
            else
            {
                recip32 = ApproxRecip32_1((uint)(sigB.V64 >> 17));
                expDiff -= 30;
                while (true)
                {
                    q64 = (ulong)(uint)(rem.V64 >> 19) * recip32;
                    if (expDiff < 0)
                        break;

                    q = (uint)((q64 + 0x80000000) >> 32);
                    rem <<= 29;
                    term = sigB * q;
                    rem -= term;
                    if ((rem.V64 & 0x8000000000000000) != 0)
                        rem += sigB;

                    expDiff -= 29;
                }

                // ('expDiff' cannot be less than -29 here.)
                q = (uint)(q64 >> 32) >> (~expDiff & 31);
                rem <<= expDiff + 30;
                term = sigB * q;
                rem -= term;
                if ((rem.V64 & 0x8000000000000000) != 0)
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
            while ((rem.V64 & 0x8000000000000000) == 0);

        selectRem:
            meanRem = rem + altRem;
            if ((meanRem.V64 & 0x8000000000000000) != 0 || (meanRem.IsZero && (q & 1) != 0))
                rem = altRem;

            signRem = signA;
            if ((rem.V64 & 0x8000000000000000) != 0)
            {
                signRem = !signRem;
                rem = -rem;
            }

            return NormRoundPackToF128(context, signRem, expB - 1, rem);
        }

        // f128_sqrt
        public Float128 SquareRoot(SoftFloatContext context)
        {
            Span<uint> qs = stackalloc uint[3];
            ulong uiA64, uiA0, x64, sig64Z, sigZExtra;
            int expA, expZ;
            SFUInt128 sigA, rem, y, term, sigZ;
            uint sig32A, recipSqrt32, sig32Z, q;
            bool signA;

            uiA64 = _v64;
            uiA0 = _v0;
            signA = SignF128UI64(uiA64);
            expA = ExpF128UI64(uiA64);
            sigA = new SFUInt128(upper: FracF128UI64(uiA64), lower: uiA0);

            if (expA == 0x7FFF)
            {
                if (!sigA.IsZero)
                    return context.PropagateNaNFloat128Bits(uiA64, uiA0, 0, 0);

                if (!signA)
                    return this;

                context.RaiseFlags(ExceptionFlags.Invalid);
                return context.DefaultNaNFloat128;
            }

            if (signA)
            {
                if (((uint)expA | sigA.V64 | sigA.V00) == 0)
                    return this;

                context.RaiseFlags(ExceptionFlags.Invalid);
                return context.DefaultNaNFloat128;
            }

            if (expA == 0)
            {
                if (sigA.IsZero)
                    return this;

                (expA, sigA) = NormSubnormalF128Sig(sigA);
            }

            // ('sig32Z' is guaranteed to be a lower bound on the square root of 'sig32A', which makes 'sig32Z' also a lower bound on the
            // square root of 'sigA'.)
            expZ = ((expA - 0x3FFF) >> 1) + 0x3FFE;
            expA &= 1;
            sigA.V64 |= 0x0001000000000000;
            sig32A = (uint)(sigA.V64 >> 17);
            recipSqrt32 = ApproxRecipSqrt32_1((uint)expA, sig32A);
            sig32Z = (uint)(((ulong)sig32A * recipSqrt32) >> 32);
            if (expA != 0)
            {
                sig32Z >>= 1;
                rem = sigA << 12;
            }
            else
            {
                rem = sigA << 13;
            }

            qs[2] = sig32Z;
            rem.V64 -= (ulong)sig32Z * sig32Z;

            q = (uint)(((uint)(rem.V64 >> 2) * (ulong)recipSqrt32) >> 32);
            x64 = (ulong)sig32Z << 32;
            sig64Z = x64 + ((ulong)q << 3);
            y = rem << 29;

            // (Repeating this loop is a rare occurrence.)
            while (true)
            {
                term = Mul64ByShifted32To128(x64 + sig64Z, q);
                rem = y - term;
                if ((rem.V64 & 0x8000000000000000) == 0)
                    break;

                --q;
                sig64Z -= 1U << 3;
            }

            qs[1] = q;

            q = (uint)(((rem.V64 >> 2) * recipSqrt32) >> 32);
            y = rem << 29;
            sig64Z <<= 1;

            // (Repeating this loop is a rare occurrence.)
            while (true)
            {
                term = (SFUInt128)sig64Z << 32;
                term += (SFUInt128)((ulong)q << 6);
                term *= q;
                rem = y - term;
                if ((rem.V64 & 0x8000000000000000) == 0)
                    break;

                --q;
            }

            qs[0] = q;

            q = (uint)((((rem.V64 >> 2) * recipSqrt32) >> 32) + 2);
            sigZExtra = (ulong)q << 59;
            term = (SFUInt128)qs[1] << 53;
            sigZ = new SFUInt128(upper: (ulong)qs[2] << 18, lower: ((ulong)qs[0] << 24) + (q >> 5)) + term;

            if ((q & 0xF) <= 2)
            {
                q &= ~3U;
                sigZExtra = (ulong)q << 59;
                y = sigZ << 6;
                y.V00 |= sigZExtra >> 58;
                term = y - q;
                y = Mul64ByShifted32To128(term.V00, q);
                term = Mul64ByShifted32To128(term.V64, q);
                term += y.V64;
                rem <<= 20;
                term -= rem;

                // The concatenation of `term' and `y.v0' is now the negative remainder (3 words altogether).
                if ((term.V64 & 0x8000000000000000) != 0)
                {
                    sigZExtra |= 1;
                }
                else if ((term.V64 | term.V00 | y.V00) != 0)
                {
                    if (sigZExtra != 0)
                    {
                        --sigZExtra;
                    }
                    else
                    {
                        sigZ -= 1;
                        sigZExtra = ~0UL;
                    }
                }
            }

            return RoundPackToF128(context, false, expZ, sigZ, sigZExtra);
        }

        #endregion

        #region Comparison Operations

        // f128_eq (signaling=false) & f128_eq_signaling (signaling=true)
        public static bool CompareEqual(SoftFloatContext context, Float128 a, Float128 b, bool signaling)
        {
            ulong uiA64, uiA0, uiB64, uiB0;

            uiA64 = a._v64;
            uiA0 = a._v0;
            uiB64 = b._v64;
            uiB0 = b._v0;

            if (IsNaNF128UI(uiA64, uiA0) || IsNaNF128UI(uiB64, uiB0))
            {
                if (signaling || context.IsSignalingNaNFloat128Bits(uiA64, uiA0) || context.IsSignalingNaNFloat128Bits(uiB64, uiB0))
                    context.RaiseFlags(ExceptionFlags.Invalid);

                return false;
            }

            return uiA0 == uiB0 && (uiA64 == uiB64 || (uiA0 == 0 && ((uiA64 | uiB64) & 0x7FFFFFFFFFFFFFFF) == 0));
        }

        // f128_le (signaling=true) & f128_le_quiet (signaling=false)
        public static bool CompareLessThanOrEqual(SoftFloatContext context, Float128 a, Float128 b, bool signaling)
        {
            ulong uiA64, uiA0, uiB64, uiB0;
            bool signA, signB;

            uiA64 = a._v64;
            uiA0 = a._v0;
            uiB64 = b._v64;
            uiB0 = b._v0;

            if (IsNaNF128UI(uiA64, uiA0) || IsNaNF128UI(uiB64, uiB0))
            {
                if (signaling || context.IsSignalingNaNFloat128Bits(uiA64, uiA0) || context.IsSignalingNaNFloat128Bits(uiB64, uiB0))
                    context.RaiseFlags(ExceptionFlags.Invalid);

                return false;
            }

            signA = SignF128UI64(uiA64);
            signB = SignF128UI64(uiB64);

            return (signA != signB)
                ? (signA || (((uiA64 | uiB64) & 0x7FFFFFFFFFFFFFFF) | uiA0 | uiB0) == 0)
                : (uiA64 == uiB64 && uiA0 == uiB0) || (signA ^ LT128(uiA64, uiA0, uiB64, uiB0));
        }

        // f128_lt (signaling=true) & f128_lt_quiet (signaling=false)
        public static bool CompareLessThan(SoftFloatContext context, Float128 a, Float128 b, bool signaling)
        {
            ulong uiA64, uiA0, uiB64, uiB0;
            bool signA, signB;

            uiA64 = a._v64;
            uiA0 = a._v0;
            uiB64 = b._v64;
            uiB0 = b._v0;

            if (IsNaNF128UI(uiA64, uiA0) || IsNaNF128UI(uiB64, uiB0))
            {
                if (signaling || context.IsSignalingNaNFloat128Bits(uiA64, uiA0) || context.IsSignalingNaNFloat128Bits(uiB64, uiB0))
                    context.RaiseFlags(ExceptionFlags.Invalid);

                return false;
            }

            signA = SignF128UI64(uiA64);
            signB = SignF128UI64(uiB64);

            return (signA != signB)
                ? (signA && (((uiA64 | uiB64) & 0x7FFFFFFFFFFFFFFF) | uiA0 | uiB0) != 0)
                : ((uiA64 != uiB64 || uiA0 != uiB0) && (signA ^ LT128(uiA64, uiA0, uiB64, uiB0)));
        }

        #endregion

        #endregion
    }
}
