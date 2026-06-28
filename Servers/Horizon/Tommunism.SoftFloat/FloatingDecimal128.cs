#region Copyright
// Ported to C# by Thomas Kaiser (2022).
// Note that this is under different licensing terms than the rest of the
// SoftFloat library, because it was ported from the Ryu source code.
// Original C Source Code: https://github.com/ulfjack/ryu

// Copyright 2018 Ulf Adams
//
// The contents of this file may be used under the terms of the Apache License,
// Version 2.0.
//
//    (See accompanying file LICENSE-Apache or copy at
//     http://www.apache.org/licenses/LICENSE-2.0)
//
// Alternatively, the contents of this file may be used under the terms of
// the Boost Software License, Version 1.0.
//    (See accompanying file LICENSE-Boost or copy at
//     https://www.boost.org/LICENSE_1_0.txt)
//
// Unless required by applicable law or agreed to in writing, this software
// is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, either express or implied.
#endregion

using System;
using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace Tommunism.SoftFloat;

// NOTE: This requires .NET 7+, because it uses the UInt128 integer type.
// (This can be avoided, but currently SoftFloatCommonNaN uses it, so it's fine for now.)
// This also needs the UInt256M type, but only for the field storage (none of its operators are required).

/// <summary>
/// A floating decimal representing (-1)^s * m * 10^e.
/// </summary>
public readonly partial struct FloatingDecimal128 : ISpanFormattable, IEquatable<FloatingDecimal128>
{
    #region Fields

    /// <summary>
    /// If the exponent is equal to this value, then this represents a special floating-point value such as NaN or Infinity, which is
    /// determined by the mantissa.
    /// </summary>
    private const int ExceptionalExponent = int.MaxValue;

    private readonly UInt128 _mantissa;
    private readonly int _exponent;
    private readonly bool _sign;

    #endregion

    #region Constructors

    /// <summary>
    /// Converts the given bits into a generic floating point decimal representation.
    /// </summary>
    /// <param name="bits">The raw floating-point bits from the value.</param>
    /// <param name="mantissaBits">The number of bits required for the mantissa (significand or fraction) part of the value.</param>
    /// <param name="exponentBits">The number of bits required for the exponent part of the value.</param>
    /// <param name="explicitLeadingBit">Indicates whether there is an explicit leading bit in the mantissa. This is generally used for extended floating-point types (such as <see cref="ExtFloat80"/>).</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="mantissaBits"/> is less than or equal to zero or greater than or equal to 128 or <paramref name="exponentBits"/> is less than or equal to zero or greater than 32.</exception>
    /// <exception cref="ArgumentException">The sum of <paramref name="exponentBits"/> and <paramref name="mantissaBits"/> is greater than or equal to 128.</exception>
    public FloatingDecimal128(
        UInt128 bits,
        int mantissaBits,
        int exponentBits,
        bool explicitLeadingBit
    )
    {
        if (mantissaBits is <= 0 or >= 128)
            throw new ArgumentOutOfRangeException(nameof(mantissaBits));
        if (exponentBits is <= 0 or > 32)
            throw new ArgumentOutOfRangeException(nameof(exponentBits));
        if (mantissaBits + exponentBits >= 128)
            throw new ArgumentException(
                "Mantissa bits, exponent bits, and sign bit exceeds 128 bits."
            );

        var bias = (1U << (exponentBits - 1)) - 1;
        var ieeeSign = ((uint)(bits >> (mantissaBits + exponentBits)) & 1) != 0;
        var ieeeMantissa = bits & ((UInt128.One << mantissaBits) - 1);
        var ieeeExponent = (uint)(bits >> mantissaBits) & (uint)((1UL << exponentBits) - 1);

        // Handle +/- zero.
        if (ieeeExponent == 0 && ieeeMantissa == 0)
        {
            _mantissa = 0;
            _exponent = 0;
            _sign = ieeeSign;
            return;
        }

        // Handle exceptional values (NaN and +/- Infinity).
        if (ieeeExponent == ((1UL << exponentBits) - 1))
        {
            _mantissa = explicitLeadingBit
                ? (ieeeMantissa & ((UInt128.One << (mantissaBits - 1)) - 1))
                : ieeeMantissa;
            _exponent = ExceptionalExponent;
            _sign = ieeeSign;
            return;
        }

        int e2;
        UInt128 m2;

        // We subtract 2 in all cases so that the bounds computation has 2 additional bits.
        if (explicitLeadingBit)
        {
            // mantissaBits includes the explicit leading bit, so we need to correct for that here.
            e2 =
                (ieeeExponent == 0)
                    ? (int)(1 - bias - mantissaBits + 1 - 2)
                    : (int)(ieeeExponent - bias - mantissaBits + 1 - 2);
            m2 = ieeeMantissa;
        }
        else
        {
            if (ieeeExponent == 0)
            {
                e2 = (int)(1 - bias - mantissaBits - 2);
                m2 = ieeeMantissa;
            }
            else
            {
                e2 = (int)(ieeeExponent - bias - mantissaBits - 2);
                m2 = (UInt128.One << mantissaBits) | ieeeMantissa;
            }
        }

        var even = ((uint)m2 & 1) == 0;
        var acceptBounds = even;

        // Step 2: Determine the interval of legal decimal representations.
        var mv = m2 << 2; // multiply by 4
        var mmShift =
            (
                ieeeMantissa != (explicitLeadingBit ? (UInt128.One << (mantissaBits - 1)) : 0)
                || ieeeExponent == 0
            )
                ? 1U
                : 0;

        // Step 3: Convert to a decimal power base using 128-bit arithmetic.
        UInt128 vr,
            vp,
            vm;
        int e10;
        var vmIsTrailingZeros = false;
        var vrIsTrailingZeros = false;
        if (e2 >= 0)
        {
            // I tried special-casing q == 0, but there was no effect on performance.
            // This expression is slightly faster than max(0, log10Pow2(e2) - 1).
            var q = Log10Pow2(e2) - ((e2 > 3) ? 1U : 0);
            e10 = (int)q;
            var k = FLOAT_128_POW5_INV_BITCOUNT + (int)Pow5Bits((int)q) - 1;
            var i = -e2 + (int)q + k;
            var pow5 = ComputeInvPow5((int)q);
            vr = MultiplyAndShift(mv, pow5, i);
            vp = MultiplyAndShift(mv + 2, pow5, i);
            vm = MultiplyAndShift(mv - 1 - mmShift, pow5, i);

            // floor(log_5(2^128)) = 55, this is very conservative
            if (q <= 55)
            {
                // Only one of mp, mv, and mm can be a multiple of 5, if any.
                if ((mv % 5) == 0)
                {
                    vrIsTrailingZeros = MultipleOfPowerOf5(mv, q - 1);
                }
                else if (acceptBounds)
                {
                    // Same as min(e2 + (~mm & 1), pow5Factor(mm)) >= q
                    // <=> e2 + (~mm & 1) >= q && pow5Factor(mm) >= q
                    // <=> true && pow5Factor(mm) >= q, since e2 >= q.
                    vmIsTrailingZeros = MultipleOfPowerOf5(mv - 1 - mmShift, q);
                }
                else
                {
                    // Same as min(e2 + 1, pow5Factor(mp)) >= q.
                    if (MultipleOfPowerOf5(mv + 2, q))
                        vp--;
                }
            }
        }
        else
        {
            // This expression is slightly faster than max(0, log10Pow5(-e2) - 1).
            var q = Log10Pow5(-e2) - ((-e2 > 1) ? 1U : 0);
            e10 = (int)q + e2;
            var i = -e2 - (int)q;
            var k = (int)Pow5Bits(i) - FLOAT_128_POW5_BITCOUNT;
            var j = (int)q - k;
            var pow5 = ComputePow5(i);
            vr = MultiplyAndShift(mv, pow5, j);
            vp = MultiplyAndShift(mv + 2, pow5, j);
            vm = MultiplyAndShift(mv - 1 - mmShift, pow5, j);

            if (q <= 1)
            {
                // {vr,vp,vm} is trailing zeros if {mv,mp,mm} has at least q trailing 0 bits.
                // mv = 4 m2, so it always has at least two trailing 0 bits.
                vrIsTrailingZeros = true;
                if (acceptBounds)
                {
                    // mm = mv - 1 - mmShift, so it has 1 trailing 0 bit iff mmShift == 1.
                    vmIsTrailingZeros = mmShift == 1;
                }
                else
                {
                    // mp = mv + 2, so it always has at least one trailing 0 bit.
                    --vp;
                }
            }
            else if (q < 127)
            {
                // We need to compute min(ntz(mv), pow5Factor(mv) - e2) >= q-1
                // <=> ntz(mv) >= q-1  &&  pow5Factor(mv) - e2 >= q-1
                // <=> ntz(mv) >= q-1    (e2 is negative and -e2 >= q)
                // <=> (mv & ((1 << (q-1)) - 1)) == 0
                // We also need to make sure that the left shift does not overflow.
                vrIsTrailingZeros = MultipleOfPowerOf2(mv, q - 1);
            }
        }

        // Step 4: Find the shortest decimal representation in the interval of legal representations.
        uint removed = 0;
        uint lastRemovedDigit = 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static UInt128 DivRem(UInt128 left, UInt128 right, out uint remainder)
        {
            var (quotient, rem) = UInt128.DivRem(left, right);
            Debug.Assert(rem >= uint.MinValue && rem <= uint.MaxValue);
            remainder = (uint)rem;
            return quotient;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static uint DivRem2(UInt128 left, UInt128 right, out UInt128 quotient)
        {
            (quotient, var rem) = UInt128.DivRem(left, right);
            Debug.Assert(rem >= uint.MinValue && rem <= uint.MaxValue);
            return (uint)rem;
        }

        UInt128 ten = 10; // this is a constant
        UInt128 vpDiv10,
            vmDiv10,
            vrDiv10; // avoid performing the same division operations multiple times
        while ((vpDiv10 = vp / ten) > (vmDiv10 = DivRem(vm, ten, out var vmMod10)))
        {
            vmIsTrailingZeros &= vmMod10 == 0;
            vrIsTrailingZeros &= lastRemovedDigit == 0;
            vrDiv10 = DivRem(vr, ten, out lastRemovedDigit);
            vr = vrDiv10;
            vp = vpDiv10;
            vm = vmDiv10;
            ++removed;
        }

        if (vmIsTrailingZeros)
        {
            // NOTE: The result of DivRem2 is the remainder, not the quotient!
            while (DivRem2(vm, ten, out vmDiv10) == 0)
            {
                vrIsTrailingZeros &= lastRemovedDigit == 0;
                vrDiv10 = DivRem(vr, ten, out lastRemovedDigit);
                vr = vrDiv10;
                vp /= ten;
                vm = vmDiv10;
                ++removed;
            }
        }

        if (vrIsTrailingZeros && lastRemovedDigit == 5 && ((uint)vr & 1) == 0)
        {
            // Round even if the exact numbers is .....50..0.
            lastRemovedDigit = 4;
        }

        _mantissa = vr;
        _exponent = e10 + (int)removed;
        _sign = ieeeSign;

        // We need to take vr+1 if vr is outside bounds or we need to round up.
        if ((vr == vm && (!acceptBounds || !vmIsTrailingZeros)) || lastRemovedDigit >= 5)
            _mantissa++;
    }

    /// <summary>
    /// Converts the given value into a generic floating point decimal representation.
    /// </summary>
    public FloatingDecimal128(Float16 value)
        : this(value.ToUInt16Bits(), 10, 5, false) { }

    /// <summary>
    /// Converts the given value into a generic floating point decimal representation.
    /// </summary>
    public FloatingDecimal128(Float32 value)
        : this(value.ToUInt32Bits(), 23, 8, false) { }

    /// <summary>
    /// Converts the given value into a generic floating point decimal representation.
    /// </summary>
    public FloatingDecimal128(Float64 value)
        : this(value.ToUInt64Bits(), 52, 11, false) { }

    /// <summary>
    /// Converts the given value into a generic floating point decimal representation.
    /// </summary>
    public FloatingDecimal128(ExtFloat80 value)
        : this(value.ToUInt128Bits(), 64, 15, true) { }

    /// <summary>
    /// Converts the given value into a generic floating point decimal representation.
    /// </summary>
    public FloatingDecimal128(Float128 value)
        : this(value.ToUInt128Bits(), 112, 15, false) { }

    /// <summary>
    /// Converts the given value into a generic floating point decimal representation.
    /// </summary>
    public FloatingDecimal128(Half value)
        : this(BitConverter.HalfToUInt16Bits(value), 10, 5, false) { }

    /// <summary>
    /// Converts the given value into a generic floating point decimal representation.
    /// </summary>
    public FloatingDecimal128(float value)
        : this(BitConverter.SingleToUInt32Bits(value), 23, 8, false) { }

    /// <summary>
    /// Converts the given value into a generic floating point decimal representation.
    /// </summary>
    public FloatingDecimal128(double value)
        : this(BitConverter.DoubleToUInt64Bits(value), 52, 11, false) { }

    #endregion

    #region Properties

    public UInt128 Mantissa => _mantissa;

    public int Exponent => _exponent;

    public bool IsFinite => _exponent != ExceptionalExponent;

    public bool IsNaN => _exponent == ExceptionalExponent && _mantissa != 0;

    public bool IsInfinity => _exponent == ExceptionalExponent && _mantissa == 0;

    // This does not necessarily mean it is valid if this is true. This is just the sign bit.
    public bool IsNegative => _sign;

    #endregion

    #region Methods

    public override bool Equals(object? obj) => obj is FloatingDecimal128 value && Equals(value);

    public bool Equals(FloatingDecimal128 other) =>
        _mantissa.Equals(other._mantissa) && _exponent == other._exponent && _sign == other._sign;

    public override int GetHashCode() => HashCode.Combine(_mantissa, _exponent, _sign);

    #region String Formatting

    // NOTE: Even if this fails, there may have been characters written to the destination buffer.
    public bool TryFormat(
        Span<char> destination,
        out int charsWritten,
        ReadOnlySpan<char> format = default,
        IFormatProvider? provider = null
    )
    {
        // NOTE: No need to dispose of builder, because it will never grow.
        var builder = new ValueStringBuilder(destination, canGrow: false);
        try
        {
            FormatValue(ref builder, format, NumberFormatInfo.GetInstance(provider));
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

    public override string ToString() => ToString(null, null);

    public string ToString(string? format) => ToString(format, null);

    public string ToString(IFormatProvider? formatProvider) => ToString(null, formatProvider);

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        // According to the Ryu source code, the maximum char buffer requirement is:
        // sign + mantissa digits + decimal dot + 'E' + exponent sign + exponent digits
        // = 1 + 39 + 1 + 1 + 1 + 10 = 53

        // NOTE: Theoretically no exceptions should be thrown by ValueStringBuilder, because its internal buffer is
        // allowed to grow to any reasonable size.
        var builder = new ValueStringBuilder(stackalloc char[64]);
        FormatValue(ref builder, format, NumberFormatInfo.GetInstance(formatProvider));
        return builder.ToString();
    }

    // NOTE: Precision may be set to -1 if the format does not specify the precision (use the default precision for the given format code).
    // NOTE: If the returned value is '\0', then it is not a valid standard format.
    private static char ParseStandardFormat(
        ReadOnlySpan<char> format,
        out int precision,
        string defaultFormat
    )
    {
        // Use the general format if not specified.
        if (format.IsEmpty)
            format = defaultFormat;

        // The first character of the format string is the format code. Make sure it is an ASCII letter.
        if (!char.IsAsciiLetter(format[0]))
        {
            precision = default;
            return '\0';
        }

        if (format.Length <= 1)
        {
            // No precision specified, use -1.
            precision = -1;
        }
        else
        {
            // Try to parse the non-negative precision value up to a defined maximum precision.
            if (
                !int.TryParse(format[1..], NumberStyles.None, null, out precision)
                || precision is < 0 or > 999_999_999
            )
            {
                precision = default;
                return '\0';
            }
        }

        return format[0];
    }

    private void FormatValue(
        ref ValueStringBuilder builder,
        ReadOnlySpan<char> format,
        NumberFormatInfo info
    )
    {
        var formatCode = ParseStandardFormat(format, out var precision, "G");
        switch (formatCode)
        {
            case 'C':
            case 'c':
            {
                FormatCurrency(ref builder, info, precision);
                break;
            }
            case 'P':
            case 'p':
            {
                FormatPercent(ref builder, info, precision);
                break;
            }
            case 'N':
            case 'n':
            {
                FormatNumeric(ref builder, info, precision);
                break;
            }
            case 'E':
            case 'e':
            {
                FormatScientific(ref builder, info, precision, exponentSymbol: formatCode);
                break;
            }
            case 'F':
            case 'f':
            {
                if (_sign)
                    builder.Append(info.NegativeSign);
                FormatNumericDigits(
                    ref builder,
                    Array.Empty<int>(),
                    string.Empty,
                    info.NumberDecimalSeparator,
                    precision
                );
                break;
            }
            case 'R':
            case 'r':
            {
                // NOTE: Round-trip is the same as general except it always uses the default (unspecified) precision value.
                // Additionally, no trailing zeroes will ever be emitted, to reduce the chances of rounding errors.
                FormatGeneral(
                    ref builder,
                    info,
                    exponentSymbol: formatCode == 'R' ? 'E' : 'e',
                    roundTrip: true
                );
                break;
            }
            case 'G':
            case 'g':
            {
                FormatGeneral(ref builder, info, exponentSymbol: formatCode == 'G' ? 'E' : 'e');
                break;
            }
            default:
            {
                throw new ArgumentException("Invalid format specified.", nameof(format));
            }
        }
    }

    private void FormatSpecial(ref ValueStringBuilder builder, NumberFormatInfo info)
    {
        Debug.Assert(_exponent == ExceptionalExponent, "Decimal value is not special.");
        var specialValue =
            (_mantissa != 0)
                ? info.NaNSymbol
                : (_sign ? info.NegativeInfinitySymbol : info.PositiveInfinitySymbol);

        builder.Append(specialValue);
    }

    private void FormatScientific(
        ref ValueStringBuilder builder,
        NumberFormatInfo info,
        int precision,
        char exponentSymbol
    )
    {
        // Currently all digits are rendered.
        if (precision != -1)
            throw new NotImplementedException(
                "Only default (unspecified) precision is supported for this number format."
            );

        // Handle special floating-point values.
        if (_exponent == ExceptionalExponent)
        {
            FormatSpecial(ref builder, info);
            return;
        }

        // Step 5: Print the decimal representation.
        if (_sign)
            builder.Append(info.NegativeSign);

        var output = _mantissa;
        var digitsLength = DecimalLength(output);
        Debug.Assert(digitsLength > 0);

        var hasDecimalSeparator = digitsLength > 1;
        var outputLength = digitsLength;
        if (hasDecimalSeparator)
            outputLength += info.NumberDecimalSeparator.Length;

        var buffer = builder.AppendSpan(outputLength);

        UInt128 ten = 10; // constant
        for (var i = 1; i < digitsLength; i++)
        {
            (output, var c) = UInt128.DivRem(output, ten);
            buffer[outputLength - i] = (char)('0' + (int)c);
        }

        Debug.Assert(output < ten, "Highest output digit is greater than or equal to 10!");
        buffer[0] = (char)('0' + (uint)output);

        // Print decimal point if needed (if there is more than one digit).
        if (hasDecimalSeparator)
            info.NumberDecimalSeparator.CopyTo(buffer[1..]);

        // Print the exponent.
        var exp = _exponent + (digitsLength - 1);
        builder.Append(exponentSymbol);

        // Always shown the exponent sign. (Exponent is always positive after this statement.)
        if (exp < 0)
        {
            builder.Append(info.NegativeSign);
            exp = -exp; // absolute value
        }
        else
        {
            builder.Append(info.PositiveSign);
        }

        // Build the exponent (with a minimum of 3 digits).
        var expLength = Math.Max(3, DecimalLength((uint)exp));
        buffer = builder.AppendSpan(expLength);
        for (var i = 0; i < expLength; i++)
        {
            (exp, var c) = Math.DivRem(exp, 10);
            buffer[expLength - i - 1] = (char)('0' + c);
        }
    }

    private void FormatCurrency(
        ref ValueStringBuilder builder,
        NumberFormatInfo info,
        int precision
    )
    {
        // Handle special floating-point values.
        if (_exponent == ExceptionalExponent)
        {
            FormatSpecial(ref builder, info);
            return;
        }

        // Set default precision if not defined.
        if (precision == -1)
            precision = info.CurrencyDecimalDigits;

        if (_sign)
        {
            // Trade off code size and table lookups for a single switch statement.
            switch (info.CurrencyNegativePattern)
            {
                case 0: // ($n)
                {
                    builder.Append('(');
                    builder.Append(info.CurrencySymbol);
                    FormatCurrencyDigits(ref builder, info, precision);
                    builder.Append(')');
                    break;
                }
                case 1: // -$n
                {
                    builder.Append(info.NegativeSign);
                    builder.Append(info.CurrencySymbol);
                    FormatCurrencyDigits(ref builder, info, precision);
                    break;
                }
                case 2: // $-n
                {
                    builder.Append(info.CurrencySymbol);
                    builder.Append(info.NegativeSign);
                    FormatCurrencyDigits(ref builder, info, precision);
                    break;
                }
                case 3: // $n-
                {
                    builder.Append(info.CurrencySymbol);
                    FormatCurrencyDigits(ref builder, info, precision);
                    builder.Append(info.NegativeSign);
                    break;
                }
                case 4: // (n$)
                {
                    builder.Append('(');
                    FormatCurrencyDigits(ref builder, info, precision);
                    builder.Append(info.CurrencySymbol);
                    builder.Append(')');
                    break;
                }
                case 5: // -n$
                {
                    builder.Append(info.NegativeSign);
                    FormatCurrencyDigits(ref builder, info, precision);
                    builder.Append(info.CurrencySymbol);
                    break;
                }
                case 6: // n-$
                {
                    FormatCurrencyDigits(ref builder, info, precision);
                    builder.Append(info.NegativeSign);
                    builder.Append(info.CurrencySymbol);
                    break;
                }
                case 7: // n$-
                {
                    FormatCurrencyDigits(ref builder, info, precision);
                    builder.Append(info.CurrencySymbol);
                    builder.Append(info.NegativeSign);
                    break;
                }
                case 8: // -n $
                {
                    builder.Append(info.NegativeSign);
                    FormatCurrencyDigits(ref builder, info, precision);
                    builder.Append(' ');
                    builder.Append(info.CurrencySymbol);
                    break;
                }
                case 9: // -$ n
                {
                    builder.Append(info.NegativeSign);
                    builder.Append(info.CurrencySymbol);
                    builder.Append(' ');
                    FormatCurrencyDigits(ref builder, info, precision);
                    break;
                }
                case 10: // n $-
                {
                    FormatCurrencyDigits(ref builder, info, precision);
                    builder.Append(' ');
                    builder.Append(info.CurrencySymbol);
                    builder.Append(info.NegativeSign);
                    break;
                }
                case 11: // $ n-
                {
                    builder.Append(info.CurrencySymbol);
                    builder.Append(' ');
                    FormatCurrencyDigits(ref builder, info, precision);
                    builder.Append(info.NegativeSign);
                    break;
                }
                case 12: // $ -n
                {
                    builder.Append(info.CurrencySymbol);
                    builder.Append(' ');
                    builder.Append(info.NegativeSign);
                    FormatCurrencyDigits(ref builder, info, precision);
                    break;
                }
                case 13: // n- $
                {
                    FormatCurrencyDigits(ref builder, info, precision);
                    builder.Append(info.NegativeSign);
                    builder.Append(' ');
                    builder.Append(info.CurrencySymbol);
                    break;
                }
                case 14: // ($ n)
                {
                    builder.Append('(');
                    builder.Append(info.CurrencySymbol);
                    builder.Append(' ');
                    FormatCurrencyDigits(ref builder, info, precision);
                    builder.Append(')');
                    break;
                }
                case 15: // (n $)
                {
                    builder.Append('(');
                    FormatCurrencyDigits(ref builder, info, precision);
                    builder.Append(' ');
                    builder.Append(info.CurrencySymbol);
                    builder.Append(')');
                    break;
                }
                default:
                {
                    throw new InvalidOperationException(
                        "Unexpected negative currency pattern value."
                    );
                }
            }
        }
        else
        {
            // Trade off code size and bit patterns for a single switch statement.
            switch (info.CurrencyPositivePattern)
            {
                case 0: // $n
                {
                    builder.Append(info.CurrencySymbol);
                    FormatCurrencyDigits(ref builder, info, precision);
                    break;
                }
                case 1: // n$
                {
                    FormatCurrencyDigits(ref builder, info, precision);
                    builder.Append(info.CurrencySymbol);
                    break;
                }
                case 2: // $ n
                {
                    builder.Append(info.CurrencySymbol);
                    builder.Append(' ');
                    FormatCurrencyDigits(ref builder, info, precision);
                    break;
                }
                case 3: // n $
                {
                    FormatCurrencyDigits(ref builder, info, precision);
                    builder.Append(' ');
                    builder.Append(info.CurrencySymbol);
                    break;
                }
                default:
                {
                    throw new InvalidOperationException(
                        "Unexpected positive currency pattern value."
                    );
                }
            }
        }
    }

    private void FormatPercent(ref ValueStringBuilder builder, NumberFormatInfo info, int precision)
    {
        // Handle special floating-point values.
        if (_exponent == ExceptionalExponent)
        {
            FormatSpecial(ref builder, info);
            return;
        }

        // Set default precision if not defined.
        if (precision == -1)
            precision = info.PercentDecimalDigits;

        if (_sign)
        {
            switch (info.PercentNegativePattern)
            {
                case 0: // -n %
                {
                    builder.Append(info.NegativeSign);
                    FormatPercentDigits(ref builder, info, precision);
                    builder.Append(' ');
                    builder.Append(info.PercentSymbol);
                    break;
                }
                case 1: // -n%
                {
                    builder.Append(info.NegativeSign);
                    FormatPercentDigits(ref builder, info, precision);
                    builder.Append(info.PercentSymbol);
                    break;
                }
                case 2: // -%n
                {
                    builder.Append(info.NegativeSign);
                    builder.Append(info.PercentSymbol);
                    FormatPercentDigits(ref builder, info, precision);
                    break;
                }
                case 3: // %-n
                {
                    builder.Append(info.PercentSymbol);
                    builder.Append(info.NegativeSign);
                    FormatPercentDigits(ref builder, info, precision);
                    break;
                }
                case 4: // %n-
                {
                    builder.Append(info.PercentSymbol);
                    FormatPercentDigits(ref builder, info, precision);
                    builder.Append(info.NegativeSign);
                    break;
                }
                case 5: // n-%
                {
                    FormatPercentDigits(ref builder, info, precision);
                    builder.Append(info.PercentSymbol);
                    break;
                }
                case 6: // n%-
                {
                    FormatPercentDigits(ref builder, info, precision);
                    builder.Append(info.PercentSymbol);
                    builder.Append(info.NegativeSign);
                    break;
                }
                case 7: // -% n
                {
                    builder.Append(info.NegativeSign);
                    builder.Append(info.PercentSymbol);
                    builder.Append(' ');
                    FormatPercentDigits(ref builder, info, precision);
                    break;
                }
                case 8: // n %-
                {
                    FormatPercentDigits(ref builder, info, precision);
                    builder.Append(' ');
                    builder.Append(info.PercentSymbol);
                    builder.Append(info.NegativeSign);
                    break;
                }
                case 9: // % n-
                {
                    builder.Append(info.PercentSymbol);
                    builder.Append(' ');
                    FormatPercentDigits(ref builder, info, precision);
                    builder.Append(info.NegativeSign);
                    break;
                }
                case 10: // % -n
                {
                    builder.Append(info.PercentSymbol);
                    builder.Append(' ');
                    builder.Append(info.NegativeSign);
                    FormatPercentDigits(ref builder, info, precision);
                    break;
                }
                case 11: // n- %
                {
                    FormatPercentDigits(ref builder, info, precision);
                    builder.Append(info.NegativeSign);
                    builder.Append(' ');
                    builder.Append(info.PercentSymbol);
                    break;
                }
                default:
                {
                    throw new InvalidOperationException(
                        "Unexpected negative percent pattern value."
                    );
                }
            }
        }
        else
        {
            switch (info.PercentNegativePattern)
            {
                case 0: // n %
                {
                    FormatPercentDigits(ref builder, info, precision);
                    builder.Append(' ');
                    builder.Append(info.PercentSymbol);
                    break;
                }
                case 1: // n%
                {
                    FormatPercentDigits(ref builder, info, precision);
                    builder.Append(info.PercentSymbol);
                    break;
                }
                case 2: // %n
                {
                    builder.Append(info.PercentSymbol);
                    FormatPercentDigits(ref builder, info, precision);
                    break;
                }
                case 3: // % n
                {
                    builder.Append(info.PercentSymbol);
                    builder.Append(' ');
                    FormatPercentDigits(ref builder, info, precision);
                    break;
                }
                default:
                {
                    throw new InvalidOperationException(
                        "Unexpected positive percent pattern value."
                    );
                }
            }
        }
    }

    private void FormatNumeric(ref ValueStringBuilder builder, NumberFormatInfo info, int precision)
    {
        // Handle special floating-point values.
        if (_exponent == ExceptionalExponent)
        {
            FormatSpecial(ref builder, info);
            return;
        }

        // Set default precision if not defined.
        if (precision == -1)
            precision = info.NumberDecimalDigits;

        if (_sign)
        {
            switch (info.NumberNegativePattern)
            {
                case 0: // (n)
                {
                    builder.Append('(');
                    FormatNumericDigits(ref builder, info, precision);
                    builder.Append(')');
                    break;
                }
                case 1: // -n
                {
                    builder.Append(info.NegativeSign);
                    FormatNumericDigits(ref builder, info, precision);
                    break;
                }
                case 2: // - n
                {
                    builder.Append(info.NegativeSign);
                    builder.Append(' ');
                    FormatNumericDigits(ref builder, info, precision);
                    break;
                }
                case 3: // n-
                {
                    FormatNumericDigits(ref builder, info, precision);
                    builder.Append(info.NegativeSign);
                    break;
                }
                case 4: // n -
                {
                    FormatNumericDigits(ref builder, info, precision);
                    builder.Append(' ');
                    builder.Append(info.NegativeSign);
                    break;
                }
                default:
                {
                    throw new InvalidOperationException(
                        "Unexpected negative number pattern value."
                    );
                }
            }
        }
        else
        {
            FormatNumericDigits(ref builder, info, precision);
        }
    }

    private void FormatCurrencyDigits(
        ref ValueStringBuilder builder,
        NumberFormatInfo info,
        int precision
    ) =>
        FormatNumericDigits(
            ref builder,
            info.CurrencyGroupSizes,
            info.CurrencyGroupSeparator,
            info.CurrencyDecimalSeparator,
            precision
        );

    private void FormatPercentDigits(
        ref ValueStringBuilder builder,
        NumberFormatInfo info,
        int precision
    ) =>
        FormatNumericDigits(
            ref builder,
            info.PercentGroupSizes,
            info.PercentGroupSeparator,
            info.PercentDecimalSeparator,
            precision,
            exponentModifier: 2
        );

    private void FormatNumericDigits(
        ref ValueStringBuilder builder,
        NumberFormatInfo info,
        int precision
    ) =>
        FormatNumericDigits(
            ref builder,
            info.NumberGroupSizes,
            info.NumberGroupSeparator,
            info.NumberDecimalSeparator,
            precision
        );

    // NOTE: This does not emit the negative sign (because it may be in many places, depending on the NumberFormatInfo
    // instance and whether this is for currency, percent, or general numbers). The exponent modifier is added to the
    // exponent value before rendering digits (necessary for percentages).
    // WARNING: FloatingDecimal128 automatically removes "extra" digits (and thus loses a lot of precision), because it
    // attempts to get the fewest number of digits that is closest to the actual value (so it should still be able to
    // round-trip, even with the lost precision). This may also affect the way that rounding is performed.
    private void FormatNumericDigits(
        ref ValueStringBuilder builder,
        int[] groupSizes,
        string groupSeparator,
        string decimalSeparator,
        int precision,
        int exponentModifier = 0
    )
    {
        Debug.Assert(precision >= 0, "Precision is undefined.");
        Debug.Assert(
            !string.IsNullOrEmpty(decimalSeparator),
            "Decimal separator is null or empty!"
        );
        Debug.Assert(_exponent != ExceptionalExponent, "Special values cannot be handled here.");
        Debug.Assert(
            exponentModifier >= 0 && exponentModifier < PowersOf10.Length,
            "Unsupported exponent modifier."
        );

        // Get the length of the mantissa.
        var digits = _mantissa;

        // Get the exponent.
        var exp = _exponent;

        if (exponentModifier != 0)
        {
            // Apply exponent modifier.
            // Precision shouldn't be lost here as long as there are at least 7 unused bits at the top of the mantissa.
            digits *= PowersOf10[exponentModifier];
        }

        // Divide digits by 10 (with rounding) until the exponent matches the precision.
        // TODO: Optimize this to use fewer divide operations?
        var tempExp = -exp;
        while (tempExp > precision)
        {
            (digits, var rem) = UInt128.DivRem(digits, 10);

            // Apply rounding (away from zero).
            if (rem >= 5)
                digits++;

            --tempExp;
        }

        // Update exponent.
        exp = -tempExp;

        // Calculate the number of digits.
        var digitsLength = DecimalLength(digits);
        Debug.Assert(digitsLength > 0);

        // Figure out how many leading and trailing zeroes there are.
        var leadingZeroes = 0;
        var trailingZeroes = 0;
        if (exp < 0)
        {
            if (-exp < precision)
                trailingZeroes = exp + precision;
            else
                leadingZeroes = Math.Max(0, -(digitsLength + exp - 1));
        }
        else
        {
            trailingZeroes = exp + precision;
        }

        // Calculate the total length required for the digits and decimal separator (excluding digit separators).
        var totalLength = digitsLength + leadingZeroes + trailingZeroes;
        if (precision > 0)
        {
            // Add an extra leading zero to fulfill the precision contract.
            var tempDecimalIndex = totalLength - precision;
            if (tempDecimalIndex == 0)
            {
                leadingZeroes++;
                totalLength++;
            }

            totalLength += decimalSeparator.Length;
        }

        // Make sure space is available in buffer first before appending everything else.
        var digitsStartIndex = builder.Length;
        builder.EnsureCapacity(digitsStartIndex + totalLength);

        // Append leading zeroes.
        builder.Append('0', leadingZeroes);

        // Append digits.
        var digitsBuffer = builder.AppendSpan(digitsLength);
        if (
            !digits.TryFormat(digitsBuffer, out var digitsWritten, default, null)
            || digitsWritten != digitsLength
        )
            throw new FormatException("Unable to format digits.");

        // Append trailing zeroes.
        builder.Append('0', trailingZeroes);

        // Insert decimal digits if required.
        var decimalIndex = builder.Length - precision;
        if (precision > 0)
            builder.Insert(decimalIndex, decimalSeparator);

        // Insert digit groupings.
        if (groupSizes.Length > 0 && groupSeparator.Length > 0)
        {
            var lastGroupIndex = decimalIndex;
            var lastGroupSize = 0;
            for (var i = 0; i < groupSizes.Length; i++)
            {
                var groupSize = groupSizes[i];
                if (groupSize > 0)
                {
                    lastGroupIndex -= groupSize;
                    if (lastGroupIndex > digitsStartIndex)
                    {
                        builder.Insert(lastGroupIndex, groupSeparator);
                    }
                    else
                    {
                        // Set last group size to zero to prevent remaining digits from being checked.
                        lastGroupSize = 0;
                        break;
                    }
                }

                lastGroupSize = groupSize;
            }

            // Apply last group size to remaining digits (if last group size was not zero).
            if (lastGroupSize > 0 && lastGroupIndex > digitsStartIndex)
            {
                lastGroupIndex -= lastGroupSize;
                while (lastGroupIndex > digitsStartIndex)
                {
                    builder.Insert(lastGroupIndex, groupSeparator);
                    lastGroupIndex -= lastGroupSize;
                }
            }
        }
    }

    // TODO: Try to improve performance a little bit if there is a decimal point? Currently requires using ValueStringBuilder.Insert.
    private void FormatGeneral(
        ref ValueStringBuilder builder,
        NumberFormatInfo info,
        char exponentSymbol,
        bool roundTrip = false
    )
    {
        // Handle special floating-point values.
        if (_exponent == ExceptionalExponent)
        {
            FormatSpecial(ref builder, info);
            return;
        }

        Debug.Assert(
            !string.IsNullOrEmpty(info.NumberDecimalSeparator),
            "Decimal separator is null or empty!"
        );
        Debug.Assert(!string.IsNullOrEmpty(info.PositiveSign), "Positive sign is null or empty!");
        Debug.Assert(!string.IsNullOrEmpty(info.NegativeSign), "Negative sign is null or empty!");

        // Output the negative sign if needed.
        var digitsStartIndex = 0;
        if (_sign)
        {
            builder.Append(info.NegativeSign);
            digitsStartIndex = info.NegativeSign.Length;
        }

        // Get the length of the mantissa.
        var digits = _mantissa;
        var digitsLength = DecimalLength(digits);
        Debug.Assert(digitsLength > 0);

        // Output the mantissa digits.
        var digitsBuffer = builder.AppendSpan(digitsLength);
        if (
            !digits.TryFormat(digitsBuffer, out var digitsWritten, default, info)
            || digitsWritten != digitsLength
        )
            throw new FormatException("Could not append mantissa digits.");

        // Shortcut if the exponent is zero (just treat it as an integer value).
        if (_exponent == 0)
            return;

        // Calculate the displayed exponent length (including the symbol and exponent sign).
        var exp = _exponent + (digitsLength - 1);
        var expAbs = Math.Abs(exp);
        var expLength = Math.Max(2, DecimalLength((uint)expAbs));
        var expTotalLength = expLength + 2; // includes exponent symbol and exponent sign

        if (_exponent < 0)
        {
            // Insert the decimal point if it is located inside the digits range.
            if (_exponent > -digitsLength)
            {
                // Insert decimal at given decimal index.
                var decimalIndex = builder.Length + _exponent;
                builder.Insert(decimalIndex, info.NumberDecimalSeparator);
                return;
            }

            // Prepend zeroes and insert decimal point if it is shorter without the exponent.
            var zeroCount = -(digitsLength + _exponent - 1);
            if (zeroCount > 0 && zeroCount <= expTotalLength)
            {
                builder.Insert(digitsStartIndex, '0', zeroCount);
                builder.Insert(digitsStartIndex + 1, info.NumberDecimalSeparator);
                return;
            }
        }
        else if (!roundTrip || _exponent == 0)
        {
            // Append zeroes if it is shorter without the exponent (and decimal separator).
            if (_exponent <= expTotalLength + info.CurrencyDecimalSeparator.Length)
            {
                // Append zeroes to result and omit the exponent.
                builder.Append('0', _exponent);
                return;
            }
        }

        // Insert the decimal point after the first digit (if there is more than one digit).
        if (digitsLength > 1)
            builder.Insert(digitsStartIndex + 1, info.NumberDecimalSeparator);

        // Get the exponent length and output the exponent symbol
        builder.Append(exponentSymbol);

        // Always show the exponent sign.
        builder.Append((exp < 0) ? info.NegativeSign : info.PositiveSign);

        // Output the exponent digits (minimum number of digits is two).
        digitsBuffer = builder.AppendSpan(expLength);
        if (
            !((uint)expAbs).TryFormat(digitsBuffer, out var expDigitsWritten, "D2", info)
            || expDigitsWritten != expLength
        )
            throw new FormatException("Could not append exponent digits.");
    }

    #endregion

    // Returns e == 0 ? 1 : ceil(log_2(5^e)); requires 0 <= e <= 32768.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint Pow5Bits(int e)
    {
        Debug.Assert(e is >= 0 and <= (1 << 15));
        return (uint)((((uint)e * 163391164108059UL) >> 46) + 1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static UInt128 Multiply64x64(ulong a, ulong b)
    {
        // This should be faster than UInt128 multiplication especially if the internal hardware intrinsics can be used.
        var z64 = Math.BigMul(a, b, out var z0);
        return new(z64, z0);
    }

    private static UInt256M Multiply128By256Shift(UInt128 a, UInt256M b, int shift, uint correction)
    {
        Debug.Assert(shift is > 0 and < 256);

        var a0 = a.GetLowerUI64();
        var b00 = Multiply64x64(a0, b.V000); // 0
        var b01 = Multiply64x64(a0, b.V064); // 64
        var b02 = Multiply64x64(a0, b.V128); // 128
        var b03 = Multiply64x64(a0, b.V192); // 192
        var a1 = a.GetUpperUI64();
        var b10 = Multiply64x64(a1, b.V000); // 64
        var b11 = Multiply64x64(a1, b.V064); // 128
        var b12 = Multiply64x64(a1, b.V128); // 192
        var b13 = Multiply64x64(a1, b.V192); // 256

        var s0 = b00; // 0   x
        var s1 = b01 + b10; // 64  x
        UInt128 c1x = new((s1 < b01) ? 1U : 0, 0); // 192 x
        var s2 = b02 + b11; // 128 x
        UInt128 c2 = new(0, (s2 < b02) ? 1U : 0); // 256 x
        var s3 = b03 + b12; // 192 x
        UInt128 c3x = new((s3 < b03) ? 1U : 0, 0); // 320

        var p0 = s0 + (s1 << 64); // 0
        var d0 = (p0 < b00) ? UInt128.One : UInt128.Zero; // 128
        var q1 = s2 + (s1 >> 64) + (s3 << 64); // 128
        var d1 = (q1 < s2) ? UInt128.One : UInt128.Zero; // 256
        var p1 = q1 + c1x + d0; // 128
        var d2 = (p1 < q1) ? UInt128.One : UInt128.Zero; // 256
        var p2 = b13 + (s3 >> 64) + c2 + c3x + d1 + d2; // 256

        // Perform the shifts.
        UInt128 r0,
            r1;
        if (shift < 128)
        {
            r0 = (p0 >> shift) | (p1 << -shift);
            r1 = (p1 >> shift) | (p2 << -shift);
        }
        else if (shift == 128)
        {
            r0 = p1;
            r1 = p2;
        }
        else
        {
            r0 = (p1 >> (shift - 128)) | (p2 << -shift);
            r1 = p2 >> (shift - 128);
        }

        // Add the correction here instead of doing it above during the shift (no point inlining it, it's exactly the
        // same with the three different shift paths above).
        r0 += correction;
        if (r0 < correction)
            r1++;

#if false
        // Make sure the math is correct.
        var bigNumA = (BigInteger)a;
        var bigNumB = (BigInteger)(UInt128)b.V000_UI128 | ((BigInteger)(UInt128)b.V128_UI128 << 128);
        var bigNumResult = ((bigNumA * bigNumB) >> shift) + correction;

        var bigNumMask128 = (BigInteger.One << 128) - 1;
        var result0 = (UInt128)(bigNumResult & bigNumMask128);
        var result1 = (UInt128)((bigNumResult >> 128) & bigNumMask128);

        Debug.Assert(r0 == result0);
        Debug.Assert(r1 == result1);
#endif

        return new(r1, r0);
    }

    // Computes 5^i in the form required by Ryu, and returns the 256-bit result.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static UInt256M ComputePow5(int i)
    {
        var base1 = i / POW5_TABLE_SIZE;
        var base2 = base1 * POW5_TABLE_SIZE;
        var mul = GENERIC_POW5_SPLIT(base1);

        if (i == base2)
            return mul;

        var offset = i - base2;
        var m = GENERIC_POW5_TABLE(offset);
        var delta = (int)Pow5Bits(i) - (int)Pow5Bits(base2);
        var corr = GetPow5Error(i);
        return Multiply128By256Shift(m, mul, delta, corr);
    }

    // Computes 5^-i in the form required by Ryu, and returns the 256-bit result.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static UInt256M ComputeInvPow5(int i)
    {
        var base1 = (i + POW5_TABLE_SIZE - 1) / POW5_TABLE_SIZE;
        var base2 = base1 * POW5_TABLE_SIZE;
        var mul = GENERIC_POW5_INV_SPLIT(base1); // 1/5^base2

        if (i == base2)
        {
            // None of the values in the split table have the first 64 bits set, so incrementing the "low" field is safe.
            mul.V000++;
            return mul;
        }

        var offset = base2 - i;
        var m = GENERIC_POW5_TABLE(offset); // 5^offset
        var delta = (int)Pow5Bits(base2) - (int)Pow5Bits(i);
        var corr = GetInvPow5Error(i) + 1;
        return Multiply128By256Shift(m, mul, delta, corr);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint Pow5Factor(UInt128 value)
    {
        UInt128 five = 5; // constant
        for (uint count = 0; value > 0; ++count)
        {
            (value, var rem) = UInt128.DivRem(value, five);
            if (rem != 0)
                return count;
        }

        return 0;
    }

    // Returns true if value is divisible by 5^p.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool MultipleOfPowerOf5(UInt128 value, uint p)
    {
        // I tried a case distinction on p, but there was no performance difference.
        return Pow5Factor(value) >= p;
    }

    // Returns true if value is divisible by 2^p.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool MultipleOfPowerOf2(UInt128 value, uint p)
    {
        Debug.Assert(p is >= 0 and < 128);
        return (value & ((UInt128.One << (int)p) - 1)) == 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static UInt128 MultiplyAndShift(UInt128 a, UInt256M b, int shift)
    {
        Debug.Assert(shift > 128);
        var result = Multiply128By256Shift(a, b, shift, 0);
        return new(result.V064, result.V000);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int DecimalLength(uint v)
    {
        // See: https://graphics.stanford.edu/~seander/bithacks.html#IntegerLog10
        // Modified to always return the floor value plus one.
        var t = (int)(uint)((((uint)BitOperations.Log2(v) + 1) * 1233UL) >> 12);
        return t + (v < PowersOf10[t] ? 0 : 1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int DecimalLength(UInt128 v)
    {
        UInt128 p10 = new(0x4B3B4CA85A86C47A, 0x98A224000000000); // LARGEST_POW10
        for (var i = 39; i > 0; i--)
        {
            if (v >= p10)
                return i;

            p10 /= 10;
        }

        return 1;
    }

    // Returns floor(log_10(2^e)).
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint Log10Pow2(int e)
    {
        // The first value this approximation fails for is 2^1651 which is just greater than 10^297.
        Debug.Assert(e is >= 0 and <= (1 << 15));
        return (uint)(((ulong)e * 169464822037455) >> 49);
    }

    // Returns floor(log_10(5^e)).
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint Log10Pow5(int e)
    {
        // The first value this approximation fails for is 5^2621 which is just greater than 10^1832.
        Debug.Assert(e is >= 0 and <= (1 << 15));
        return (uint)(((ulong)e * 196742565691928) >> 48);
    }

    public static bool operator ==(FloatingDecimal128 left, FloatingDecimal128 right) =>
        left.Equals(right);

    public static bool operator !=(FloatingDecimal128 left, FloatingDecimal128 right) =>
        !(left == right);

    #endregion
}
