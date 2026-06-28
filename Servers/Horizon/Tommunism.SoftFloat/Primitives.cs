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
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Tommunism.SoftFloat;

internal static class Primitives
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong GetUpperUI64(this UInt128 value) => (ulong)(value >> 64);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong GetLowerUI64(this UInt128 value) => (ulong)value;

    // softfloat_shortShiftRightJam64
    /// <summary>
    /// Shifts <paramref name="a"/> right by the number of bits given in <paramref name="dist"/>, which must be in the range 1 to 63. If
    /// any nonzero bits are shifted off, they are "jammed" into the least-significant bit of the shifted value by setting the
    /// least-significant bit to 1. This shifted-and-jammed value is returned.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong ShortShiftRightJam(this ulong a, int dist) =>
        (a >> dist) | ((a & ((1UL << dist) - 1)) != 0 ? 1UL : 0UL);

    // softfloat_shiftRightJam32
    /// <summary>
    /// Shifts <paramref name="a"/> right by the number of bits given in <paramref name="dist"/>, which must not be zero. If any nonzero
    /// bits are shifted off, they are "jammed" into the least-significant bit of the shifted value by setting the least-significant bit to
    /// 1. This shifted-and-jammed value is returned.
    /// </summary>
    /// <remarks>
    /// The value of <paramref name="dist"/> can be arbitrarily large. In particular, if <paramref name="dist"/> is greater than 32, the
    /// result will be either 0 or 1, depending on whether <paramref name="a"/> is zero or nonzero.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint ShiftRightJam(this uint a, int dist) =>
        (dist < 31) ? ((a >> dist) | ((a << -dist) != 0 ? 1U : 0U)) : (a != 0 ? 1U : 0U);

    // softfloat_shiftRightJam64
    /// <summary>
    /// Shifts <paramref name="a"/> right by the number of bits given in <paramref name="dist"/>, which must not be zero. If any nonzero
    /// bits are shifted off, they are "jammed" into the least-significant bit of the shifted value by setting the least-significant bit to
    /// 1. This shifted-and-jammed value is returned.
    /// </summary>
    /// <remarks>
    /// The value of <paramref name="dist"/> can be arbitrarily large.  In particular, if <paramref name="dist"/> is greater than 64, the
    /// result will be either 0 or 1, depending on whether <paramref name="a"/> is zero or nonzero.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong ShiftRightJam(this ulong a, int dist) =>
        (dist < 63) ? ((a >> dist) | ((a << -dist) != 0 ? 1UL : 0UL)) : (a != 0 ? 1UL : 0UL);

    // softfloat_countLeadingZeros16
    /// <summary>
    /// Returns the number of leading 0 bits before the most-significant 1 bit of <paramref name="a"/>. If <paramref name="a"/> is zero, 16
    /// is returned.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int CountLeadingZeroes16(uint a)
    {
        Debug.Assert((a & ~0xFFFFU) == 0);
        return BitOperations.LeadingZeroCount(a) - 16;
    }

    // softfloat_countLeadingZeros32
    /// <summary>
    /// Returns the number of leading 0 bits before the most-significant 1 bit of <paramref name="a"/>. If <paramref name="a"/> is zero, 32
    /// is returned.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int CountLeadingZeroes32(uint a) => BitOperations.LeadingZeroCount(a);

    // softfloat_countLeadingZeros64
    /// <summary>
    /// Returns the number of leading 0 bits before the most-significant 1 bit of <paramref name="a"/>. If <paramref name="a"/> is zero, 64
    /// is returned.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int CountLeadingZeroes64(ulong a) => BitOperations.LeadingZeroCount(a);

    // softfloat_approxRecip32_1 -- assumes fast 64-bit integer divide is available
    /// <summary>
    /// Returns an approximation to the reciprocal of the number represented by <paramref name="a"/>, where <paramref name="a"/> is
    /// interpreted as an unsigned fixed-point number with one integer bit and 31 fraction bits. The <paramref name="a"/> input must be
    /// "normalized", meaning that its most-significant bit (bit 31) must be 1. Thus, if A is the value of the fixed-point interpretation
    /// of <paramref name="a"/>, then 1 <= A < 2. The returned value is interpreted as a pure unsigned fraction, having no integer bits and
    /// 32 fraction bits. The approximation returned is never greater than the true reciprocal 1/A, and it differs from the true reciprocal
    /// by at most 2.006 ulp (units in the last place).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint ApproxRecip32_1(uint a) => (uint)(0x7FFFFFFFFFFFFFFFUL / a);

    // softfloat_approxRecipSqrt_1k0s
    internal static readonly ushort[] ApproxRecipSqrt_1k0s = new ushort[16]
    {
        0xB4C9,
        0xFFAB,
        0xAA7D,
        0xF11C,
        0xA1C5,
        0xE4C7,
        0x9A43,
        0xDA29,
        0x93B5,
        0xD0E5,
        0x8DED,
        0xC8B7,
        0x88C6,
        0xC16D,
        0x8424,
        0xBAE1,
    };

    // softfloat_approxRecipSqrt_1k1s
    internal static readonly ushort[] ApproxRecipSqrt_1k1s = new ushort[16]
    {
        0xA5A5,
        0xEA42,
        0x8C21,
        0xC62D,
        0x788F,
        0xAA7F,
        0x6928,
        0x94B6,
        0x5CC7,
        0x8335,
        0x52A6,
        0x74E2,
        0x4A3E,
        0x68FE,
        0x432B,
        0x5EFD,
    };

    // softfloat_approxRecipSqrt32_1
    /// <summary>
    /// Returns an approximation to the reciprocal of the square root of the number represented by <paramref name="a"/>, where
    /// <paramref name="a"/> is interpreted as an unsigned fixed-point number either with one integer bit and 31 fraction bits or with two
    /// integer bits and 30 fraction bits. The format of <paramref name="a"/> is determined by <paramref name="oddExpA"/>, which must be
    /// either 0 or 1. If <paramref name="oddExpA"/> is 1, <paramref name="a"/> is interpreted as having one integer bit, and if
    /// <paramref name="oddExpA"/> is 0, <paramref name="a"/> is interpreted as having two integer bits. The <paramref name="a"/> input
    /// must be "normalized", meaning that its most-significant bit (bit 31) must be 1. Thus, if A is the value of the fixed-point
    /// interpretation of <paramref name="a"/>, it follows that 1 <= A < 2 when <paramref name="oddExpA"/> is 1, and 2 <= A < 4 when
    /// <paramref name="oddExpA"/> is 0.
    /// </summary>
    /// <remarks>
    /// The returned value is interpreted as a pure unsigned fraction, having no integer bits and 32 fraction bits. The approximation
    /// returned is never greater than the true reciprocal 1/sqrt(A), and it differs from the true reciprocal by at most 2.06 ulp (units in
    /// the last place). The approximation returned is also always within the range 0.5 to 1; thus, the most-significant bit of the result
    /// is always set.
    /// </remarks>
    public static uint ApproxRecipSqrt32_1(uint oddExpA, uint a)
    {
        var index = (a >> 27 & 0xE) + oddExpA;
        var eps = (a >> 12) & 0xFFFF;
        var r0 = ApproxRecipSqrt_1k0s[index] - ((ApproxRecipSqrt_1k1s[index] * eps) >> 20);

        var ESqrR0 = r0 * r0;
        if (oddExpA == 0)
            ESqrR0 <<= 1;

        var sigma0 = ~(uint)((ESqrR0 * (ulong)a) >> 23);
        var r = (uint)((r0 << 16) + ((r0 * (ulong)sigma0) >> 25));
        var sqrSigma0 = (uint)(((ulong)sigma0 * sigma0) >> 32);
        r += (uint)((((r >> 1) + (r >> 3) - (r0 << 14)) * (ulong)sqrSigma0) >> 48);
        return (r & 0x80000000) != 0 ? r : 0x80000000;
    }
}
