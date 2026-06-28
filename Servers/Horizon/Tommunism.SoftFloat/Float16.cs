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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Tommunism.SoftFloat;

using static Internals;
using static Primitives;

[StructLayout(LayoutKind.Sequential, Pack = sizeof(ushort), Size = sizeof(ushort))]
public readonly struct Float16 : ISpanFormattable
{
    #region Fields

    public const int ExponentBias = 0xF;

    // WARNING: DO NOT ADD OR CHANGE ANY OF THESE FIELDS!!!
    private readonly ushort _v;

    #endregion

    #region Constructors

    [SuppressMessage(
        "Style",
        "IDE0060:Remove unused parameter",
        Justification = "Used to avoid accidentally calling other overloads."
    )]
    private Float16(ushort v, bool dummy)
    {
        _v = v;
    }

    public Float16(Half value)
    {
        _v = BitConverter.HalfToUInt16Bits(value);
    }

    // NOTE: The exponential is the biased exponent value (not the bit encoded value).
    public Float16(bool sign, int exponent, uint significand)
    {
        exponent += ExponentBias;
        if ((exponent >> 5) != 0)
            throw new ArgumentOutOfRangeException(nameof(exponent));

        if ((significand >> 10) != 0)
            throw new ArgumentOutOfRangeException(nameof(significand));

        _v = PackToUI(sign, exponent, significand);
    }

    #endregion

    #region Properties

    public bool Sign => GetSignUI(_v);

    public int Exponent => GetExpUI(_v) - ExponentBias; // offset-binary

    public uint Significand => GetFracUI(_v);

    public bool IsNaN => IsNaNUI(_v);

    public bool IsInfinity => IsInfUI(_v);

    public bool IsFinite => IsFiniteUI(_v);

    #endregion

    #region Methods

    public static explicit operator Float16(Half value) => new(value);

    public static implicit operator Half(Float16 value) => BitConverter.UInt16BitsToHalf(value._v);

    public static Float16 FromUIntBits(ushort value) => new(value, dummy: false);

    public ushort ToUInt16Bits() => _v;

    // THIS IS THE INTERNAL CONSTRUCTOR FOR RAW BITS.
    // TODO: Allow value to be a full 32-bit integer (reduces total number of "unnecessary" casts).
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Float16 FromBitsUI16(uint v) => new((ushort)v, dummy: false);

    public bool TryFormat(
        Span<char> destination,
        out int charsWritten,
        ReadOnlySpan<char> format,
        IFormatProvider? provider
    )
    {
        if (IsHexFormat(format, out var isLowerCase))
        {
            var builder = new ValueStringBuilder(destination, canGrow: false);
            try
            {
                FormatValueHex(ref builder, isLowerCase);
                charsWritten = builder.Length;
                return true;
            }
            catch (FormatException)
            {
                // This exception is thrown if ValueStringBuilder wants to grow but cannot.
                charsWritten = default;
                return false;
            }
        }
        else if (IsExpFormatOrDefault(format, out var replacedFormat))
        {
            // FloatingDecimal128 is only "good" when using compact formats like exponent or possibly general.
            var floatingDecimal = new FloatingDecimal128(this);
            return floatingDecimal.TryFormat(
                destination,
                out charsWritten,
                replacedFormat != null ? replacedFormat.AsSpan() : format,
                provider
            );
        }
        else
        {
            throw new ArgumentException(
                "Given format is not currently implemented or supported.",
                nameof(format)
            );
        }
    }

    public override string ToString() => ToString(null, null);

    public string ToString(string? format) => ToString(format, null);

    public string ToString(IFormatProvider? formatProvider) => ToString(null, formatProvider);

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        if (IsHexFormat(format, out var isLowerCase))
        {
            var builder = new ValueStringBuilder(stackalloc char[7]);
            FormatValueHex(ref builder, isLowerCase);
            return builder.ToString();
        }
        else if (IsExpFormatOrDefault(format, out var replacedFormat))
        {
            // FloatingDecimal128 is only "good" when using compact formats like exponent or possibly general.
            var floatingDecimal = new FloatingDecimal128(this);
            return floatingDecimal.ToString(replacedFormat ?? format, formatProvider);
        }
        else
        {
            throw new ArgumentException(
                "Given format is not currently implemented or supported.",
                nameof(format)
            );
        }
    }

    private void FormatValueHex(ref ValueStringBuilder builder, bool isLowerCase)
    {
        // NOTE: This is the raw exponent and significand encoded in hexadecimal, separated by a period, and prefixed with the sign.
        // Value Format: -1F.3FF
        builder.Append(GetSignUI(_v) ? '-' : '+');
        builder.AppendHex((uint)GetExpUI(_v), 5, isLowerCase);
        builder.Append('.');
        builder.AppendHex(GetFracUI(_v), 10, isLowerCase);
    }

    #region Integer-to-floating-point Conversions

    // ui32_to_f16
    public static Float16 FromUInt32(SoftFloatContext context, uint a)
    {
        var shiftDist = CountLeadingZeroes32(a) - 21;
        if (0 <= shiftDist)
            return FromBitsUI16(a != 0 ? PackToUI(false, 0x18 - shiftDist, a << shiftDist) : 0U);

        shiftDist += 4;
        var sig =
            (shiftDist < 0)
                ? ((a >> (-shiftDist)) | ((a << shiftDist) != 0 ? 1U : 0))
                : (a << shiftDist);
        return RoundPack(context, false, 0x1C - shiftDist, sig);
    }

    // ui64_to_f16
    public static Float16 FromUInt64(SoftFloatContext context, ulong a)
    {
        var shiftDist = CountLeadingZeroes64(a) - 53;
        if (0 <= shiftDist)
            return FromBitsUI16(
                a != 0 ? PackToUI(false, 0x18 - shiftDist, (uint)a << shiftDist) : 0U
            );

        shiftDist += 4;
        var sig = (shiftDist < 0) ? (uint)a.ShortShiftRightJam(-shiftDist) : ((uint)a << shiftDist);
        return RoundPack(context, false, 0x1C - shiftDist, sig);
    }

    // i32_to_f16
    public static Float16 FromInt32(SoftFloatContext context, int a)
    {
        var sign = a < 0;
        var absA = (uint)(sign ? -a : a);
        var shiftDist = CountLeadingZeroes32(absA) - 21;
        if (0 <= shiftDist)
            return FromBitsUI16(a != 0 ? PackToUI(sign, 0x18 - shiftDist, absA << shiftDist) : 0U);

        shiftDist += 4;
        var sig =
            (shiftDist < 0)
                ? (absA >> (-shiftDist)) | ((absA << shiftDist) != 0 ? 1U : 0U)
                : (absA << shiftDist);
        return RoundPack(context, sign, 0x1C - shiftDist, sig);
    }

    // i64_to_f16
    public static Float16 FromInt64(SoftFloatContext context, long a)
    {
        var sign = a < 0;
        var absA = (ulong)(sign ? -a : a);
        var shiftDist = CountLeadingZeroes64(absA) - 53;
        if (0 <= shiftDist)
            return FromBitsUI16(
                a != 0 ? PackToUI(sign, 0x18 - shiftDist, (uint)absA << shiftDist) : 0U
            );

        shiftDist += 4;
        var sig =
            (shiftDist < 0) ? (uint)absA.ShortShiftRightJam(-shiftDist) : ((uint)absA << shiftDist);
        return RoundPack(context, sign, 0x1C - shiftDist, sig);
    }

    #endregion

    #region Floating-point-to-integer Conversions

    public uint ToUInt32(SoftFloatContext context, bool exact) =>
        ToUInt32(context, context.Rounding, exact);

    public ulong ToUInt64(SoftFloatContext context, bool exact) =>
        ToUInt64(context, context.Rounding, exact);

    public int ToInt32(SoftFloatContext context, bool exact) =>
        ToInt32(context, context.Rounding, exact);

    public long ToInt64(SoftFloatContext context, bool exact) =>
        ToInt64(context, context.Rounding, exact);

    // f16_to_ui32
    public uint ToUInt32(SoftFloatContext context, RoundingMode roundingMode, bool exact)
    {
        uint uiA,
            frac;
        bool sign;
        int exp,
            shiftDist;
        uint sig32;

        uiA = _v;
        sign = GetSignUI(uiA);
        exp = GetExpUI(uiA);
        frac = GetFracUI(uiA);

        if (exp == 0x1F)
        {
            context.RaiseFlags(ExceptionFlags.Invalid);
            return (frac != 0) ? context.UInt32FromNaN : context.UInt32FromOverflow(sign);
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
        uint uiA,
            frac;
        bool sign;
        int exp,
            shiftDist;
        uint sig32;

        uiA = _v;
        sign = GetSignUI(uiA);
        exp = GetExpUI(uiA);
        frac = GetFracUI(uiA);

        if (exp == 0x1F)
        {
            context.RaiseFlags(ExceptionFlags.Invalid);
            return (frac != 0) ? context.UInt64FromNaN : context.UInt64FromOverflow(sign);
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
        uint uiA,
            frac;
        bool sign;
        int exp,
            shiftDist;
        int sig32;

        uiA = _v;
        sign = GetSignUI(uiA);
        exp = GetExpUI(uiA);
        frac = GetFracUI(uiA);

        if (exp == 0x1F)
        {
            context.RaiseFlags(ExceptionFlags.Invalid);
            return (frac != 0) ? context.Int32FromNaN : context.Int32FromOverflow(sign);
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
        uint uiA,
            frac;
        bool sign;
        int exp,
            shiftDist;
        int sig32;

        uiA = _v;
        sign = GetSignUI(uiA);
        exp = GetExpUI(uiA);
        frac = GetFracUI(uiA);

        if (exp == 0x1F)
        {
            context.RaiseFlags(ExceptionFlags.Invalid);
            return (frac != 0) ? context.Int64FromNaN : context.Int64FromOverflow(sign);
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
        uint uiA,
            frac;
        int exp,
            shiftDist;
        bool sign;
        uint alignedSig;

        uiA = _v;
        exp = GetExpUI(uiA);
        frac = GetFracUI(uiA);

        shiftDist = exp - 0x0F;
        if (shiftDist < 0)
        {
            if (exact && ((uint)exp | frac) != 0)
                context.ExceptionFlags |= ExceptionFlags.Inexact;

            return 0;
        }

        sign = GetSignUI(uiA);
        if (sign || exp == 0x1F)
        {
            context.RaiseFlags(ExceptionFlags.Invalid);
            return (frac != 0) ? context.UInt32FromNaN : context.UInt32FromOverflow(sign);
        }

        alignedSig = (frac | 0x0400) << shiftDist;
        if (exact && (alignedSig & 0x3FF) != 0)
            context.ExceptionFlags |= ExceptionFlags.Inexact;

        return alignedSig >> 10;
    }

    // f16_to_ui64_r_minMag
    public ulong ToUInt64RoundMinMag(SoftFloatContext context, bool exact)
    {
        uint uiA,
            frac;
        int exp,
            shiftDist;
        bool sign;
        uint alignedSig;

        uiA = _v;
        exp = GetExpUI(uiA);
        frac = GetFracUI(uiA);

        shiftDist = exp - 0x0F;
        if (shiftDist < 0)
        {
            if (exact && ((uint)exp | frac) != 0)
                context.ExceptionFlags |= ExceptionFlags.Inexact;

            return 0;
        }

        sign = GetSignUI(uiA);
        if (sign || exp == 0x1F)
        {
            context.RaiseFlags(ExceptionFlags.Invalid);
            return (frac != 0) ? context.UInt64FromNaN : context.UInt64FromOverflow(sign);
        }

        alignedSig = (frac | 0x0400) << shiftDist;
        if (exact && (alignedSig & 0x3FF) != 0)
            context.ExceptionFlags |= ExceptionFlags.Inexact;

        return alignedSig >> 10;
    }

    // f16_to_i32_r_minMag
    public int ToInt32RoundMinMag(SoftFloatContext context, bool exact)
    {
        uint uiA,
            frac;
        bool sign;
        int exp,
            shiftDist;
        int alignedSig;

        uiA = _v;
        exp = GetExpUI(uiA);
        frac = GetFracUI(uiA);

        shiftDist = exp - 0x0F;
        if (shiftDist < 0)
        {
            if (exact && ((uint)exp | frac) != 0)
                context.ExceptionFlags |= ExceptionFlags.Inexact;

            return 0;
        }

        sign = GetSignUI(uiA);
        if (exp == 0x1F)
        {
            context.RaiseFlags(ExceptionFlags.Invalid);
            return (frac != 0) ? context.Int32FromNaN : context.Int32FromOverflow(sign);
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
        uint uiA,
            frac;
        bool sign;
        int exp,
            shiftDist;
        int alignedSig;

        uiA = _v;
        exp = GetExpUI(uiA);
        frac = GetFracUI(uiA);

        shiftDist = exp - 0x0F;
        if (shiftDist < 0)
        {
            if (exact && ((uint)exp | frac) != 0)
                context.ExceptionFlags |= ExceptionFlags.Inexact;

            return 0;
        }

        sign = GetSignUI(uiA);
        if (exp == 0x1F)
        {
            context.RaiseFlags(ExceptionFlags.Invalid);
            return (frac != 0) ? context.Int64FromNaN : context.Int64FromOverflow(sign);
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
        uint uiA,
            frac;
        bool sign;
        int exp;

        uiA = _v;
        sign = GetSignUI(uiA);
        exp = GetExpUI(uiA);
        frac = GetFracUI(uiA);

        if (exp == 0x1F)
        {
            if (frac != 0)
            {
                context.Float16BitsToCommonNaN(uiA, out var commonNaN);
                return context.CommonNaNToFloat32(in commonNaN);
            }

            return Float32.Pack(sign, 0xFF, 0);
        }
        else if (exp == 0)
        {
            if (frac == 0)
                return Float32.Pack(sign, 0, 0);

            (exp, frac) = NormSubnormalSig(frac);
            exp--;
        }

        return Float32.Pack(sign, exp + 0x70, frac << 13);
    }

    // f16_to_f64
    public Float64 ToFloat64(SoftFloatContext context)
    {
        uint uiA,
            frac;
        bool sign;
        int exp;

        uiA = _v;
        sign = GetSignUI(uiA);
        exp = GetExpUI(uiA);
        frac = GetFracUI(uiA);

        if (exp == 0x1F)
        {
            if (frac != 0)
            {
                context.Float16BitsToCommonNaN(uiA, out var commonNaN);
                return context.CommonNaNToFloat64(in commonNaN);
            }

            return Float64.Pack(sign, 0x7FF, 0);
        }
        else if (exp == 0)
        {
            if (frac == 0)
                return Float64.Pack(sign, 0, 0);

            (exp, frac) = NormSubnormalSig(frac);
            exp--;
        }

        return Float64.Pack(sign, exp + 0x3F0, (ulong)frac << 42);
    }

    // f16_to_extF80
    public ExtFloat80 ToExtFloat80(SoftFloatContext context)
    {
        uint uiA,
            frac;
        bool sign;
        int exp;

        uiA = _v;
        sign = GetSignUI(uiA);
        exp = GetExpUI(uiA);
        frac = GetFracUI(uiA);

        if (exp == 0x1F)
        {
            if (frac != 0)
            {
                context.Float16BitsToCommonNaN(uiA, out var commonNaN);
                return context.CommonNaNToExtFloat80(in commonNaN);
            }

            return ExtFloat80.Pack(sign, 0x7FFF, 0x8000000000000000);
        }
        else if (exp == 0)
        {
            if (frac == 0)
                return ExtFloat80.Pack(sign, 0, 0);

            (exp, frac) = NormSubnormalSig(frac);
        }

        return ExtFloat80.Pack(sign, exp + 0x3FF0, (ulong)(frac | 0x0400) << 53);
    }

    // f16_to_f128
    public Float128 ToFloat128(SoftFloatContext context)
    {
        uint uiA,
            frac;
        bool sign;
        int exp;

        uiA = _v;
        sign = GetSignUI(uiA);
        exp = GetExpUI(uiA);
        frac = GetFracUI(uiA);

        if (exp == 0x1F)
        {
            if (frac != 0)
            {
                context.Float16BitsToCommonNaN(uiA, out var commonNaN);
                return context.CommonNaNToFloat128(in commonNaN);
            }

            return Float128.Pack(sign, 0x7FFF, 0, 0);
        }
        else if (exp == 0)
        {
            if (frac == 0)
                return Float128.Pack(sign, 0, 0, 0);

            (exp, frac) = NormSubnormalSig(frac);
            exp--;
        }

        return Float128.Pack(sign, exp + 0x3FF0, (ulong)frac << 38, 0);
    }

    #endregion

    #region Arithmetic Operations

    public Float16 RoundToInt(SoftFloatContext context, bool exact) =>
        RoundToInt(context, context.Rounding, exact);

    // f16_roundToInt
    public Float16 RoundToInt(SoftFloatContext context, RoundingMode roundingMode, bool exact)
    {
        uint uiA,
            uiZ,
            lastBitMask,
            roundBitsMask;
        int exp;

        uiA = _v;
        exp = GetExpUI(uiA);

        if (exp <= 0xE)
        {
            if ((ushort)(uiA << 1) == 0)
                return this;

            if (exact)
                context.ExceptionFlags |= ExceptionFlags.Inexact;

            uiZ = uiA & PackToUI(true, 0, 0);
            switch (roundingMode)
            {
                case RoundingMode.NearEven:
                {
                    if (GetFracUI(uiA) != 0)
                        goto case RoundingMode.NearMaxMag;

                    break;
                }
                case RoundingMode.NearMaxMag:
                {
                    if (exp == 0xE)
                        uiZ |= PackToUI(false, 0xF, 0);

                    break;
                }
                case RoundingMode.Min:
                {
                    if (uiZ != 0)
                        uiZ = PackToUI(true, 0xF, 0);

                    break;
                }
                case RoundingMode.Max:
                {
                    if (uiZ == 0)
                        uiZ = PackToUI(false, 0xF, 0);

                    break;
                }
                case RoundingMode.Odd:
                {
                    uiZ |= PackToUI(false, 0xF, 0);
                    break;
                }
            }

            return FromBitsUI16(uiZ);
        }

        if (0x19 <= exp)
        {
            return exp == 0x1F && GetFracUI(uiA) != 0 ? context.PropagateNaNFloat16(uiA, 0) : this;
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
        else if (roundingMode == (GetSignUI(uiZ) ? RoundingMode.Min : RoundingMode.Max))
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

        return FromBitsUI16(uiZ);
    }

    // f16_add
    public static Float16 Add(SoftFloatContext context, Float16 a, Float16 b)
    {
        uint uiA,
            uiB;

        uiA = a._v;
        uiB = b._v;

        return GetSignUI(uiA ^ uiB) ? SubMags(context, uiA, uiB) : AddMags(context, uiA, uiB);
    }

    // f16_sub
    public static Float16 Subtract(SoftFloatContext context, Float16 a, Float16 b)
    {
        uint uiA,
            uiB;

        uiA = a._v;
        uiB = b._v;

        return GetSignUI(uiA ^ uiB) ? AddMags(context, uiA, uiB) : SubMags(context, uiA, uiB);
    }

    // f16_mul
    public static Float16 Multiply(SoftFloatContext context, Float16 a, Float16 b)
    {
        uint uiA,
            sigA,
            uiB,
            sigB,
            sigZ;
        int expA,
            expB,
            expZ;
        uint sig32Z;
        bool signA,
            signB,
            signZ;

        uiA = a._v;
        signA = GetSignUI(uiA);
        expA = GetExpUI(uiA);
        sigA = GetFracUI(uiA);
        uiB = b._v;
        signB = GetSignUI(uiB);
        expB = GetExpUI(uiB);
        sigB = GetFracUI(uiB);
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

            return Pack(signZ, 0x1F, 0);
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

            return Pack(signZ, 0x1F, 0);
        }

        if (expA == 0)
        {
            if (sigA == 0)
                return Pack(signZ, 0, 0);

            (expA, sigA) = NormSubnormalSig(sigA);
        }

        if (expB == 0)
        {
            if (sigB == 0)
                return Pack(signZ, 0, 0);

            (expB, sigB) = NormSubnormalSig(sigB);
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

        return RoundPack(context, signZ, expZ, sigZ);
    }

    // f16_mulAdd
    public static Float16 MultiplyAndAdd(
        SoftFloatContext context,
        Float16 a,
        Float16 b,
        Float16 c
    ) => MulAdd(context, a._v, b._v, c._v, MulAddOperation.None);

    // WARNING: This method overload is experimental and has not been thoroughly tested!
    public static Float16 MultiplyAndAdd(
        SoftFloatContext context,
        Float16 a,
        Float16 b,
        Float16 c,
        MulAddOperation operation
    )
    {
        return
            operation
                is not MulAddOperation.None
                    and not MulAddOperation.SubtractC
                    and not MulAddOperation.SubtractProduct
            ? throw new ArgumentException("Invalid multiply-and-add operation.", nameof(operation))
            : MulAdd(context, a._v, b._v, c._v, operation);
    }

    // f16_div
    public static Float16 Divide(SoftFloatContext context, Float16 a, Float16 b)
    {
        uint uiA,
            sigA,
            uiB,
            sigB,
            sig32A,
            sigZ;
        int expA,
            expB,
            expZ;
        bool signA,
            signB,
            signZ;

        uiA = a._v;
        signA = GetSignUI(uiA);
        expA = GetExpUI(uiA);
        sigA = GetFracUI(uiA);
        uiB = b._v;
        signB = GetSignUI(uiB);
        expB = GetExpUI(uiB);
        sigB = GetFracUI(uiB);
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

            return Pack(signZ, 0x1F, 0);
        }
        else if (expB == 0x1F)
        {
            return sigB != 0 ? context.PropagateNaNFloat16(uiA, uiB) : Pack(signZ, 0, 0);
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
                return Pack(signZ, 0x1F, 0);
            }

            (expB, sigB) = NormSubnormalSig(sigB);
        }

        if (expA == 0)
        {
            if (sigA == 0)
                return Pack(signZ, 0, 0);

            (expA, sigA) = NormSubnormalSig(sigA);
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

        return RoundPack(context, signZ, expZ, sigZ);
    }

    // f16_rem
    public static Float16 Remainder(SoftFloatContext context, Float16 a, Float16 b)
    {
        uint uiA,
            sigA,
            uiB,
            sigB,
            q;
        int expA,
            expB,
            expDiff;
        ushort rem,
            altRem,
            meanRem;
        uint recip32,
            q32;
        bool signA,
            signRem;

        uiA = a._v;
        signA = GetSignUI(uiA);
        expA = GetExpUI(uiA);
        sigA = GetFracUI(uiA);
        uiB = b._v;
        expB = GetExpUI(uiB);
        sigB = GetFracUI(uiB);

        if (expA == 0x1F)
        {
            if (sigA != 0 || (expB == 0x1F && sigB != 0))
                return context.PropagateNaNFloat16(uiA, uiB);

            context.RaiseFlags(ExceptionFlags.Invalid);
            return context.DefaultNaNFloat16;
        }
        else if (expB == 0x1F)
        {
            return sigB != 0 ? context.PropagateNaNFloat16(uiA, uiB) : a;
        }

        if (expB == 0)
        {
            if (sigB == 0)
            {
                context.RaiseFlags(ExceptionFlags.Invalid);
                return context.DefaultNaNFloat16;
            }

            (expB, sigB) = NormSubnormalSig(sigB);
        }

        if (expA == 0)
        {
            if (sigA == 0)
                return a;

            (expA, sigA) = NormSubnormalSig(sigA);
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
        } while ((rem & 0x8000) == 0);

        meanRem = (ushort)(rem + altRem);
        if ((meanRem & 0x8000) != 0 || (meanRem == 0 && (q & 1) != 0))
            rem = altRem;

        signRem = signA;
        if (0x8000 <= rem)
        {
            signRem = !signRem;
            rem = (ushort)(-(short)rem);
        }

        return NormRoundPack(context, signRem, expB, rem);
    }

    // f16_sqrt
    public Float16 SquareRoot(SoftFloatContext context)
    {
        uint uiA,
            sigA,
            r0,
            recipSqrt16,
            sigZ,
            shiftedSigZ;
        int expA,
            expZ;
        int index;
        uint ESqrR0;
        ushort sigma0,
            negRem;
        bool signA;

        uiA = _v;
        signA = GetSignUI(uiA);
        expA = GetExpUI(uiA);
        sigA = GetFracUI(uiA);

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

            (expA, sigA) = NormSubnormalSig(sigA);
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

        return RoundPack(context, false, expZ, sigZ);
    }

    #endregion

    #region Comparison Operations

    // f16_eq (signaling=false) & f16_eq_signaling (signaling=true)
    public static bool CompareEqual(SoftFloatContext context, Float16 a, Float16 b, bool signaling)
    {
        uint uiA,
            uiB;

        uiA = a._v;
        uiB = b._v;

        if (IsNaNUI(uiA) || IsNaNUI(uiB))
        {
            if (
                signaling
                || context.IsSignalingNaNFloat16Bits(uiA)
                || context.IsSignalingNaNFloat16Bits(uiB)
            )
                context.RaiseFlags(ExceptionFlags.Invalid);

            return false;
        }

        return (uiA == uiB) || (ushort)((uiA | uiB) << 1) == 0;
    }

    // f16_le (signaling=true) & f16_le_quiet (signaling=false)
    public static bool CompareLessThanOrEqual(
        SoftFloatContext context,
        Float16 a,
        Float16 b,
        bool signaling
    )
    {
        uint uiA,
            uiB;
        bool signA,
            signB;

        uiA = a._v;
        uiB = b._v;

        if (IsNaNUI(uiA) || IsNaNUI(uiB))
        {
            if (
                signaling
                || context.IsSignalingNaNFloat16Bits(uiA)
                || context.IsSignalingNaNFloat16Bits(uiB)
            )
                context.RaiseFlags(ExceptionFlags.Invalid);

            return false;
        }

        signA = GetSignUI(uiA);
        signB = GetSignUI(uiB);

        return (signA != signB)
            ? (signA || (ushort)((uiA | uiB) << 1) == 0)
            : (uiA == uiB || (signA ^ (uiA < uiB)));
    }

    // f16_lt (signaling=true) & f16_lt_quiet (signaling=false)
    public static bool CompareLessThan(
        SoftFloatContext context,
        Float16 a,
        Float16 b,
        bool signaling
    )
    {
        uint uiA,
            uiB;
        bool signA,
            signB;

        uiA = a._v;
        uiB = b._v;

        if (IsNaNUI(uiA) || IsNaNUI(uiB))
        {
            if (
                signaling
                || context.IsSignalingNaNFloat16Bits(uiA)
                || context.IsSignalingNaNFloat16Bits(uiB)
            )
                context.RaiseFlags(ExceptionFlags.Invalid);

            return false;
        }

        signA = GetSignUI(uiA);
        signB = GetSignUI(uiB);

        return (signA != signB)
            ? (signA && (ushort)((uiA | uiB) << 1) != 0)
            : (uiA != uiB && (signA ^ (uiA < uiB)));
    }

    #endregion

    #region Internals

    // signF16UI
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool GetSignUI(uint a) => ((a >> 15) & 1) != 0;

    // expF16UI
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int GetExpUI(uint a) => (int)((a >> 10) & 0x1F);

    // fracF16UI
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static uint GetFracUI(uint a) => a & 0x03FF;

    // packToF16UI
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ushort PackToUI(bool sign, int exp, uint sig) =>
        (ushort)((sign ? (1U << 15) : 0U) + (ushort)((uint)exp << 10) + sig);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Float16 Pack(bool sign, int exp, uint sig) =>
        FromBitsUI16(PackToUI(sign, exp, sig));

    // isNaNF16UI
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsNaNUI(uint a) => (~a & 0x7C00) == 0 && (a & 0x03FF) != 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsInfUI(uint a) => (~a & 0x7C00) == 0 && (a & 0x03FF) == 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsFiniteUI(uint a) => (~a & 0x7C00) != 0;

    // softfloat_normSubnormalF16Sig
    internal static (int exp, uint sig) NormSubnormalSig(uint sig)
    {
        var shiftDist = CountLeadingZeroes16(sig) - 5;
        return (exp: 1 - shiftDist, sig: sig << shiftDist);
    }

    // softfloat_roundPackToF16
    internal static Float16 RoundPack(SoftFloatContext context, bool sign, int exp, uint sig)
    {
        var roundingMode = context.Rounding;
        var roundNearEven = roundingMode == RoundingMode.NearEven;
        var roundIncrement =
            (!roundNearEven && roundingMode != RoundingMode.NearMaxMag)
                ? ((roundingMode == (sign ? RoundingMode.Min : RoundingMode.Max)) ? 0xFU : 0)
                : 0x8U;
        var roundBits = sig & 0xF;

        if (0x1D <= (uint)exp)
        {
            if (exp < 0)
            {
                var isTiny =
                    context.DetectTininess == TininessMode.BeforeRounding
                    || exp < -1
                    || sig + roundIncrement < 0x8000;
                sig = sig.ShiftRightJam(-exp);
                exp = 0;
                roundBits = sig & 0xF;

                if (isTiny && roundBits != 0)
                    context.RaiseFlags(ExceptionFlags.Underflow);
            }
            else if (0x1D < exp || 0x8000 <= sig + roundIncrement)
            {
                context.RaiseFlags(ExceptionFlags.Overflow | ExceptionFlags.Inexact);
                return FromBitsUI16(PackToUI(sign, 0x1F, 0) - (roundIncrement == 0 ? 1U : 0U));
            }
        }

        sig = ((sig + roundIncrement) >> 4);
        if (roundBits != 0)
        {
            context.ExceptionFlags |= ExceptionFlags.Inexact;
            if (roundingMode == RoundingMode.Odd)
            {
                sig |= 1;
                return Pack(sign, exp, sig);
            }
        }

        sig &= ~(((roundBits ^ 8) == 0 & roundNearEven) ? 1U : 0U);
        if (sig == 0)
            exp = 0;

        return Pack(sign, exp, sig);
    }

    // softfloat_normRoundPackToF16
    internal static Float16 NormRoundPack(SoftFloatContext context, bool sign, int exp, uint sig)
    {
        var shiftDist = CountLeadingZeroes16(sig) - 1;
        exp -= shiftDist;
        return 4 <= shiftDist && (uint)exp < 0x1D
            ? Pack(sign, sig != 0 ? exp : 0, sig << (shiftDist - 4))
            : RoundPack(context, sign, exp, sig << shiftDist);
    }

    // softfloat_addMagsF16
    internal static Float16 AddMags(SoftFloatContext context, uint uiA, uint uiB)
    {
        int expA,
            expB,
            expDiff,
            expZ,
            shiftDist;
        uint sigA,
            sigB,
            sigZ,
            uiZ,
            sigX,
            sigY;
        uint sig32Z;
        bool signZ;

        expA = GetExpUI(uiA);
        sigA = GetFracUI(uiA);
        expB = GetExpUI(uiB);
        sigB = GetFracUI(uiB);

        expDiff = expA - expB;
        if (expDiff == 0)
        {
            if (expA == 0)
                return FromBitsUI16(uiA + sigB);

            if (expA == 0x1F)
            {
                return (sigA | sigB) != 0
                    ? context.PropagateNaNFloat16(uiA, uiB)
                    : FromBitsUI16(uiA);
            }

            signZ = GetSignUI(uiA);
            expZ = expA;
            sigZ = 0x0800 + sigA + sigB;
            if ((sigZ & 1) == 0 && expZ < 0x1E)
                return Pack(signZ, expZ, sigZ >> 1);

            sigZ <<= 3;
        }
        else
        {
            signZ = GetSignUI(uiA);
            if (expDiff < 0)
            {
                if (expB == 0x1F)
                {
                    return sigB != 0 ? context.PropagateNaNFloat16(uiA, uiB) : Pack(signZ, 0x1F, 0);
                }

                if (expDiff <= -13)
                {
                    uiZ = PackToUI(signZ, expB, sigB);
                    if (((uint)expA | sigA) != 0)
                        goto addEpsilon;

                    return FromBitsUI16(uiZ);
                }

                expZ = expB;
                sigX = sigB | 0x0400;
                sigY = sigA + (expA != 0 ? 0x0400 : sigA);
                shiftDist = 19 + expDiff;
            }
            else
            {
                uiZ = uiA;
                if (expA == 0x1F)
                {
                    return sigA != 0 ? context.PropagateNaNFloat16(uiA, uiB) : FromBitsUI16(uiZ);
                }

                if (13 <= expDiff)
                {
                    if (((uint)expB | sigB) != 0)
                        goto addEpsilon;

                    return FromBitsUI16(uiZ);
                }

                expZ = expA;
                sigX = sigA | 0x0400;
                sigY = sigB + (expB != 0 ? 0x0400 : sigB);
                shiftDist = 19 - expDiff;
            }

            sig32Z = (sigX << 19) + (sigY << shiftDist);
            if (sig32Z < 0x40000000)
            {
                --expZ;
                sig32Z <<= 1;
            }

            sigZ = sig32Z >> 16;
            if ((sig32Z & 0xFFFF) != 0)
            {
                sigZ |= 1;
            }
            else
            {
                if ((sigZ & 0xF) == 0 && expZ < 0x1E)
                {
                    sigZ >>= 4;
                    return Pack(signZ, expZ, sigZ);
                }
            }
        }

        return RoundPack(context, signZ, expZ, sigZ);

        addEpsilon:
        var roundingMode = context.Rounding;
        if (roundingMode != RoundingMode.NearEven)
        {
            if (roundingMode == (GetSignUI(uiZ) ? RoundingMode.Min : RoundingMode.Max))
            {
                ++uiZ;
                if ((ushort)(uiZ << 1) == 0xF800U)
                    context.RaiseFlags(ExceptionFlags.Overflow | ExceptionFlags.Inexact);
            }
            else if (roundingMode == RoundingMode.Odd)
            {
                uiZ |= 1;
            }
        }

        context.ExceptionFlags |= ExceptionFlags.Inexact;
        return FromBitsUI16(uiZ);
    }

    // softfloat_subMagsF16
    internal static Float16 SubMags(SoftFloatContext context, uint uiA, uint uiB)
    {
        int expA,
            expB,
            expDiff,
            expZ,
            shiftDist;
        uint sigA,
            sigB,
            uiZ,
            sigZ,
            sigX,
            sigY;
        int sigDiff;
        bool signZ;

        expA = GetExpUI(uiA);
        sigA = GetFracUI(uiA);
        expB = GetExpUI(uiB);
        sigB = GetFracUI(uiB);

        expDiff = expA - expB;
        if (expDiff == 0)
        {
            if (expA == 0x1F)
            {
                if ((sigA | sigB) != 0)
                    return context.PropagateNaNFloat16(uiA, uiB);

                context.RaiseFlags(ExceptionFlags.Invalid);
                return context.DefaultNaNFloat16;
            }

            sigDiff = (int)sigA - (int)sigB;
            if (sigDiff == 0)
                return Pack(context.Rounding == RoundingMode.Min, 0, 0);

            if (expA != 0)
                --expA;

            signZ = GetSignUI(uiA);
            if (sigDiff < 0)
            {
                signZ = !signZ;
                sigDiff = -sigDiff;
            }

            Debug.Assert(sigDiff >= 0);
            shiftDist = CountLeadingZeroes16((uint)sigDiff) - 5;
            expZ = expA - shiftDist;
            if (expZ < 0)
            {
                shiftDist = expA;
                expZ = 0;
            }

            return Pack(signZ, expZ, (uint)sigDiff << shiftDist);
        }
        else
        {
            signZ = GetSignUI(uiA);
            if (expDiff < 0)
            {
                signZ = !signZ;
                if (expB == 0x1F)
                {
                    return sigB != 0 ? context.PropagateNaNFloat16(uiA, uiB) : Pack(signZ, 0x1F, 0);
                }

                if (expDiff <= -13)
                {
                    uiZ = PackToUI(signZ, expB, sigB);
                    if (((uint)expA | sigA) != 0)
                        goto subEpsilon;

                    return FromBitsUI16(uiZ);
                }

                expZ = expA + 19;
                sigX = sigB | 0x0400;
                sigY = sigA + (expA != 0 ? 0x0400 : sigA);
                expDiff = -expDiff;
            }
            else
            {
                uiZ = uiA;
                if (expA == 0x1F)
                {
                    return sigA != 0 ? context.PropagateNaNFloat16(uiA, uiB) : FromBitsUI16(uiZ);
                }

                if (13 <= expDiff)
                {
                    if (((uint)expB | sigB) != 0)
                        goto subEpsilon;

                    return FromBitsUI16(uiZ);
                }

                expZ = expB + 19;
                sigX = sigA | 0x0400;
                sigY = sigB + (expB != 0 ? 0x0400 : sigB);
            }

            var sig32Z = (sigX << expDiff) - sigY;
            shiftDist = CountLeadingZeroes32(sig32Z) - 1;
            sig32Z <<= shiftDist;
            expZ -= shiftDist;
            sigZ = sig32Z >> 16;
            if ((sig32Z & 0xFFFF) != 0)
            {
                sigZ |= 1;
            }
            else
            {
                if ((sigZ & 0xF) == 0 && (uint)expZ < 0x1E)
                {
                    sigZ >>= 4;
                    return Pack(signZ, expZ, sigZ);
                }
            }

            return RoundPack(context, signZ, expZ, sigZ);
        }

        subEpsilon:
        var roundingMode = context.Rounding;
        if (roundingMode != RoundingMode.NearEven)
        {
            if (
                roundingMode == RoundingMode.MinMag
                || (roundingMode == (GetSignUI(uiZ) ? RoundingMode.Max : RoundingMode.Min))
            )
            {
                --uiZ;
            }
            else if (roundingMode == RoundingMode.Odd)
            {
                uiZ = (uiZ - 1) | 1;
            }
        }

        context.ExceptionFlags |= ExceptionFlags.Inexact;
        return FromBitsUI16(uiZ);
    }

    // softfloat_mulAddF16
    internal static Float16 MulAdd(
        SoftFloatContext context,
        uint uiA,
        uint uiB,
        uint uiC,
        MulAddOperation op
    )
    {
        Debug.Assert(
            op
                is MulAddOperation.None
                    or MulAddOperation.SubtractC
                    or MulAddOperation.SubtractProduct,
            "Invalid MulAdd operation."
        );

        bool signA,
            signB,
            signC,
            signProd,
            signZ;
        int expA,
            expB,
            expC,
            expProd,
            expZ,
            expDiff,
            shiftDist;
        uint sigA,
            sigB,
            sigC,
            magBits,
            uiZ,
            sigZ;
        uint sigProd,
            sig32Z,
            sig32C;

        signA = GetSignUI(uiA);
        expA = GetExpUI(uiA);
        sigA = GetFracUI(uiA);

        signB = GetSignUI(uiB);
        expB = GetExpUI(uiB);
        sigB = GetFracUI(uiB);

        signC = GetSignUI(uiC) ^ (op == MulAddOperation.SubtractC);
        expC = GetExpUI(uiC);
        sigC = GetFracUI(uiC);

        signProd = signA ^ signB ^ (op == MulAddOperation.SubtractProduct);

        if (expA == 0x1F)
        {
            if (sigA != 0 || (expB == 0x1F && sigB != 0))
                return context.PropagateNaNFloat16(uiA, uiB, uiC);

            magBits = (uint)expB | sigB;
            goto infProdArg;
        }

        if (expB == 0x1F)
        {
            if (sigB != 0)
                return context.PropagateNaNFloat16(uiA, uiB, uiC);

            magBits = (uint)expA | sigA;
            goto infProdArg;
        }

        if (expC == 0x1F)
        {
            return sigC != 0 ? context.PropagateNaNFloat16(0, uiC) : FromBitsUI16(uiC);
        }

        if (expA == 0)
        {
            if (sigA == 0)
            {
                return ((uint)expC | sigC) == 0 && signProd != signC
                    ? Pack(context.Rounding == RoundingMode.Min, 0, 0)
                    : FromBitsUI16(uiC);
            }

            (expA, sigA) = NormSubnormalSig(sigA);
        }

        if (expB == 0)
        {
            if (sigB == 0)
            {
                return ((uint)expC | sigC) == 0 && signProd != signC
                    ? Pack(context.Rounding == RoundingMode.Min, 0, 0)
                    : FromBitsUI16(uiC);
            }

            (expB, sigB) = NormSubnormalSig(sigB);
        }

        expProd = expA + expB - 0xE;
        sigA = (sigA | 0x0400) << 4;
        sigB = (sigB | 0x0400) << 4;
        sigProd = sigA * sigB;

        if (sigProd < 0x20000000)
        {
            --expProd;
            sigProd <<= 1;
        }

        signZ = signProd;
        if (expC == 0)
        {
            if (sigC == 0)
            {
                expZ = expProd - 1;
                sigZ = (sigProd >> 15) | ((sigProd & 0x7FFF) != 0 ? 1U : 0U);
                return RoundPack(context, signZ, expZ, sigZ);
            }

            (expC, sigC) = NormSubnormalSig(sigC);
        }

        sigC = (sigC | 0x0400) << 3;
        expDiff = expProd - expC;

        if (signProd == signC)
        {
            if (expDiff <= 0)
            {
                expZ = expC;
                sigZ = sigC + sigProd.ShiftRightJam(16 - expDiff);
            }
            else
            {
                expZ = expProd;
                sig32Z = sigProd + (sigC << 16).ShiftRightJam(expDiff);
                sigZ = (sig32Z >> 16) | ((sig32Z & 0xFFFF) != 0 ? 1U : 0U);
            }

            if (sigZ < 0x4000)
            {
                --expZ;
                sigZ <<= 1;
            }
        }
        else
        {
            sig32C = sigC << 16;
            if (expDiff < 0)
            {
                signZ = signC;
                expZ = expC;
                sig32Z = sig32C - sigProd.ShiftRightJam(-expDiff);
            }
            else if (expDiff == 0)
            {
                expZ = expProd;
                sig32Z = sigProd - sig32C;
                if (sig32Z == 0)
                    return Pack(context.Rounding == RoundingMode.Min, 0, 0);

                if ((sig32Z & 0x80000000) != 0)
                {
                    signZ = !signZ;
                    sig32Z = (uint)(-(int)sig32Z);
                }
            }
            else
            {
                expZ = expProd;
                sig32Z = sigProd - sig32C.ShiftRightJam(expDiff);
            }

            shiftDist = CountLeadingZeroes32(sig32Z) - 1;
            expZ -= shiftDist;
            shiftDist -= 16;
            sigZ =
                (shiftDist < 0)
                    ? (sig32Z >> (-shiftDist)) | ((sig32Z << (shiftDist & 31)) != 0 ? 1U : 0U)
                    : sig32Z << shiftDist;
        }

        return RoundPack(context, signZ, expZ, sigZ);

        infProdArg:
        if (magBits != 0)
        {
            uiZ = PackToUI(signProd, 0x1F, 0);
            if (expC != 0x1F)
                return FromBitsUI16(uiZ);

            if (sigC != 0)
                return context.PropagateNaNFloat16(uiZ, uiC);

            if (signProd == signC)
                return FromBitsUI16(uiZ);
        }

        context.RaiseFlags(ExceptionFlags.Invalid);
        return context.PropagateNaNFloat16(context.DefaultNaNFloat16Bits, uiC);
    }

    #endregion

    #endregion
}
