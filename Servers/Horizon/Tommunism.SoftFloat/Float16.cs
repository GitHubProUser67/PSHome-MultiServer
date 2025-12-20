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

    [StructLayout(LayoutKind.Sequential, Pack = sizeof(ushort), Size = sizeof(ushort))]
    public readonly struct Float16
    {
        #region Fields

        // WARNING: DO NOT ADD OR CHANGE ANY OF THESE FIELDS!!!
        private readonly ushort _v;

        #endregion

        #region Constructors

        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Used to avoid accidentally calling other overloads.")]
        private Float16(ushort v, bool dummy)
        {
            _v = v;
        }

        public Float16(Half value)
        {
            _v = BitConverter.HalfToUInt16Bits(value);
        }

        #endregion

        #region Methods

        public static explicit operator Float16(Half value) => new(value);
        public static implicit operator Half(Float16 value) => BitConverter.UInt16BitsToHalf(value._v);

        public static Float16 FromUIntBits(ushort value) => FromBitsUI16(value);

        public ushort ToUInt16Bits() => _v;

        // THIS IS THE INTERNAL CONSTRUCTOR FOR RAW BITS.
        // TODO: Allow value to be a full 32-bit integer (reduces total number of "unnecessary" casts).
        internal static Float16 FromBitsUI16(ushort v) => new(v, dummy: false);

        #region Integer-to-floating-point Conversions

        // ui32_to_f16
        public static Float16 FromUInt32(SoftFloatContext context, uint a)
        {
            int shiftDist = CountLeadingZeroes32(a) - 21;
            if (0 <= shiftDist)
                return FromBitsUI16(a != 0 ? PackToF16UI(false, 0x18 - shiftDist, a << shiftDist) : (ushort)0);

            shiftDist += 4;
            uint sig = (shiftDist < 0)
                ? ((a >> (-shiftDist)) | ((a << shiftDist) != 0 ? 1U : 0))
                : (a << shiftDist);
            return RoundPackToF16(context, false, 0x1C - shiftDist, sig);
        }

        // ui64_to_f16
        public static Float16 FromUInt64(SoftFloatContext context, ulong a)
        {
            var shiftDist = CountLeadingZeroes64(a) - 53;
            if (0 <= shiftDist)
                return FromBitsUI16(a != 0 ? PackToF16UI(false, 0x18 - shiftDist, (uint)a << shiftDist) : (ushort)0);

            shiftDist += 4;
            var sig = (shiftDist < 0)
                ? (uint)ShortShiftRightJam64(a, -shiftDist)
                : ((uint)a << shiftDist);
            return RoundPackToF16(context, false, 0x1C - shiftDist, sig);
        }

        // i32_to_f16
        public static Float16 FromInt32(SoftFloatContext context, int a)
        {
            var sign = a < 0;
            var absA = (uint)(sign ? -a : a);
            var shiftDist = CountLeadingZeroes32(absA) - 21;
            if (0 <= shiftDist)
                return FromBitsUI16(a != 0 ? PackToF16UI(sign, 0x18 - shiftDist, absA << shiftDist) : (ushort)0);

            shiftDist += 4;
            var sig = (shiftDist < 0)
                ? (absA >> (-shiftDist)) | ((absA << shiftDist) != 0 ? 1U : 0U)
                : (absA << shiftDist);
            return RoundPackToF16(context, sign, 0x1C - shiftDist, sig);
        }

        // i64_to_f16
        public static Float16 FromInt64(SoftFloatContext context, long a)
        {
            var sign = a < 0;
            var absA = (ulong)(sign ? -a : a);
            var shiftDist = CountLeadingZeroes64(absA) - 53;
            if (0 <= shiftDist)
                return FromBitsUI16(a != 0 ? PackToF16UI(sign, 0x18 - shiftDist, (uint)absA << shiftDist) : (ushort)0);

            shiftDist += 4;
            var sig = (shiftDist < 0)
                ? (uint)ShortShiftRightJam64(absA, -shiftDist)
                : ((uint)absA << shiftDist);
            return RoundPackToF16(context, sign, 0x1C - shiftDist, sig);
        }

        #endregion

        #region Floating-point-to-integer Conversions

        public uint ToUInt32(SoftFloatContext context, bool exact) => ToUInt32(context, context.Rounding, exact);

        public ulong ToUInt64(SoftFloatContext context, bool exact) => ToUInt64(context, context.Rounding, exact);

        public int ToInt32(SoftFloatContext context, bool exact) => ToInt32(context, context.Rounding, exact);

        public long ToInt64(SoftFloatContext context, bool exact) => ToInt64(context, context.Rounding, exact);

        // f16_to_ui32
        public uint ToUInt32(SoftFloatContext context, RoundingMode roundingMode, bool exact)
        {
            uint uiA, frac;
            bool sign;
            int exp, shiftDist;
            uint sig32;

            uiA = _v;
            sign = SignF16UI(uiA);
            exp = ExpF16UI(uiA);
            frac = FracF16UI(uiA);

            if (exp == 0x1F)
            {
                context.RaiseFlags(ExceptionFlags.Invalid);
                return (frac != 0)
                    ? context.UInt32FromNaN
                    : context.UInt32FromOverflow(sign);
            }

            sig32 = frac;
            if (exp != 0)
            {
                sig32 |= 0x0400;
                shiftDist = exp - 0x19;
                if (0 <= shiftDist && !sign)
                    return sig32 << shiftDist;

                shiftDist = exp - 0x0D;
                if (0 < shiftDist)
                    sig32 <<= shiftDist;
            }

            return RoundToUI32(context, sign, sig32, roundingMode, exact);
        }

        // f16_to_ui64
        public ulong ToUInt64(SoftFloatContext context, RoundingMode roundingMode, bool exact)
        {
            uint uiA, frac;
            bool sign;
            int exp, shiftDist;
            uint sig32;

            uiA = _v;
            sign = SignF16UI(uiA);
            exp = ExpF16UI(uiA);
            frac = FracF16UI(uiA);

            if (exp == 0x1F)
            {
                context.RaiseFlags(ExceptionFlags.Invalid);
                return (frac != 0)
                    ? context.UInt64FromNaN
                    : context.UInt64FromOverflow(sign);
            }

            sig32 = frac;
            if (exp != 0)
            {
                sig32 |= 0x0400;
                shiftDist = exp - 0x19;
                if (0 <= shiftDist && !sign)
                    return sig32 << shiftDist;

                shiftDist = exp - 0x0D;
                if (0 < shiftDist)
                    sig32 <<= shiftDist;
            }

            return RoundToUI64(context, sign, sig32 >> 12, (ulong)sig32 << 52, roundingMode, exact);
        }

        // f16_to_i32
        public int ToInt32(SoftFloatContext context, RoundingMode roundingMode, bool exact)
        {
            uint uiA, frac;
            bool sign;
            int exp, shiftDist;
            int sig32;

            uiA = _v;
            sign = SignF16UI(uiA);
            exp = ExpF16UI(uiA);
            frac = FracF16UI(uiA);

            if (exp == 0x1F)
            {
                context.RaiseFlags(ExceptionFlags.Invalid);
                return (frac != 0)
                    ? context.Int32FromNaN
                    : context.Int32FromOverflow(sign);
            }

            sig32 = (int)frac;
            if (exp != 0)
            {
                sig32 |= 0x0400;
                shiftDist = exp - 0x19;
                if (0 <= shiftDist)
                {
                    sig32 <<= shiftDist;
                    return sign ? -sig32 : sig32;
                }

                shiftDist = exp - 0x0D;
                if (0 < shiftDist)
                    sig32 <<= shiftDist;
            }

            return RoundToI32(context, sign, (uint)sig32, roundingMode, exact);
        }

        // f16_to_i64
        public long ToInt64(SoftFloatContext context, RoundingMode roundingMode, bool exact)
        {
            uint uiA, frac;
            bool sign;
            int exp, shiftDist;
            int sig32;

            uiA = _v;
            sign = SignF16UI(uiA);
            exp = ExpF16UI(uiA);
            frac = FracF16UI(uiA);

            if (exp == 0x1F)
            {
                context.RaiseFlags(ExceptionFlags.Invalid);
                return (frac != 0)
                    ? context.Int64FromNaN
                    : context.Int64FromOverflow(sign);
            }

            sig32 = (int)frac;
            if (exp != 0)
            {
                sig32 |= 0x0400;
                shiftDist = exp - 0x19;
                if (0 <= shiftDist)
                {
                    sig32 <<= shiftDist;
                    return sign ? -sig32 : sig32;
                }

                shiftDist = exp - 0x0D;
                if (0 < shiftDist)
                    sig32 <<= shiftDist;
            }

            return RoundToI32(context, sign, (uint)sig32, roundingMode, exact);
        }

        // f16_to_ui32_r_minMag
        public uint ToUInt32RoundMinMag(SoftFloatContext context, bool exact)
        {
            uint uiA, frac;
            int exp, shiftDist;
            bool sign;
            uint alignedSig;

            uiA = _v;
            exp = ExpF16UI(uiA);
            frac = FracF16UI(uiA);

            shiftDist = exp - 0x0F;
            if (shiftDist < 0)
            {
                if (exact && ((uint)exp | frac) != 0)
                    context.ExceptionFlags |= ExceptionFlags.Inexact;

                return 0;
            }

            sign = SignF16UI(uiA);
            if (sign || exp == 0x1F)
            {
                context.RaiseFlags(ExceptionFlags.Invalid);
                return (frac != 0)
                    ? context.UInt32FromNaN
                    : context.UInt32FromOverflow(sign);
            }

            alignedSig = (frac | 0x0400) << shiftDist;
            if (exact && (alignedSig & 0x3FF) != 0)
                context.ExceptionFlags |= ExceptionFlags.Inexact;

            return alignedSig >> 10;
        }

        // f16_to_ui64_r_minMag
        public ulong ToUInt64RoundMinMag(SoftFloatContext context, bool exact)
        {
            uint uiA, frac;
            int exp, shiftDist;
            bool sign;
            uint alignedSig;

            uiA = _v;
            exp = ExpF16UI(uiA);
            frac = FracF16UI(uiA);

            shiftDist = exp - 0x0F;
            if (shiftDist < 0)
            {
                if (exact && ((uint)exp | frac) != 0)
                    context.ExceptionFlags |= ExceptionFlags.Inexact;

                return 0;
            }

            sign = SignF16UI(uiA);
            if (sign || exp == 0x1F)
            {
                context.RaiseFlags(ExceptionFlags.Invalid);
                return (frac != 0)
                    ? context.UInt64FromNaN
                    : context.UInt64FromOverflow(sign);
            }

            alignedSig = (frac | 0x0400) << shiftDist;
            if (exact && (alignedSig & 0x3FF) != 0)
                context.ExceptionFlags |= ExceptionFlags.Inexact;

            return alignedSig >> 10;
        }

        // f16_to_i32_r_minMag
        public int ToInt32RoundMinMag(SoftFloatContext context, bool exact)
        {
            uint uiA, frac;
            bool sign;
            int exp, shiftDist;
            int alignedSig;

            uiA = _v;
            exp = ExpF16UI(uiA);
            frac = FracF16UI(uiA);

            shiftDist = exp - 0x0F;
            if (shiftDist < 0)
            {
                if (exact && ((uint)exp | frac) != 0)
                    context.ExceptionFlags |= ExceptionFlags.Inexact;

                return 0;
            }

            sign = SignF16UI(uiA);
            if (exp == 0x1F)
            {
                context.RaiseFlags(ExceptionFlags.Invalid);
                return (frac != 0)
                    ? context.Int32FromNaN
                    : context.Int32FromOverflow(sign);
            }

            alignedSig = (int)(frac | 0x0400) << shiftDist;
            if (exact && (alignedSig & 0x3FF) != 0)
                context.ExceptionFlags |= ExceptionFlags.Inexact;

            alignedSig >>= 10;
            return sign ? -alignedSig : alignedSig;
        }

        // f16_to_i64_r_minMag
        public long ToInt64RoundMinMag(SoftFloatContext context, bool exact)
        {
            uint uiA, frac;
            bool sign;
            int exp, shiftDist;
            int alignedSig;

            uiA = _v;
            exp = ExpF16UI(uiA);
            frac = FracF16UI(uiA);

            shiftDist = exp - 0x0F;
            if (shiftDist < 0)
            {
                if (exact && ((uint)exp | frac) != 0)
                    context.ExceptionFlags |= ExceptionFlags.Inexact;

                return 0;
            }

            sign = SignF16UI(uiA);
            if (exp == 0x1F)
            {
                context.RaiseFlags(ExceptionFlags.Invalid);
                return (frac != 0)
                    ? context.Int64FromNaN
                    : context.Int64FromOverflow(sign);
            }

            alignedSig = (int)(frac | 0x0400) << shiftDist;
            if (exact && (alignedSig & 0x3FF) != 0)
                context.ExceptionFlags |= ExceptionFlags.Inexact;

            alignedSig >>= 10;
            return sign ? -alignedSig : alignedSig;
        }

        #endregion

        #region Floating-point-to-floating-point Conversions

        // f16_to_f32
        public Float32 ToFloat32(SoftFloatContext context)
        {
            uint uiA, frac;
            bool sign;
            int exp;

            uiA = _v;
            sign = SignF16UI(uiA);
            exp = ExpF16UI(uiA);
            frac = FracF16UI(uiA);

            if (exp == 0x1F)
            {
                if (frac != 0)
                {
                    context.Float16BitsToCommonNaN(uiA, out var commonNaN);
                    return context.CommonNaNToFloat32(in commonNaN);
                }

                return PackToF32(sign, 0xFF, 0);
            }
            else if (exp == 0)
            {
                if (frac == 0)
                    return PackToF32(sign, 0, 0);

                (exp, frac) = NormSubnormalF16Sig(frac);
                exp--;
            }

            return PackToF32(sign, exp + 0x70, frac << 13);
        }

        // f16_to_f64
        public Float64 ToFloat64(SoftFloatContext context)
        {
            uint uiA, frac;
            bool sign;
            int exp;

            uiA = _v;
            sign = SignF16UI(uiA);
            exp = ExpF16UI(uiA);
            frac = FracF16UI(uiA);

            if (exp == 0x1F)
            {
                if (frac != 0)
                {
                    context.Float16BitsToCommonNaN(uiA, out var commonNaN);
                    return context.CommonNaNToFloat64(in commonNaN);
                }

                return PackToF64(sign, 0x7FF, 0);
            }
            else if (exp == 0)
            {
                if (frac == 0)
                    return PackToF64(sign, 0, 0);

                (exp, frac) = NormSubnormalF16Sig(frac);
                exp--;
            }

            return PackToF64(sign, exp + 0x3F0, (ulong)frac << 42);
        }

        // f16_to_extF80
        public ExtFloat80 ToExtFloat80(SoftFloatContext context)
        {
            uint uiA, frac;
            bool sign;
            int exp;

            uiA = _v;
            sign = SignF16UI(uiA);
            exp = ExpF16UI(uiA);
            frac = FracF16UI(uiA);

            if (exp == 0x1F)
            {
                if (frac != 0)
                {
                    context.Float16BitsToCommonNaN(uiA, out var commonNaN);
                    return context.CommonNaNToExtFloat80(in commonNaN);
                }

                return PackToExtF80(sign, 0x7FFF, 0x8000000000000000);
            }
            else if (exp == 0)
            {
                if (frac == 0)
                    return PackToExtF80(sign, 0, 0);

                (exp, frac) = NormSubnormalF16Sig(frac);
            }

            return PackToExtF80(sign, exp + 0x3FF0, (ulong)(frac | 0x0400) << 53);
        }

        // f16_to_f128
        public Float128 ToFloat128(SoftFloatContext context)
        {
            uint uiA, frac;
            bool sign;
            int exp;

            uiA = _v;
            sign = SignF16UI(uiA);
            exp = ExpF16UI(uiA);
            frac = FracF16UI(uiA);

            if (exp == 0x1F)
            {
                if (frac != 0)
                {
                    context.Float16BitsToCommonNaN(uiA, out var commonNaN);
                    return context.CommonNaNToFloat128(in commonNaN);
                }

                return PackToF128(sign, 0x7FFF, 0, 0);
            }
            else if (exp == 0)
            {
                if (frac == 0)
                    return PackToF128(sign, 0, 0, 0);

                (exp, frac) = NormSubnormalF16Sig(frac);
                exp--;
            }

            return PackToF128(sign, exp + 0x3FF0, (ulong)frac << 38, 0);
        }

        #endregion

        #region Arithmetic Operations

        public Float16 RoundToInt(SoftFloatContext context, bool exact) => RoundToInt(context, context.Rounding, exact);

        // f16_roundToInt
        public Float16 RoundToInt(SoftFloatContext context, RoundingMode roundingMode, bool exact)
        {
            uint uiA, uiZ, lastBitMask, roundBitsMask;
            int exp;

            uiA = _v;
            exp = ExpF16UI(uiA);

            if (exp <= 0xE)
            {
                if ((ushort)(uiA << 1) == 0)
                    return this;

                if (exact)
                    context.ExceptionFlags |= ExceptionFlags.Inexact;

                uiZ = uiA & PackToF16UI(true, 0, 0);
                switch (roundingMode)
                {
                    case RoundingMode.NearEven:
                        {
                            if (FracF16UI(uiA) != 0)
                                goto case RoundingMode.NearMaxMag;

                            break;
                        }
                    case RoundingMode.NearMaxMag:
                        {
                            if (exp == 0xE)
                                uiZ |= PackToF16UI(false, 0xF, 0);

                            break;
                        }
                    case RoundingMode.Min:
                        {
                            if (uiZ != 0)
                                uiZ = PackToF16UI(true, 0xF, 0);

                            break;
                        }
                    case RoundingMode.Max:
                        {
                            if (uiZ == 0)
                                uiZ = PackToF16UI(false, 0xF, 0);

                            break;
                        }
                    case RoundingMode.Odd:
                        {
                            uiZ |= PackToF16UI(false, 0xF, 0);
                            break;
                        }
                }

                return FromBitsUI16((ushort)uiZ);
            }

            if (0x19 <= exp)
            {
                return exp == 0x1F && FracF16UI(uiA) != 0
                    ? context.PropagateNaNFloat16(uiA, 0)
                    : this;
            }

            uiZ = uiA;
            lastBitMask = 1U << (0x19 - exp);
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
            else if (roundingMode == (SignF16UI(uiZ) ? RoundingMode.Min : RoundingMode.Max))
            {
                uiZ += roundBitsMask;
            }

            uiZ &= ~roundBitsMask;
            if (uiZ != uiA)
            {
                if (roundingMode == RoundingMode.Odd)
                    uiZ |= lastBitMask;

                if (exact)
                    context.ExceptionFlags |= ExceptionFlags.Inexact;
            }

            return FromBitsUI16((ushort)uiZ);
        }

        // f16_add
        public static Float16 Add(SoftFloatContext context, Float16 a, Float16 b)
        {
            uint uiA, uiB;

            uiA = a._v;
            uiB = b._v;

            return SignF16UI(uiA ^ uiB)
                ? SubMagsF16(context, uiA, uiB)
                : AddMagsF16(context, uiA, uiB);
        }

        // f16_sub
        public static Float16 Subtract(SoftFloatContext context, Float16 a, Float16 b)
        {
            uint uiA, uiB;

            uiA = a._v;
            uiB = b._v;

            return SignF16UI(uiA ^ uiB)
                ? AddMagsF16(context, uiA, uiB)
                : SubMagsF16(context, uiA, uiB);
        }

        // f16_mul
        public static Float16 Multiply(SoftFloatContext context, Float16 a, Float16 b)
        {
            uint uiA, sigA, uiB, sigB, sigZ;
            int expA, expB, expZ;
            uint sig32Z;
            bool signA, signB, signZ;

            uiA = a._v;
            signA = SignF16UI(uiA);
            expA = ExpF16UI(uiA);
            sigA = FracF16UI(uiA);
            uiB = b._v;
            signB = SignF16UI(uiB);
            expB = ExpF16UI(uiB);
            sigB = FracF16UI(uiB);
            signZ = signA ^ signB;

            if (expA == 0x1F)
            {
                if (sigA != 0 || ((expB == 0x1F) && sigB != 0))
                    return context.PropagateNaNFloat16(uiA, uiB);

                if (((uint)expB | sigB) == 0)
                {
                    context.RaiseFlags(ExceptionFlags.Invalid);
                    return context.DefaultNaNFloat16;
                }

                return PackToF16(signZ, 0x1F, 0);
            }
            else if (expB == 0x1F)
            {
                if (sigB != 0)
                    return context.PropagateNaNFloat16(uiA, uiB);

                if (((uint)expA | sigA) == 0)
                {
                    context.RaiseFlags(ExceptionFlags.Invalid);
                    return context.DefaultNaNFloat16;
                }

                return PackToF16(signZ, 0x1F, 0);
            }

            if (expA == 0)
            {
                if (sigA == 0)
                    return PackToF16(signZ, 0, 0);

                (expA, sigA) = NormSubnormalF16Sig(sigA);
            }

            if (expB == 0)
            {
                if (sigB == 0)
                    return PackToF16(signZ, 0, 0);

                (expB, sigB) = NormSubnormalF16Sig(sigB);
            }

            expZ = expA + expB - 0xF;
            sigA = (sigA | 0x0400) << 4;
            sigB = (sigB | 0x0400) << 5;
            sig32Z = sigA * sigB;
            sigZ = sig32Z >> 16;
            if ((sig32Z & 0xFFFF) != 0)
                sigZ |= 1;

            if (sigZ < 0x4000)
            {
                --expZ;
                sigZ <<= 1;
            }

            return RoundPackToF16(context, signZ, expZ, sigZ);
        }

        // f16_mulAdd
        public static Float16 MultiplyAndAdd(SoftFloatContext context, Float16 a, Float16 b, Float16 c) =>
            MulAddF16(context, a._v, b._v, c._v, MulAddOperation.None);

        // f16_div
        public static Float16 Divide(SoftFloatContext context, Float16 a, Float16 b)
        {
            uint uiA, sigA, uiB, sigB, sig32A, sigZ;
            int expA, expB, expZ;
            bool signA, signB, signZ;

            uiA = a._v;
            signA = SignF16UI(uiA);
            expA = ExpF16UI(uiA);
            sigA = FracF16UI(uiA);
            uiB = b._v;
            signB = SignF16UI(uiB);
            expB = ExpF16UI(uiB);
            sigB = FracF16UI(uiB);
            signZ = signA ^ signB;

            if (expA == 0x1F)
            {
                if (sigA != 0)
                    return context.PropagateNaNFloat16(uiA, uiB);

                if (expB == 0x1F)
                {
                    if (sigB != 0)
                        return context.PropagateNaNFloat16(uiA, uiB);

                    context.RaiseFlags(ExceptionFlags.Invalid);
                    return context.DefaultNaNFloat16;
                }

                return PackToF16(signZ, 0x1F, 0);
            }
            else if (expB == 0x1F)
            {
                if (sigB != 0)
                    return context.PropagateNaNFloat16(uiA, uiB);

                return PackToF16(signZ, 0, 0);
            }

            if (expB == 0)
            {
                if (sigB == 0)
                {
                    if (((uint)expA | sigA) == 0)
                    {
                        context.RaiseFlags(ExceptionFlags.Invalid);
                        return context.DefaultNaNFloat16;
                    }

                    context.RaiseFlags(ExceptionFlags.Infinite);
                    return PackToF16(signZ, 0x1F, 0);
                }

                (expB, sigB) = NormSubnormalF16Sig(sigB);
            }

            if (expA == 0)
            {
                if (sigA == 0)
                    return PackToF16(signZ, 0, 0);

                (expA, sigA) = NormSubnormalF16Sig(sigA);
            }

            expZ = expA - expB + 0xE;
            sigA |= 0x0400;
            sigB |= 0x0400;

            if (sigA < sigB)
            {
                --expZ;
                sig32A = sigA << 15;
            }
            else
            {
                sig32A = sigA << 14;
            }

            sigZ = sig32A / sigB;
            if ((sigZ & 7) == 0)
                sigZ |= (sigB * sigZ != sig32A) ? 1U : 0;

            return RoundPackToF16(context, signZ, expZ, sigZ);
        }

        // f16_rem
        public static Float16 Modulus(SoftFloatContext context, Float16 a, Float16 b)
        {
            uint uiA, sigA, uiB, sigB, q;
            int expA, expB, expDiff;
            ushort rem, altRem, meanRem;
            uint recip32, q32;
            bool signA, signRem;

            uiA = a._v;
            signA = SignF16UI(uiA);
            expA = ExpF16UI(uiA);
            sigA = FracF16UI(uiA);
            uiB = b._v;
            expB = ExpF16UI(uiB);
            sigB = FracF16UI(uiB);

            if (expA == 0x1F)
            {
                if (sigA != 0 || (expB == 0x1F && sigB != 0))
                    return context.PropagateNaNFloat16(uiA, uiB);

                context.RaiseFlags(ExceptionFlags.Invalid);
                return context.DefaultNaNFloat16;
            }
            else if (expB == 0x1F)
            {
                if (sigB != 0)
                    return context.PropagateNaNFloat16(uiA, uiB);

                return a;
            }

            if (expB == 0)
            {
                if (sigB == 0)
                {
                    context.RaiseFlags(ExceptionFlags.Invalid);
                    return context.DefaultNaNFloat16;
                }

                (expB, sigB) = NormSubnormalF16Sig(sigB);
            }

            if (expA == 0)
            {
                if (sigA == 0)
                    return a;

                (expA, sigA) = NormSubnormalF16Sig(sigA);
            }

            rem = (ushort)(sigA | 0x0400);
            sigB |= 0x0400;
            expDiff = expA - expB;
            if (expDiff < 1)
            {
                if (expDiff < -1)
                    return a;

                sigB <<= 3;
                if (expDiff != 0)
                {
                    rem <<= 2;
                    q = 0;
                }
                else
                {
                    rem <<= 3;
                    q = (sigB <= rem) ? 1U : 0;
                    if (q != 0)
                        rem -= (ushort)sigB;
                }
            }
            else
            {
                recip32 = ApproxRecip32_1(sigB << 21);

                // Changing the shift of 'rem' here requires also changing the initial subtraction from 'expDiff'.
                rem <<= 4;
                expDiff -= 31;

                // The scale of 'sigB' affects how many bits are obtained during each cycle of the loop. Currently this is 29 bits per loop
                // iteration, which is believed to be the maximum possible.
                sigB <<= 3;
                while (true)
                {
                    q32 = (uint)((rem * (ulong)recip32) >> 16);
                    if (expDiff < 0)
                        break;

                    rem = (ushort)(-(int)(q32 * sigB));
                    expDiff -= 29;
                }

                // ('expDiff' cannot be less than -30 here.)
                q32 >>= ~expDiff;
                q = q32;
                rem = (ushort)(((uint)rem << (expDiff + 30)) - q * sigB);
            }

            do
            {
                altRem = rem;
                ++q;
                rem -= (ushort)sigB;
            }
            while ((rem & 0x8000) == 0);

            meanRem = (ushort)(rem + altRem);
            if ((meanRem & 0x8000) != 0 || (meanRem == 0 && (q & 1) != 0))
                rem = altRem;

            signRem = signA;
            if (0x8000 <= rem)
            {
                signRem = !signRem;
                rem = (ushort)(-(short)rem);
            }

            return NormRoundPackToF16(context, signRem, expB, rem);
        }

        // f16_sqrt
        public Float16 SquareRoot(SoftFloatContext context)
        {
            uint uiA, sigA, r0, recipSqrt16, sigZ, shiftedSigZ;
            int expA, expZ;
            int index;
            uint ESqrR0;
            ushort sigma0, negRem;
            bool signA;

            uiA = _v;
            signA = SignF16UI(uiA);
            expA = ExpF16UI(uiA);
            sigA = FracF16UI(uiA);

            if (expA == 0x1F)
            {
                if (sigA != 0)
                    return context.PropagateNaNFloat16(uiA, 0);

                if (!signA)
                    return this;

                context.RaiseFlags(ExceptionFlags.Invalid);
                return context.DefaultNaNFloat16;
            }

            if (signA)
            {
                if (((uint)expA | sigA) == 0)
                    return this;

                context.RaiseFlags(ExceptionFlags.Invalid);
                return context.DefaultNaNFloat16;
            }

            if (expA == 0)
            {
                if (sigA == 0)
                    return this;

                (expA, sigA) = NormSubnormalF16Sig(sigA);
            }

            expZ = ((expA - 0xF) >> 1) + 0xE;
            expA &= 1;
            sigA |= 0x0400;
            index = (int)(sigA >> 6 & 0xE) + expA;
            r0 = ApproxRecipSqrt_1k0s[index] - ((ApproxRecipSqrt_1k1s[index] * (sigA & 0x7F)) >> 11);
            ESqrR0 = (r0 * r0) >> 1;
            if (expA != 0)
                ESqrR0 >>= 1;

            sigma0 = (ushort)~((ESqrR0 * sigA) >> 16);
            recipSqrt16 = r0 + ((r0 * sigma0) >> 25);
            if ((recipSqrt16 & 0x8000) == 0)
                recipSqrt16 = 0x8000;

            sigZ = ((sigA << 5) * recipSqrt16) >> 16;
            if (expA != 0)
                sigZ >>= 1;

            ++sigZ;
            if ((sigZ & 7) == 0)
            {
                shiftedSigZ = sigZ >> 1;
                negRem = (ushort)(shiftedSigZ * shiftedSigZ);
                sigZ &= ~1U;
                if ((negRem & 0x8000) != 0)
                {
                    sigZ |= 1;
                }
                else
                {
                    if (negRem != 0)
                        --sigZ;
                }
            }

            return RoundPackToF16(context, false, expZ, sigZ);
        }

        #endregion

        #region Comparison Operations

        // f16_eq (signaling=false) & f16_eq_signaling (signaling=true)
        public static bool CompareEqual(SoftFloatContext context, Float16 a, Float16 b, bool signaling)
        {
            uint uiA, uiB;

            uiA = a._v;
            uiB = b._v;

            if (IsNaNF16UI(uiA) || IsNaNF16UI(uiB))
            {
                if (signaling || context.IsSignalingNaNFloat16Bits(uiA) || context.IsSignalingNaNFloat16Bits(uiB))
                    context.RaiseFlags(ExceptionFlags.Invalid);

                return false;
            }

            return (uiA == uiB) || (ushort)((uiA | uiB) << 1) == 0;
        }

        // f16_le (signaling=true) & f16_le_quiet (signaling=false)
        public static bool CompareLessThanOrEqual(SoftFloatContext context, Float16 a, Float16 b, bool signaling)
        {
            uint uiA, uiB;
            bool signA, signB;

            uiA = a._v;
            uiB = b._v;

            if (IsNaNF16UI(uiA) || IsNaNF16UI(uiB))
            {
                if (signaling || context.IsSignalingNaNFloat16Bits(uiA) || context.IsSignalingNaNFloat16Bits(uiB))
                    context.RaiseFlags(ExceptionFlags.Invalid);

                return false;
            }

            signA = SignF16UI(uiA);
            signB = SignF16UI(uiB);

            return (signA != signB)
                ? (signA || (ushort)((uiA | uiB) << 1) == 0)
                : (uiA == uiB || (signA ^ (uiA < uiB)));
        }

        // f16_lt (signaling=true) & f16_lt_quiet (signaling=false)
        public static bool CompareLessThan(SoftFloatContext context, Float16 a, Float16 b, bool signaling)
        {
            uint uiA, uiB;
            bool signA, signB;

            uiA = a._v;
            uiB = b._v;

            if (IsNaNF16UI(uiA) || IsNaNF16UI(uiB))
            {
                if (signaling || context.IsSignalingNaNFloat16Bits(uiA) || context.IsSignalingNaNFloat16Bits(uiB))
                    context.RaiseFlags(ExceptionFlags.Invalid);

                return false;
            }

            signA = SignF16UI(uiA);
            signB = SignF16UI(uiB);

            return (signA != signB)
                ? (signA && (ushort)((uiA | uiB) << 1) != 0)
                : (uiA != uiB && (signA ^ (uiA < uiB)));
        }

        #endregion

        #endregion
    }
}
