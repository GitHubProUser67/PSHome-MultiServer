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

[StructLayout(LayoutKind.Sequential, Pack = sizeof(uint), Size = sizeof(uint))]
public readonly struct Float32 : ISpanFormattable
{
    #region Fields

    public const int ExponentBias = 0x7F;

    // WARNING: DO NOT ADD OR CHANGE ANY OF THESE FIELDS!!!
    private readonly uint _v;

    #endregion

    #region Constructors

    [SuppressMessage(
        "Style",
        "IDE0060:Remove unused parameter",
        Justification = "Used to avoid accidentally calling other overloads."
    )]
    private Float32(uint v, bool dummy)
    {
        _v = v;
    }

    public Float32(float value)
    {
        _v = BitConverter.SingleToUInt32Bits(value);
    }

    // NOTE: The exponential is the biased exponent value (not the bit encoded value).
    public Float32(bool sign, int exponent, uint significand)
    {
        exponent += ExponentBias;
        if ((exponent >> 8) != 0)
            throw new ArgumentOutOfRangeException(nameof(exponent));

        if ((significand >> 23) != 0)
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

    public static explicit operator Float32(float value) => new(value);

    public static implicit operator float(Float32 value) =>
        BitConverter.UInt32BitsToSingle(value._v);

    public static Float32 FromUIntBits(uint value) => FromBitsUI32(value);

    public uint ToUInt32Bits() => _v;

    // THIS IS THE INTERNAL CONSTRUCTOR FOR RAW BITS.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Float32 FromBitsUI32(uint v) => new(v, dummy: false);

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
            var builder = new ValueStringBuilder(stackalloc char[10]);
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
        // Value Format: -FF.7FFFFF
        builder.Append(GetSignUI(_v) ? '-' : '+');
        builder.AppendHex((uint)GetExpUI(_v), 8, isLowerCase);
        builder.Append('.');
        builder.AppendHex(GetFracUI(_v), 23, isLowerCase);
    }

    #region Integer-to-floating-point Conversions

    // ui32_to_f32
    public static Float32 FromUInt32(SoftFloatContext context, uint a)
    {
        return a == 0 ? FromBitsUI32(0)
            : (a & 0x80000000) != 0 ? RoundPack(context, false, 0x9D, (a >> 1) | (a & 1))
            : NormRoundPack(context, false, 0x9C, a);
    }

    // ui64_to_f32
    public static Float32 FromUInt64(SoftFloatContext context, ulong a)
    {
        var shiftDist = CountLeadingZeroes64(a) - 40;
        if (0 <= shiftDist)
            return FromBitsUI32(
                a != 0 ? PackToUI(false, 0x95 - shiftDist, (uint)a << shiftDist) : 0U
            );

        shiftDist += 7;
        var sig = (shiftDist < 0) ? (uint)a.ShortShiftRightJam(-shiftDist) : ((uint)a << shiftDist);
        return RoundPack(context, false, 0x9C - shiftDist, sig);
    }

    // i32_to_f32
    public static Float32 FromInt32(SoftFloatContext context, int a)
    {
        var sign = a < 0;
        if ((a & 0x7FFFFFFF) == 0)
            return FromBitsUI32(sign ? PackToUI(true, 0x9E, 0U) : 0U);

        var absA = (uint)(sign ? -a : a);
        return NormRoundPack(context, sign, 0x9C, absA);
    }

    // i64_to_f32
    public static Float32 FromInt64(SoftFloatContext context, long a)
    {
        var sign = a < 0;
        var absA = (ulong)(sign ? -a : a);
        var shiftDist = CountLeadingZeroes64(absA) - 40;
        if (0 <= shiftDist)
            return FromBitsUI32(
                a != 0 ? PackToUI(sign, 0x95 - shiftDist, (uint)absA << shiftDist) : 0U
            );

        shiftDist += 7;
        var sig =
            (shiftDist < 0) ? (uint)absA.ShortShiftRightJam(-shiftDist) : ((uint)absA << shiftDist);
        return RoundPack(context, sign, 0x9C - shiftDist, sig);
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

    // f32_to_ui32
    public uint ToUInt32(SoftFloatContext context, RoundingMode roundingMode, bool exact)
    {
        uint sig;
        int exp,
            shiftDist;
        ulong sig64;
        bool sign;

        sign = GetSignUI(_v);
        exp = GetExpUI(_v);
        sig = GetFracUI(_v);

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
            sig64 = sig64.ShiftRightJam(shiftDist);

        return RoundToUI32(context, sign, sig64, roundingMode, exact);
    }

    // f32_to_ui64
    public ulong ToUInt64(SoftFloatContext context, RoundingMode roundingMode, bool exact)
    {
        int exp,
            shiftDist;
        uint sig;
        ulong sig64,
            extra;
        bool sign;

        sign = GetSignUI(_v);
        exp = GetExpUI(_v);
        sig = GetFracUI(_v);

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
            (extra, sig64) = new UInt64Extra(sig64, 0).ShiftRightJam(shiftDist);

        return RoundToUI64(context, sign, sig64, extra, roundingMode, exact);
    }

    // f32_to_i32
    public int ToInt32(SoftFloatContext context, RoundingMode roundingMode, bool exact)
    {
        uint sig;
        int exp,
            shiftDist;
        ulong sig64;
        bool sign;

        sign = GetSignUI(_v);
        exp = GetExpUI(_v);
        sig = GetFracUI(_v);

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
            sig64 = sig64.ShiftRightJam(shiftDist);

        return RoundToI32(context, sign, sig64, roundingMode, exact);
    }

    // f32_to_i64
    public long ToInt64(SoftFloatContext context, RoundingMode roundingMode, bool exact)
    {
        uint sig;
        int exp,
            shiftDist;
        ulong sig64,
            extra;
        bool sign;

        sign = GetSignUI(_v);
        exp = GetExpUI(_v);
        sig = GetFracUI(_v);

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
            (extra, sig64) = new UInt64Extra(sig64, 0).ShiftRightJam(shiftDist);

        return RoundToI64(context, sign, sig64, extra, roundingMode, exact);
    }

    // f32_to_ui32_r_minMag
    public uint ToUInt32RoundMinMag(SoftFloatContext context, bool exact)
    {
        uint sig,
            z;
        int exp,
            shiftDist;
        bool sign;

        exp = GetExpUI(_v);
        sig = GetFracUI(_v);

        shiftDist = 0x9E - exp;
        if (32 <= shiftDist)
        {
            if (exact && ((uint)exp | sig) != 0)
                context.ExceptionFlags |= ExceptionFlags.Inexact;

            return 0;
        }

        sign = GetSignUI(_v);
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
        int exp,
            shiftDist;
        ulong sig64,
            z;
        bool sign;

        exp = GetExpUI(_v);
        sig = GetFracUI(_v);

        shiftDist = 0xBE - exp;
        if (64 <= shiftDist)
        {
            if (exact && ((uint)exp | sig) != 0)
                context.ExceptionFlags |= ExceptionFlags.Inexact;

            return 0;
        }

        sign = GetSignUI(_v);
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
        int exp,
            shiftDist;
        int absZ;
        bool sign;

        exp = GetExpUI(_v);
        sig = GetFracUI(_v);

        shiftDist = 0x9E - exp;
        if (32 <= shiftDist)
        {
            if (exact && ((uint)exp | sig) != 0)
                context.ExceptionFlags |= ExceptionFlags.Inexact;

            return 0;
        }

        sign = GetSignUI(_v);
        if (shiftDist <= 0)
        {
            if (_v == PackToUI(true, 0x9E, 0))
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
        int exp,
            shiftDist;
        ulong sig64;
        long absZ;
        bool sign;

        exp = GetExpUI(_v);
        sig = GetFracUI(_v);

        shiftDist = 0xBE - exp;
        if (64 <= shiftDist)
        {
            if (exact && ((uint)exp | sig) != 0)
                context.ExceptionFlags |= ExceptionFlags.Inexact;

            return 0;
        }

        sign = GetSignUI(_v);
        if (shiftDist <= 0)
        {
            if (_v == PackToUI(true, 0xBE, 0))
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

        sign = GetSignUI(_v);
        exp = GetExpUI(_v);
        frac = GetFracUI(_v);

        if (exp == 0xFF)
        {
            if (frac != 0)
            {
                context.Float32BitsToCommonNaN(_v, out var commonNaN);
                return context.CommonNaNToFloat16(in commonNaN);
            }
            else
            {
                return Float16.Pack(sign, 0x1F, 0);
            }
        }

        frac16 = frac >> 9 | ((frac & 0x1FF) != 0 ? 1U : 0);
        return ((uint)exp | frac16) == 0
            ? Float16.Pack(sign, 0, 0)
            : Float16.RoundPack(context, sign, exp - 0x71, frac16 | 0x4000);
    }

    // f32_to_f64
    public Float64 ToFloat64(SoftFloatContext context)
    {
        uint frac;
        int exp;
        bool sign;

        sign = GetSignUI(_v);
        exp = GetExpUI(_v);
        frac = GetFracUI(_v);

        if (exp == 0xFF)
        {
            if (frac != 0)
            {
                context.Float32BitsToCommonNaN(_v, out var commonNaN);
                return context.CommonNaNToFloat64(in commonNaN);
            }
            else
            {
                return Float64.Pack(sign, 0x7FF, 0);
            }
        }

        if (exp == 0)
        {
            if (frac == 0)
                return Float64.Pack(sign, 0, 0);

            (exp, frac) = NormSubnormalSig(frac);
            exp--;
        }

        return Float64.Pack(sign, exp + 0x380, (ulong)frac << 29);
    }

    // f32_to_extF80
    public ExtFloat80 ToExtFloat80(SoftFloatContext context)
    {
        uint frac;
        int exp;
        bool sign;

        sign = GetSignUI(_v);
        exp = GetExpUI(_v);
        frac = GetFracUI(_v);

        if (exp == 0xFF)
        {
            if (frac != 0)
            {
                context.Float32BitsToCommonNaN(_v, out var commonNaN);
                return context.CommonNaNToExtFloat80(in commonNaN);
            }
            else
            {
                return ExtFloat80.Pack(sign, 0x7FFF, 0x8000000000000000);
            }
        }

        if (exp == 0)
        {
            if (frac == 0)
                return ExtFloat80.Pack(sign, 0, 0);

            (exp, frac) = NormSubnormalSig(frac);
        }

        return ExtFloat80.Pack(sign, exp + 0x3F80, (ulong)(frac | 0x00800000) << 40);
    }

    // f32_to_f128
    public Float128 ToFloat128(SoftFloatContext context)
    {
        int exp;
        uint frac;
        bool sign;

        sign = GetSignUI(_v);
        exp = GetExpUI(_v);
        frac = GetFracUI(_v);

        if (exp == 0xFF)
        {
            if (frac != 0)
            {
                context.Float32BitsToCommonNaN(_v, out var commonNaN);
                return context.CommonNaNToFloat128(in commonNaN);
            }
            else
            {
                return Float128.Pack(sign, 0x7FFF, 0, 0);
            }
        }

        if (exp == 0)
        {
            if (frac == 0)
                return Float128.Pack(sign, 0, 0, 0);

            (exp, frac) = NormSubnormalSig(frac);
            exp--;
        }

        return Float128.Pack(sign, exp + 0x3F80, (ulong)frac << 25, 0);
    }

    #endregion

    #region Arithmetic Operations

    // f32_roundToInt
    public Float32 RoundToInt(SoftFloatContext context, RoundingMode roundingMode, bool exact)
    {
        int exp;
        uint uiZ,
            lastBitMask,
            roundBitsMask;

        exp = GetExpUI(_v);

        if (exp <= 0x7E)
        {
            if ((_v << 1) == 0)
                return this;

            if (exact)
                context.ExceptionFlags |= ExceptionFlags.Inexact;

            uiZ = _v & PackToUI(true, 0, 0);
            switch (roundingMode)
            {
                case RoundingMode.NearEven:
                {
                    if (GetFracUI(_v) == 0)
                        break;

                    goto case RoundingMode.NearMaxMag;
                }
                case RoundingMode.NearMaxMag:
                {
                    if (exp == 0x7E)
                        uiZ |= PackToUI(false, 0x7F, 0);

                    break;
                }
                case RoundingMode.Min:
                {
                    if (uiZ != 0)
                        uiZ = PackToUI(true, 0x7F, 0);

                    break;
                }
                case RoundingMode.Max:
                {
                    if (uiZ == 0)
                        uiZ = PackToUI(false, 0x7F, 0);

                    break;
                }
                case RoundingMode.Odd:
                {
                    uiZ |= PackToUI(false, 0x7F, 0);
                    break;
                }
            }

            return Float32.FromBitsUI32(uiZ);
        }

        if (0x96 <= exp)
        {
            return exp == 0xFF && GetFracUI(_v) != 0
                ? context.PropagateNaNFloat32Bits(_v, 0)
                : this;
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
        else if (roundingMode == (GetSignUI(_v) ? RoundingMode.Min : RoundingMode.Max))
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
        uint uiA,
            uiB;

        uiA = a._v;
        uiB = b._v;

        return GetSignUI(uiA ^ uiB) ? SubMags(context, uiA, uiB) : AddMags(context, uiA, uiB);
    }

    // f32_sub
    public static Float32 Subtract(SoftFloatContext context, Float32 a, Float32 b)
    {
        uint uiA,
            uiB;

        uiA = a._v;
        uiB = b._v;

        return GetSignUI(uiA ^ uiB) ? AddMags(context, uiA, uiB) : SubMags(context, uiA, uiB);
    }

    // f32_mul
    public static Float32 Multiply(SoftFloatContext context, Float32 a, Float32 b)
    {
        uint uiA,
            sigA,
            uiB,
            sigB,
            magBits,
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
                return Pack(signZ, 0xFF, 0);
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
                return Pack(signZ, 0xFF, 0);
            }
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

        expZ = expA + expB - 0x7F;
        sigA = (sigA | 0x00800000) << 7;
        sigB = (sigB | 0x00800000) << 8;
        sigZ = (uint)((ulong)sigA * sigB).ShortShiftRightJam(32);
        if (sigZ < 0x40000000)
        {
            --expZ;
            sigZ <<= 1;
        }

        return RoundPack(context, signZ, expZ, sigZ);
    }

    // f32_mulAdd
    public static Float32 MultiplyAndAdd(
        SoftFloatContext context,
        Float32 a,
        Float32 b,
        Float32 c
    ) => MulAdd(context, a._v, b._v, c._v, MulAddOperation.None);

    // WARNING: This method overload is experimental and has not been thoroughly tested!
    public static Float32 MultiplyAndAdd(
        SoftFloatContext context,
        Float32 a,
        Float32 b,
        Float32 c,
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

    // f32_div
    public static Float32 Divide(SoftFloatContext context, Float32 a, Float32 b)
    {
        uint uiA,
            sigA,
            uiB,
            sigB,
            sigZ;
        int expA,
            expB,
            expZ;
        ulong sig64A;
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

            return Pack(signZ, 0xFF, 0);
        }

        if (expB == 0xFF)
        {
            return sigB != 0 ? context.PropagateNaNFloat32Bits(uiA, uiB) : Pack(signZ, 0, 0);
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
                return Pack(signZ, 0xFF, 0);
            }

            (expB, sigB) = NormSubnormalSig(sigB);
        }

        if (expA == 0)
        {
            if (sigA == 0)
                return Pack(signZ, 0, 0);

            (expA, sigA) = NormSubnormalSig(sigA);
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

        return RoundPack(context, signZ, expZ, sigZ);
    }

    // f32_rem
    public static Float32 Remainder(SoftFloatContext context, Float32 a, Float32 b)
    {
        uint uiA,
            sigA,
            uiB,
            sigB;
        int expA,
            expB,
            expDiff;
        uint rem,
            q,
            recip32,
            altRem,
            meanRem;
        bool signA,
            signRem;

        uiA = a._v;
        signA = GetSignUI(uiA);
        expA = GetExpUI(uiA);
        sigA = GetFracUI(uiA);
        uiB = b._v;
        expB = GetExpUI(uiB);
        sigB = GetFracUI(uiB);

        if (expA == 0xFF)
        {
            if (sigA != 0 || (expB == 0xFF && sigB != 0))
                return context.PropagateNaNFloat32Bits(uiA, uiB);

            context.RaiseFlags(ExceptionFlags.Invalid);
            return context.DefaultNaNFloat32;
        }

        if (expB == 0xFF)
        {
            return sigB != 0 ? context.PropagateNaNFloat32Bits(uiA, uiB) : a;
        }

        if (expB == 0)
        {
            if (sigB == 0)
            {
                context.RaiseFlags(ExceptionFlags.Invalid);
                return context.DefaultNaNFloat32;
            }

            (expB, sigB) = NormSubnormalSig(sigB);
        }

        if (expA == 0)
        {
            if (sigA == 0)
                return a;

            (expA, sigA) = NormSubnormalSig(sigA);
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
        } while ((rem & 0x80000000) == 0);

        meanRem = rem + altRem;
        if ((meanRem & 0x80000000) != 0 || (meanRem == 0 && (q & 1) != 0))
            rem = altRem;

        signRem = signA;
        if (0x80000000 <= rem)
        {
            signRem = !signRem;
            rem = (uint)(-(int)rem);
        }

        return NormRoundPack(context, signRem, expB, rem);
    }

    // f32_sqrt
    public Float32 SquareRoot(SoftFloatContext context)
    {
        uint uiA,
            sigA,
            sigZ,
            shiftedSigZ;
        int expA,
            expZ;
        uint negRem;
        bool signA;

        uiA = _v;
        signA = GetSignUI(uiA);
        expA = GetExpUI(uiA);
        sigA = GetFracUI(uiA);

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

            (expA, sigA) = NormSubnormalSig(sigA);
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

        return RoundPack(context, false, expZ, sigZ);
    }

    #endregion

    #region Comparison Operations

    // f32_eq (signaling=false) & f32_eq_signaling (signaling=true)
    public static bool CompareEqual(SoftFloatContext context, Float32 a, Float32 b, bool signaling)
    {
        uint uiA,
            uiB;

        uiA = a._v;
        uiB = b._v;

        if (IsNaNUI(uiA) || IsNaNUI(uiB))
        {
            if (
                signaling
                || context.IsSignalingNaNFloat32Bits(uiA)
                || context.IsSignalingNaNFloat32Bits(uiB)
            )
                context.RaiseFlags(ExceptionFlags.Invalid);

            return false;
        }

        return (uiA == uiB) || ((uiA | uiB) << 1) == 0;
    }

    // f32_le (signaling=true) & f32_le_quiet (signaling=false)
    public static bool CompareLessThanOrEqual(
        SoftFloatContext context,
        Float32 a,
        Float32 b,
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
                || context.IsSignalingNaNFloat32Bits(uiA)
                || context.IsSignalingNaNFloat32Bits(uiB)
            )
                context.RaiseFlags(ExceptionFlags.Invalid);

            return false;
        }

        signA = GetSignUI(uiA);
        signB = GetSignUI(uiB);

        return (signA != signB)
            ? (signA || ((uiA | uiB) << 1) == 0)
            : (uiA == uiB || (signA ^ (uiA < uiB)));
    }

    // f32_lt (signaling=true) & f32_lt_quiet (signaling=false)
    public static bool CompareLessThan(
        SoftFloatContext context,
        Float32 a,
        Float32 b,
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
                || context.IsSignalingNaNFloat32Bits(uiA)
                || context.IsSignalingNaNFloat32Bits(uiB)
            )
                context.RaiseFlags(ExceptionFlags.Invalid);

            return false;
        }

        signA = GetSignUI(uiA);
        signB = GetSignUI(uiB);

        return (signA != signB)
            ? (signA && ((uiA | uiB) << 1) != 0)
            : (uiA != uiB && (signA ^ (uiA < uiB)));
    }

    #endregion

    #region Internals

    // signF32UI
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool GetSignUI(uint a) => (a >> 31) != 0;

    // expF32UI
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int GetExpUI(uint a) => (int)((a >> 23) & 0xFF);

    // fracF32UI
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static uint GetFracUI(uint a) => a & 0x007FFFFF;

    // packToF32UI
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static uint PackToUI(bool sign, int exp, uint sig) =>
        (sign ? (1U << 31) : 0U) + ((uint)exp << 23) + sig;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Float32 Pack(bool sign, int exp, uint sig) =>
        FromBitsUI32(PackToUI(sign, exp, sig));

    // isNaNF32UI
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsNaNUI(uint a) => (~a & 0x7F800000) == 0 && (a & 0x007FFFFF) != 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsInfUI(uint a) => (~a & 0x7F800000) == 0 && (a & 0x007FFFFF) == 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsFiniteUI(uint a) => (~a & 0x7F800000) != 0;

    // softfloat_normSubnormalF32Sig
    internal static (int exp, uint sig) NormSubnormalSig(uint sig)
    {
        var shiftDist = CountLeadingZeroes32(sig) - 8;
        return (exp: 1 - shiftDist, sig: sig << shiftDist);
    }

    // softfloat_roundPackToF32
    internal static Float32 RoundPack(SoftFloatContext context, bool sign, int exp, uint sig)
    {
        var roundingMode = context.Rounding;
        var roundNearEven = roundingMode == RoundingMode.NearEven;
        var roundIncrement =
            (!roundNearEven && roundingMode != RoundingMode.NearMaxMag)
                ? ((roundingMode == (sign ? RoundingMode.Min : RoundingMode.Max)) ? 0x7FU : 0)
                : 0x40U;
        var roundBits = sig & 0x7F;

        if (0xFD <= (uint)exp)
        {
            if (exp < 0)
            {
                var isTiny =
                    context.DetectTininess == TininessMode.BeforeRounding
                    || exp < -1
                    || sig + roundIncrement < 0x80000000;
                sig = sig.ShiftRightJam(-exp);
                exp = 0;
                roundBits = sig & 0x7F;

                if (isTiny && roundBits != 0)
                    context.RaiseFlags(ExceptionFlags.Underflow);
            }
            else if (0xFD < exp || 0x80000000 <= sig + roundIncrement)
            {
                context.RaiseFlags(ExceptionFlags.Overflow | ExceptionFlags.Inexact);
                return FromBitsUI32(PackToUI(sign, 0xFF, 0) - (roundIncrement == 0 ? 1U : 0U));
            }
        }

        sig = (sig + roundIncrement) >> 7;
        if (roundBits != 0)
        {
            context.ExceptionFlags |= ExceptionFlags.Inexact;
            if (roundingMode == RoundingMode.Odd)
            {
                sig |= 1;
                return Pack(sign, exp, sig);
            }
        }

        sig &= ~(((roundBits ^ 0x40) == 0 & roundNearEven) ? 1U : 0U);
        if (sig == 0)
            exp = 0;

        return Pack(sign, exp, sig);
    }

    // softfloat_normRoundPackToF32
    internal static Float32 NormRoundPack(SoftFloatContext context, bool sign, int exp, uint sig)
    {
        var shiftDist = CountLeadingZeroes32(sig) - 1;
        exp -= shiftDist;
        return 7 <= shiftDist && (uint)exp < 0xFD
            ? Pack(sign, sig != 0 ? exp : 0, sig << (shiftDist - 7))
            : RoundPack(context, sign, exp, sig << shiftDist);
    }

    // softfloat_addMagsF32
    internal static Float32 AddMags(SoftFloatContext context, uint uiA, uint uiB)
    {
        int expA,
            expB,
            expDiff,
            expZ;
        uint sigA,
            sigB,
            sigZ;
        bool signZ;

        expA = GetExpUI(uiA);
        sigA = GetFracUI(uiA);
        expB = GetExpUI(uiB);
        sigB = GetFracUI(uiB);

        expDiff = expA - expB;
        if (expDiff == 0)
        {
            if (expA == 0)
                return FromBitsUI32(uiA + sigB);

            if (expA == 0xFF)
            {
                return (sigA | sigB) != 0
                    ? context.PropagateNaNFloat32Bits(uiA, uiB)
                    : FromBitsUI32(uiA);
            }

            signZ = GetSignUI(uiA);
            expZ = expA;
            sigZ = 0x01000000 + sigA + sigB;
            if ((sigZ & 1) == 0 && expZ < 0xFE)
                return Pack(signZ, expZ, sigZ >> 1);

            sigZ <<= 6;
        }
        else
        {
            signZ = GetSignUI(uiA);
            sigA <<= 6;
            sigB <<= 6;
            if (expDiff < 0)
            {
                if (expB == 0xFF)
                {
                    return sigB != 0
                        ? context.PropagateNaNFloat32Bits(uiA, uiB)
                        : Pack(signZ, 0xFF, 0);
                }

                expZ = expB;
                sigA += expA != 0 ? 0x20000000 : sigA;
                sigA = sigA.ShiftRightJam(-expDiff);
            }
            else
            {
                if (expA == 0xFF)
                {
                    return sigA != 0
                        ? context.PropagateNaNFloat32Bits(uiA, uiB)
                        : FromBitsUI32(uiA);
                }

                expZ = expA;
                sigB += expB != 0 ? 0x20000000 : sigB;
                sigB = sigB.ShiftRightJam(expDiff);
            }

            sigZ = 0x20000000 + sigA + sigB;
            if (sigZ < 0x40000000)
            {
                --expZ;
                sigZ <<= 1;
            }
        }

        return RoundPack(context, signZ, expZ, sigZ);
    }

    // softfloat_subMagsF32
    internal static Float32 SubMags(SoftFloatContext context, uint uiA, uint uiB)
    {
        int expA,
            expB,
            expDiff,
            expZ;
        uint sigA,
            sigB,
            sigX,
            sigY;
        int sigDiff;
        int shiftDist;
        bool signZ;

        expA = GetExpUI(uiA);
        sigA = GetFracUI(uiA);
        expB = GetExpUI(uiB);
        sigB = GetFracUI(uiB);

        expDiff = expA - expB;
        if (expDiff == 0)
        {
            if (expA == 0xFF)
            {
                if ((sigA | sigB) != 0)
                    return context.PropagateNaNFloat32Bits(uiA, uiB);

                context.RaiseFlags(ExceptionFlags.Invalid);
                return context.DefaultNaNFloat32;
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
            shiftDist = CountLeadingZeroes32((uint)sigDiff) - 8;
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
            sigA <<= 7;
            sigB <<= 7;

            if (expDiff < 0)
            {
                signZ = !signZ;
                if (expB == 0xFF)
                {
                    return sigB != 0
                        ? context.PropagateNaNFloat32Bits(uiA, uiB)
                        : Pack(signZ, 0xFF, 0);
                }

                expZ = expB - 1;
                sigX = sigB | 0x40000000;
                sigY = sigA + (expA != 0 ? 0x40000000U : sigA);
                expDiff = -expDiff;
            }
            else
            {
                if (expA == 0xFF)
                {
                    return sigA != 0
                        ? context.PropagateNaNFloat32Bits(uiA, uiB)
                        : FromBitsUI32(uiA);
                }

                expZ = expA - 1;
                sigX = sigA | 0x40000000;
                sigY = sigB + (expB != 0 ? 0x40000000 : sigB);
            }

            return NormRoundPack(context, signZ, expZ, sigX - sigY.ShiftRightJam(expDiff));
        }
    }

    // softfloat_mulAddF32
    internal static Float32 MulAdd(
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
            expDiff;
        uint sigA,
            sigB,
            sigC,
            magBits,
            uiZ,
            sigZ;
        ulong sigProd,
            sig64Z,
            sig64C;
        int shiftDist;

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

        if (expA == 0xFF)
        {
            if (sigA != 0 || (expB == 0xFF && sigB != 0))
                return context.PropagateNaNFloat32Bits(uiA, uiB, uiC);

            magBits = (uint)expB | sigB;
            goto infProdArg;
        }

        if (expB == 0xFF)
        {
            if (sigB != 0)
                return context.PropagateNaNFloat32Bits(uiA, uiB, uiC);

            magBits = (uint)expA | sigA;
            goto infProdArg;
        }

        if (expC == 0xFF)
        {
            return sigC != 0 ? context.PropagateNaNFloat32Bits(0, uiC) : FromBitsUI32(uiC);
        }

        if (expA == 0)
        {
            if (sigA == 0)
            {
                return ((uint)expC | sigC) == 0 && signProd != signC
                    ? Pack(context.Rounding == RoundingMode.Min, 0, 0)
                    : FromBitsUI32(uiC);
            }

            (expA, sigA) = NormSubnormalSig(sigA);
        }

        if (expB == 0)
        {
            if (sigB == 0)
            {
                return ((uint)expC | sigC) == 0 && signProd != signC
                    ? Pack(context.Rounding == RoundingMode.Min, 0, 0)
                    : FromBitsUI32(uiC);
            }

            (expB, sigB) = NormSubnormalSig(sigB);
        }

        expProd = expA + expB - 0x7E;
        sigA = (sigA | 0x00800000) << 7;
        sigB = (sigB | 0x00800000) << 7;
        sigProd = (ulong)sigA * sigB;

        if (sigProd < 0x2000000000000000)
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
                sigZ = (uint)sigProd.ShortShiftRightJam(31);
                return RoundPack(context, signZ, expZ, sigZ);
            }

            (expC, sigC) = NormSubnormalSig(sigC);
        }

        sigC = (sigC | 0x00800000) << 6;
        expDiff = expProd - expC;

        if (signProd == signC)
        {
            if (expDiff <= 0)
            {
                expZ = expC;
                sigZ = (uint)(sigC + sigProd.ShiftRightJam(32 - expDiff));
            }
            else
            {
                expZ = expProd;
                sig64Z = sigProd + ((ulong)sigC << 32).ShiftRightJam(expDiff);
                sigZ = (uint)sig64Z.ShortShiftRightJam(32);
            }

            if (sigZ < 0x40000000)
            {
                --expZ;
                sigZ <<= 1;
            }
        }
        else
        {
            sig64C = (ulong)sigC << 32;
            if (expDiff < 0)
            {
                signZ = signC;
                expZ = expC;
                sig64Z = sig64C - sigProd.ShiftRightJam(-expDiff);
            }
            else if (expDiff == 0)
            {
                expZ = expProd;
                sig64Z = sigProd - sig64C;
                if (sig64Z == 0)
                    return Pack(context.Rounding == RoundingMode.Min, 0, 0);

                if ((sig64Z & 0x8000000000000000) != 0)
                {
                    signZ = !signZ;
                    sig64Z = (ulong)(-(long)sig64Z);
                }
            }
            else
            {
                expZ = expProd;
                sig64Z = sigProd - sig64C.ShiftRightJam(expDiff);
            }

            shiftDist = CountLeadingZeroes64(sig64Z) - 1;
            expZ -= shiftDist;
            shiftDist -= 32;
            sigZ =
                (shiftDist < 0)
                    ? (uint)sig64Z.ShortShiftRightJam(-shiftDist)
                    : (uint)sig64Z << shiftDist;
        }

        return RoundPack(context, signZ, expZ, sigZ);

        infProdArg:
        if (magBits != 0)
        {
            uiZ = PackToUI(signProd, 0xFF, 0);
            if (expC != 0xFF)
                return FromBitsUI32(uiZ);

            if (sigC != 0)
                return context.PropagateNaNFloat32Bits(uiZ, uiC);

            if (signProd == signC)
                return FromBitsUI32(uiZ);
        }

        context.RaiseFlags(ExceptionFlags.Invalid);
        return context.PropagateNaNFloat32Bits(context.DefaultNaNFloat32Bits, uiC);
    }

    #endregion

    #endregion
}
