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

[StructLayout(LayoutKind.Sequential, Pack = sizeof(ulong), Size = sizeof(ulong))]
public readonly struct Float64 : ISpanFormattable
{
    #region Fields

    public const int ExponentBias = 0x3FF;

    // WARNING: DO NOT ADD OR CHANGE ANY OF THESE FIELDS!!!
    private readonly ulong _v;

    #endregion

    #region Constructors

    [SuppressMessage(
        "Style",
        "IDE0060:Remove unused parameter",
        Justification = "Used to avoid accidentally calling other overloads."
    )]
    private Float64(ulong v, bool dummy)
    {
        _v = v;
    }

    public Float64(double value)
    {
        _v = BitConverter.DoubleToUInt64Bits(value);
    }

    // NOTE: The exponential is the biased exponent value (not the bit encoded value).
    public Float64(bool sign, int exponent, ulong significand)
    {
        exponent += ExponentBias;
        if ((exponent >> 11) != 0)
            throw new ArgumentOutOfRangeException(nameof(exponent));

        if ((significand >> 52) != 0)
            throw new ArgumentOutOfRangeException(nameof(significand));

        _v = PackToUI(sign, exponent, significand);
    }

    #endregion

    #region Properties

    public bool Sign => GetSignUI(_v);

    public int Exponent => GetExpUI(_v) - ExponentBias; // offset-binary

    public ulong Significand => GetFracUI(_v);

    public bool IsNaN => IsNaNUI(_v);

    public bool IsInfinity => IsInfUI(_v);

    public bool IsFinite => IsFiniteUI(_v);

    #endregion

    #region Methods

    public static explicit operator Float64(double value) => new(value);

    public static implicit operator double(Float64 value) =>
        BitConverter.UInt64BitsToDouble(value._v);

    public static Float64 FromUIntBits(ulong value) => FromBitsUI64(value);

    public ulong ToUInt64Bits() => _v;

    // THIS IS THE INTERNAL CONSTRUCTOR FOR RAW BITS.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Float64 FromBitsUI64(ulong v) => new(v, dummy: false);

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
            var builder = new ValueStringBuilder(stackalloc char[18]);
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
        // Value Format: -7FF.FFFFFFFFFFFFF
        builder.Append(GetSignUI(_v) ? '-' : '+');
        builder.AppendHex((uint)GetExpUI(_v), 11, isLowerCase);
        builder.Append('.');
        builder.AppendHex(GetFracUI(_v), 52, isLowerCase);
    }

    #region Integer-to-floating-point Conversions

    // ui32_to_f64
    [SuppressMessage(
        "Style",
        "IDE0060:Remove unused parameter",
        Justification = "API consistency and possible future use."
    )]
    public static Float64 FromUInt32(SoftFloatContext context, uint a)
    {
        if (a == 0)
            return FromBitsUI64(0);

        var shiftDist = CountLeadingZeroes32(a) + 21;
        return Pack(false, 0x432 - shiftDist, (ulong)a << shiftDist);
    }

    // ui64_to_f64
    public static Float64 FromUInt64(SoftFloatContext context, ulong a)
    {
        return a == 0 ? FromBitsUI64(0)
            : (a & 0x8000000000000000) != 0
                ? RoundPack(context, false, 0x43D, a.ShortShiftRightJam(1))
            : NormRoundPack(context, false, 0x43C, a);
    }

    // i32_to_f64
    [SuppressMessage(
        "Style",
        "IDE0060:Remove unused parameter",
        Justification = "API consistency and possible future use."
    )]
    public static Float64 FromInt32(SoftFloatContext context, int a)
    {
        if (a == 0)
            return FromBitsUI64(0);

        var sign = a < 0;
        var absA = (uint)(sign ? -a : a);
        var shiftDist = CountLeadingZeroes32(absA) + 21;
        return Pack(sign, 0x432 - shiftDist, (ulong)absA << shiftDist);
    }

    // i64_to_f64
    public static Float64 FromInt64(SoftFloatContext context, long a)
    {
        var sign = a < 0;
        if ((a & 0x7FFFFFFFFFFFFFFF) == 0)
            return FromBitsUI64(sign ? PackToUI(true, 0x43E, 0UL) : 0UL);

        var absA = (ulong)(sign ? -a : a);
        return NormRoundPack(context, sign, 0x43C, absA);
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

    // f64_to_ui32
    public uint ToUInt32(SoftFloatContext context, RoundingMode roundingMode, bool exact)
    {
        ulong sig;
        int exp,
            shiftDist;
        bool sign;

        sign = GetSignUI(_v);
        exp = GetExpUI(_v);
        sig = GetFracUI(_v);

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
            sig = sig.ShiftRightJam(shiftDist);

        return RoundToUI32(context, sign, sig, roundingMode, exact);
    }

    // f64_to_ui64
    public ulong ToUInt64(SoftFloatContext context, RoundingMode roundingMode, bool exact)
    {
        ulong sig;
        int exp,
            shiftDist;
        UInt64Extra sigExtra;
        bool sign;

        sign = GetSignUI(_v);
        exp = GetExpUI(_v);
        sig = GetFracUI(_v);

        if (exp != 0)
            sig |= 0x0010000000000000;

        shiftDist = 0x433 - exp;
        if (shiftDist <= 0)
        {
            if (shiftDist < -11)
            {
                context.RaiseFlags(ExceptionFlags.Invalid);
                return (exp == 0x7FF && GetFracUI(_v) != 0)
                    ? context.UInt64FromNaN
                    : context.UInt64FromOverflow(sign);
            }

            sigExtra.V = sig << (-shiftDist);
            sigExtra.Extra = 0;
        }
        else
        {
            sigExtra = new UInt64Extra(sig, 0).ShiftRightJam(shiftDist);
        }

        return RoundToUI64(context, sign, sigExtra.V, sigExtra.Extra, roundingMode, exact);
    }

    // f64_to_i32
    public int ToInt32(SoftFloatContext context, RoundingMode roundingMode, bool exact)
    {
        ulong sig;
        int exp,
            shiftDist;
        bool sign;

        sign = GetSignUI(_v);
        exp = GetExpUI(_v);
        sig = GetFracUI(_v);

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
            sig = sig.ShiftRightJam(shiftDist);

        return RoundToI32(context, sign, sig, roundingMode, exact);
    }

    // f64_to_i64
    public long ToInt64(SoftFloatContext context, RoundingMode roundingMode, bool exact)
    {
        ulong sig;
        int exp,
            shiftDist;
        UInt64Extra sigExtra;
        bool sign;

        sign = GetSignUI(_v);
        exp = GetExpUI(_v);
        sig = GetFracUI(_v);

        if (exp != 0)
            sig |= 0x0010000000000000;

        shiftDist = 0x433 - exp;
        if (shiftDist <= 0)
        {
            if (shiftDist < -11)
            {
                context.RaiseFlags(ExceptionFlags.Invalid);
                return (exp == 0x7FF && GetFracUI(_v) != 0)
                    ? context.Int64FromNaN
                    : context.Int64FromOverflow(sign);
            }

            sigExtra.V = sig << (-shiftDist);
            sigExtra.Extra = 0;
        }
        else
        {
            sigExtra = new UInt64Extra(sig, 0).ShiftRightJam(shiftDist);
        }

        return RoundToI64(context, sign, sigExtra.V, sigExtra.Extra, roundingMode, exact);
    }

    // f64_to_ui32_r_minMag
    public uint ToUInt32RoundMinMag(SoftFloatContext context, bool exact)
    {
        ulong sig;
        int exp,
            shiftDist;
        uint z;
        bool sign;

        exp = GetExpUI(_v);
        sig = GetFracUI(_v);

        shiftDist = 0x433 - exp;
        if (53 <= shiftDist)
        {
            if (exact && ((uint)exp | sig) != 0)
                context.ExceptionFlags |= ExceptionFlags.Inexact;

            return 0;
        }

        sign = GetSignUI(_v);
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
        ulong sig,
            z;
        int exp,
            shiftDist;
        bool sign;

        exp = GetExpUI(_v);
        sig = GetFracUI(_v);

        shiftDist = 0x433 - exp;
        if (53 <= shiftDist)
        {
            if (exact && ((uint)exp | sig) != 0)
                context.ExceptionFlags |= ExceptionFlags.Inexact;

            return 0;
        }

        sign = GetSignUI(_v);
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
        int exp,
            shiftDist;
        int absZ;
        bool sign;

        exp = GetExpUI(_v);
        sig = GetFracUI(_v);

        shiftDist = 0x433 - exp;
        if (53 <= shiftDist)
        {
            if (exact && ((uint)exp | sig) != 0)
                context.ExceptionFlags |= ExceptionFlags.Inexact;

            return 0;
        }

        sign = GetSignUI(_v);
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
        int exp,
            shiftDist;
        long absZ;
        bool sign;

        sign = GetSignUI(_v);
        exp = GetExpUI(_v);
        sig = GetFracUI(_v);

        shiftDist = 0x433 - exp;
        if (shiftDist <= 0)
        {
            if (shiftDist < -10)
            {
                if (_v == PackToUI(true, 0x43E, 0))
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

        sign = GetSignUI(_v);
        exp = GetExpUI(_v);
        frac = GetFracUI(_v);

        if (exp == 0x7FF)
        {
            if (frac != 0)
            {
                context.Float64BitsToCommonNaN(_v, out var commonNaN);
                return context.CommonNaNToFloat16(in commonNaN);
            }

            return Float16.Pack(sign, 0x1F, 0);
        }

        frac16 = (uint)frac.ShortShiftRightJam(38);
        return ((uint)exp | frac16) == 0
            ? Float16.Pack(sign, 0, 0)
            : Float16.RoundPack(context, sign, exp - 0x3F1, frac16 | 0x4000);
    }

    // f64_to_f32
    public Float32 ToFloat32(SoftFloatContext context)
    {
        ulong frac;
        int exp;
        uint frac32;
        bool sign;

        sign = GetSignUI(_v);
        exp = GetExpUI(_v);
        frac = GetFracUI(_v);

        if (exp == 0x7FF)
        {
            if (frac != 0)
            {
                context.Float64BitsToCommonNaN(_v, out var commonNaN);
                return context.CommonNaNToFloat32(in commonNaN);
            }

            return Float32.Pack(sign, 0xFF, 0);
        }

        frac32 = (uint)frac.ShortShiftRightJam(22);
        return ((uint)exp | frac32) == 0
            ? Float32.Pack(sign, 0, 0)
            : Float32.RoundPack(context, sign, exp - 0x381, frac32 | 0x40000000);
    }

    // f64_to_extF80
    public ExtFloat80 ToExtFloat80(SoftFloatContext context)
    {
        ulong frac;
        int exp;
        bool sign;

        sign = GetSignUI(_v);
        exp = GetExpUI(_v);
        frac = GetFracUI(_v);

        if (exp == 0x7FF)
        {
            if (frac != 0)
            {
                context.Float64BitsToCommonNaN(_v, out var commonNaN);
                return context.CommonNaNToExtFloat80(in commonNaN);
            }

            return ExtFloat80.Pack(sign, 0x7FFF, 0x8000000000000000);
        }

        if (exp == 0)
        {
            if (frac == 0)
                return ExtFloat80.Pack(sign, 0, 0);

            (exp, frac) = NormSubnormalSig(frac);
        }

        return ExtFloat80.Pack(sign, exp + 0x3C00, (frac | 0x0010000000000000) << 11);
    }

    // f64_to_f128
    public Float128 ToFloat128(SoftFloatContext context)
    {
        ulong frac;
        int exp;
        UInt128M frac128;
        bool sign;

        sign = GetSignUI(_v);
        exp = GetExpUI(_v);
        frac = GetFracUI(_v);

        if (exp == 0x7FF)
        {
            if (frac != 0)
            {
                context.Float64BitsToCommonNaN(_v, out var commonNaN);
                return context.CommonNaNToFloat128(in commonNaN);
            }

            return Float128.Pack(sign, 0x7FFF, 0, 0);
        }

        if (exp == 0)
        {
            if (frac == 0)
                return Float128.Pack(sign, 0, 0, 0);

            (exp, frac) = NormSubnormalSig(frac);
            exp--;
        }

        frac128 = (UInt128M)frac << 60;
        return Float128.Pack(sign, exp + 0x3C00, frac128.V64, frac128.V00);
    }

    #endregion

    #region Arithmetic Operations

    public Float64 RoundToInt(SoftFloatContext context, bool exact) =>
        RoundToInt(context, context.Rounding, exact);

    // f64_roundToInt
    public Float64 RoundToInt(SoftFloatContext context, RoundingMode roundingMode, bool exact)
    {
        ulong uiZ,
            lastBitMask,
            roundBitsMask;
        int exp;

        exp = GetExpUI(_v);
        if (exp <= 0x3FE)
        {
            if ((_v & 0x7FFFFFFFFFFFFFFF) == 0)
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
                    if (exp == 0x3FE)
                        uiZ |= PackToUI(false, 0x3FF, 0);

                    break;
                }
                case RoundingMode.Min:
                {
                    if (uiZ != 0)
                        uiZ = PackToUI(true, 0x3FF, 0);

                    break;
                }
                case RoundingMode.Max:
                {
                    if (uiZ == 0)
                        uiZ = PackToUI(false, 0x3FF, 0);

                    break;
                }
                case RoundingMode.Odd:
                {
                    uiZ |= PackToUI(false, 0x3FF, 0);
                    break;
                }
            }

            return Float64.FromBitsUI64(uiZ);
        }

        if (0x433 <= exp)
        {
            return exp == 0x7FF && GetFracUI(_v) != 0
                ? context.PropagateNaNFloat64Bits(_v, 0)
                : this;
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

        return Float64.FromBitsUI64(uiZ);
    }

    // f64_add
    public static Float64 Add(SoftFloatContext context, Float64 a, Float64 b)
    {
        var signA = GetSignUI(a._v);
        var signB = GetSignUI(b._v);

        return (signA == signB)
            ? AddMags(context, a._v, b._v, signA)
            : SubMags(context, a._v, b._v, signA);
    }

    // f64_sub
    public static Float64 Subtract(SoftFloatContext context, Float64 a, Float64 b)
    {
        var signA = GetSignUI(a._v);
        var signB = GetSignUI(b._v);

        return (signA == signB)
            ? SubMags(context, a._v, b._v, signA)
            : AddMags(context, a._v, b._v, signA);
    }

    // f64_mul
    public static Float64 Multiply(SoftFloatContext context, Float64 a, Float64 b)
    {
        ulong uiA,
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
        UInt128M sig128Z;

        uiA = a._v;
        signA = GetSignUI(uiA);
        expA = GetExpUI(uiA);
        sigA = GetFracUI(uiA);
        uiB = b._v;
        signB = GetSignUI(uiB);
        expB = GetExpUI(uiB);
        sigB = GetFracUI(uiB);
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
                return Pack(signZ, 0x7FF, 0);
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
                return Pack(signZ, 0x7FF, 0);
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

        expZ = expA + expB - 0x3FF;
        sigA = (sigA | 0x0010000000000000) << 10;
        sigB = (sigB | 0x0010000000000000) << 11;
        sig128Z = UInt128M.Multiply(sigA, sigB);
        sigZ = sig128Z.V64 | (sig128Z.V00 != 0 ? 1U : 0);

        if (sigZ < 0x4000000000000000)
        {
            --expZ;
            sigZ <<= 1;
        }

        return RoundPack(context, signZ, expZ, sigZ);
    }

    // f64_mulAdd
    public static Float64 MultiplyAndAdd(
        SoftFloatContext context,
        Float64 a,
        Float64 b,
        Float64 c
    ) => MulAdd(context, a._v, b._v, c._v, MulAddOperation.None);

    // WARNING: This method overload is experimental and has not been thoroughly tested!
    public static Float64 MultiplyAndAdd(
        SoftFloatContext context,
        Float64 a,
        Float64 b,
        Float64 c,
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

    // f64_div
    public static Float64 Divide(SoftFloatContext context, Float64 a, Float64 b)
    {
        ulong uiA,
            sigA,
            uiB,
            sigB,
            rem,
            sigZ;
        int expA,
            expB,
            expZ;
        uint recip32,
            sig32Z,
            doubleTerm,
            q;
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

            return Pack(signZ, 0x7FF, 0);
        }

        if (expB == 0x7FF)
        {
            return sigB != 0 ? context.PropagateNaNFloat64Bits(uiA, uiB) : Pack(signZ, 0, 0);
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
                return Pack(signZ, 0x7FF, 0);
            }

            (expB, sigB) = NormSubnormalSig(sigB);
        }

        if (expA == 0)
        {
            if (sigA == 0)
                return Pack(signZ, 0, 0);

            (expA, sigA) = NormSubnormalSig(sigA);
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
        rem =
            ((sigA - (ulong)doubleTerm * (uint)(sigB >> 32)) << 28)
            - (ulong)doubleTerm * ((uint)sigB >> 4);
        q = (uint)((((uint)(rem >> 32) * (ulong)recip32) >> 32) + 4);
        sigZ = ((ulong)sig32Z << 32) + ((ulong)q << 4);

        if ((sigZ & 0x1FF) < (4 << 4))
        {
            q &= ~7U;
            sigZ &= ~(ulong)0x7F;
            doubleTerm = q << 1;
            rem =
                ((rem - (ulong)doubleTerm * (uint)(sigB >> 32)) << 28)
                - (ulong)doubleTerm * ((uint)sigB >> 4);
            if ((rem & 0x8000000000000000) != 0)
                sigZ -= 1 << 7;
            else if (rem != 0)
                sigZ |= 1;
        }

        return RoundPack(context, signZ, expZ, sigZ);
    }

    // f64_rem
    public static Float64 Remainder(SoftFloatContext context, Float64 a, Float64 b)
    {
        ulong uiA,
            sigA,
            uiB,
            sigB,
            q64;
        int expA,
            expB,
            expDiff;
        ulong rem,
            altRem,
            meanRem;
        uint q,
            recip32;
        bool signA,
            signRem;

        uiA = a._v;
        signA = GetSignUI(uiA);
        expA = GetExpUI(uiA);
        sigA = GetFracUI(uiA);
        uiB = b._v;
        expB = GetExpUI(uiB);
        sigB = GetFracUI(uiB);

        if (expA == 0x7FF)
        {
            if (sigA != 0 || (expB == 0x7FF && sigB != 0))
                return context.PropagateNaNFloat64Bits(uiA, uiB);

            context.RaiseFlags(ExceptionFlags.Invalid);
            return context.DefaultNaNFloat64;
        }

        if (expB == 0x7FF)
        {
            return sigB != 0 ? context.PropagateNaNFloat64Bits(uiA, uiB) : a;
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

            (expB, sigB) = NormSubnormalSig(sigB);
        }

        if (expA == 0)
        {
            if (sigA == 0)
                return a;

            (expA, sigA) = NormSubnormalSig(sigA);
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
        } while ((rem & 0x8000000000000000) == 0);

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

        return NormRoundPack(context, signRem, expB, rem);
    }

    // f64_sqrt
    public Float64 SquareRoot(SoftFloatContext context)
    {
        ulong uiA,
            sigA,
            rem,
            sigZ,
            shiftedSigZ;
        int expA,
            expZ;
        uint sig32A,
            recipSqrt32,
            sig32Z,
            q;
        bool signA;

        uiA = _v;
        signA = GetSignUI(uiA);
        expA = GetExpUI(uiA);
        sigA = GetFracUI(uiA);

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

            (expA, sigA) = NormSubnormalSig(sigA);
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

        return RoundPack(context, false, expZ, sigZ);
    }

    #endregion

    #region Comparison Operations

    // f64_eq (signaling=false) & f64_eq_signaling (signaling=true)
    public static bool CompareEqual(SoftFloatContext context, Float64 a, Float64 b, bool signaling)
    {
        ulong uiA,
            uiB;

        uiA = a._v;
        uiB = b._v;

        if (IsNaNUI(uiA) || IsNaNUI(uiB))
        {
            if (
                signaling
                || context.IsSignalingNaNFloat64Bits(uiA)
                || context.IsSignalingNaNFloat64Bits(uiB)
            )
                context.RaiseFlags(ExceptionFlags.Invalid);

            return false;
        }

        return uiA == uiB || ((uiA | uiB) & 0x7FFFFFFFFFFFFFFF) == 0;
    }

    // f64_le (signaling=true) & f64_le_quiet (signaling=false)
    public static bool CompareLessThanOrEqual(
        SoftFloatContext context,
        Float64 a,
        Float64 b,
        bool signaling
    )
    {
        ulong uiA,
            uiB;
        bool signA,
            signB;

        uiA = a._v;
        uiB = b._v;

        if (IsNaNUI(uiA) || IsNaNUI(uiB))
        {
            if (
                signaling
                || context.IsSignalingNaNFloat64Bits(uiA)
                || context.IsSignalingNaNFloat64Bits(uiB)
            )
                context.RaiseFlags(ExceptionFlags.Invalid);

            return false;
        }

        signA = GetSignUI(uiA);
        signB = GetSignUI(uiB);

        return (signA != signB)
            ? (signA || ((uiA | uiB) & 0x7FFFFFFFFFFFFFFF) == 0)
            : (uiA == uiB || (signA ^ (uiA < uiB)));
    }

    // f64_lt (signaling=true) & f64_lt_quiet (signaling=false)
    public static bool CompareLessThan(
        SoftFloatContext context,
        Float64 a,
        Float64 b,
        bool signaling
    )
    {
        ulong uiA,
            uiB;
        bool signA,
            signB;

        uiA = a._v;
        uiB = b._v;

        if (IsNaNUI(uiA) || IsNaNUI(uiB))
        {
            if (
                signaling
                || context.IsSignalingNaNFloat64Bits(uiA)
                || context.IsSignalingNaNFloat64Bits(uiB)
            )
                context.RaiseFlags(ExceptionFlags.Invalid);

            return false;
        }

        signA = GetSignUI(uiA);
        signB = GetSignUI(uiB);

        return (signA != signB)
            ? (signA && ((uiA | uiB) & 0x7FFFFFFFFFFFFFFF) != 0)
            : (uiA != uiB && (signA ^ (uiA < uiB)));
    }

    #endregion

    #region Internals

    // signF64UI
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool GetSignUI(ulong a) => (a >> 63) != 0;

    // expF64UI
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int GetExpUI(ulong a) => (int)((uint)(a >> 52) & 0x7FF);

    // fracF64UI
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ulong GetFracUI(ulong a) => a & 0x000FFFFFFFFFFFFF;

    // packToF64UI
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ulong PackToUI(bool sign, int exp, ulong sig) =>
        (sign ? (1UL << 63) : 0UL) + ((ulong)exp << 52) + sig;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Float64 Pack(bool sign, int exp, ulong sig) =>
        FromBitsUI64(PackToUI(sign, exp, sig));

    // isNaNF64UI
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsNaNUI(ulong a) =>
        (~a & 0x7FF0000000000000) == 0 && (a & 0x000FFFFFFFFFFFFF) != 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsInfUI(ulong a) =>
        (~a & 0x7FF0000000000000) == 0 && (a & 0x000FFFFFFFFFFFFF) == 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsFiniteUI(ulong a) => (~a & 0x7FF0000000000000) != 0;

    // softfloat_normSubnormalF64Sig
    internal static (int exp, ulong sig) NormSubnormalSig(ulong sig)
    {
        var shiftDist = CountLeadingZeroes64(sig) - 11;
        return (exp: 1 - shiftDist, sig: sig << shiftDist);
    }

    // softfloat_roundPackToF64
    internal static Float64 RoundPack(SoftFloatContext context, bool sign, int exp, ulong sig)
    {
        var roundingMode = context.Rounding;
        var roundNearEven = roundingMode == RoundingMode.NearEven;
        var roundIncrement =
            (!roundNearEven && roundingMode != RoundingMode.NearMaxMag)
                ? ((roundingMode == (sign ? RoundingMode.Min : RoundingMode.Max)) ? 0x3FFU : 0)
                : 0x200U;
        var roundBits = sig & 0x3FF;

        if (0x7FD <= (uint)exp)
        {
            if (exp < 0)
            {
                var isTiny =
                    context.DetectTininess == TininessMode.BeforeRounding
                    || exp < -1
                    || sig + roundIncrement < 0x8000000000000000;
                sig = sig.ShiftRightJam(-exp);
                exp = 0;
                roundBits = sig & 0x3FF;

                if (isTiny && roundBits != 0)
                    context.RaiseFlags(ExceptionFlags.Underflow);
            }
            else if (0x7FD < exp || 0x8000000000000000 <= sig + roundIncrement)
            {
                context.RaiseFlags(ExceptionFlags.Overflow | ExceptionFlags.Inexact);
                return FromBitsUI64(PackToUI(sign, 0x7FF, 0) - (roundIncrement == 0 ? 1UL : 0));
            }
        }

        sig = (sig + roundIncrement) >> 10;
        if (roundBits != 0)
        {
            context.ExceptionFlags |= ExceptionFlags.Inexact;
            if (roundingMode == RoundingMode.Odd)
                return Pack(sign, exp, sig | 1);
        }

        sig &= ~(((roundBits ^ 0x200) == 0 & roundNearEven) ? 1UL : 0);
        if (sig == 0)
            exp = 0;

        return Pack(sign, exp, sig);
    }

    // softfloat_normRoundPackToF64
    internal static Float64 NormRoundPack(SoftFloatContext context, bool sign, int exp, ulong sig)
    {
        var shiftDist = CountLeadingZeroes64(sig) - 1;
        exp -= shiftDist;
        return (10 <= shiftDist && ((uint)exp < 0x7FD))
            ? Pack(sign, sig != 0 ? exp : 0, sig << (shiftDist - 10))
            : RoundPack(context, sign, exp, sig << shiftDist);
    }

    // softfloat_addMagsF64
    internal static Float64 AddMags(SoftFloatContext context, ulong uiA, ulong uiB, bool signZ)
    {
        int expA,
            expB,
            expDiff,
            expZ;
        ulong sigA,
            sigB,
            sigZ;

        expA = GetExpUI(uiA);
        sigA = GetFracUI(uiA);
        expB = GetExpUI(uiB);
        sigB = GetFracUI(uiB);

        expDiff = expA - expB;
        if (expDiff == 0)
        {
            if (expA == 0)
                return FromBitsUI64(uiA + sigB);

            if (expA == 0x7FF)
            {
                return ((sigA | sigB) != 0)
                    ? context.PropagateNaNFloat64Bits(uiA, uiB)
                    : FromBitsUI64(uiA);
            }

            expZ = expA;
            sigZ = 0x0020000000000000 + sigA + sigB;
            sigZ <<= 9;
        }
        else
        {
            sigA <<= 9;
            sigB <<= 9;
            if (expDiff < 0)
            {
                if (expB == 0x7FF)
                {
                    return (sigB != 0)
                        ? context.PropagateNaNFloat64Bits(uiA, uiB)
                        : Pack(signZ, 0x7FF, 0);
                }

                expZ = expB;
                sigA = ((expA != 0) ? (sigA + 0x2000000000000000) : (sigA << 1)).ShiftRightJam(
                    -expDiff
                );
            }
            else
            {
                if (expA == 0x7FF)
                {
                    return (sigA != 0)
                        ? context.PropagateNaNFloat64Bits(uiA, uiB)
                        : FromBitsUI64(uiA);
                }

                expZ = expA;
                sigB = ((expB != 0) ? (sigB + 0x2000000000000000) : (sigB << 1)).ShiftRightJam(
                    expDiff
                );
            }

            sigZ = 0x2000000000000000 + sigA + sigB;
            if (sigZ < 0x4000000000000000)
            {
                --expZ;
                sigZ <<= 1;
            }
        }

        return RoundPack(context, signZ, expZ, sigZ);
    }

    // softfloat_subMagsF64
    internal static Float64 SubMags(SoftFloatContext context, ulong uiA, ulong uiB, bool signZ)
    {
        int expA,
            expB,
            expDiff,
            expZ;
        ulong sigA,
            sigB,
            sigZ;
        long sigDiff;
        int shiftDist;

        expA = GetExpUI(uiA);
        sigA = GetFracUI(uiA);
        expB = GetExpUI(uiB);
        sigB = GetFracUI(uiB);

        expDiff = expA - expB;
        if (expDiff == 0)
        {
            if (expA == 0x7FF)
            {
                if ((sigA | sigB) != 0)
                    return context.PropagateNaNFloat64Bits(uiA, uiB);

                context.RaiseFlags(ExceptionFlags.Invalid);
                return context.DefaultNaNFloat64;
            }

            sigDiff = (long)sigA - (long)sigB;
            if (sigDiff == 0)
                return Pack(context.Rounding == RoundingMode.Min, 0, 0);

            if (expA != 0)
                --expA;

            if (sigDiff < 0)
            {
                signZ = !signZ;
                sigDiff = -sigDiff;
            }

            Debug.Assert(sigDiff >= 0);
            shiftDist = CountLeadingZeroes64((ulong)sigDiff) - 11;
            expZ = expA - shiftDist;
            if (expZ < 0)
            {
                shiftDist = expA;
                expZ = 0;
            }

            return Pack(signZ, expZ, (ulong)sigDiff << shiftDist);
        }
        else
        {
            sigA <<= 10;
            sigB <<= 10;
            if (expDiff < 0)
            {
                signZ = !signZ;
                if (expB == 0x7FF)
                {
                    return (sigB != 0)
                        ? context.PropagateNaNFloat64Bits(uiA, uiB)
                        : Pack(signZ, 0x7FF, 0);
                }

                sigA += expA != 0 ? 0x4000000000000000 : sigA;
                sigA = sigA.ShiftRightJam(-expDiff);
                sigB |= 0x4000000000000000;
                expZ = expB;
                sigZ = sigB - sigA;
            }
            else
            {
                if (expA == 0x7FF)
                {
                    return (sigA != 0)
                        ? context.PropagateNaNFloat64Bits(uiA, uiB)
                        : FromBitsUI64(uiA);
                }

                sigB += expB != 0 ? 0x4000000000000000 : sigB;
                sigB = sigB.ShiftRightJam(expDiff);
                sigA |= 0x4000000000000000;
                expZ = expA;
                sigZ = sigA - sigB;
            }

            return NormRoundPack(context, signZ, expZ - 1, sigZ);
        }
    }

    // softfloat_mulAddF64
    internal static Float64 MulAdd(
        SoftFloatContext context,
        ulong uiA,
        ulong uiB,
        ulong uiC,
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
            expDiff;
        ulong sigA,
            sigB,
            sigC,
            magBits,
            uiZ,
            sigZ;
        UInt128M sig128Z;
        int shiftDist;

        Unsafe.SkipInit(out UInt128M sig128C); // workaround weird spaghetti code logic

        signA = GetSignUI(uiA);
        expA = GetExpUI(uiA);
        sigA = GetFracUI(uiA);

        signB = GetSignUI(uiB);
        expB = GetExpUI(uiB);
        sigB = GetFracUI(uiB);

        signC = GetSignUI(uiC) ^ (op == MulAddOperation.SubtractC);
        expC = GetExpUI(uiC);
        sigC = GetFracUI(uiC);

        signZ = signA ^ signB ^ (op == MulAddOperation.SubtractProduct);

        if (expA == 0x7FF)
        {
            if (sigA != 0 || (expB == 0x7FF && sigB != 0))
                return context.PropagateNaNFloat64Bits(uiA, uiB, uiC);

            magBits = (ulong)(long)expB | sigB;
            goto infProdArg;
        }

        if (expB == 0x7FF)
        {
            if (sigB != 0)
                return context.PropagateNaNFloat64Bits(uiA, uiB, uiC);

            magBits = (ulong)(long)expA | sigA;
            goto infProdArg;
        }

        if (expC == 0x7FF)
        {
            return (sigC != 0) ? context.PropagateNaNFloat64Bits(0, uiC) : FromBitsUI64(uiC);
        }

        if (expA == 0)
        {
            if (sigA == 0)
            {
                return ((ulong)(long)expC | sigC) == 0 && signZ != signC
                    ? Pack(context.Rounding == RoundingMode.Min, 0, 0)
                    : FromBitsUI64(uiC);
            }

            (expA, sigA) = NormSubnormalSig(sigA);
        }

        if (expB == 0)
        {
            if (sigB == 0)
            {
                return ((ulong)(long)expC | sigC) == 0 && signZ != signC
                    ? Pack(context.Rounding == RoundingMode.Min, 0, 0)
                    : FromBitsUI64(uiC);
            }

            (expB, sigB) = NormSubnormalSig(sigB);
        }

        expZ = expA + expB - 0x3FE;
        sigA = (sigA | 0x0010000000000000) << 10;
        sigB = (sigB | 0x0010000000000000) << 10;
        sig128Z = UInt128M.Multiply(sigA, sigB);

        if (sig128Z.V64 < 0x2000000000000000)
        {
            --expZ;
            sig128Z <<= 1;
        }

        if (expC == 0)
        {
            if (sigC == 0)
            {
                --expZ;
                sigZ = (sig128Z.V64 << 1) | (sig128Z.V00 != 0 ? 1UL : 0);
                return RoundPack(context, signZ, expZ, sigZ);
            }

            (expC, sigC) = NormSubnormalSig(sigC);
        }

        sigC = (sigC | 0x0010000000000000) << 9;

        expDiff = expZ - expC;
        if (expDiff < 0)
        {
            expZ = expC;
            if (signZ == signC || expDiff < -1)
            {
                sig128Z.V64 = sig128Z.V64.ShiftRightJam(-expDiff);
            }
            else
            {
                sig128Z = sig128Z.ShortShiftRightJam(1);
            }
        }
        else if (expDiff != 0)
        {
            sig128C = new UInt128M(sigC, 0).ShiftRightJam(expDiff);
        }

        if (signZ == signC)
        {
            if (expDiff <= 0)
            {
                sigZ = (sigC + sig128Z.V64) | (sig128Z.V00 != 0 ? 1UL : 0UL);
            }
            else
            {
                sig128Z += sig128C;
                sigZ = sig128Z.V64 | (sig128Z.V00 != 0 ? 1UL : 0UL);
            }

            if (sigZ < 0x4000000000000000)
            {
                --expZ;
                sigZ <<= 1;
            }
        }
        else
        {
            if (expDiff < 0)
            {
                signZ = signC;
                sig128Z = new UInt128M(sigC, 0) - sig128Z;
            }
            else if (expDiff == 0)
            {
                sig128Z.V64 -= sigC;
                if ((sig128Z.V64 | sig128Z.V00) == 0)
                    return Pack(context.Rounding == RoundingMode.Min, 0, 0);

                if ((sig128Z.V64 & 0x8000000000000000) != 0)
                {
                    signZ = !signZ;
                    sig128Z = -sig128Z;
                }
            }
            else
            {
                sig128Z -= sig128C;
            }

            if (sig128Z.V64 == 0)
            {
                expZ -= 64;
                sig128Z.V64 = sig128Z.V00;
                sig128Z.V00 = 0;
            }

            shiftDist = CountLeadingZeroes64(sig128Z.V64) - 1;
            expZ -= shiftDist;
            if (shiftDist < 0)
            {
                sigZ = sig128Z.V64.ShortShiftRightJam(-shiftDist);
            }
            else
            {
                sig128Z <<= shiftDist;
                sigZ = sig128Z.V64;
            }

            sigZ |= (sig128Z.V00 != 0 ? 1UL : 0);
        }

        return RoundPack(context, signZ, expZ, sigZ);

        infProdArg:
        if (magBits != 0)
        {
            uiZ = PackToUI(signZ, 0x7FF, 0);
            if (expC != 0x7FF)
                return FromBitsUI64(uiZ);

            if (sigC != 0)
                return context.PropagateNaNFloat64Bits(uiZ, uiC);

            if (signZ == signC)
                return FromBitsUI64(uiZ);
        }

        context.RaiseFlags(ExceptionFlags.Invalid);
        return context.PropagateNaNFloat64Bits(context.DefaultNaNFloat64Bits, uiC);
    }

    #endregion

    #endregion
}
