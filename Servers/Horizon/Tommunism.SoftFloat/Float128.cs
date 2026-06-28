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

[StructLayout(LayoutKind.Sequential, Pack = sizeof(ulong), Size = sizeof(ulong) * 2)]
public readonly struct Float128 : ISpanFormattable
{
    #region Fields

    public const int ExponentBias = 0x3FFF;

    // WARNING: DO NOT ADD OR CHANGE ANY OF THESE FIELDS!!!
    private readonly ulong _v64;

    #endregion

    #region Constructors

    private Float128(UInt128M v)
    {
        Significand00 = v.V00;
        _v64 = v.V64;
    }

    private Float128(ulong v64, ulong v0)
    {
        Significand00 = v0;
        _v64 = v64;
    }

    // NOTE: The exponential is the biased exponent value (not the bit encoded value).
    public Float128(bool sign, int exponent, ulong significand64, ulong significand0)
    {
        exponent += ExponentBias;
        if ((exponent >> 15) != 0)
            throw new ArgumentOutOfRangeException(nameof(exponent));

        if ((significand64 >> 48) != 0)
            throw new ArgumentOutOfRangeException(nameof(significand64));

        _v64 = PackToUI64(sign, exponent, significand64);
        Significand00 = significand0;
    }

    // NOTE: The exponential is the biased exponent value (not the bit encoded value).
    public Float128(bool sign, int exponent, UInt128 significand)
    {
        exponent += ExponentBias;
        if (exponent is < 0 or > 0x7FFF)
            throw new ArgumentOutOfRangeException(nameof(exponent));

        if ((significand >> 112) != 0)
            throw new ArgumentOutOfRangeException(nameof(significand));

        _v64 = PackToUI64(sign, exponent, significand.GetUpperUI64());
        Significand00 = significand.GetLowerUI64();
    }

    #endregion

    #region Properties

    public bool Sign => GetSignUI64(_v64);

    public int Exponent => GetExpUI64(_v64) - ExponentBias; // offset-binary

    public ulong Significand64 => GetFracUI64(_v64);

    public ulong Significand00 { get; }

    public UInt128 Significand => new(GetFracUI64(_v64), Significand00);

    public bool IsNaN => IsNaNUI(_v64, Significand00);

    public bool IsInfinity => IsInfUI(_v64, Significand00);

    public bool IsFinite => IsFiniteUI(_v64);

    #endregion

    #region Methods

    public static Float128 FromUIntBits(ulong upper, ulong lower) => new(v64: upper, v0: lower);

    public (ulong hi, ulong lo) ToUInt64x2Bits() => (_v64, Significand00);

    public static Float128 FromUIntBits(UInt128 value) =>
        new(v64: value.GetUpperUI64(), v0: value.GetLowerUI64());

    public UInt128 ToUInt128Bits() => new(_v64, Significand00);

    // THIS IS THE INTERNAL CONSTRUCTOR FOR RAW BITS.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Float128 FromBitsUI128(UInt128M v) => new(v);

    // THIS IS THE INTERNAL CONSTRUCTOR FOR RAW BITS.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Float128 FromBitsUI128(ulong v64, ulong v0) => new(v64, v0);

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
            return floatingDecimal.TryFormat(destination, out charsWritten, format, provider);
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
            var builder = new ValueStringBuilder(stackalloc char[34]);
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

    // NOTE: This is the raw exponent and significand encoded in hexadecimal, separated by a period, and prefixed with the sign.
    private void FormatValueHex(ref ValueStringBuilder builder, bool isLowerCase)
    {
        // Value Format: -7FFF.FFFFFFFFFFFFFFFFFFFFFFFFFFFF
        builder.Append(GetSignUI64(_v64) ? '-' : '+');
        builder.AppendHex((uint)GetExpUI64(_v64), 15, isLowerCase);
        builder.Append('.');
        builder.AppendHex(GetFracUI64(_v64), 48, isLowerCase);
        builder.AppendHex(Significand00, 64);
    }

    #region Integer-to-floating-point Conversions

    // ui32_to_f128
    [SuppressMessage(
        "Style",
        "IDE0060:Remove unused parameter",
        Justification = "API consistency and possible future use."
    )]
    public static Float128 FromUInt32(SoftFloatContext context, uint a)
    {
        if (a != 0)
        {
            var shiftDist = CountLeadingZeroes32(a) + 17;
            return Pack(false, 0x402E - shiftDist, (ulong)a << shiftDist, 0);
        }

        return FromBitsUI128(v64: 0, v0: 0);
    }

    // ui64_to_f128
    [SuppressMessage(
        "Style",
        "IDE0060:Remove unused parameter",
        Justification = "API consistency and possible future use."
    )]
    public static Float128 FromUInt64(SoftFloatContext context, ulong a)
    {
        if (a != 0)
        {
            var shiftDist = CountLeadingZeroes64(a) + 49;
            var zSig =
                (64 <= shiftDist)
                    ? new UInt128M(a << (shiftDist - 64), 0)
                    : ((UInt128M)a << shiftDist);
            return Pack(false, 0x406E - shiftDist, zSig.V64, zSig.V00);
        }

        return FromBitsUI128(v64: 0, v0: 0);
    }

    // i32_to_f128
    [SuppressMessage(
        "Style",
        "IDE0060:Remove unused parameter",
        Justification = "API consistency and possible future use."
    )]
    public static Float128 FromInt32(SoftFloatContext context, int a)
    {
        if (a != 0)
        {
            var sign = a < 0;
            var absA = (uint)(sign ? -a : a);
            var shiftDist = CountLeadingZeroes32(absA) + 17;
            return Pack(sign, 0x402E - shiftDist, (ulong)absA << shiftDist, 0);
        }

        return FromBitsUI128(v64: 0, v0: 0);
    }

    // i64_to_f128
    [SuppressMessage(
        "Style",
        "IDE0060:Remove unused parameter",
        Justification = "API consistency and possible future use."
    )]
    public static Float128 FromInt64(SoftFloatContext context, long a)
    {
        if (a != 0)
        {
            var sign = a < 0;
            var absA = (ulong)(sign ? -a : a);
            var shiftDist = CountLeadingZeroes64(absA) + 49;
            var zSig =
                (64 <= shiftDist)
                    ? new UInt128M(absA << (shiftDist - 64), 0)
                    : ((UInt128M)absA << shiftDist);
            return Pack(sign, 0x406E - shiftDist, zSig.V64, zSig.V00);
        }

        return FromBitsUI128(v64: 0, v0: 0);
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

    // f128_to_ui32
    public uint ToUInt32(SoftFloatContext context, RoundingMode roundingMode, bool exact)
    {
        ulong uiA64,
            uiA0,
            sig64;
        int exp,
            shiftDist;
        bool sign;

        uiA64 = _v64;
        uiA0 = Significand00;
        sign = GetSignUI64(uiA64);
        exp = GetExpUI64(uiA64);
        sig64 = GetFracUI64(uiA64) | (uiA0 != 0 ? 1U : 0);

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
            sig64 = sig64.ShiftRightJam(shiftDist);

        return RoundToUI32(context, sign, sig64, roundingMode, exact);
    }

    // f128_to_ui64
    public ulong ToUInt64(SoftFloatContext context, RoundingMode roundingMode, bool exact)
    {
        ulong uiA64,
            uiA0,
            sig64,
            sig0;
        int exp,
            shiftDist;
        bool sign;

        uiA64 = _v64;
        uiA0 = Significand00;
        sign = GetSignUI64(uiA64);
        exp = GetExpUI64(uiA64);
        sig64 = GetFracUI64(uiA64);
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
                (sig64, sig0) = new UInt128M(sig64, sig0) << -shiftDist;
        }
        else
        {
            if (exp != 0)
                sig64 |= 0x0001000000000000;

            (sig0, sig64) = new UInt64Extra(sig64, sig0).ShiftRightJam(shiftDist);
        }

        return RoundToUI64(context, sign, sig64, sig0, roundingMode, exact);
    }

    // f128_to_i32
    public int ToInt32(SoftFloatContext context, RoundingMode roundingMode, bool exact)
    {
        ulong uiA64,
            uiA0,
            sig64,
            sig0;
        int exp,
            shiftDist;
        bool sign;

        uiA64 = _v64;
        uiA0 = Significand00;
        sign = GetSignUI64(uiA64);
        exp = GetExpUI64(uiA64);
        sig64 = GetFracUI64(uiA64);
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
            sig64 = sig64.ShiftRightJam(shiftDist);

        return RoundToI32(context, sign, sig64, roundingMode, exact);
    }

    // f128_to_i64
    public long ToInt64(SoftFloatContext context, RoundingMode roundingMode, bool exact)
    {
        ulong uiA64,
            uiA0,
            sig64,
            sig0;
        int exp,
            shiftDist;
        bool sign;

        uiA64 = _v64;
        uiA0 = Significand00;
        sign = GetSignUI64(uiA64);
        exp = GetExpUI64(uiA64);
        sig64 = GetFracUI64(uiA64);
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
                (sig64, sig0) = new UInt128M(sig64, sig0) << -shiftDist;
        }
        else
        {
            if (exp != 0)
                sig64 |= 0x0001000000000000;

            (sig0, sig64) = new UInt64Extra(sig64, sig0).ShiftRightJam(shiftDist);
        }

        return RoundToI64(context, sign, sig64, sig0, roundingMode, exact);
    }

    // f128_to_ui32_r_minMag
    public uint ToUInt32RoundMinMag(SoftFloatContext context, bool exact)
    {
        ulong uiA64,
            uiA0,
            sig64;
        int exp,
            shiftDist;
        uint z;
        bool sign;

        uiA64 = _v64;
        uiA0 = Significand00;
        exp = GetExpUI64(uiA64);
        sig64 = GetFracUI64(uiA64) | (uiA0 != 0 ? 1U : 0);

        shiftDist = 0x402F - exp;
        if (49 <= shiftDist)
        {
            if (exact && ((uint)exp | sig64) != 0)
                context.ExceptionFlags |= ExceptionFlags.Inexact;

            return 0;
        }

        sign = GetSignUI64(uiA64);
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
        ulong uiA64,
            uiA0,
            sig64,
            sig0,
            z;
        int exp,
            shiftDist;
        int negShiftDist;
        bool sign;

        uiA64 = _v64;
        uiA0 = Significand00;
        sign = GetSignUI64(uiA64);
        exp = GetExpUI64(uiA64);
        sig64 = GetFracUI64(uiA64);
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
        ulong uiA64,
            uiA0,
            sig64;
        int exp,
            shiftDist,
            absZ;
        bool sign;

        uiA64 = _v64;
        uiA0 = Significand00;
        exp = GetExpUI64(uiA64);
        sig64 = GetFracUI64(uiA64) | (uiA0 != 0 ? 1U : 0);

        shiftDist = 0x402F - exp;
        if (49 <= shiftDist)
        {
            if (exact && ((uint)exp | sig64) != 0)
                context.ExceptionFlags |= ExceptionFlags.Inexact;

            return 0;
        }

        sign = GetSignUI64(uiA64);
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
        ulong uiA64,
            uiA0,
            sig64,
            sig0;
        int exp,
            shiftDist;
        int negShiftDist;
        long absZ;
        bool sign;

        uiA64 = _v64;
        uiA0 = Significand00;
        sign = GetSignUI64(uiA64);
        exp = GetExpUI64(uiA64);
        sig64 = GetFracUI64(uiA64);
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
        ulong uiA64,
            uiA0,
            frac64;
        int exp;
        uint frac16;
        bool sign;

        uiA64 = _v64;
        uiA0 = Significand00;
        sign = GetSignUI64(uiA64);
        exp = GetExpUI64(uiA64);
        frac64 = GetFracUI64(uiA64) | (uiA0 != 0 ? 1U : 0);

        if (exp == 0x7FFF)
        {
            if (frac64 != 0)
            {
                context.Float128BitsToCommonNaN(uiA64, uiA0, out var commonNaN);
                return context.CommonNaNToFloat16(in commonNaN);
            }

            return Float16.Pack(sign, 0x1F, 0);
        }

        frac16 = (uint)frac64.ShortShiftRightJam(34);
        if (((uint)exp | frac16) == 0)
            return Float16.Pack(sign, 0, 0);

        exp -= 0x3FF1;
        if (sizeof(int) < sizeof(int) && exp < -0x40)
            exp = -0x40;

        return Float16.RoundPack(context, sign, exp, frac16 | 0x4000);
    }

    // f128_to_f32
    public Float32 ToFloat32(SoftFloatContext context)
    {
        ulong uiA64,
            uiA0,
            frac64;
        int exp;
        uint frac32;
        bool sign;

        uiA64 = _v64;
        uiA0 = Significand00;
        sign = GetSignUI64(uiA64);
        exp = GetExpUI64(uiA64);
        frac64 = GetFracUI64(uiA64) | (uiA0 != 0 ? 1U : 0);

        if (exp == 0x7FFF)
        {
            if (frac64 != 0)
            {
                context.Float128BitsToCommonNaN(uiA64, uiA0, out var commonNaN);
                return context.CommonNaNToFloat32(in commonNaN);
            }

            return Float32.Pack(sign, 0xFF, 0);
        }

        frac32 = (uint)frac64.ShortShiftRightJam(18);
        if (((uint)exp | frac32) == 0)
            return Float32.Pack(sign, 0, 0);

        exp -= 0x3F81;
        if (sizeof(int) < sizeof(int) && exp < -0x1000)
            exp = -0x1000;

        return Float32.RoundPack(context, sign, exp, frac32 | 0x40000000);
    }

    // f128_to_f64
    public Float64 ToFloat64(SoftFloatContext context)
    {
        ulong uiA64,
            uiA0,
            frac64,
            frac0;
        int exp;
        UInt128M frac128;
        bool sign;

        uiA64 = _v64;
        uiA0 = Significand00;
        sign = GetSignUI64(uiA64);
        exp = GetExpUI64(uiA64);
        frac64 = GetFracUI64(uiA64);
        frac0 = uiA0;

        if (exp == 0x7FFF)
        {
            if ((frac64 | frac0) != 0)
            {
                context.Float128BitsToCommonNaN(uiA64, uiA0, out var commonNaN);
                return context.CommonNaNToFloat64(in commonNaN);
            }

            return Float64.Pack(sign, 0x7FF, 0);
        }

        frac128 = new UInt128M(frac64, frac0) << 14;
        frac64 = frac128.V64 | (frac128.V00 != 0 ? 1U : 0);
        if (((uint)exp | frac64) == 0)
            return Float64.Pack(sign, 0, 0);

        exp -= 0x3C01;
        if (sizeof(int) < sizeof(int) && exp < -0x1000)
            exp = -0x1000;

        return Float64.RoundPack(context, sign, exp, frac64 | 0x4000000000000000);
    }

    // f128_to_extF80
    public ExtFloat80 ToExtFloat80(SoftFloatContext context)
    {
        ulong uiA64,
            uiA0,
            frac64,
            frac0;
        int exp;
        UInt128M sig128;
        bool sign;

        uiA64 = _v64;
        uiA0 = Significand00;
        sign = GetSignUI64(uiA64);
        exp = GetExpUI64(uiA64);
        frac64 = GetFracUI64(uiA64);
        frac0 = uiA0;

        if (exp == 0x7FFF)
        {
            if ((frac64 | frac0) != 0)
            {
                context.Float128BitsToCommonNaN(uiA64, uiA0, out var commonNaN);
                return context.CommonNaNToExtFloat80(in commonNaN);
            }

            return ExtFloat80.Pack(sign, 0x7FFF, 0x8000000000000000);
        }

        if (exp == 0)
        {
            if ((frac64 | frac0) == 0)
                return ExtFloat80.Pack(sign, 0, 0);

            (exp, (frac64, frac0)) = NormSubnormalSig(frac64, frac0);
        }

        sig128 = new UInt128M(frac64 | 0x0001000000000000, frac0) << 15;
        return ExtFloat80.RoundPack(
            context,
            sign,
            exp,
            sig128.V64,
            sig128.V00,
            ExtFloat80RoundingPrecision._80
        );
    }

    #endregion

    #region Arithmetic Operations

    public Float128 RoundToInt(SoftFloatContext context, bool exact) =>
        RoundToInt(context, context.Rounding, exact);

    // f128_roundToInt
    public Float128 RoundToInt(SoftFloatContext context, RoundingMode roundingMode, bool exact)
    {
        ulong uiA64,
            uiA0,
            lastBitMask0,
            roundBitsMask,
            lastBitMask64;
        int exp;
        UInt128M uiZ;
        bool roundNearEven;

        uiA64 = _v64;
        uiA0 = Significand00;
        exp = GetExpUI64(uiA64);

        if (0x402F <= exp)
        {
            if (0x406F <= exp)
            {
                return exp == 0x7FFF && (GetFracUI64(uiA64) | uiA0) != 0
                    ? context.PropagateNaNFloat128Bits(uiA64, uiA0, 0, 0)
                    : this;
            }

            lastBitMask0 = (ulong)2 << (0x406E - exp);
            roundBitsMask = lastBitMask0 - 1;
            uiZ = new UInt128M(uiA64, uiA0);
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
                    uiZ += (UInt128M)(lastBitMask0 >> 1);
                    if (roundNearEven && (uiZ.V00 & roundBitsMask) == 0)
                        uiZ.V00 &= ~lastBitMask0;
                }
            }
            else if (roundingMode == (GetSignUI64(uiZ.V64) ? RoundingMode.Min : RoundingMode.Max))
            {
                uiZ += (UInt128M)roundBitsMask;
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

                uiZ = new UInt128M(uiA64 & PackToUI64(true, 0, 0), 0);
                switch (roundingMode)
                {
                    case RoundingMode.NearEven:
                    {
                        if ((GetFracUI64(uiA64) | uiA0) == 0)
                            break;

                        goto case RoundingMode.NearMaxMag;
                    }
                    case RoundingMode.NearMaxMag:
                    {
                        if (exp == 0x3FFE)
                            uiZ.V64 |= PackToUI64(false, 0x3FFF, 0);

                        break;
                    }
                    case RoundingMode.Min:
                    {
                        if (uiZ.V64 != 0)
                            uiZ.V64 = PackToUI64(true, 0x3FFF, 0);

                        break;
                    }
                    case RoundingMode.Max:
                    {
                        if (uiZ.V64 == 0)
                            uiZ.V64 = PackToUI64(false, 0x3FFF, 0);

                        break;
                    }
                    case RoundingMode.Odd:
                    {
                        uiZ.V64 |= PackToUI64(false, 0x3FFF, 0);
                        break;
                    }
                }

                return Float128.FromBitsUI128(uiZ);
            }

            uiZ = new UInt128M(uiA64, 0);
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
            else if (roundingMode == (GetSignUI64(uiZ.V64) ? RoundingMode.Min : RoundingMode.Max))
            {
                uiZ.V64 = (uiZ.V64 | (uiA0 != 0 ? 1U : 0)) + roundBitsMask;
            }

            uiZ.V64 &= ~roundBitsMask;
            lastBitMask0 = 0;
        }

        if (uiZ.V64 != uiA64 || uiZ.V00 != uiA0)
        {
            if (roundingMode == RoundingMode.Odd)
                uiZ = new UInt128M(uiZ.V64 | lastBitMask64, uiZ.V00 | lastBitMask0);

            if (exact)
                context.ExceptionFlags |= ExceptionFlags.Inexact;
        }

        return Float128.FromBitsUI128(uiZ);
    }

    // f128_add
    public static Float128 Add(SoftFloatContext context, Float128 a, Float128 b)
    {
        var signA = GetSignUI64(a._v64);
        var signB = GetSignUI64(b._v64);

        return (signA == signB)
            ? AddMags(context, a._v64, a.Significand00, b._v64, b.Significand00, signA)
            : SubMags(context, a._v64, a.Significand00, b._v64, b.Significand00, signA);
    }

    // f128_sub
    public static Float128 Subtract(SoftFloatContext context, Float128 a, Float128 b)
    {
        var signA = GetSignUI64(a._v64);
        var signB = GetSignUI64(b._v64);

        return (signA == signB)
            ? SubMags(context, a._v64, a.Significand00, b._v64, b.Significand00, signA)
            : AddMags(context, a._v64, a.Significand00, b._v64, b.Significand00, signA);
    }

    // f128_mul
    public static Float128 Multiply(SoftFloatContext context, Float128 a, Float128 b)
    {
        ulong uiA64,
            uiA0,
            uiB64,
            uiB0,
            magBits,
            sigZExtra;
        int expA,
            expB,
            expZ;
        UInt128M sigA,
            sigB,
            sigZ;
        UInt256M sig256Z;
        bool signA,
            signB,
            signZ;

        uiA64 = a._v64;
        uiA0 = a.Significand00;
        signA = GetSignUI64(uiA64);
        expA = GetExpUI64(uiA64);
        sigA = new UInt128M(GetFracUI64(uiA64), uiA0);
        uiB64 = b._v64;
        uiB0 = b.Significand00;
        signB = GetSignUI64(uiB64);
        expB = GetExpUI64(uiB64);
        sigB = new UInt128M(GetFracUI64(uiB64), uiB0);
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

            return Pack(signZ, 0x7FFF, 0, 0);
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

            return Pack(signZ, 0x7FFF, 0, 0);
        }

        if (expA == 0)
        {
            if (sigA.IsZero)
                return Pack(signZ, 0, 0, 0);

            (expA, sigA) = NormSubnormalSig(sigA);
        }

        if (expB == 0)
        {
            if (sigB.IsZero)
                return Pack(signZ, 0, 0, 0);

            (expB, sigB) = NormSubnormalSig(sigB);
        }

        expZ = expA + expB - 0x4000;
        sigA.V64 |= 0x0001000000000000;
        sigB <<= 16;
        sig256Z = UInt256M.Multiply(sigA, sigB);
        sigZExtra = sig256Z.V064 | (sig256Z.V000 != 0 ? 1U : 0);
        sigZ = sig256Z.V128_UI128 + sigA;

        if (0x0002000000000000 <= sigZ.V64)
        {
            ++expZ;
            (sigZExtra, sigZ) = new UInt128Extra(sigZ, sigZExtra).ShortShiftRightJam(1);
        }

        return RoundPack(context, signZ, expZ, sigZ, sigZExtra);
    }

    // f128_mulAdd
    public static Float128 MultiplyAndAdd(
        SoftFloatContext context,
        Float128 a,
        Float128 b,
        Float128 c
    ) =>
        MulAdd(
            context,
            a._v64,
            a.Significand00,
            b._v64,
            b.Significand00,
            c._v64,
            c.Significand00,
            MulAddOperation.None
        );

    // WARNING: This method overload is experimental and has not been thoroughly tested!
    public static Float128 MultiplyAndAdd(
        SoftFloatContext context,
        Float128 a,
        Float128 b,
        Float128 c,
        MulAddOperation operation
    )
    {
        return
            operation
                is not MulAddOperation.None
                    and not MulAddOperation.SubtractC
                    and not MulAddOperation.SubtractProduct
            ? throw new ArgumentException("Invalid multiply-and-add operation.", nameof(operation))
            : MulAdd(
                context,
                a._v64,
                a.Significand00,
                b._v64,
                b.Significand00,
                c._v64,
                c.Significand00,
                operation
            );
    }

    // f128_div
    public static Float128 Divide(SoftFloatContext context, Float128 a, Float128 b)
    {
        Span<uint> qs = stackalloc uint[3];
        ulong uiA64,
            uiA0,
            uiB64,
            uiB0,
            q64,
            sigZExtra;
        int expA,
            expB,
            expZ;
        UInt128M sigA,
            sigB,
            rem,
            term,
            sigZ;
        uint recip32,
            q;
        bool signA,
            signB,
            signZ;

        uiA64 = a._v64;
        uiA0 = a.Significand00;
        signA = GetSignUI64(uiA64);
        expA = GetExpUI64(uiA64);
        sigA = new UInt128M(GetFracUI64(uiA64), uiA0);
        uiB64 = b._v64;
        uiB0 = b.Significand00;
        signB = GetSignUI64(uiB64);
        expB = GetExpUI64(uiB64);
        sigB = new UInt128M(GetFracUI64(uiB64), uiB0);
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

            return Pack(signZ, 0x7FFF, 0, 0);
        }

        if (expB == 0x7FFF)
        {
            return !sigB.IsZero
                ? context.PropagateNaNFloat128Bits(uiA64, uiA0, uiB64, uiB0)
                : Pack(signZ, 0, 0, 0);
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
                return Pack(signZ, 0x7FFF, 0, 0);
            }

            (expB, sigB) = NormSubnormalSig(sigB);
        }

        if (expA == 0)
        {
            if (sigA.IsZero)
                return Pack(signZ, 0, 0, 0);

            (expA, sigA) = NormSubnormalSig(sigA);
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
        for (var ix = 3; ; )
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
        term = (UInt128M)qs[1] << 54;
        sigZ = new UInt128M((ulong)qs[2] << 19, ((ulong)qs[0] << 25) + (q >> 4)) + term;

        return RoundPack(context, signZ, expZ, sigZ, sigZExtra);
    }

    // f128_rem
    public static Float128 Remainder(SoftFloatContext context, Float128 a, Float128 b)
    {
        ulong uiA64,
            uiA0,
            uiB64,
            uiB0,
            q64;
        int expA,
            expB,
            expDiff;
        UInt128M sigA,
            sigB,
            rem,
            term,
            altRem,
            meanRem;
        uint q,
            recip32;
        bool signA,
            signRem;

        uiA64 = a._v64;
        uiA0 = a.Significand00;
        signA = GetSignUI64(uiA64);
        expA = GetExpUI64(uiA64);
        sigA = new UInt128M(GetFracUI64(uiA64), uiA0);
        uiB64 = b._v64;
        uiB0 = b.Significand00;
        expB = GetExpUI64(uiB64);
        sigB = new UInt128M(GetFracUI64(uiB64), uiB0);

        if (expA == 0x7FFF)
        {
            if (!sigA.IsZero || (expB == 0x7FFF && !sigB.IsZero))
                return context.PropagateNaNFloat128Bits(uiA64, uiA0, uiB64, uiB0);

            context.RaiseFlags(ExceptionFlags.Invalid);
            return context.DefaultNaNFloat128;
        }

        if (expB == 0x7FFF)
        {
            return !sigB.IsZero ? context.PropagateNaNFloat128Bits(uiA64, uiA0, uiB64, uiB0) : a;
        }

        if (expB == 0)
        {
            if (sigB.IsZero)
            {
                context.RaiseFlags(ExceptionFlags.Invalid);
                return context.DefaultNaNFloat128;
            }

            (expB, sigB) = NormSubnormalSig(sigB);
        }

        if (expA == 0)
        {
            if (sigA.IsZero)
                return a;

            (expA, sigA) = NormSubnormalSig(sigA);
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
        } while ((rem.V64 & 0x8000000000000000) == 0);

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

        return NormRoundPack(context, signRem, expB - 1, rem);
    }

    // f128_sqrt
    public Float128 SquareRoot(SoftFloatContext context)
    {
        Span<uint> qs = stackalloc uint[3];
        ulong uiA64,
            uiA0,
            x64,
            sig64Z,
            sigZExtra;
        int expA,
            expZ;
        UInt128M sigA,
            rem,
            y,
            term,
            sigZ;
        uint sig32A,
            recipSqrt32,
            sig32Z,
            q;
        bool signA;

        uiA64 = _v64;
        uiA0 = Significand00;
        signA = GetSignUI64(uiA64);
        expA = GetExpUI64(uiA64);
        sigA = new UInt128M(GetFracUI64(uiA64), uiA0);

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

            (expA, sigA) = NormSubnormalSig(sigA);
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
            term = UInt128M.Multiply64ByShifted32(x64 + sig64Z, q);
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
            term = (UInt128M)sig64Z << 32;
            term += (UInt128M)((ulong)q << 6);
            term *= q;
            rem = y - term;
            if ((rem.V64 & 0x8000000000000000) == 0)
                break;

            --q;
        }

        qs[0] = q;

        q = (uint)((((rem.V64 >> 2) * recipSqrt32) >> 32) + 2);
        sigZExtra = (ulong)q << 59;
        term = (UInt128M)qs[1] << 53;
        sigZ = new UInt128M((ulong)qs[2] << 18, ((ulong)qs[0] << 24) + (q >> 5)) + term;

        if ((q & 0xF) <= 2)
        {
            q &= ~3U;
            sigZExtra = (ulong)q << 59;
            y = sigZ << 6;
            y.V00 |= sigZExtra >> 58;
            term = y - q;
            y = UInt128M.Multiply64ByShifted32(term.V00, q);
            term = UInt128M.Multiply64ByShifted32(term.V64, q);
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

        return RoundPack(context, false, expZ, sigZ, sigZExtra);
    }

    #endregion

    #region Comparison Operations

    // f128_eq (signaling=false) & f128_eq_signaling (signaling=true)
    public static bool CompareEqual(
        SoftFloatContext context,
        Float128 a,
        Float128 b,
        bool signaling
    )
    {
        ulong uiA64,
            uiA0,
            uiB64,
            uiB0;

        uiA64 = a._v64;
        uiA0 = a.Significand00;
        uiB64 = b._v64;
        uiB0 = b.Significand00;

        if (IsNaNUI(uiA64, uiA0) || IsNaNUI(uiB64, uiB0))
        {
            if (
                signaling
                || context.IsSignalingNaNFloat128Bits(uiA64, uiA0)
                || context.IsSignalingNaNFloat128Bits(uiB64, uiB0)
            )
                context.RaiseFlags(ExceptionFlags.Invalid);

            return false;
        }

        return uiA0 == uiB0
            && (uiA64 == uiB64 || (uiA0 == 0 && ((uiA64 | uiB64) & 0x7FFFFFFFFFFFFFFF) == 0));
    }

    // f128_le (signaling=true) & f128_le_quiet (signaling=false)
    public static bool CompareLessThanOrEqual(
        SoftFloatContext context,
        Float128 a,
        Float128 b,
        bool signaling
    )
    {
        ulong uiA64,
            uiA0,
            uiB64,
            uiB0;
        bool signA,
            signB;

        uiA64 = a._v64;
        uiA0 = a.Significand00;
        uiB64 = b._v64;
        uiB0 = b.Significand00;

        if (IsNaNUI(uiA64, uiA0) || IsNaNUI(uiB64, uiB0))
        {
            if (
                signaling
                || context.IsSignalingNaNFloat128Bits(uiA64, uiA0)
                || context.IsSignalingNaNFloat128Bits(uiB64, uiB0)
            )
                context.RaiseFlags(ExceptionFlags.Invalid);

            return false;
        }

        signA = GetSignUI64(uiA64);
        signB = GetSignUI64(uiB64);

        return (signA != signB)
            ? (signA || (((uiA64 | uiB64) & 0x7FFFFFFFFFFFFFFF) | uiA0 | uiB0) == 0)
            : (uiA64 == uiB64 && uiA0 == uiB0)
                || (signA ^ new UInt128M(uiA64, uiA0) < new UInt128M(uiB64, uiB0));
    }

    // f128_lt (signaling=true) & f128_lt_quiet (signaling=false)
    public static bool CompareLessThan(
        SoftFloatContext context,
        Float128 a,
        Float128 b,
        bool signaling
    )
    {
        ulong uiA64,
            uiA0,
            uiB64,
            uiB0;
        bool signA,
            signB;

        uiA64 = a._v64;
        uiA0 = a.Significand00;
        uiB64 = b._v64;
        uiB0 = b.Significand00;

        if (IsNaNUI(uiA64, uiA0) || IsNaNUI(uiB64, uiB0))
        {
            if (
                signaling
                || context.IsSignalingNaNFloat128Bits(uiA64, uiA0)
                || context.IsSignalingNaNFloat128Bits(uiB64, uiB0)
            )
                context.RaiseFlags(ExceptionFlags.Invalid);

            return false;
        }

        signA = GetSignUI64(uiA64);
        signB = GetSignUI64(uiB64);

        return (signA != signB)
            ? (signA && (((uiA64 | uiB64) & 0x7FFFFFFFFFFFFFFF) | uiA0 | uiB0) != 0)
            : (
                (uiA64 != uiB64 || uiA0 != uiB0)
                && (signA ^ new UInt128M(uiA64, uiA0) < new UInt128M(uiB64, uiB0))
            );
    }

    #endregion

    #region Internals

    // signF128UI64
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool GetSignUI64(ulong a64) => (a64 >> 63) != 0;

    // expF128UI64
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int GetExpUI64(ulong a64) => (int)(a64 >> 48) & 0x7FFF;

    // fracF128UI64
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ulong GetFracUI64(ulong a64) => a64 & 0x0000FFFFFFFFFFFF;

    // packToF128UI64
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ulong PackToUI64(bool sign, int exp, ulong sig64) =>
        (sign ? (1UL << 63) : 0UL) + ((ulong)exp << 48) + sig64;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Float128 Pack(bool sign, int exp, ulong sig64, ulong sig0) =>
        FromBitsUI128(v64: PackToUI64(sign, exp, sig64), v0: sig0);

    // isNaNF128UI
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsNaNUI(ulong a64, ulong a0) =>
        (~a64 & 0x7FFF000000000000) == 0 && (a0 != 0 || (a64 & 0x0000FFFFFFFFFFFF) != 0);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsInfUI(ulong a64, ulong a0) =>
        (~a64 & 0x7FFF000000000000) == 0 && a0 == 0 && (a64 & 0x0000FFFFFFFFFFFF) == 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsFiniteUI(ulong a64) => (~a64 & 0x7FFF000000000000) != 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static (int exp, UInt128M sig) NormSubnormalSig(UInt128M sig) =>
        NormSubnormalSig(sig.V64, sig.V00);

    // softfloat_normSubnormalF128Sig
    internal static (int exp, UInt128M sig) NormSubnormalSig(ulong sig64, ulong sig0)
    {
        int shiftDist;
        if (sig64 == 0)
        {
            shiftDist = CountLeadingZeroes64(sig0) - 15;
            return (
                exp: -63 - shiftDist,
                sig: (shiftDist < 0)
                    ? new UInt128M(sig0 >> (-shiftDist), sig0 << shiftDist)
                    : new UInt128M(sig0 << shiftDist, 0)
            );
        }
        else
        {
            shiftDist = CountLeadingZeroes64(sig64) - 15;
            return (exp: 1 - shiftDist, sig: new UInt128M(sig64, sig0) << shiftDist);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Float128 RoundPack(
        SoftFloatContext context,
        bool sign,
        int exp,
        UInt128M sig,
        ulong sigExtra
    ) => RoundPack(context, sign, exp, sig.V64, sig.V00, sigExtra);

    // softfloat_roundPackToF128
    internal static Float128 RoundPack(
        SoftFloatContext context,
        bool sign,
        int exp,
        ulong sig64,
        ulong sig0,
        ulong sigExtra
    )
    {
        var roundingMode = context.Rounding;
        var roundNearEven = roundingMode == RoundingMode.NearEven;
        var roundIncrement =
            (!roundNearEven && roundingMode != RoundingMode.NearMaxMag)
                ? (roundingMode == (sign ? RoundingMode.Min : RoundingMode.Max) && sigExtra != 0)
                : (0x8000000000000000 <= sigExtra);

        if (0x7FFD <= (uint)exp)
        {
            if (exp < 0)
            {
                var isTiny =
                    context.DetectTininess == TininessMode.BeforeRounding
                    || exp < -1
                    || !roundIncrement
                    || new UInt128M(sig64, sig0)
                        < new UInt128M(0x0001FFFFFFFFFFFF, 0xFFFFFFFFFFFFFFFF);
                (sigExtra, sig64, sig0) = new UInt128Extra(sig64, sig0, sigExtra).ShiftRightJam(
                    -exp
                );
                exp = 0;
                if (isTiny && sigExtra != 0)
                    context.RaiseFlags(ExceptionFlags.Underflow);

                roundIncrement =
                    (!roundNearEven && roundingMode != RoundingMode.NearMaxMag)
                        ? (
                            roundingMode == (sign ? RoundingMode.Min : RoundingMode.Max)
                            && sigExtra != 0
                        )
                        : (0x8000000000000000 <= sigExtra);
            }
            else if (
                0x7FFD < exp
                || (
                    exp == 0x7FFD
                    && new UInt128M(sig64, sig0)
                        == new UInt128M(0x0001FFFFFFFFFFFF, 0xFFFFFFFFFFFFFFFF)
                    && roundIncrement
                )
            )
            {
                context.RaiseFlags(ExceptionFlags.Overflow | ExceptionFlags.Inexact);
                return (
                    roundNearEven
                    || roundingMode == RoundingMode.NearMaxMag
                    || roundingMode == (sign ? RoundingMode.Min : RoundingMode.Max)
                )
                    ? Pack(sign, 0x7FFF, 0, 0)
                    : Pack(sign, 0x7FFE, 0x0000FFFFFFFFFFFF, 0xFFFFFFFFFFFFFFFF);
            }
        }

        if (sigExtra != 0)
        {
            context.ExceptionFlags |= ExceptionFlags.Inexact;
            if (roundingMode == RoundingMode.Odd)
                return Pack(sign, exp, sig64, sig0 | 1);
        }

        if (roundIncrement)
        {
            (sig64, sig0) = new UInt128M(sig64, sig0) + UInt128M.One;
            sig0 &= ~((sigExtra & 0x7FFFFFFFFFFFFFFF) == 0 && roundNearEven ? 1UL : 0);
        }
        else
        {
            if ((sig64 | sig0) == 0)
                exp = 0;
        }

        return Pack(sign, exp, sig64, sig0);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Float128 NormRoundPack(
        SoftFloatContext context,
        bool sign,
        int exp,
        UInt128M sig
    ) => NormRoundPack(context, sign, exp, sig.V64, sig.V00);

    // softfloat_normRoundPackToF128
    internal static Float128 NormRoundPack(
        SoftFloatContext context,
        bool sign,
        int exp,
        ulong sig64,
        ulong sig0
    )
    {
        ulong sigExtra;

        if (sig64 == 0)
        {
            exp -= 64;
            sig64 = sig0;
            sig0 = 0;
        }

        var shiftDist = CountLeadingZeroes64(sig64) - 15;
        exp -= shiftDist;
        if (0 <= shiftDist)
        {
            if (shiftDist != 0)
                (sig64, sig0) = new UInt128M(sig64, sig0) << shiftDist;

            if ((uint)exp < 0x7FFD)
                return Pack(sign, (sig64 | sig0) != 0 ? exp : 0, sig64, sig0);

            sigExtra = 0;
        }
        else
        {
            (sigExtra, sig64, sig0) = new UInt128Extra(sig64, sig0).ShortShiftRightJam(-shiftDist);
        }

        return RoundPack(context, sign, exp, sig64, sig0, sigExtra);
    }

    // softfloat_addMagsF128
    internal static Float128 AddMags(
        SoftFloatContext context,
        ulong uiA64,
        ulong uiA0,
        ulong uiB64,
        ulong uiB0,
        bool signZ
    )
    {
        int expA,
            expB,
            expDiff,
            expZ;
        UInt128M sigA,
            sigB,
            sigZ;
        ulong sigZExtra;

        expA = GetExpUI64(uiA64);
        sigA = new UInt128M(GetFracUI64(uiA64), uiA0);
        expB = GetExpUI64(uiB64);
        sigB = new UInt128M(GetFracUI64(uiB64), uiB0);

        expDiff = expA - expB;
        if (expDiff == 0)
        {
            if (expA == 0x7FFF)
            {
                return (!sigA.IsZero || !sigB.IsZero)
                    ? context.PropagateNaNFloat128Bits(uiA64, uiA0, uiB64, uiB0)
                    : FromBitsUI128(v64: uiA64, v0: uiA0);
            }

            sigZ = sigA + sigB;
            if (expA == 0)
                return Pack(signZ, 0, sigZ.V64, sigZ.V00);

            expZ = expA;
            sigZ.V64 |= 0x0002000000000000;
            sigZExtra = 0;
        }
        else
        {
            if (expDiff < 0)
            {
                if (expB == 0x7FFF)
                {
                    return !sigB.IsZero
                        ? context.PropagateNaNFloat128Bits(uiA64, uiA0, uiB64, uiB0)
                        : Pack(signZ, 0x7FFF, 0, 0);
                }

                expZ = expB;
                if (expA != 0)
                {
                    sigA.V64 |= 0x0001000000000000;
                }
                else
                {
                    ++expDiff;
                    sigZExtra = 0;
                    if (expDiff == 0)
                        goto newlyAligned;
                }

                (sigZExtra, sigA) = new UInt128Extra(sigA).ShiftRightJam(-expDiff);
            }
            else
            {
                if (expA == 0x7FFF)
                {
                    return !sigA.IsZero
                        ? context.PropagateNaNFloat128Bits(uiA64, uiA0, uiB64, uiB0)
                        : FromBitsUI128(v64: uiA64, v0: uiA0);
                }

                expZ = expA;
                if (expB != 0)
                {
                    sigB.V64 |= 0x0001000000000000;
                }
                else
                {
                    --expDiff;
                    sigZExtra = 0;
                    if (expDiff == 0)
                        goto newlyAligned;
                }

                (sigZExtra, sigB) = new UInt128Extra(sigB).ShiftRightJam(expDiff);
            }

            newlyAligned:
            sigA.V64 |= 0x0001000000000000;
            sigZ = sigA + sigB;
            --expZ;
            if (sigZ.V64 < 0x0002000000000000)
                return RoundPack(context, signZ, expZ, sigZ, sigZExtra);

            ++expZ;
        }

        (sigZExtra, sigZ) = new UInt128Extra(sigZ, sigZExtra).ShortShiftRightJam(1);
        return RoundPack(context, signZ, expZ, sigZ, sigZExtra);
    }

    // softfloat_subMagsF128
    internal static Float128 SubMags(
        SoftFloatContext context,
        ulong uiA64,
        ulong uiA0,
        ulong uiB64,
        ulong uiB0,
        bool signZ
    )
    {
        int expA,
            expB,
            expDiff,
            expZ;
        UInt128M sigA,
            sigB,
            sigZ;

        expA = GetExpUI64(uiA64);
        sigA = new UInt128M(GetFracUI64(uiA64), uiA0);
        expB = GetExpUI64(uiB64);
        sigB = new UInt128M(GetFracUI64(uiB64), uiB0);

        sigA <<= 4;
        sigB <<= 4;
        expDiff = expA - expB;

        if (expDiff == 0)
        {
            if (expA == 0x7FFF)
            {
                if (!sigA.IsZero || !sigB.IsZero)
                    return context.PropagateNaNFloat128Bits(uiA64, uiA0, uiB64, uiB0);

                context.RaiseFlags(ExceptionFlags.Invalid);
                return context.DefaultNaNFloat128;
            }

            expZ = expA;
            if (expZ == 0)
                expZ = 1;

            // Use CompareTo() and a switch statement instead of spaghetti code (comparison operators are more computationally expensive).
            switch (sigA.CompareTo(sigB))
            {
                case 1:
                {
                    sigZ = sigA - sigB;
                    break;
                }
                case -1:
                {
                    signZ = !signZ;
                    sigZ = sigB - sigA;
                    break;
                }
                default:
                {
                    return Pack(context.Rounding == RoundingMode.Min, 0, 0, 0);
                }
            }
        }
        else if (0 < expDiff)
        {
            if (expA == 0x7FFF)
            {
                return !sigA.IsZero
                    ? context.PropagateNaNFloat128Bits(uiA64, uiA0, uiB64, uiB0)
                    : FromBitsUI128(v64: uiA64, v0: uiA0);
            }

            if (expB != 0)
            {
                sigB.V64 |= 0x0010000000000000;
                sigB = sigB.ShiftRightJam(expDiff);
            }
            else
            {
                --expDiff;
                if (expDiff != 0)
                    sigB = sigB.ShiftRightJam(expDiff);
            }

            expZ = expA;
            sigA.V64 |= 0x0010000000000000;

            sigZ = sigA - sigB;
        }
        else //if (expDiff < 0)
        {
            if (expB == 0x7FFF)
            {
                return !sigB.IsZero
                    ? context.PropagateNaNFloat128Bits(uiA64, uiA0, uiB64, uiB0)
                    : Pack(!signZ, 0x7FFF, 0, 0);
            }

            if (expA != 0)
            {
                sigA.V64 |= 0x0010000000000000;
                sigA = sigA.ShiftRightJam(-expDiff);
            }
            else
            {
                ++expDiff;
                if (expDiff != 0)
                    sigA = sigA.ShiftRightJam(-expDiff);
            }

            expZ = expB;
            sigB.V64 |= 0x0010000000000000;

            signZ = !signZ;
            sigZ = sigB - sigA;
        }

        return NormRoundPack(context, signZ, expZ - 5, sigZ);
    }

    // softfloat_mulAddF128
    internal static Float128 MulAdd(
        SoftFloatContext context,
        ulong uiA64,
        ulong uiA0,
        ulong uiB64,
        ulong uiB0,
        ulong uiC64,
        ulong uiC0,
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
            signZ;
        int expA,
            expB,
            expC,
            expZ,
            shiftDist,
            expDiff;
        UInt128M sigA,
            sigB,
            sigC,
            uiZ,
            sigZ,
            x128;
        ulong magBits,
            sigZExtra,
            sig256Z0;
        UInt256M sig256Z;

        Unsafe.SkipInit(out UInt256M sig256C); // workaround weird spaghetti code logic

        signA = GetSignUI64(uiA64);
        expA = GetExpUI64(uiA64);
        sigA = new UInt128M(GetFracUI64(uiA64), uiA0);

        signB = GetSignUI64(uiB64);
        expB = GetExpUI64(uiB64);
        sigB = new UInt128M(GetFracUI64(uiB64), uiB0);

        signC = GetSignUI64(uiC64) ^ (op == MulAddOperation.SubtractC);
        expC = GetExpUI64(uiC64);
        sigC = new UInt128M(GetFracUI64(uiC64), uiC0);

        signZ = signA ^ signB ^ (op == MulAddOperation.SubtractProduct);

        if (expA == 0x7FFF)
        {
            if (!sigA.IsZero || (expB == 0x7FFF && !sigB.IsZero))
                return context.PropagateNaNFloat128Bits(uiA64, uiA0, uiB64, uiB0, uiC64, uiC0);

            magBits = (uint)expB | sigB.V64 | sigB.V00;
            goto infProdArg;
        }

        if (expB == 0x7FFF)
        {
            if (!sigB.IsZero)
                return context.PropagateNaNFloat128Bits(uiA64, uiA0, uiB64, uiB0, uiC64, uiC0);

            magBits = (uint)expA | sigA.V64 | sigA.V00;
            goto infProdArg;
        }

        if (expC == 0x7FFF)
        {
            if (!sigC.IsZero)
            {
                uiZ = UInt128M.Zero;
                return context.PropagateNaNFloat128Bits(uiZ.V64, uiZ.V00, uiC64, uiC0);
            }

            return FromBitsUI128(v64: uiC64, v0: uiC0);
        }

        if (expA == 0)
        {
            if (sigA.IsZero)
            {
                return ((uint)expC | sigC.V64 | sigC.V00) == 0 && signZ != signC
                    ? Pack(context.Rounding == RoundingMode.Min, 0, 0, 0)
                    : FromBitsUI128(v64: uiC64, v0: uiC0);
            }

            (expA, sigA) = NormSubnormalSig(sigA);
        }

        if (expB == 0)
        {
            if (sigB.IsZero)
            {
                return ((uint)expC | sigC.V64 | sigC.V00) == 0 && signZ != signC
                    ? Pack(context.Rounding == RoundingMode.Min, 0, 0, 0)
                    : FromBitsUI128(v64: uiC64, v0: uiC0);
            }

            (expB, sigB) = NormSubnormalSig(sigB);
        }

        expZ = expA + expB - 0x3FFE;
        sigA.V64 |= 0x0001000000000000;
        sigB.V64 |= 0x0001000000000000;
        sigA <<= 8;
        sigB <<= 15;

        sig256Z = UInt256M.Multiply(sigA, sigB);
        sigZ = sig256Z.V128_UI128;

        shiftDist = 0;
        if ((sigZ.V64 & 0x0100000000000000) == 0)
        {
            --expZ;
            shiftDist = -1;
        }

        if (expC == 0)
        {
            if (sigC.IsZero)
            {
                shiftDist += 8;
                sigZExtra = sig256Z.V064 | sig256Z.V000;
                sigZExtra = (sigZ.V00 << (64 - shiftDist)) | (sigZExtra != 0 ? 1UL : 0);
                sigZ >>= shiftDist;
                return RoundPack(context, signZ, expZ - 1, sigZ, sigZExtra);
            }

            (expC, sigC) = NormSubnormalSig(sigC);
        }

        sigC.V64 |= 0x0001000000000000;
        sigC <<= 8;

        expDiff = expZ - expC;
        if (expDiff < 0)
        {
            expZ = expC;
            if (signZ == signC || expDiff < -1)
            {
                shiftDist -= expDiff;
                if (shiftDist != 0)
                    sigZ = sigZ.ShiftRightJam(shiftDist);
            }
            else
            {
                if (shiftDist == 0)
                {
                    x128 = sig256Z.V000_UI128 >> 1;
                    sig256Z.V064 = (sigZ.V00 << 63) | x128.V64;
                    sig256Z.V000 = x128.V00;
                    sigZ >>= 1;

                    sig256Z.V192 = sigZ.V64;
                    sig256Z.V128 = sigZ.V00;
                }
            }
        }
        else
        {
            if (shiftDist != 0)
                sig256Z += sig256Z; // <<= 1

            if (expDiff == 0)
            {
                sigZ = sig256Z.V128_UI128;
            }
            else
            {
                sig256C = new UInt256M(v128: sigC, v0: UInt128M.Zero);
                sig256C = sig256C.ShiftRightJam(expDiff);
            }
        }

        shiftDist = 8;
        if (signZ == signC)
        {
            if (expDiff <= 0)
            {
                sigZ = sigC + sigZ;
            }
            else
            {
                sig256Z += sig256C;
                sigZ = sig256Z.V128_UI128;
            }

            if ((sigZ.V64 & 0x0200000000000000) != 0)
            {
                ++expZ;
                shiftDist = 9;
            }
        }
        else
        {
            if (expDiff < 0)
            {
                signZ = signC;
                if (expDiff < -1)
                {
                    sigZ = sigC - sigZ;
                    sigZExtra = sig256Z.V064 | sig256Z.V000;
                    if (sigZExtra != 0)
                        sigZ--;

                    if ((sigZ.V64 & 0x0100000000000000) == 0)
                    {
                        --expZ;
                        shiftDist = 7;
                    }

                    sigZExtra = (sigZ.V00 << (64 - shiftDist)) | (sigZExtra != 0 ? 1UL : 0);
                    sigZ >>= shiftDist;
                    return RoundPack(context, signZ, expZ - 1, sigZ, sigZExtra);
                }
                else
                {
                    sig256C = new UInt256M(v128: sigC, v0: UInt128M.Zero);
                    sig256Z = sig256C - sig256Z;
                }
            }
            else if (expDiff == 0)
            {
                sigZ -= sigC;
                if (sigZ.IsZero && sig256Z.V064 == 0 && sig256Z.V000 == 0)
                    return Pack(context.Rounding == RoundingMode.Min, 0, 0, 0);

                sig256Z.V128_UI128 = sigZ;

                if ((sigZ.V64 & 0x8000000000000000) != 0)
                {
                    signZ = !signZ;
                    sig256Z = -sig256Z;
                }
            }
            else
            {
                sig256Z -= sig256C;

                if (1 < expDiff)
                {
                    sigZ = sig256Z.V128_UI128;
                    if ((sigZ.V64 & 0x0100000000000000) == 0)
                    {
                        --expZ;
                        shiftDist = 7;
                    }

                    sigZExtra = sig256Z.V064 | sig256Z.V000;
                    sigZExtra = (sigZ.V00 << (64 - shiftDist)) | (sigZExtra != 0 ? 1UL : 0);
                    sigZ >>= shiftDist;
                    return RoundPack(context, signZ, expZ - 1, sigZ, sigZExtra);
                }
            }

            sigZ = sig256Z.V128_UI128;
            sigZExtra = sig256Z.V064;
            sig256Z0 = sig256Z.V000;
            if (sigZ.V64 != 0)
            {
                if (sig256Z0 != 0)
                    sigZExtra |= 1;
            }
            else
            {
                expZ -= 64;
                sigZ.V64 = sigZ.V00;
                sigZ.V00 = sigZExtra;
                sigZExtra = sig256Z0;
                if (sigZ.V64 == 0)
                {
                    expZ -= 64;
                    sigZ.V64 = sigZ.V00;
                    sigZ.V00 = sigZExtra;
                    sigZExtra = 0;
                    if (sigZ.V64 == 0)
                    {
                        expZ -= 64;
                        sigZ.V64 = sigZ.V00;
                        sigZ.V00 = 0;
                    }
                }
            }

            shiftDist = CountLeadingZeroes64(sigZ.V64);
            expZ += 7 - shiftDist;
            shiftDist = 15 - shiftDist;
            if (0 < shiftDist)
            {
                sigZExtra = (sigZ.V00 << (64 - shiftDist)) | (sigZExtra != 0 ? 1UL : 0);
                sigZ >>= shiftDist;
                return RoundPack(context, signZ, expZ - 1, sigZ, sigZExtra);
            }
            else if (shiftDist != 0)
            {
                shiftDist = -shiftDist;
                sigZ <<= shiftDist;
                x128 = (UInt128M)sigZExtra << shiftDist;
                sigZ.V00 |= x128.V64;
                sigZExtra = x128.V00;
            }

            return RoundPack(context, signZ, expZ - 1, sigZ, sigZExtra);
        }

        sigZExtra = sig256Z.V064 | sig256Z.V000;
        sigZExtra = (sigZ.V00 << (64 - shiftDist)) | (sigZExtra != 0 ? 1UL : 0);
        sigZ >>= shiftDist;
        return RoundPack(context, signZ, expZ - 1, sigZ, sigZExtra);

        infProdArg:
        if (magBits != 0)
        {
            uiZ = new UInt128M(PackToUI64(signZ, 0x7FFF, 0), 0);
            if (expC != 0x7FFF)
                return FromBitsUI128(uiZ);

            if (!sigC.IsZero)
                return context.PropagateNaNFloat128Bits(uiZ.V64, uiZ.V00, uiC64, uiC0);

            if (signZ == signC)
                return FromBitsUI128(uiZ);
        }

        context.RaiseFlags(ExceptionFlags.Invalid);

        var defaultNaNBits = context.DefaultNaNFloat128Bits;
        return context.PropagateNaNFloat128Bits(
            defaultNaNBits.GetUpperUI64(),
            defaultNaNBits.GetLowerUI64(),
            uiC64,
            uiC0
        );
    }

    #endregion

    #endregion
}
