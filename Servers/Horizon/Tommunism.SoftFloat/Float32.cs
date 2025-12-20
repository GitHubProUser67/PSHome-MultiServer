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

    [StructLayout(LayoutKind.Sequential, Pack = sizeof(uint), Size = sizeof(uint))]
    public readonly struct Float32
    {
        #region Fields

        // WARNING: DO NOT ADD OR CHANGE ANY OF THESE FIELDS!!!
        private readonly uint _v;

        private const int MantissaBits = 23;

        #endregion

        #region Constructors

        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Used to avoid accidentally calling other overloads.")]
        private Float32(uint v, bool dummy)
        {
            _v = v;
        }

        public Float32(float value)
        {
            _v = BitConverter.SingleToUInt32Bits(value);
        }

        #endregion

        #region Methods

        public static explicit operator Float32(float value) => new(value);
        public static implicit operator float(Float32 value) => BitConverter.UInt32BitsToSingle(value._v);

        public static Float32 FromUIntBits(uint value) => FromBitsUI32(value);

        public uint ToUInt32Bits() => _v;

        public uint RawMantissa => _v & 0x7FFFFF;
        public int Mantissa
        {
            get
            {
                if (((byte)(_v >> MantissaBits)) != 0)
                {
                    uint sign = (uint)((int)_v >> 31);
                    return (int)(((RawMantissa | 0x800000) ^ sign) - sign);
                }
                else
                {
                    uint sign = (uint)((int)_v >> 31);
                    return (int)(((RawMantissa) ^ sign) - sign);
                }
            }
        }

        // THIS IS THE INTERNAL CONSTRUCTOR FOR RAW BITS.
        internal static Float32 FromBitsUI32(uint v) => new(v, dummy: false);

        #region Integer-to-floating-point Conversions

        // ui32_to_f32
        public static Float32 FromUInt32(SoftFloatContext context, uint a)
        {
            if (a == 0)
                return FromBitsUI32(0);

            return (a & 0x80000000) != 0
                ? RoundPackToF32(context, false, 0x9D, (a >> 1) | (a & 1))
                : NormRoundPackToF32(context, false, 0x9C, a);
        }

        // ui64_to_f32
        public static Float32 FromUInt64(SoftFloatContext context, ulong a)
        {
            var shiftDist = CountLeadingZeroes64(a) - 40;
            if (0 <= shiftDist)
                return FromBitsUI32(a != 0 ? PackToF32UI(false, 0x95 - shiftDist, (uint)a << shiftDist) : 0U);

            shiftDist += 7;
            var sig = (shiftDist < 0)
                ? (uint)ShortShiftRightJam64(a, -shiftDist)
                : ((uint)a << shiftDist);
            return RoundPackToF32(context, false, 0x9C - shiftDist, sig);
        }

        // i32_to_f32
        public static Float32 FromInt32(SoftFloatContext context, int a)
        {
            var sign = a < 0;
            if ((a & 0x7FFFFFFF) == 0)
                return FromBitsUI32(sign ? PackToF32UI(true, 0x9E, 0U) : 0U);

            var absA = (uint)(sign ? -a : a);
            return NormRoundPackToF32(context, sign, 0x9C, absA);
        }

        // i64_to_f32
        public static Float32 FromInt64(SoftFloatContext context, long a)
        {
            var sign = a < 0;
            var absA = (ulong)(sign ? -a : a);
            var shiftDist = CountLeadingZeroes64(absA) - 40;
            if (0 <= shiftDist)
                return FromBitsUI32(a != 0 ? PackToF32UI(sign, 0x95 - shiftDist, (uint)absA << shiftDist) : 0U);

            shiftDist += 7;
            var sig = (shiftDist < 0)
                ? (uint)ShortShiftRightJam64(absA, -shiftDist)
                : ((uint)absA << shiftDist);
            return RoundPackToF32(context, sign, 0x9C - shiftDist, sig);
        }

        #endregion

        #region Floating-point-to-integer Conversions

        public uint ToUInt32(SoftFloatContext context, bool exact) => ToUInt32(context, context.Rounding, exact);

        public ulong ToUInt64(SoftFloatContext context, bool exact) => ToUInt64(context, context.Rounding, exact);

        public int ToInt32(SoftFloatContext context, bool exact) => ToInt32(context, context.Rounding, exact);

        public long ToInt64(SoftFloatContext context, bool exact) => ToInt64(context, context.Rounding, exact);

        // f32_to_ui32
        public uint ToUInt32(SoftFloatContext context, RoundingMode roundingMode, bool exact)
        {
            uint sig;
            int exp, shiftDist;
            ulong sig64;
            bool sign;

            sign = SignF32UI(_v);
            exp = ExpF32UI(_v);
            sig = FracF32UI(_v);

            if (exp == 0xFF && sig != 0)
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
                sig |= 0x00800000;

            sig64 = (ulong)sig << 32;
            shiftDist = 0xAA - exp;
            if (0 < shiftDist)
                sig64 = ShiftRightJam64(sig64, shiftDist);

            return RoundToUI32(context, sign, sig64, roundingMode, exact);
        }

        // f32_to_ui64
        public ulong ToUInt64(SoftFloatContext context, RoundingMode roundingMode, bool exact)
        {
            int exp, shiftDist;
            uint sig;
            ulong sig64, extra;
            bool sign;

            sign = SignF32UI(_v);
            exp = ExpF32UI(_v);
            sig = FracF32UI(_v);

            shiftDist = 0xBE - exp;
            if (shiftDist < 0)
            {
                context.RaiseFlags(ExceptionFlags.Invalid);
                return (exp == 0xFF && sig != 0)
                    ? context.UInt64FromNaN
                    : context.UInt64FromOverflow(sign);
            }

            if (exp != 0)
                sig |= 0x00800000;

            sig64 = (ulong)sig << 40;
            extra = 0;
            if (shiftDist != 0)
                (extra, sig64) = ShiftRightJam64Extra(sig64, 0, shiftDist);

            return RoundToUI64(context, sign, sig64, extra, roundingMode, exact);
        }

        // f32_to_i32
        public int ToInt32(SoftFloatContext context, RoundingMode roundingMode, bool exact)
        {
            uint sig;
            int exp, shiftDist;
            ulong sig64;
            bool sign;

            sign = SignF32UI(_v);
            exp = ExpF32UI(_v);
            sig = FracF32UI(_v);

            if (exp == 0xFF && sig != 0)
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
                sig |= 0x00800000;

            sig64 = (ulong)sig << 32;
            shiftDist = 0xAA - exp;
            if (0 < shiftDist)
                sig64 = ShiftRightJam64(sig64, shiftDist);

            return RoundToI32(context, sign, sig64, roundingMode, exact);
        }

        // f32_to_i64
        public long ToInt64(SoftFloatContext context, RoundingMode roundingMode, bool exact)
        {
            uint sig;
            int exp, shiftDist;
            ulong sig64, extra;
            bool sign;

            sign = SignF32UI(_v);
            exp = ExpF32UI(_v);
            sig = FracF32UI(_v);

            shiftDist = 0xBE - exp;
            if (shiftDist < 0)
            {
                context.RaiseFlags(ExceptionFlags.Invalid);
                return (exp == 0xFF && sig != 0)
                    ? context.Int64FromNaN
                    : context.Int64FromOverflow(sign);
            }

            if (exp != 0)
                sig |= 0x00800000;

            sig64 = (ulong)sig << 40;
            extra = 0;
            if (shiftDist != 0)
                (extra, sig64) = ShiftRightJam64Extra(sig64, 0, shiftDist);

            return RoundToI64(context, sign, sig64, extra, roundingMode, exact);
        }

        // f32_to_ui32_r_minMag
        public uint ToUInt32RoundMinMag(SoftFloatContext context, bool exact)
        {
            uint sig, z;
            int exp, shiftDist;
            bool sign;

            exp = ExpF32UI(_v);
            sig = FracF32UI(_v);

            shiftDist = 0x9E - exp;
            if (32 <= shiftDist)
            {
                if (exact && ((uint)exp | sig) != 0)
                    context.ExceptionFlags |= ExceptionFlags.Inexact;

                return 0;
            }

            sign = SignF32UI(_v);
            if (sign || shiftDist < 0)
            {
                context.RaiseFlags(ExceptionFlags.Invalid);
                return (exp == 0xFF && sig != 0)
                    ? context.UInt32FromNaN
                    : context.UInt32FromOverflow(sign);
            }

            sig = (sig | 0x00800000) << 8;
            z = sig >> shiftDist;
            if (exact && (z << shiftDist) != sig)
                context.ExceptionFlags |= ExceptionFlags.Inexact;

            return z;
        }

        // f32_to_ui64_r_minMag
        public ulong ToUInt64RoundMinMag(SoftFloatContext context, bool exact)
        {
            uint sig;
            int exp, shiftDist;
            ulong sig64, z;
            bool sign;

            exp = ExpF32UI(_v);
            sig = FracF32UI(_v);

            shiftDist = 0xBE - exp;
            if (64 <= shiftDist)
            {
                if (exact && ((uint)exp | sig) != 0)
                    context.ExceptionFlags |= ExceptionFlags.Inexact;

                return 0;
            }

            sign = SignF32UI(_v);
            if (sign || (shiftDist < 0))
            {
                context.RaiseFlags(ExceptionFlags.Invalid);
                return (exp == 0xFF && sig != 0)
                    ? context.UInt64FromNaN
                    : context.UInt64FromOverflow(sign);
            }

            sig |= 0x00800000;
            sig64 = (ulong)sig << 40;
            z = sig64 >> shiftDist;
            shiftDist = 40 - shiftDist;
            if (exact && shiftDist < 0 && (sig << shiftDist) != 0)
                context.ExceptionFlags |= ExceptionFlags.Inexact;

            return z;
        }

        // f32_to_i32_r_minMag
        public int ToInt32RoundMinMag(SoftFloatContext context, bool exact)
        {
            uint sig;
            int exp, shiftDist;
            int absZ;
            bool sign;

            exp = ExpF32UI(_v);
            sig = FracF32UI(_v);

            shiftDist = 0x9E - exp;
            if (32 <= shiftDist)
            {
                if (exact && ((uint)exp | sig) != 0)
                    context.ExceptionFlags |= ExceptionFlags.Inexact;

                return 0;
            }

            sign = SignF32UI(_v);
            if (shiftDist <= 0)
            {
                if (_v == PackToF32UI(true, 0x9E, 0))
                    return -0x7FFFFFFF - 1;

                context.RaiseFlags(ExceptionFlags.Invalid);
                return (exp == 0xFF && sig != 0)
                    ? context.Int32FromNaN
                    : context.Int32FromOverflow(sign);
            }

            sig = (sig | 0x00800000) << 8;
            absZ = (int)(sig >> shiftDist);
            if (exact && ((uint)absZ << shiftDist) != sig)
                context.ExceptionFlags |= ExceptionFlags.Inexact;

            return sign ? -absZ : absZ;
        }

        // f32_to_i64_r_minMag
        public long ToInt64RoundMinMag(SoftFloatContext context, bool exact)
        {
            uint sig;
            int exp, shiftDist;
            ulong sig64;
            long absZ;
            bool sign;

            exp = ExpF32UI(_v);
            sig = FracF32UI(_v);

            shiftDist = 0xBE - exp;
            if (64 <= shiftDist)
            {
                if (exact && ((uint)exp | sig) != 0)
                    context.ExceptionFlags |= ExceptionFlags.Inexact;

                return 0;
            }

            sign = SignF32UI(_v);
            if (shiftDist <= 0)
            {
                if (_v == PackToF32UI(true, 0xBE, 0))
                    return -0x7FFFFFFFFFFFFFFF - 1;

                context.RaiseFlags(ExceptionFlags.Invalid);
                return (exp == 0xFF && sig != 0)
                    ? context.Int64FromNaN
                    : context.Int64FromOverflow(sign);
            }

            sig |= 0x00800000;
            sig64 = (ulong)sig << 40;
            absZ = (long)(sig64 >> shiftDist);
            shiftDist = 40 - shiftDist;
            if (exact && shiftDist < 0 && (sig << shiftDist) != 0)
                context.ExceptionFlags |= ExceptionFlags.Inexact;

            return sign ? -absZ : absZ;
        }

        #endregion

        #region Floating-point-to-floating-point Conversions

        // f32_to_f16
        public Float16 ToFloat16(SoftFloatContext context)
        {
            uint frac;
            int exp;
            uint frac16;
            bool sign;

            sign = SignF32UI(_v);
            exp = ExpF32UI(_v);
            frac = FracF32UI(_v);

            if (exp == 0xFF)
            {
                if (frac != 0)
                {
                    context.Float32BitsToCommonNaN(_v, out var commonNaN);
                    return context.CommonNaNToFloat16(in commonNaN);
                }
                else
                {
                    return PackToF16(sign, 0x1F, 0);
                }
            }

            frac16 = frac >> 9 | ((frac & 0x1FF) != 0 ? 1U : 0);
            if (((uint)exp | frac16) == 0)
                return PackToF16(sign, 0, 0);

            return RoundPackToF16(context, sign, exp - 0x71, frac16 | 0x4000);
        }

        // f32_to_f64
        public Float64 ToFloat64(SoftFloatContext context)
        {
            uint frac;
            int exp;
            bool sign;

            sign = SignF32UI(_v);
            exp = ExpF32UI(_v);
            frac = FracF32UI(_v);

            if (exp == 0xFF)
            {
                if (frac != 0)
                {
                    context.Float32BitsToCommonNaN(_v, out var commonNaN);
                    return context.CommonNaNToFloat64(in commonNaN);
                }
                else
                {
                    return PackToF64(sign, 0x7FF, 0);
                }
            }

            if (exp == 0)
            {
                if (frac == 0)
                    return PackToF64(sign, 0, 0);

                (exp, frac) = NormSubnormalF32Sig(frac);
                exp--;
            }

            return PackToF64(sign, exp + 0x380, (ulong)frac << 29);
        }

        // f32_to_extF80
        public ExtFloat80 ToExtFloat80(SoftFloatContext context)
        {
            uint frac;
            int exp;
            bool sign;

            sign = SignF32UI(_v);
            exp = ExpF32UI(_v);
            frac = FracF32UI(_v);

            if (exp == 0xFF)
            {
                if (frac != 0)
                {
                    context.Float32BitsToCommonNaN(_v, out var commonNaN);
                    return context.CommonNaNToExtFloat80(in commonNaN);
                }
                else
                {
                    return PackToExtF80(sign, 0x7FFF, 0x8000000000000000);
                }
            }

            if (exp == 0)
            {
                if (frac == 0)
                    return PackToExtF80(sign, 0, 0);

                (exp, frac) = NormSubnormalF32Sig(frac);
            }

            return PackToExtF80(sign, exp + 0x3F80, (ulong)(frac | 0x00800000) << 40);
        }

        // f32_to_f128
        public Float128 ToFloat128(SoftFloatContext context)
        {
            int exp;
            uint frac;
            bool sign;

            sign = SignF32UI(_v);
            exp = ExpF32UI(_v);
            frac = FracF32UI(_v);

            if (exp == 0xFF)
            {
                if (frac != 0)
                {
                    context.Float32BitsToCommonNaN(_v, out var commonNaN);
                    return context.CommonNaNToFloat128(in commonNaN);
                }
                else
                {
                    return PackToF128(sign, 0x7FFF, 0, 0);
                }
            }

            if (exp == 0)
            {
                if (frac == 0)
                    return PackToF128(sign, 0, 0, 0);

                (exp, frac) = NormSubnormalF32Sig(frac);
                exp--;
            }

            return PackToF128(sign, exp + 0x3F80, (ulong)frac << 25, 0);
        }

        #endregion

        #region Arithmetic Operations

        // f32_roundToInt
        public Float32 RoundToInt(SoftFloatContext context, RoundingMode roundingMode, bool exact)
        {
            int exp;
            uint uiZ, lastBitMask, roundBitsMask;

            exp = ExpF32UI(_v);

            if (exp <= 0x7E)
            {
                if ((_v << 1) == 0)
                    return this;

                if (exact)
                    context.ExceptionFlags |= ExceptionFlags.Inexact;

                uiZ = _v & PackToF32UI(true, 0, 0);
                switch (roundingMode)
                {
                    case RoundingMode.NearEven:
                        {
                            if (FracF32UI(_v) == 0)
                                break;

                            goto case RoundingMode.NearMaxMag;
                        }
                    case RoundingMode.NearMaxMag:
                        {
                            if (exp == 0x7E)
                                uiZ |= PackToF32UI(false, 0x7F, 0);

                            break;
                        }
                    case RoundingMode.Min:
                        {
                            if (uiZ != 0)
                                uiZ = PackToF32UI(true, 0x7F, 0);

                            break;
                        }
                    case RoundingMode.Max:
                        {
                            if (uiZ == 0)
                                uiZ = PackToF32UI(false, 0x7F, 0);

                            break;
                        }
                    case RoundingMode.Odd:
                        {
                            uiZ |= PackToF32UI(false, 0x7F, 0);
                            break;
                        }
                }

                return Float32.FromBitsUI32(uiZ);
            }

            if (0x96 <= exp)
            {
                if (exp == 0xFF && FracF32UI(_v) != 0)
                    return context.PropagateNaNFloat32Bits(_v, 0);

                return this;
            }

            uiZ = _v;
            lastBitMask = (uint)1 << (0x96 - exp);
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
            else if (roundingMode == (SignF32UI(_v) ? RoundingMode.Min : RoundingMode.Max))
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

            return Float32.FromBitsUI32(uiZ);
        }

        // f32_add
        public static Float32 Add(SoftFloatContext context, Float32 a, Float32 b)
        {
            uint uiA, uiB;

            uiA = a._v;
            uiB = b._v;

            return SignF32UI(uiA ^ uiB)
                ? SubMagsF32(context, uiA, uiB)
                : AddMagsF32(context, uiA, uiB);
        }

        // f32_sub
        public static Float32 Subtract(SoftFloatContext context, Float32 a, Float32 b)
        {
            uint uiA, uiB;

            uiA = a._v;
            uiB = b._v;

            return SignF32UI(uiA ^ uiB)
                ? AddMagsF32(context, uiA, uiB)
                : SubMagsF32(context, uiA, uiB);
        }

        // f32_mul
        public static Float32 Multiply(SoftFloatContext context, Float32 a, Float32 b)
        {
            uint uiA, sigA, uiB, sigB, magBits, sigZ;
            int expA, expB, expZ;
            bool signA, signB, signZ;

            uiA = a._v;
            signA = SignF32UI(uiA);
            expA = ExpF32UI(uiA);
            sigA = FracF32UI(uiA);
            uiB = b._v;
            signB = SignF32UI(uiB);
            expB = ExpF32UI(uiB);
            sigB = FracF32UI(uiB);
            signZ = signA ^ signB;

            if (expA == 0xFF)
            {
                if (sigA != 0 || (expB == 0xFF && sigB != 0))
                    return context.PropagateNaNFloat32Bits(uiA, uiB);

                magBits = (uint)expB | sigB;
                if (magBits == 0)
                {
                    context.RaiseFlags(ExceptionFlags.Invalid);
                    return context.DefaultNaNFloat32;
                }
                else
                {
                    return PackToF32(signZ, 0xFF, 0);
                }
            }

            if (expB == 0xFF)
            {
                if (sigB != 0)
                    return context.PropagateNaNFloat32Bits(uiA, uiB);

                magBits = (uint)expA | sigA;
                if (magBits == 0)
                {
                    context.RaiseFlags(ExceptionFlags.Invalid);
                    return context.DefaultNaNFloat32;
                }
                else
                {
                    return PackToF32(signZ, 0xFF, 0);
                }
            }

            if (expA == 0)
            {
                if (sigA == 0)
                    return PackToF32(signZ, 0, 0);

                (expA, sigA) = NormSubnormalF32Sig(sigA);
            }

            if (expB == 0)
            {
                if (sigB == 0)
                    return PackToF32(signZ, 0, 0);

                (expB, sigB) = NormSubnormalF32Sig(sigB);
            }

            expZ = expA + expB - 0x7F;
            sigA = (sigA | 0x00800000) << 7;
            sigB = (sigB | 0x00800000) << 8;
            sigZ = (uint)ShortShiftRightJam64((ulong)sigA * sigB, 32);
            if (sigZ < 0x40000000)
            {
                --expZ;
                sigZ <<= 1;
            }

            return RoundPackToF32(context, signZ, expZ, sigZ);
        }

        // f32_mulAdd
        public static Float32 MultiplyAndAdd(SoftFloatContext context, Float32 a, Float32 b, Float32 c) =>
            MulAddF32(context, a._v, b._v, c._v, MulAddOperation.None);

        // f32_div
        public static Float32 Divide(SoftFloatContext context, Float32 a, Float32 b)
        {
            uint uiA, sigA, uiB, sigB, sigZ;
            int expA, expB, expZ;
            ulong sig64A;
            bool signA, signB, signZ;

            uiA = a._v;
            signA = SignF32UI(uiA);
            expA = ExpF32UI(uiA);
            sigA = FracF32UI(uiA);
            uiB = b._v;
            signB = SignF32UI(uiB);
            expB = ExpF32UI(uiB);
            sigB = FracF32UI(uiB);
            signZ = signA ^ signB;

            if (expA == 0xFF)
            {
                if (sigA != 0)
                    return context.PropagateNaNFloat32Bits(uiA, uiB);

                if (expB == 0xFF)
                {
                    if (sigB != 0)
                        return context.PropagateNaNFloat32Bits(uiA, uiB);

                    context.RaiseFlags(ExceptionFlags.Invalid);
                    return context.DefaultNaNFloat32;
                }

                return PackToF32(signZ, 0xFF, 0);
            }

            if (expB == 0xFF)
            {
                if (sigB != 0)
                    return context.PropagateNaNFloat32Bits(uiA, uiB);

                return PackToF32(signZ, 0, 0);
            }

            if (expB == 0)
            {
                if (sigB == 0)
                {
                    if (((uint)expA | sigA) == 0)
                    {
                        context.RaiseFlags(ExceptionFlags.Invalid);
                        return context.DefaultNaNFloat32;
                    }

                    context.RaiseFlags(ExceptionFlags.Infinite);
                    return PackToF32(signZ, 0xFF, 0);
                }

                (expB, sigB) = NormSubnormalF32Sig(sigB);
            }

            if (expA == 0)
            {
                if (sigA == 0)
                    return PackToF32(signZ, 0, 0);

                (expA, sigA) = NormSubnormalF32Sig(sigA);
            }

            expZ = expA - expB + 0x7E;
            sigA |= 0x00800000;
            sigB |= 0x00800000;
            if (sigA < sigB)
            {
                --expZ;
                sig64A = (ulong)sigA << 31;
            }
            else
            {
                sig64A = (ulong)sigA << 30;
            }

            sigZ = (uint)(sig64A / sigB);
            if ((sigZ & 0x3F) == 0)
                sigZ |= ((ulong)sigB * sigZ != sig64A) ? 1U : 0;

            return RoundPackToF32(context, signZ, expZ, sigZ);
        }

        // f32_rem
        public static Float32 Modulus(SoftFloatContext context, Float32 a, Float32 b)
        {
            uint uiA, sigA, uiB, sigB;
            int expA, expB, expDiff;
            uint rem, q, recip32, altRem, meanRem;
            bool signA, signRem;

            uiA = a._v;
            signA = SignF32UI(uiA);
            expA = ExpF32UI(uiA);
            sigA = FracF32UI(uiA);
            uiB = b._v;
            expB = ExpF32UI(uiB);
            sigB = FracF32UI(uiB);

            if (expA == 0xFF)
            {
                if (sigA != 0 || (expB == 0xFF && sigB != 0))
                    return context.PropagateNaNFloat32Bits(uiA, uiB);

                context.RaiseFlags(ExceptionFlags.Invalid);
                return context.DefaultNaNFloat32;
            }

            if (expB == 0xFF)
            {
                if (sigB != 0)
                    return context.PropagateNaNFloat32Bits(uiA, uiB);

                return a;
            }

            if (expB == 0)
            {
                if (sigB == 0)
                {
                    context.RaiseFlags(ExceptionFlags.Invalid);
                    return context.DefaultNaNFloat32;
                }

                (expB, sigB) = NormSubnormalF32Sig(sigB);
            }

            if (expA == 0)
            {
                if (sigA == 0)
                    return a;

                (expA, sigA) = NormSubnormalF32Sig(sigA);
            }

            rem = sigA | 0x00800000;
            sigB |= 0x00800000;
            expDiff = expA - expB;
            if (expDiff < 1)
            {
                if (expDiff < -1)
                    return a;

                sigB <<= 6;
                if (expDiff != 0)
                {
                    rem <<= 5;
                    q = 0;
                }
                else
                {
                    rem <<= 6;
                    q = (sigB <= rem) ? 1U : 0;
                    if (q != 0)
                        rem -= sigB;
                }
            }
            else
            {
                recip32 = ApproxRecip32_1(sigB << 8);

                // Changing the shift of 'rem' here requires also changing the initial subtraction from 'expDiff'.
                rem <<= 7;
                expDiff -= 31;

                // The scale of 'sigB' affects how many bits are obtained during each cycle of the loop. Currently this is 29 bits per loop
                // iteration, which is believed to be the maximum possible.
                sigB <<= 6;
                while (true)
                {
                    q = (uint)((rem * (ulong)recip32) >> 32);
                    if (expDiff < 0)
                        break;

                    rem = (uint)(-(int)(q * sigB));
                    expDiff -= 29;
                }

                // ('expDiff' cannot be less than -30 here.)
                q >>= ~expDiff;
                rem = (rem << (expDiff + 30)) - (q * sigB);
            }

            do
            {
                altRem = rem;
                ++q;
                rem -= sigB;
            }
            while ((rem & 0x80000000) == 0);

            meanRem = rem + altRem;
            if ((meanRem & 0x80000000) != 0 || (meanRem == 0 && (q & 1) != 0))
                rem = altRem;

            signRem = signA;
            if (0x80000000 <= rem)
            {
                signRem = !signRem;
                rem = (uint)(-(int)rem);
            }

            return NormRoundPackToF32(context, signRem, expB, rem);
        }

        // f32_sqrt
        public Float32 SquareRoot(SoftFloatContext context)
        {
            uint uiA, sigA, sigZ, shiftedSigZ;
            int expA, expZ;
            uint negRem;
            bool signA;

            uiA = _v;
            signA = SignF32UI(uiA);
            expA = ExpF32UI(uiA);
            sigA = FracF32UI(uiA);

            if (expA == 0xFF)
            {
                if (sigA != 0)
                    return context.PropagateNaNFloat32Bits(uiA, 0);

                if (!signA)
                    return this;

                context.RaiseFlags(ExceptionFlags.Invalid);
                return context.DefaultNaNFloat32;
            }

            if (signA)
            {
                if (((uint)expA | sigA) == 0)
                    return this;

                context.RaiseFlags(ExceptionFlags.Invalid);
                return context.DefaultNaNFloat32;
            }

            if (expA == 0)
            {
                if (sigA == 0)
                    return this;

                (expA, sigA) = NormSubnormalF32Sig(sigA);
            }

            expZ = ((expA - 0x7F) >> 1) + 0x7E;
            expA &= 1;
            sigA = (sigA | 0x00800000) << 8;
            sigZ = (uint)(((ulong)sigA * ApproxRecipSqrt32_1((uint)expA, sigA)) >> 32);
            if (expA != 0)
                sigZ >>= 1;

            sigZ += 2;
            if ((sigZ & 0x3F) < 2)
            {
                shiftedSigZ = sigZ >> 2;
                negRem = shiftedSigZ * shiftedSigZ;
                sigZ &= ~3U;
                if ((negRem & 0x80000000) != 0)
                    sigZ |= 1;
                else if (negRem != 0)
                    --sigZ;
            }

            return RoundPackToF32(context, false, expZ, sigZ);
        }

        #endregion

        #region Comparison Operations

        // f32_eq (signaling=false) & f32_eq_signaling (signaling=true)
        public static bool CompareEqual(SoftFloatContext context, Float32 a, Float32 b, bool signaling)
        {
            uint uiA, uiB;

            uiA = a._v;
            uiB = b._v;

            if (IsNaNF32UI(uiA) || IsNaNF32UI(uiB))
            {
                if (signaling || context.IsSignalingNaNFloat32Bits(uiA) || context.IsSignalingNaNFloat32Bits(uiB))
                    context.RaiseFlags(ExceptionFlags.Invalid);

                return false;
            }

            return (uiA == uiB) || ((uiA | uiB) << 1) == 0;
        }

        // f32_le (signaling=true) & f32_le_quiet (signaling=false)
        public static bool CompareLessThanOrEqual(SoftFloatContext context, Float32 a, Float32 b, bool signaling)
        {
            uint uiA, uiB;
            bool signA, signB;

            uiA = a._v;
            uiB = b._v;

            if (IsNaNF32UI(uiA) || IsNaNF32UI(uiB))
            {
                if (signaling || context.IsSignalingNaNFloat32Bits(uiA) || context.IsSignalingNaNFloat32Bits(uiB))
                    context.RaiseFlags(ExceptionFlags.Invalid);

                return false;
            }

            signA = SignF32UI(uiA);
            signB = SignF32UI(uiB);

            return (signA != signB)
                ? (signA || ((uiA | uiB) << 1) == 0)
                : (uiA == uiB || (signA ^ (uiA < uiB)));
        }

        // f32_lt (signaling=true) & f32_lt_quiet (signaling=false)
        public static bool CompareLessThan(SoftFloatContext context, Float32 a, Float32 b, bool signaling)
        {
            uint uiA, uiB;
            bool signA, signB;

            uiA = a._v;
            uiB = b._v;

            if (IsNaNF32UI(uiA) || IsNaNF32UI(uiB))
            {
                if (signaling || context.IsSignalingNaNFloat32Bits(uiA) || context.IsSignalingNaNFloat32Bits(uiB))
                    context.RaiseFlags(ExceptionFlags.Invalid);

                return false;
            }

            signA = SignF32UI(uiA);
            signB = SignF32UI(uiB);

            return (signA != signB)
                ? (signA && ((uiA | uiB) << 1) != 0)
                : (uiA != uiB && (signA ^ (uiA < uiB)));
        }

        #endregion

        #endregion
    }

}