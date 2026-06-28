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

[StructLayout(LayoutKind.Sequential)]
public readonly struct ExtFloat80 : ISpanFormattable
{
    #region Fields

    public const int ExponentBias = 0x3FFF;

    // WARNING: DO NOT ADD OR CHANGE ANY OF THESE FIELDS!!!

    /// <summary>
    /// The complete 64-bit significand of the floating-point value.
    /// </summary>
    #endregion

    #region Constructors

    internal ExtFloat80(ushort signExp, ulong signif)
    {
        Significand = signif;
        SignAndExponent = signExp;
    }

    // NOTE: The exponential is the biased exponent value (not the bit encoded value).
    public ExtFloat80(bool sign, int exponent, ulong significand)
    {
        exponent += ExponentBias;
        if ((exponent >> 15) != 0)
            throw new ArgumentOutOfRangeException(nameof(exponent));

        SignAndExponent = PackToUI64(sign, exponent);
        Significand = significand;
    }

    #endregion

    #region Properties

    public bool Sign => GetSignUI64(SignAndExponent);

    public int Exponent => GetExpUI64(SignAndExponent) - ExponentBias; // offset-binary

    public ulong Significand { get; }

    public bool IsNaN => IsNaNUI(SignAndExponent, Significand);

    // The interpretation is ambiguous between 8087, 80287, and 80387.
    public bool IsInfinity => IsInfUI(SignAndExponent, Significand);

    public bool IsFinite => IsFiniteUI(SignAndExponent);

    // NOTE: This is the raw encoded sign and exponent values (not biased like the Exponent property above).
    public ushort SignAndExponent { get; }

    #endregion

    #region Methods

    public static ExtFloat80 FromUIntBits(ushort signExp, ulong significand) =>
        new(signExp, significand);

    public (ushort signExp, ulong significant) ToUIntBits() => (SignAndExponent, Significand);

    public static ExtFloat80 FromUIntBits(UInt128 value)
    {
        return (value >> 80) != UInt128.Zero
            ? throw new ArgumentOutOfRangeException(
                nameof(value),
                "ExtFloat80 cannot exceed an 80-bit integer."
            )
            : new((ushort)value.GetUpperUI64(), value.GetLowerUI64());
    }

    public UInt128 ToUInt128Bits() => new(SignAndExponent, Significand);

    // THIS IS THE INTERNAL CONSTRUCTOR FOR RAW BITS.
    // NOTE: It doesn't matter if the value exceeds 80 bits, it will always be casted down (this is intentional to simplify calling code).
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ExtFloat80 FromBitsUI128(UInt128M v) => FromBitsUI80(v.V64, v.V00);

    // THIS IS THE INTERNAL CONSTRUCTOR FOR RAW BITS.
    // NOTE: It doesn't matter if signExp exceeds 16 bits, it will always be casted down (this is intentional to simplify calling code).
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ExtFloat80 FromBitsUI80(ulong signExp, ulong signif) =>
        new((ushort)signExp, signif);

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
            var builder = new ValueStringBuilder(stackalloc char[22]);
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
        // Value Format: -7FFF.FFFFFFFFFFFFFFFF
        builder.Append(GetSignUI64(SignAndExponent) ? '-' : '+');
        builder.AppendHex((uint)GetExpUI64(SignAndExponent), 15, isLowerCase);
        builder.Append('.');
        builder.AppendHex(Significand, 64, isLowerCase);
    }

    #region Integer-to-floating-point Conversions

    // ui32_to_extF80
    [SuppressMessage(
        "Style",
        "IDE0060:Remove unused parameter",
        Justification = "API consistency and possible future use."
    )]
    public static ExtFloat80 FromUInt32(SoftFloatContext context, uint a)
    {
        uint uiZ64;
        if (a != 0)
        {
            var shiftDist = CountLeadingZeroes32(a);
            uiZ64 = 0x401E - (uint)shiftDist;
            a <<= shiftDist;
        }
        else
        {
            uiZ64 = 0;
        }

        return FromBitsUI80(uiZ64, (ulong)a << 32);
    }

    // ui64_to_extF80
    [SuppressMessage(
        "Style",
        "IDE0060:Remove unused parameter",
        Justification = "API consistency and possible future use."
    )]
    public static ExtFloat80 FromUInt64(SoftFloatContext context, ulong a)
    {
        uint uiZ64;
        if (a != 0)
        {
            var shiftDist = CountLeadingZeroes64(a);
            uiZ64 = 0x403E - (uint)shiftDist;
            a <<= shiftDist;
        }
        else
        {
            uiZ64 = 0;
        }

        return FromBitsUI80(uiZ64, a);
    }

    // i32_to_extF80
    [SuppressMessage(
        "Style",
        "IDE0060:Remove unused parameter",
        Justification = "API consistency and possible future use."
    )]
    public static ExtFloat80 FromInt32(SoftFloatContext context, int a)
    {
        uint absA;
        if (a != 0)
        {
            var sign = a < 0;
            absA = (uint)(sign ? -a : a);
            var shiftDist = CountLeadingZeroes32(absA);
            return Pack(sign, 0x401E - shiftDist, (ulong)absA << (shiftDist + 32));
        }

        return FromBitsUI80(0, 0);
    }

    // i64_to_extF80
    [SuppressMessage(
        "Style",
        "IDE0060:Remove unused parameter",
        Justification = "API consistency and possible future use."
    )]
    public static ExtFloat80 FromInt64(SoftFloatContext context, long a)
    {
        ulong absA;
        if (a != 0)
        {
            var sign = a < 0;
            absA = (ulong)(sign ? -a : a);
            var shiftDist = CountLeadingZeroes64(absA);
            return Pack(sign, 0x403E - shiftDist, absA << shiftDist);
        }

        return FromBitsUI80(0, 0);
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

    // extF80_to_ui32
    public uint ToUInt32(SoftFloatContext context, RoundingMode roundingMode, bool exact)
    {
        uint uiA64;
        int exp,
            shiftDist;
        ulong sig;
        bool sign;

        uiA64 = SignAndExponent;
        sign = GetSignUI64(uiA64);
        exp = GetExpUI64(uiA64);
        sig = Significand;

        if (exp == 0x7FFF && (sig & 0x7FFFFFFFFFFFFFFF) != 0)
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

        shiftDist = 0x4032 - exp;
        if (shiftDist <= 0)
            shiftDist = 1;

        sig = sig.ShiftRightJam(shiftDist);
        return RoundToUI32(context, sign, sig, roundingMode, exact);
    }

    // extF80_to_ui64
    public ulong ToUInt64(SoftFloatContext context, RoundingMode roundingMode, bool exact)
    {
        uint uiA64;
        bool sign;
        int exp,
            shiftDist;
        ulong sig,
            sigExtra;

        uiA64 = SignAndExponent;
        sign = GetSignUI64(uiA64);
        exp = GetExpUI64(uiA64);
        sig = Significand;

        shiftDist = 0x403E - exp;
        if (shiftDist < 0)
        {
            context.RaiseFlags(ExceptionFlags.Invalid);
            return (exp == 0x7FFF && (sig & 0x7FFFFFFFFFFFFFFF) != 0)
                ? context.UInt64FromNaN
                : context.UInt64FromOverflow(sign);
        }

        sigExtra = 0;
        if (shiftDist != 0)
            (sigExtra, sig) = new UInt64Extra(sig).ShiftRightJam(shiftDist);

        return RoundToUI64(context, sign, sig, sigExtra, roundingMode, exact);
    }

    // extF80_to_i32
    public int ToInt32(SoftFloatContext context, RoundingMode roundingMode, bool exact)
    {
        uint uiA64;
        int exp,
            shiftDist;
        ulong sig;
        bool sign;

        uiA64 = SignAndExponent;
        sign = GetSignUI64(uiA64);
        exp = GetExpUI64(uiA64);
        sig = Significand;

        if (exp == 0x7FFF && (sig & 0x7FFFFFFFFFFFFFFF) != 0)
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

        shiftDist = 0x4032 - exp;
        if (shiftDist <= 0)
            shiftDist = 1;

        sig = sig.ShiftRightJam(shiftDist);
        return RoundToI32(context, sign, sig, roundingMode, exact);
    }

    // extF80_to_i64
    public long ToInt64(SoftFloatContext context, RoundingMode roundingMode, bool exact)
    {
        uint uiA64;
        int exp,
            shiftDist;
        ulong sig,
            sigExtra;
        bool sign;

        uiA64 = SignAndExponent;
        sign = GetSignUI64(uiA64);
        exp = GetExpUI64(uiA64);
        sig = Significand;

        shiftDist = 0x403E - exp;
        if (shiftDist <= 0)
        {
            if (shiftDist != 0)
            {
                context.RaiseFlags(ExceptionFlags.Invalid);
                return (exp == 0x7FFF && (sig & 0x7FFFFFFFFFFFFFFF) != 0)
                    ? context.Int64FromNaN
                    : context.Int64FromOverflow(sign);
            }

            sigExtra = 0;
        }
        else
        {
            (sigExtra, sig) = new UInt64Extra(sig).ShiftRightJam(shiftDist);
        }

        return RoundToI64(context, sign, sig, sigExtra, roundingMode, exact);
    }

    // extF80_to_ui32_r_minMag
    public uint ToUInt32RoundMinMag(SoftFloatContext context, bool exact)
    {
        uint uiA64,
            z;
        int exp,
            shiftDist;
        ulong sig;
        bool sign;

        uiA64 = SignAndExponent;
        exp = GetExpUI64(uiA64);
        sig = Significand;

        shiftDist = 0x403E - exp;
        if (64 <= shiftDist)
        {
            if (exact && ((uint)exp | sig) != 0)
                context.ExceptionFlags |= ExceptionFlags.Inexact;

            return 0;
        }

        sign = GetSignUI64(uiA64);
        if (sign || shiftDist < 32)
        {
            context.RaiseFlags(ExceptionFlags.Invalid);
            return (exp == 0x7FFF && (sig & 0x7FFFFFFFFFFFFFFF) != 0)
                ? context.UInt32FromNaN
                : context.UInt32FromOverflow(sign);
        }

        z = (uint)(sig >> shiftDist);
        if (exact && ((ulong)z << shiftDist) != sig)
            context.ExceptionFlags |= ExceptionFlags.Inexact;

        return z;
    }

    // extF80_to_ui64_r_minMag
    public ulong ToUInt64RoundMinMag(SoftFloatContext context, bool exact)
    {
        uint uiA64;
        int exp,
            shiftDist;
        ulong sig,
            z;
        bool sign;

        uiA64 = SignAndExponent;
        exp = GetExpUI64(uiA64);
        sig = Significand;

        shiftDist = 0x403E - exp;
        if (64 <= shiftDist)
        {
            if (exact && ((uint)exp | sig) != 0)
                context.ExceptionFlags |= ExceptionFlags.Inexact;

            return 0;
        }

        sign = GetSignUI64(uiA64);
        if (sign || shiftDist < 0)
        {
            context.RaiseFlags(ExceptionFlags.Invalid);
            return (exp == 0x7FFF && (sig & 0x7FFFFFFFFFFFFFFF) != 0)
                ? context.UInt64FromNaN
                : context.UInt64FromOverflow(sign);
        }

        z = sig >> shiftDist;
        if (exact && (z << shiftDist) != sig)
            context.ExceptionFlags |= ExceptionFlags.Inexact;

        return z;
    }

    // extF80_to_i32_r_minMag
    public int ToInt32RoundMinMag(SoftFloatContext context, bool exact)
    {
        uint uiA64;
        int exp,
            shiftDist,
            absZ;
        ulong sig;
        bool sign;

        uiA64 = SignAndExponent;
        exp = GetExpUI64(uiA64);
        sig = Significand;

        shiftDist = 0x403E - exp;
        if (64 <= shiftDist)
        {
            if (exact && ((uint)exp | sig) != 0)
                context.ExceptionFlags |= ExceptionFlags.Inexact;

            return 0;
        }

        sign = GetSignUI64(uiA64);
        if (shiftDist < 33)
        {
            if (uiA64 == PackToUI64(true, 0x401E) && sig < 0x8000000100000000)
            {
                if (exact && (sig & 0x00000000FFFFFFFF) != 0)
                    context.ExceptionFlags |= ExceptionFlags.Inexact;

                return -0x7FFFFFFF - 1;
            }

            context.RaiseFlags(ExceptionFlags.Invalid);
            return (exp == 0x7FFF && (sig & 0x7FFFFFFFFFFFFFFF) != 0)
                ? context.Int32FromNaN
                : context.Int32FromOverflow(sign);
        }

        absZ = (int)(sig >> shiftDist);
        if (exact && ((ulong)(uint)absZ << shiftDist) != sig)
            context.ExceptionFlags |= ExceptionFlags.Inexact;

        return sign ? -absZ : absZ;
    }

    // extF80_to_i64_r_minMag
    public long ToInt64RoundMinMag(SoftFloatContext context, bool exact)
    {
        uint uiA64;
        int exp,
            shiftDist;
        ulong sig,
            absZ;
        bool sign;

        uiA64 = SignAndExponent;
        exp = GetExpUI64(uiA64);
        sig = Significand;

        shiftDist = 0x403E - exp;
        if (64 <= shiftDist)
        {
            if (exact && ((uint)exp | sig) != 0)
                context.ExceptionFlags |= ExceptionFlags.Inexact;

            return 0;
        }

        sign = GetSignUI64(uiA64);
        if (shiftDist <= 0)
        {
            if (uiA64 == PackToUI64(true, 0x403E) && sig == 0x8000000000000000)
                return -0x7FFFFFFFFFFFFFFF - 1;

            context.RaiseFlags(ExceptionFlags.Invalid);
            return (exp == 0x7FFF && (sig & 0x7FFFFFFFFFFFFFFF) != 0)
                ? context.Int64FromNaN
                : context.Int64FromOverflow(sign);
        }

        absZ = sig >> shiftDist;
        if (exact && (sig << (-shiftDist)) != 0)
            context.ExceptionFlags |= ExceptionFlags.Inexact;

        return sign ? -(long)absZ : (long)absZ;
    }

    #endregion

    #region Floating-point-to-floating-point Conversions

    // extF80_to_f16
    public Float16 ToFloat16(SoftFloatContext context)
    {
        uint uiA64,
            sig16;
        ulong uiA0,
            sig;
        int exp;
        bool sign;

        uiA64 = SignAndExponent;
        uiA0 = Significand;
        sign = GetSignUI64(uiA64);
        exp = GetExpUI64(uiA64);
        sig = uiA0;

        if (exp == 0x7FFF)
        {
            if ((sig & 0x7FFFFFFFFFFFFFFF) != 0)
            {
                context.ExtFloat80BitsToCommonNaN(uiA64, uiA0, out var commonNaN);
                return context.CommonNaNToFloat16(in commonNaN);
            }
            else
            {
                return Float16.Pack(sign, 0x1f, 0);
            }
        }

        sig16 = (uint)sig.ShortShiftRightJam(49);
        if (((uint)exp | sig16) == 0)
            return Float16.Pack(sign, 0, 0);

        exp -= 0x3FF1;
        if (sizeof(int) < sizeof(int) && exp < -0x40)
            exp = -0x40;

        return Float16.RoundPack(context, sign, exp, sig16);
    }

    // extF80_to_f32
    public Float32 ToFloat32(SoftFloatContext context)
    {
        uint uiA64;
        ulong uiA0,
            sig;
        int exp;
        uint sig32;
        bool sign;

        uiA64 = SignAndExponent;
        uiA0 = Significand;
        sign = GetSignUI64(uiA64);
        exp = GetExpUI64(uiA64);
        sig = uiA0;

        if (exp == 0x7FFF)
        {
            if ((sig & 0x7FFFFFFFFFFFFFFF) != 0)
            {
                context.ExtFloat80BitsToCommonNaN(uiA64, uiA0, out var commonNaN);
                return context.CommonNaNToFloat32(in commonNaN);
            }
            else
            {
                return Float32.Pack(sign, 0xFF, 0);
            }
        }

        sig32 = (uint)sig.ShortShiftRightJam(33);
        if (((uint)exp | sig32) == 0)
            return Float32.Pack(sign, 0, 0);

        exp -= 0x3F81;
        if (sizeof(int) < sizeof(int) && exp < -0x1000)
            exp = -0x1000;

        return Float32.RoundPack(context, sign, exp, sig32);
    }

    // extF80_to_f64
    public Float64 ToFloat64(SoftFloatContext context)
    {
        uint uiA64;
        ulong uiA0,
            sig;
        int exp;
        bool sign;

        uiA64 = SignAndExponent;
        uiA0 = Significand;
        sign = GetSignUI64(uiA64);
        exp = GetExpUI64(uiA64);
        sig = uiA0;

        if (((uint)exp | sig) == 0)
            return Float64.Pack(sign, 0, 0);

        if (exp == 0x7FFF)
        {
            if ((sig & 0x7FFFFFFFFFFFFFFF) != 0)
            {
                context.ExtFloat80BitsToCommonNaN(uiA64, uiA0, out var commonNaN);
                return context.CommonNaNToFloat64(in commonNaN);
            }
            else
            {
                return Float64.Pack(sign, 0x7FF, 0);
            }
        }

        sig = sig.ShortShiftRightJam(1);
        exp -= 0x3C01;
        if (sizeof(int) < sizeof(int) && exp < -0x1000)
            exp = -0x1000;

        return Float64.RoundPack(context, sign, exp, sig);
    }

    // extF80_to_f128
    public Float128 ToFloat128(SoftFloatContext context)
    {
        uint uiA64,
            exp;
        ulong uiA0,
            frac;
        UInt128M frac128;
        bool sign;

        uiA64 = SignAndExponent;
        uiA0 = Significand;
        exp = (uint)GetExpUI64(uiA64);
        frac = uiA0 & 0x7FFFFFFFFFFFFFFF;

        if (exp == 0x7FFF && frac != 0)
        {
            context.ExtFloat80BitsToCommonNaN(uiA64, uiA0, out var commonNaN);
            return context.CommonNaNToFloat128(in commonNaN);
        }
        else
        {
            sign = GetSignUI64(uiA64);
            frac128 = (UInt128M)frac << 49;
            return Float128.Pack(sign, (int)exp, frac128.V64, frac128.V00);
        }
    }

    #endregion

    #region Arithmetic Operations

    // extF80_roundToInt
    public ExtFloat80 RoundToInt(SoftFloatContext context, RoundingMode roundingMode, bool exact)
    {
        uint uiA64,
            signUI64,
            uiZ64;
        int exp;
        ulong sigA,
            sigZ,
            lastBitMask,
            roundBitsMask;

        uiA64 = SignAndExponent;
        signUI64 = uiA64 & PackToUI64(true, 0);
        exp = GetExpUI64(uiA64);
        sigA = Significand;

        if ((sigA & 0x8000000000000000) == 0 && exp != 0x7FFF)
        {
            if (sigA == 0)
                return FromBitsUI80(signUI64, 0);

            (var expTmp, sigA) = NormSubnormalSig(sigA);
            exp += expTmp;
        }

        if (0x403E <= exp)
        {
            if (exp == 0x7FFF)
            {
                if ((sigA & 0x7FFFFFFFFFFFFFFF) != 0)
                    return context.PropagateNaNExtFloat80Bits(uiA64, sigA, 0, 0);

                sigZ = 0x8000000000000000;
            }
            else
            {
                sigZ = sigA;
            }

            return FromBitsUI80(signUI64 | (uint)exp, sigZ);
        }
        else if (exp <= 0x3FFE)
        {
            if (exact)
                context.ExceptionFlags |= ExceptionFlags.Inexact;

            switch (roundingMode)
            {
                case RoundingMode.NearEven:
                {
                    if ((sigA & 0x7FFFFFFFFFFFFFFF) != 0)
                        goto case RoundingMode.NearMaxMag;

                    break;
                }
                case RoundingMode.NearMaxMag:
                {
                    if (exp == 0x3FFE)
                        return FromBitsUI80(signUI64 | 0x3FFF, 0x8000000000000000);

                    break;
                }
                case RoundingMode.Min:
                {
                    if (signUI64 != 0)
                        return FromBitsUI80(signUI64 | 0x3FFF, 0x8000000000000000);

                    break;
                }
                case RoundingMode.Max:
                {
                    if (signUI64 == 0)
                        return FromBitsUI80(signUI64 | 0x3FFF, 0x8000000000000000);

                    break;
                }
                case RoundingMode.Odd:
                {
                    return FromBitsUI80(signUI64 | 0x3FFF, 0x8000000000000000);
                }
            }

            return FromBitsUI80(signUI64, 0);
        }

        uiZ64 = signUI64 | (uint)exp;
        lastBitMask = 1UL << (0x403E - exp);
        roundBitsMask = lastBitMask - 1;
        sigZ = sigA;
        if (roundingMode == RoundingMode.NearMaxMag)
        {
            sigZ += lastBitMask >> 1;
        }
        else if (roundingMode == RoundingMode.NearEven)
        {
            sigZ += lastBitMask >> 1;
            if ((sigZ & roundBitsMask) == 0)
                sigZ &= ~lastBitMask;
        }
        else if (roundingMode == (signUI64 != 0 ? RoundingMode.Min : RoundingMode.Max))
        {
            sigZ += roundBitsMask;
        }

        sigZ &= ~roundBitsMask;
        if (sigZ == 0)
        {
            ++uiZ64;
            sigZ = 0x8000000000000000;
        }

        if (sigZ != sigA)
        {
            if (roundingMode == RoundingMode.Odd)
                sigZ |= lastBitMask;

            if (exact)
                context.ExceptionFlags |= ExceptionFlags.Inexact;
        }

        return FromBitsUI80(uiZ64, sigZ);
    }

    // extF80_add
    public static ExtFloat80 Add(SoftFloatContext context, ExtFloat80 a, ExtFloat80 b)
    {
        var signA = GetSignUI64(a.SignAndExponent);
        var signB = GetSignUI64(b.SignAndExponent);

        return signA == signB
            ? AddMags(
                context,
                a.SignAndExponent,
                a.Significand,
                b.SignAndExponent,
                b.Significand,
                signA
            )
            : SubMags(
                context,
                a.SignAndExponent,
                a.Significand,
                b.SignAndExponent,
                b.Significand,
                signA
            );
    }

    // extF80_sub
    public static ExtFloat80 Subtract(SoftFloatContext context, ExtFloat80 a, ExtFloat80 b)
    {
        var signA = GetSignUI64(a.SignAndExponent);
        var signB = GetSignUI64(b.SignAndExponent);

        return (signA == signB)
            ? SubMags(
                context,
                a.SignAndExponent,
                a.Significand,
                b.SignAndExponent,
                b.Significand,
                signA
            )
            : AddMags(
                context,
                a.SignAndExponent,
                a.Significand,
                b.SignAndExponent,
                b.Significand,
                signA
            );
    }

    // extF80_mul
    public static ExtFloat80 Multiply(SoftFloatContext context, ExtFloat80 a, ExtFloat80 b)
    {
        uint uiA64,
            uiB64;
        ulong uiA0,
            sigA,
            uiB0,
            sigB;
        int expA,
            expB,
            expZ;
        UInt128M sig128Z;
        bool signA,
            signB,
            signZ;

        uiA64 = a.SignAndExponent;
        uiA0 = a.Significand;
        signA = GetSignUI64(uiA64);
        expA = GetExpUI64(uiA64);
        sigA = uiA0;
        uiB64 = b.SignAndExponent;
        uiB0 = b.Significand;
        signB = GetSignUI64(uiB64);
        expB = GetExpUI64(uiB64);
        sigB = uiB0;
        signZ = signA ^ signB;

        if (expA == 0x7FFF)
        {
            if (
                (sigA & 0x7FFFFFFFFFFFFFFF) != 0
                || (expB == 0x7FFF && (sigB & 0x7FFFFFFFFFFFFFFF) != 0)
            )
                return context.PropagateNaNExtFloat80Bits(uiA64, uiA0, uiB64, uiB0);

            if (((uint)expB | sigB) == 0)
            {
                context.RaiseFlags(ExceptionFlags.Invalid);
                return context.DefaultNaNExtFloat80;
            }
            else
            {
                return Pack(signZ, 0x7FFF, 0x8000000000000000);
            }
        }
        else if (expB == 0x7FFF)
        {
            if ((sigB & 0x7FFFFFFFFFFFFFFF) != 0)
                return context.PropagateNaNExtFloat80Bits(uiA64, uiA0, uiB64, uiB0);

            if (((uint)expA | sigA) == 0)
            {
                context.RaiseFlags(ExceptionFlags.Invalid);
                return context.DefaultNaNExtFloat80;
            }
            else
            {
                return Pack(signZ, 0x7FFF, 0x8000000000000000);
            }
        }

        if (expA == 0)
            expA = 1;

        if ((sigA & 0x8000000000000000) == 0)
        {
            if (sigA == 0)
                return Pack(signZ, 0, 0);

            (var expTmp, sigA) = NormSubnormalSig(sigA);
            expA += expTmp;
        }

        if (expB == 0)
            expB = 1;

        if ((sigB & 0x8000000000000000) == 0)
        {
            if (sigB == 0)
                return Pack(signZ, 0, 0);

            (var expTmp, sigB) = NormSubnormalSig(sigB);
            expB += expTmp;
        }

        expZ = expA + expB - 0x3FFE;
        sig128Z = UInt128M.Multiply(sigA, sigB);
        if (sig128Z.V64 < 0x8000000000000000)
        {
            --expZ;
            sig128Z += sig128Z; // shift left by one instead?
        }

        return RoundPack(
            context,
            signZ,
            expZ,
            sig128Z.V64,
            sig128Z.V00,
            context.RoundingPrecisionExtFloat80
        );
    }

    // extF80_div
    public static ExtFloat80 Divide(SoftFloatContext context, ExtFloat80 a, ExtFloat80 b)
    {
        uint uiA64,
            uiB64,
            recip32,
            q;
        ulong uiA0,
            sigA,
            uiB0,
            sigB,
            sigZ,
            q64,
            sigZExtra;
        int expA,
            expB,
            expZ;
        UInt128M rem,
            term;
        int ix;
        bool signA,
            signB,
            signZ;

        uiA64 = a.SignAndExponent;
        uiA0 = a.Significand;
        signA = GetSignUI64(uiA64);
        expA = GetExpUI64(uiA64);
        sigA = uiA0;
        uiB64 = b.SignAndExponent;
        uiB0 = b.Significand;
        signB = GetSignUI64(uiB64);
        expB = GetExpUI64(uiB64);
        sigB = uiB0;
        signZ = signA ^ signB;

        if (expA == 0x7FFF)
        {
            if ((sigA & 0x7FFFFFFFFFFFFFFF) != 0)
                return context.PropagateNaNExtFloat80Bits(uiA64, uiA0, uiB64, uiB0);

            if (expB == 0x7FFF)
            {
                if ((sigB & 0x7FFFFFFFFFFFFFFF) != 0)
                    return context.PropagateNaNExtFloat80Bits(uiA64, uiA0, uiB64, uiB0);

                context.RaiseFlags(ExceptionFlags.Invalid);
                return context.DefaultNaNExtFloat80;
            }

            return Pack(signZ, 0x7FFF, 0x8000000000000000);
        }
        else if (expB == 0x7FFF)
        {
            return ((sigB & 0x7FFFFFFFFFFFFFFF) != 0)
                ? context.PropagateNaNExtFloat80Bits(uiA64, uiA0, uiB64, uiB0)
                : Pack(signZ, 0, 0);
        }

        if (expB == 0)
            expB = 1;

        if ((sigB & 0x8000000000000000) == 0)
        {
            if (sigB == 0)
            {
                if (sigA == 0)
                {
                    context.RaiseFlags(ExceptionFlags.Invalid);
                    return context.DefaultNaNExtFloat80;
                }

                context.RaiseFlags(ExceptionFlags.Infinite);
                return Pack(signZ, 0x7FFF, 0x8000000000000000);
            }

            (var expTmp, sigB) = NormSubnormalSig(sigB);
            expB += expTmp;
        }

        if (expA == 0)
            expA = 1;

        if ((sigA & 0x8000000000000000) == 0)
        {
            if (sigA == 0)
                return Pack(signZ, 0, 0);

            (var expTmp, sigA) = NormSubnormalSig(sigA);
            expA += expTmp;
        }

        expZ = expA - expB + 0x3FFF;
        if (sigA < sigB)
        {
            --expZ;
            rem = (UInt128M)sigA << 32;
        }
        else
        {
            rem = (UInt128M)sigA << 31;
        }

        recip32 = ApproxRecip32_1((uint)(sigB >> 32));
        sigZ = 0;
        ix = 2;
        while (true)
        {
            q64 = (ulong)(uint)(rem.V64 >> 2) * recip32;
            q = (uint)((q64 + 0x80000000) >> 32);
            if (--ix < 0)
                break;

            rem <<= 29;
            term = UInt128M.Multiply64ByShifted32(sigB, q);
            rem -= term;
            if ((rem.V64 & 0x8000000000000000) != 0)
            {
                --q;
                rem += new UInt128M(sigB >> 32, sigB << 32);
            }

            sigZ = (sigZ << 29) + q;
        }

        if (((q + 1) & 0x3FFFFF) < 2)
        {
            rem <<= 29;
            term = UInt128M.Multiply64ByShifted32(sigB, q);
            rem -= term;
            term = (UInt128M)sigB << 32;
            if ((rem.V64 & 0x8000000000000000) != 0)
            {
                --q;
                rem += term;
            }
            else if (term <= rem)
            {
                ++q;
                rem -= term;
            }

            if (!rem.IsZero)
                q |= 1;
        }

        sigZ = (sigZ << 6) + (q >> 23);
        sigZExtra = (ulong)q << 41;

        return RoundPack(
            context,
            signZ,
            expZ,
            sigZ,
            sigZExtra,
            context.RoundingPrecisionExtFloat80
        );
    }

    // extF80_rem
    public static ExtFloat80 Remainder(SoftFloatContext context, ExtFloat80 a, ExtFloat80 b)
    {
        uint uiA64,
            uiB64,
            q,
            recip32;
        ulong uiA0,
            sigA,
            uiB0,
            sigB,
            q64;
        int expA,
            expB,
            expDiff;
        UInt128M rem,
            shiftedSigB,
            term,
            altRem,
            meanRem;
        bool signA,
            signRem;

        uiA64 = a.SignAndExponent;
        uiA0 = a.Significand;
        signA = GetSignUI64(uiA64);
        expA = GetExpUI64(uiA64);
        sigA = uiA0;
        uiB64 = b.SignAndExponent;
        uiB0 = b.Significand;
        expB = GetExpUI64(uiB64);
        sigB = uiB0;

        if (expA == 0x7FFF)
        {
            if (
                (sigA & 0x7FFFFFFFFFFFFFFF) != 0
                || (expB == 0x7FFF && (sigB & 0x7FFFFFFFFFFFFFFF) != 0)
            )
                return context.PropagateNaNExtFloat80Bits(uiA64, uiA0, uiB64, uiB0);

            context.RaiseFlags(ExceptionFlags.Invalid);
            return context.DefaultNaNExtFloat80;
        }
        else if (expB == 0x7FFF)
        {
            if ((sigB & 0x7FFFFFFFFFFFFFFF) != 0)
                return context.PropagateNaNExtFloat80Bits(uiA64, uiA0, uiB64, uiB0);

            // Argument b is an infinity. Doubling 'expB' is an easy way to ensure that 'expDiff' later is less than -1, which will result
            // in returning a canonicalized version of argument 'a'.
            expB += expB;
        }

        if (expB == 0)
            expB = 1;

        if ((sigB & 0x8000000000000000) == 0)
        {
            if (sigB == 0)
            {
                context.RaiseFlags(ExceptionFlags.Invalid);
                return context.DefaultNaNExtFloat80;
            }

            (var expTmp, sigB) = NormSubnormalSig(sigB);
            expB += expTmp;
        }

        if (expA == 0)
            expA = 1;

        if ((sigA & 0x8000000000000000) == 0)
        {
            if (sigA == 0)
            {
                expA = 0;
                if (expA < 1)
                {
                    sigA >>= 1 - expA;
                    expA = 0;
                }

                return Pack(signA, expA, sigA);
            }

            (var expTmp, sigA) = NormSubnormalSig(sigA);
            expA += expTmp;
        }

        expDiff = expA - expB;
        if (expDiff < -1)
        {
            if (expA < 1)
            {
                sigA >>= 1 - expA;
                expA = 0;
            }

            return Pack(signA, expA, sigA);
        }

        rem = new UInt128M(0, sigA) << 32;
        shiftedSigB = new UInt128M(0, sigB) << 32;

        if (expDiff < 1)
        {
            if (expDiff != 0)
            {
                --expB;
                shiftedSigB = new UInt128M(0, sigB) << 33;
                q = 0;
            }
            else
            {
                q = (sigB <= sigA) ? 1U : 0;
                if (q != 0)
                    rem -= shiftedSigB;
            }
        }
        else
        {
            recip32 = ApproxRecip32_1((uint)(sigB >> 32));
            expDiff -= 30;
            while (true)
            {
                q64 = (ulong)(uint)(rem.V64 >> 2) * recip32;
                if (expDiff < 0)
                    break;

                q = (uint)((q64 + 0x80000000) >> 32);
                rem <<= 29;
                term = UInt128M.Multiply64ByShifted32(sigB, q);
                rem -= term;
                if ((rem.V64 & 0x8000000000000000) != 0)
                    rem += shiftedSigB;

                expDiff -= 29;
            }

            // ('expDiff' cannot be less than -29 here.)
            q = (uint)(q64 >> 32) >> (~expDiff);
            rem <<= expDiff + 30;
            term = UInt128M.Multiply64ByShifted32(sigB, q);
            rem -= term;
            if ((rem.V64 & 0x8000000000000000) != 0)
            {
                altRem = rem + shiftedSigB;
                goto selectRem;
            }
        }

        do
        {
            altRem = rem;
            ++q;
            rem -= shiftedSigB;
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

        return NormRoundPack(
            context,
            signRem,
            !rem.IsZero ? expB + 32 : 0,
            rem.V64,
            rem.V00,
            ExtFloat80RoundingPrecision._80
        );
    }

    // extF80_sqrt
    public ExtFloat80 SquareRoot(SoftFloatContext context)
    {
        uint uiA64;
        uint sig32A,
            recipSqrt32,
            sig32Z;
        ulong uiA0,
            sigA,
            q,
            x64,
            sigZ,
            sigZExtra;
        int expA,
            expZ;
        UInt128M rem,
            y,
            term;
        bool signA;

        uiA64 = SignAndExponent;
        uiA0 = Significand;
        signA = GetSignUI64(uiA64);
        expA = GetExpUI64(uiA64);
        sigA = uiA0;

        if (expA == 0x7FFF)
        {
            if ((sigA & 0x7FFFFFFFFFFFFFFF) != 0)
                return context.PropagateNaNExtFloat80Bits(uiA64, uiA0, 0, 0);

            if (!signA)
                return this;

            context.RaiseFlags(ExceptionFlags.Invalid);
            return context.DefaultNaNExtFloat80;
        }

        if (signA)
        {
            if (sigA == 0)
                return Pack(signA, 0, 0);

            context.RaiseFlags(ExceptionFlags.Invalid);
            return context.DefaultNaNExtFloat80;
        }

        if (expA == 0)
            expA = 1;

        if ((sigA & 0x8000000000000000) == 0)
        {
            if (sigA == 0)
                return Pack(signA, 0, 0);

            (var expTmp, sigA) = NormSubnormalSig(sigA);
            expA += expTmp;
        }

        // ('sig32Z' is guaranteed to be a lower bound on the square root of 'sig32A', which makes 'sig32Z' also a lower bound on the
        // square root of 'sigA'.)

        expZ = ((expA - 0x3FFF) >> 1) + 0x3FFF;
        expA &= 1;
        sig32A = (uint)(sigA >> 32);
        recipSqrt32 = ApproxRecipSqrt32_1((uint)expA, sig32A);
        sig32Z = (uint)(((ulong)sig32A * recipSqrt32) >> 32);
        if (expA != 0)
        {
            sig32Z >>= 1;
            rem = new UInt128M(0, sigA) << 61;
        }
        else
        {
            rem = new UInt128M(0, sigA) << 62;
        }

        rem.V64 -= (ulong)sig32Z * sig32Z;
        q = ((uint)(rem.V64 >> 2) * (ulong)recipSqrt32) >> 32;
        x64 = (ulong)sig32Z << 32;
        sigZ = x64 + (q << 3);
        y = rem << 29;

        // (Repeating this loop is a rare occurrence.)
        while (true)
        {
            term = UInt128M.Multiply64ByShifted32(x64 + sigZ, (uint)q);
            rem = y - term;
            if ((rem.V64 & 0x8000000000000000) == 0)
                break;

            --q;
            sigZ -= 1U << 3;
        }

        q = (((rem.V64 >> 2) * recipSqrt32) >> 32) + 2;
        x64 = sigZ;
        sigZ = (sigZ << 1) + (q >> 25);
        sigZExtra = q << 39;

        if ((q & 0xFFFFFF) <= 2)
        {
            q &= ~0xFFFFUL;
            sigZExtra = q << 39;
            term = UInt128M.Multiply64ByShifted32(x64 + (q >> 27), (uint)q);
            x64 = (uint)(q << 5) * (ulong)(uint)q;
            term += x64;
            rem <<= 28;
            rem -= term;
            if ((rem.V64 & 0x8000000000000000) != 0)
            {
                if (sigZExtra == 0)
                    --sigZ;

                --sigZExtra;
            }
            else
            {
                if (!rem.IsZero)
                    sigZExtra |= 1;
            }
        }

        return RoundPack(
            context,
            false,
            expZ,
            sigZ,
            sigZExtra,
            context.RoundingPrecisionExtFloat80
        );
    }

    #endregion

    #region Comparison Operations

    // extF80_eq (signaling=false) & extF80_eq_signaling (signaling=true)
    public static bool CompareEqual(
        SoftFloatContext context,
        ExtFloat80 a,
        ExtFloat80 b,
        bool signaling
    )
    {
        uint uiA64,
            uiB64;
        ulong uiA0,
            uiB0;

        uiA64 = a.SignAndExponent;
        uiA0 = a.Significand;
        uiB64 = b.SignAndExponent;
        uiB0 = b.Significand;

        if (IsNaNUI((int)uiA64, uiA0) || IsNaNUI((int)uiB64, uiB0))
        {
            if (
                signaling
                || context.IsSignalingNaNExtFloat80Bits(uiA64, uiA0)
                || context.IsSignalingNaNExtFloat80Bits(uiB64, uiB0)
            )
                context.RaiseFlags(ExceptionFlags.Invalid);

            return false;
        }

        return uiA0 == uiB0 && (uiA64 == uiB64 || (uiA0 == 0 && ((uiA64 | uiB64) & 0x7FFF) == 0));
    }

    // extF80_le (signaling=true) & extF80_le_quiet (signaling=false)
    public static bool CompareLessThanOrEqual(
        SoftFloatContext context,
        ExtFloat80 a,
        ExtFloat80 b,
        bool signaling
    )
    {
        uint uiA64,
            uiB64;
        ulong uiA0,
            uiB0;
        bool signA,
            signB;

        uiA64 = a.SignAndExponent;
        uiA0 = a.Significand;
        uiB64 = b.SignAndExponent;
        uiB0 = b.Significand;

        if (IsNaNUI((int)uiA64, uiA0) || IsNaNUI((int)uiB64, uiB0))
        {
            if (
                signaling
                || context.IsSignalingNaNExtFloat80Bits(uiA64, uiA0)
                || context.IsSignalingNaNExtFloat80Bits(uiB64, uiB0)
            )
                context.RaiseFlags(ExceptionFlags.Invalid);

            return false;
        }

        signA = GetSignUI64(uiA64);
        signB = GetSignUI64(uiB64);

        return (signA != signB)
            ? (signA || (((uiA64 | uiB64) & 0x7FFF) == 0 && (uiA0 | uiB0) == 0))
            : (uiA64 == uiB64 && uiA0 == uiB0)
                || (signA ^ new UInt128M(uiA64, uiA0) < new UInt128M(uiB64, uiB0));
    }

    // extF80_lt (signaling=true) & extF80_lt_quiet (signaling=false)
    public static bool CompareLessThan(
        SoftFloatContext context,
        ExtFloat80 a,
        ExtFloat80 b,
        bool signaling
    )
    {
        uint uiA64,
            uiB64;
        ulong uiA0,
            uiB0;
        bool signA,
            signB;

        uiA64 = a.SignAndExponent;
        uiA0 = a.Significand;
        uiB64 = b.SignAndExponent;
        uiB0 = b.Significand;

        if (IsNaNUI((int)uiA64, uiA0) || IsNaNUI((int)uiB64, uiB0))
        {
            if (
                signaling
                || context.IsSignalingNaNExtFloat80Bits(uiA64, uiA0)
                || context.IsSignalingNaNExtFloat80Bits(uiB64, uiB0)
            )
                context.RaiseFlags(ExceptionFlags.Invalid);

            return false;
        }

        signA = GetSignUI64(uiA64);
        signB = GetSignUI64(uiB64);

        return (signA != signB)
            ? (signA && (((uiA64 | uiB64) & 0x7FFF) != 0 || (uiA0 | uiB0) != 0))
            : (
                (uiA64 != uiB64 || uiA0 != uiB0)
                && (signA ^ new UInt128M(uiA64, uiA0) < new UInt128M(uiB64, uiB0))
            );
    }

    #endregion

    #region Internals

    // signExtF80UI64
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool GetSignUI64(uint a64) => (a64 >> 15) != 0;

    // expExtF80UI64
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int GetExpUI64(uint a64) => (int)(a64 & 0x7FFF);

    // packToExtF80UI64
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ushort PackToUI64(bool sign, int exp) =>
        (ushort)((sign ? (1U << 15) : 0U) | (uint)exp);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ExtFloat80 Pack(bool sign, int exp, ulong sig) =>
        FromBitsUI80(PackToUI64(sign, exp), sig);

    // isNaNExtF80UI
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsNaNUI(int a64, ulong a0) =>
        (a64 & 0x7FFF) == 0x7FFF && (a0 & 0x7FFFFFFFFFFFFFFF) != 0;

    // According to wikipedia, this can be either "Pseudo-Infinity" (valid before 80387) as well as "Infinity" (valid on 80387), depending on bit 63.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsInfUI(int a64, ulong a0) =>
        (a64 & 0x7FFF) == 0x7FFF && (a0 & 0x7FFFFFFFFFFFFFFF) == 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsFiniteUI(int a64) => (a64 & 0x7FFF) != 0x7FFF;

    // softfloat_normSubnormalExtF80Sig
    internal static (int exp, ulong sig) NormSubnormalSig(ulong sig)
    {
        var shiftDist = CountLeadingZeroes64(sig);
        return (exp: -shiftDist, sig: sig << shiftDist);
    }

    // softfloat_roundPackToExtF80
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ExtFloat80 RoundPack(
        SoftFloatContext context,
        bool sign,
        int exp,
        ulong sig,
        ulong sigExtra,
        ExtFloat80RoundingPrecision roundingPrecision
    )
    {
        Debug.Assert(
            roundingPrecision
                is ExtFloat80RoundingPrecision._32
                    or ExtFloat80RoundingPrecision._64
                    or ExtFloat80RoundingPrecision._80,
            "Unexpected rounding precision."
        );
        return roundingPrecision switch
        {
            ExtFloat80RoundingPrecision._32 => RoundPackImpl32Or64(
                context,
                sign,
                exp,
                sig,
                sigExtra,
                0x0000008000000000,
                0x000000FFFFFFFFFF
            ),
            ExtFloat80RoundingPrecision._64 => RoundPackImpl32Or64(
                context,
                sign,
                exp,
                sig,
                sigExtra,
                0x0000000000000400,
                0x00000000000007FF
            ),
            _ => RoundPackImpl80(context, sign, exp, sig, sigExtra),
        };
    }

    // Called when rounding precision is 32 or 64.
    private static ExtFloat80 RoundPackImpl32Or64(
        SoftFloatContext context,
        bool sign,
        int exp,
        ulong sig,
        ulong sigExtra,
        ulong roundIncrement,
        ulong roundMask
    )
    {
        ulong roundBits;

        var roundingMode = context.Rounding;
        var roundNearEven = (roundingMode == RoundingMode.NearEven);

        sig |= sigExtra != 0 ? 1UL : 0;
        roundBits = sig & roundMask;
        if (!roundNearEven && roundingMode != RoundingMode.NearMaxMag)
            roundIncrement =
                (roundingMode == (sign ? RoundingMode.Min : RoundingMode.Max)) ? roundMask : 0;

        if (0x7FFD <= (uint)(exp - 1))
        {
            if (exp <= 0)
            {
                var isTiny =
                    context.DetectTininess == TininessMode.BeforeRounding
                    || exp < 0
                    || sig <= sig + roundIncrement;
                sig = sig.ShiftRightJam(1 - exp);
                roundBits = sig & roundMask;
                if (roundBits != 0)
                {
                    if (isTiny)
                        context.RaiseFlags(ExceptionFlags.Underflow);

                    context.ExceptionFlags |= ExceptionFlags.Inexact;
                    if (roundingMode == RoundingMode.Odd)
                        sig |= roundMask + 1;
                }

                sig += roundIncrement;
                exp = (sig & 0x8000000000000000) != 0 ? 1 : 0;
                roundIncrement = roundMask + 1;
                if (roundNearEven && (roundBits << 1) == roundIncrement)
                    roundMask |= roundIncrement;

                sig &= ~roundMask;
                return Pack(sign, exp, sig);
            }

            if (0x7FFE < exp || (exp == 0x7FFE && sig + roundIncrement < sig))
            {
                context.RaiseFlags(ExceptionFlags.Overflow | ExceptionFlags.Inexact);
                if (
                    roundNearEven
                    || roundingMode == RoundingMode.NearMaxMag
                    || roundingMode == (sign ? RoundingMode.Min : RoundingMode.Max)
                )
                {
                    exp = 0x7FFF;
                    sig = 0x8000000000000000;
                }
                else
                {
                    exp = 0x7FFE;
                    sig = ~roundMask;
                }

                return Pack(sign, exp, sig);
            }
        }

        if (roundBits != 0)
        {
            context.ExceptionFlags |= ExceptionFlags.Inexact;
            if (roundingMode == RoundingMode.Odd)
            {
                sig = (sig & ~roundMask) | (roundMask + 1);
                return Pack(sign, exp, sig);
            }
        }

        sig += roundIncrement;
        if (sig < roundIncrement)
        {
            ++exp;
            sig = 0x8000000000000000;
        }

        roundIncrement = roundMask + 1;
        if (roundNearEven && (roundBits << 1) == roundIncrement)
            roundMask |= roundIncrement;

        sig &= ~roundMask;
        return Pack(sign, exp, sig);
    }

    // Called when rounding precision is 80 (or anything except 32 or 64).
    private static ExtFloat80 RoundPackImpl80(
        SoftFloatContext context,
        bool sign,
        int exp,
        ulong sig,
        ulong sigExtra
    )
    {
        var roundingMode = context.Rounding;
        var roundNearEven = (roundingMode == RoundingMode.NearEven);
        var roundIncrement =
            (!roundNearEven && roundingMode != RoundingMode.NearMaxMag)
                ? (roundingMode == (sign ? RoundingMode.Min : RoundingMode.Max) && sigExtra != 0)
                : (0x8000000000000000 <= sigExtra);

        if (0x7FFD <= (uint)(exp - 1))
        {
            if (exp <= 0)
            {
                var isTiny =
                    context.DetectTininess == TininessMode.BeforeRounding
                    || exp < 0
                    || !roundIncrement
                    || sig < 0xFFFFFFFFFFFFFFFF;
                (sigExtra, sig) = new UInt64Extra(sig, sigExtra).ShiftRightJam(1 - exp);
                exp = 0;
                if (sigExtra != 0)
                {
                    if (isTiny)
                        context.RaiseFlags(ExceptionFlags.Underflow);

                    context.ExceptionFlags |= ExceptionFlags.Inexact;
                    if (roundingMode == RoundingMode.Odd)
                    {
                        sig |= 1;
                        return Pack(sign, exp, sig);
                    }
                }

                roundIncrement =
                    (!roundNearEven && roundingMode != RoundingMode.NearMaxMag)
                        ? (
                            roundingMode == (sign ? RoundingMode.Min : RoundingMode.Max)
                            && sigExtra != 0
                        )
                        : (0x8000000000000000 <= sigExtra);
                if (roundIncrement)
                {
                    ++sig;
                    sig &= ~((sigExtra & 0x7FFFFFFFFFFFFFFF) == 0 & roundNearEven ? 1UL : 0);
                    exp = (sig & 0x8000000000000000) != 0 ? 1 : 0;
                }

                return Pack(sign, exp, sig);
            }

            if (0x7FFE < exp || (exp == 0x7FFE && sig == 0xFFFFFFFFFFFFFFFF && roundIncrement))
            {
                context.RaiseFlags(ExceptionFlags.Overflow | ExceptionFlags.Inexact);
                if (
                    roundNearEven
                    || roundingMode == RoundingMode.NearMaxMag
                    || roundingMode == (sign ? RoundingMode.Min : RoundingMode.Max)
                )
                {
                    exp = 0x7FFF;
                    sig = 0x8000000000000000;
                }
                else
                {
                    exp = 0x7FFE;
                    sig = ~0UL;
                }

                return Pack(sign, exp, sig);
            }
        }

        if (sigExtra != 0)
        {
            context.ExceptionFlags |= ExceptionFlags.Inexact;
            if (roundingMode == RoundingMode.Odd)
                return Pack(sign, exp, sig | 1);
        }

        if (roundIncrement)
        {
            ++sig;
            if (sig == 0)
            {
                ++exp;
                sig = 0x8000000000000000;
            }
            else
            {
                sig &= ~((sigExtra & 0x7FFFFFFFFFFFFFFF) == 0 & roundNearEven ? 1UL : 0);
            }
        }

        return Pack(sign, exp, sig);
    }

    // softfloat_normRoundPackToExtF80
    internal static ExtFloat80 NormRoundPack(
        SoftFloatContext context,
        bool sign,
        int exp,
        ulong sig,
        ulong sigExtra,
        ExtFloat80RoundingPrecision roundingPrecision
    )
    {
        if (sig == 0)
        {
            exp -= 64;
            sig = sigExtra;
            sigExtra = 0;
        }

        var shiftDist = CountLeadingZeroes64(sig);
        exp -= shiftDist;
        if (shiftDist != 0)
            (sig, sigExtra) = new UInt128M(sig, sigExtra) << shiftDist;

        return RoundPack(context, sign, exp, sig, sigExtra, roundingPrecision);
    }

    // softfloat_addMagsExtF80
    internal static ExtFloat80 AddMags(
        SoftFloatContext context,
        uint uiA64,
        ulong uiA0,
        uint uiB64,
        ulong uiB0,
        bool signZ
    )
    {
        int expA,
            expB,
            expDiff,
            expZ;
        ulong sigA,
            sigB,
            sigZ,
            sigZExtra;

        expA = GetExpUI64(uiA64);
        sigA = uiA0;
        expB = GetExpUI64(uiB64);
        sigB = uiB0;

        expDiff = expA - expB;
        if (expDiff == 0)
        {
            if (expA == 0x7FFF)
            {
                return (((sigA | sigB) & 0x7FFFFFFFFFFFFFFF) != 0)
                    ? context.PropagateNaNExtFloat80Bits(uiA64, uiA0, uiB64, uiB0)
                    : FromBitsUI80(uiA64, uiA0);
            }

            sigZ = sigA + sigB;
            sigZExtra = 0;
            if (expA == 0)
            {
                (expZ, sigZ) = NormSubnormalSig(sigZ);
                expZ++;
                return RoundPack(
                    context,
                    signZ,
                    expZ,
                    sigZ,
                    sigZExtra,
                    context.RoundingPrecisionExtFloat80
                );
            }

            expZ = expA;
        }
        else
        {
            if (expDiff < 0)
            {
                if (expB == 0x7FFF)
                {
                    return ((sigB & 0x7FFFFFFFFFFFFFFF) != 0)
                        ? context.PropagateNaNExtFloat80Bits(uiA64, uiA0, uiB64, uiB0)
                        : Pack(signZ, 0x7FFF, uiB0);
                }

                expZ = expB;
                if (expA == 0)
                {
                    ++expDiff;
                    sigZExtra = 0;
                    if (expDiff == 0)
                        goto newlyAligned;
                }

                (sigZExtra, sigA) = new UInt64Extra(sigA).ShiftRightJam(-expDiff);
            }
            else
            {
                if (expA == 0x7FFF)
                {
                    return ((sigA & 0x7FFFFFFFFFFFFFFF) != 0)
                        ? context.PropagateNaNExtFloat80Bits(uiA64, uiA0, uiB64, uiB0)
                        : FromBitsUI80(uiA64, uiA0);
                }

                expZ = expA;
                if (expB == 0)
                {
                    --expDiff;
                    sigZExtra = 0;
                    if (expDiff == 0)
                        goto newlyAligned;
                }

                (sigZExtra, sigB) = new UInt64Extra(sigB).ShiftRightJam(expDiff);
            }

            newlyAligned:
            sigZ = sigA + sigB;
            if ((sigZ & 0x8000000000000000) != 0)
                return RoundPack(
                    context,
                    signZ,
                    expZ,
                    sigZ,
                    sigZExtra,
                    context.RoundingPrecisionExtFloat80
                );
        }

        (sigZExtra, sigZ) = new UInt64Extra(sigZ, sigZExtra).ShortShiftRightJam(1);
        sigZ |= 0x8000000000000000;
        ++expZ;
        return RoundPack(
            context,
            signZ,
            expZ,
            sigZ,
            sigZExtra,
            context.RoundingPrecisionExtFloat80
        );
    }

    // softfloat_subMagsExtF80
    internal static ExtFloat80 SubMags(
        SoftFloatContext context,
        uint uiA64,
        ulong uiA0,
        uint uiB64,
        ulong uiB0,
        bool signZ
    )
    {
        int expA,
            expB,
            expDiff,
            expZ;
        ulong sigA,
            sigB,
            sigExtra;
        UInt128M sig128;

        expA = GetExpUI64(uiA64);
        sigA = uiA0;
        expB = GetExpUI64(uiB64);
        sigB = uiB0;

        expDiff = expA - expB;
        if (expDiff == 0)
        {
            if (expA == 0x7FFF)
            {
                if (((sigA | sigB) & 0x7FFFFFFFFFFFFFFF) != 0)
                    return context.PropagateNaNExtFloat80Bits(uiA64, uiA0, uiB64, uiB0);

                context.RaiseFlags(ExceptionFlags.Invalid);
                return context.DefaultNaNExtFloat80;
            }

            expZ = expA;
            if (expZ == 0)
                expZ = 1;

            sigExtra = 0;

            if (sigB < sigA)
            {
                sig128 = new UInt128M(sigA, 0) - new UInt128M(sigB, sigExtra);
            }
            else if (sigA < sigB)
            {
                signZ = !signZ;
                sig128 = new UInt128M(sigB, 0) - new UInt128M(sigA, sigExtra);
            }
            else
            {
                return Pack(context.Rounding == RoundingMode.Min, 0, 0);
            }
        }
        else if (0 < expDiff)
        {
            if (expA == 0x7FFF)
            {
                return ((sigA & 0x7FFFFFFFFFFFFFFF) != 0)
                    ? context.PropagateNaNExtFloat80Bits(uiA64, uiA0, uiB64, uiB0)
                    : FromBitsUI80(uiA64, uiA0);
            }

            if (expB == 0)
            {
                --expDiff;
                sigExtra = 0;
                if (expDiff != 0)
                    (sigB, sigExtra) = new UInt128M(sigB, 0).ShiftRightJam(expDiff);
            }
            else
            {
                (sigB, sigExtra) = new UInt128M(sigB, 0).ShiftRightJam(expDiff);
            }

            expZ = expA;
            sig128 = new UInt128M(sigA, 0) - new UInt128M(sigB, sigExtra);
        }
        else //if (expDiff < 0)
        {
            if (expB == 0x7FFF)
            {
                return ((sigB & 0x7FFFFFFFFFFFFFFF) != 0)
                    ? context.PropagateNaNExtFloat80Bits(uiA64, uiA0, uiB64, uiB0)
                    : Pack(!signZ, 0x7FFF, 0x8000000000000000);
            }

            if (expA == 0)
            {
                ++expDiff;
                sigExtra = 0;
                if (expDiff != 0)
                    (sigA, sigExtra) = new UInt128M(sigA, 0).ShiftRightJam(-expDiff);
            }
            else
            {
                (sigA, sigExtra) = new UInt128M(sigA, 0).ShiftRightJam(-expDiff);
            }

            signZ = !signZ;
            expZ = expB;
            sig128 = new UInt128M(sigB, 0) - new UInt128M(sigA, sigExtra);
        }

        return NormRoundPack(
            context,
            signZ,
            expZ,
            sig128.V64,
            sig128.V00,
            context.RoundingPrecisionExtFloat80
        );
    }

    #endregion

    #endregion
}
