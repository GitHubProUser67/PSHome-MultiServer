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

namespace Tommunism.SoftFloat
{
    internal static class Primitives
    {
        #region Big/Little Endian Index Helpers

        public static readonly int WordIncrement = BitConverter.IsLittleEndian ? 1 : -1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IndexWord(int total, int n) => BitConverter.IsLittleEndian ? n : (total - 1 - n);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IndexWordHi(int total) => BitConverter.IsLittleEndian ? (total - 1) : 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IndexWordLo(int total) => BitConverter.IsLittleEndian ? 0 : (total - 1);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IndexMultiword(int total, int m, int n) => BitConverter.IsLittleEndian ? n : (total - 1 - m);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IndexMultiwordHi(int total, int n) => BitConverter.IsLittleEndian ? (total - n) : 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IndexMultiwordLo(int total, int n) => BitConverter.IsLittleEndian ? 0 : (total - n);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IndexMultiwordHiBut(int total, int n) => BitConverter.IsLittleEndian ? n : 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IndexMultiwordLoBut(int total, int n) => BitConverter.IsLittleEndian ? 0 : n;

        #endregion

#if NET7_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong GetUpperUI64(this UInt128 value) => (ulong)(value >> 64);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong GetLowerUI64(this UInt128 value) => (ulong)value;
#endif

        // softfloat_shortShiftRightJam64
        /// <summary>
        /// Shifts <paramref name="a"/> right by the number of bits given in <paramref name="dist"/>, which must be in the range 1 to 63. If
        /// any nonzero bits are shifted off, they are "jammed" into the least-significant bit of the shifted value by setting the
        /// least-significant bit to 1. This shifted-and-jammed value is returned.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ShortShiftRightJam64(ulong a, int dist) => (a >> dist) | ((a & ((1UL << dist) - 1)) != 0 ? 1UL : 0UL);

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
        public static uint ShiftRightJam32(uint a, int dist) => (dist < 31) ? (a >> dist) | ((a << (-dist)) != 0 ? 1U : 0U) : (a != 0 ? 1U : 0U);

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
        public static ulong ShiftRightJam64(ulong a, int dist) => (dist < 63) ? (a >> dist) | ((a << (-dist)) != 0 ? 1UL : 0UL) : (a != 0 ? 1UL : 0UL);

        // softfloat_countLeadingZeros8
        /// <summary>
        /// A constant table that translates an 8-bit unsigned integer (the array index) into the number of leading 0 bits before the
        /// most-significant 1 of that integer. For integer zero (index 0), the corresponding table element is 8.
        /// </summary>
        public static ReadOnlySpan<byte> CountLeadingZeroes8 => new byte[256]
        {
        8, 7, 6, 6, 5, 5, 5, 5, 4, 4, 4, 4, 4, 4, 4, 4,
        3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
        2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
        2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
        };

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

        // softfloat_approxRecip_1k0s
        internal static readonly ushort[] ApproxRecip_1k0s = new ushort[16]
        {
        0xFFC4, 0xF0BE, 0xE363, 0xD76F, 0xCCAD, 0xC2F0, 0xBA16, 0xB201,
        0xAA97, 0xA3C6, 0x9D7A, 0x97A6, 0x923C, 0x8D32, 0x887E, 0x8417
        };

        // softfloat_approxRecip_1k1s
        internal static readonly ushort[] ApproxRecip_1k1s = new ushort[16]
        {
        0xF0F1, 0xD62C, 0xBFA1, 0xAC77, 0x9C0A, 0x8DDB, 0x8185, 0x76BA,
        0x6D3B, 0x64D4, 0x5D5C, 0x56B1, 0x50B6, 0x4B55, 0x4679, 0x4211
        };

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
        0xB4C9, 0xFFAB, 0xAA7D, 0xF11C, 0xA1C5, 0xE4C7, 0x9A43, 0xDA29,
        0x93B5, 0xD0E5, 0x8DED, 0xC8B7, 0x88C6, 0xC16D, 0x8424, 0xBAE1
        };

        // softfloat_approxRecipSqrt_1k1s
        internal static readonly ushort[] ApproxRecipSqrt_1k1s = new ushort[16]
        {
        0xA5A5, 0xEA42, 0x8C21, 0xC62D, 0x788F, 0xAA7F, 0x6928, 0x94B6,
        0x5CC7, 0x8335, 0x52A6, 0x74E2, 0x4A3E, 0x68FE, 0x432B, 0x5EFD
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

        // softfloat_eq128
        /// <summary>
        /// Returns true if the 128-bit unsigned integer formed by concatenating <paramref name="a64"/> and <paramref name="a0"/> is equal to
        /// the 128-bit unsigned integer formed by concatenating <paramref name="b64"/> and <paramref name="b0"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EQ128(ulong a64, ulong a0, ulong b64, ulong b0)
        {
#if NET7_0_OR_GREATER
        return new UInt128(upper: a64, lower: a0) == new UInt128(upper: b64, lower: b0);
#else
            return (a64 == b64) && (a0 == b0);
#endif
        }

        // softfloat_le128
        /// <summary>
        /// Returns true if the 128-bit unsigned integer formed by concatenating <paramref name="a64"/> and <paramref name="a0"/> is less than
        /// or equal to the 128-bit unsigned integer formed by concatenating <paramref name="b64"/> and <paramref name="b0"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool LE128(ulong a64, ulong a0, ulong b64, ulong b0)
        {
#if NET7_0_OR_GREATER
        return new UInt128(upper: a64, lower: a0) <= new UInt128(upper: b64, lower: b0);
#else
            return (a64 < b64) || ((a64 == b64) && (a0 <= b0));
#endif
        }

        // softfloat_lt128
        /// <summary>
        /// Returns true if the 128-bit unsigned integer formed by concatenating <paramref name="a64"/> and <paramref name="a0"/> is less than
        /// the 128-bit unsigned integer formed by concatenating <paramref name="b64"/> and <paramref name="b0"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool LT128(ulong a64, ulong a0, ulong b64, ulong b0)
        {
#if NET7_0_OR_GREATER
        return new UInt128(upper: a64, lower: a0) < new UInt128(upper: b64, lower: b0);
#else
            return (a64 < b64) || ((a64 == b64) && (a0 < b0));
#endif
        }

        // softfloat_shortShiftLeft128
        /// <summary>
        /// Shifts the 128 bits formed by concatenating <paramref name="a64"/> and <paramref name="a0"/> left by the number of bits given in
        /// <paramref name="dist"/>, which must be in the range 1 to 63.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SFUInt128 ShortShiftLeft128(ulong a64, ulong a0, int dist)
        {
#if NET7_0_OR_GREATER
        return new UInt128(upper: a64, lower: a0) << dist;
#else
            // An out of range shift is fine, internally C# requires 32-bit shifts are ANDed by 63 anyways.
            return new SFUInt128(
                upper: (a64 << dist) | (a0 >> (-dist)),
                lower: a0 << dist
            );
#endif
        }

        // softfloat_shortShiftRight128
        /// <summary>
        /// Shifts the 128 bits formed by concatenating <paramref name="a64"/> and <paramref name="a0"/> right by the number of bits given in
        /// <paramref name="dist"/>, which must be in the range 1 to 63.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SFUInt128 ShortShiftRight128(ulong a64, ulong a0, int dist)
        {
#if NET7_0_OR_GREATER
        return new UInt128(upper: a64, lower: a0) >> dist;
#else
            // An out of range shift is fine, internally C# requires 32-bit shifts are ANDed by 63 anyways.
            return new SFUInt128(
                upper: a64 >> dist,
                lower: (a64 << (-dist)) | (a0 >> dist)
            );
#endif
        }

        // softfloat_shortShiftRightJam64Extra
        /// <summary>
        /// This function is the same as <see cref="ShiftRightJam64Extra(ulong,ulong,int)"/>, except that <paramref name="dist"/> must be in
        /// the range 1 to 63.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt64Extra ShortShiftRightJam64Extra(ulong a, ulong extra, int dist)
        {
            Debug.Assert(dist is >= 1 and < 64, "Shift amount is out of range.");
            return new UInt64Extra(
                v: a >> dist,
                extra: (a << (-dist)) | (extra != 0 ? 1UL : 0UL)
            );
        }

        // softfloat_shortShiftRightJam128
        /// <summary>
        /// Shifts the 128 bits formed by concatenating <paramref name="a64"/> and <paramref name="a0"/> right by the number of bits given in
        /// <paramref name="dist"/>, which must be in the range 1 to 63. If any nonzero bits are shifted off, they are "jammed" into the
        /// least-significant bit of the shifted value by setting the least-significant bit to 1. This shifted-and-jammed value is returned.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SFUInt128 ShortShiftRightJam128(ulong a64, ulong a0, int dist)
        {
            Debug.Assert(dist is >= 1 and < 64, "Shift amount is out of range.");
#if NET7_0_OR_GREATER
        var a = new UInt128(upper: a64, lower: a0);
        return (a >> dist) | (((ulong)a << (-dist)) != 0 ? UInt128.One : UInt128.Zero);
#else
            var negDist = -dist;
            return new SFUInt128(
                upper: a64 >> dist,
                lower: (a64 << negDist) | (a0 >> dist) | ((a0 << negDist) != 0 ? 1UL : 0UL)
            );
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt128Extra ShortShiftRightJam128Extra(SFUInt128 a, ulong extra, int dist) => ShortShiftRightJam128Extra(a.V64, a.V00, extra, dist);

        // softfloat_shortShiftRightJam128Extra
        /// <summary>
        /// This function is the same as <see cref="ShiftRightJam128Extra(ulong,ulong,ulong,int)"/>, except that <paramref name="dist"/> must
        /// be in the range 1 to 63.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt128Extra ShortShiftRightJam128Extra(ulong a64, ulong a0, ulong extra, int dist)
        {
            Debug.Assert(dist is >= 1 and < 64, "Shift amount is out of range.");
            var negDist = -dist;
            return new UInt128Extra(
                v: new SFUInt128(
                    upper: a64 >> dist,
                    lower: (a64 << negDist) | (a0 >> dist)
                ),
                extra: (a0 << negDist) | (extra != 0 ? 1UL : 0UL)
            );
        }

        // softfloat_shiftRightJam64Extra
        /// <summary>
        /// Shifts the 128 bits formed by concatenating <paramref name="a"/> and <paramref name="extra"/> right by 64 <i>plus</i> the number of
        /// bits given in <paramref name="dist"/>, which must not be zero. This shifted value is at most 64 nonzero bits and is returned in the
        /// <see cref="UInt64Extra.V"/> field of the <see cref="UInt64Extra"/> result.  The 64-bit <see cref="UInt64Extra.Extra"/> field of the
        /// result contains a value formed as follows from the bits that were shifted off: The <i>last</i> bit shifted off is the
        /// most-significant bit of the <see cref="UInt64Extra.Extra"/> field, and the other 63 bits of the <see cref="UInt64Extra.Extra"/>
        /// field are all zero if and only if <i>all but the last </i> bits shifted off were all zero.
        /// </summary>
        /// <remarks>
        /// This function makes more sense if <paramref name="a"/> and <paramref name="extra"/> are considered to form an unsigned fixed-point
        /// number with binary point between <paramref name="a"/> and <paramref name="extra"/>. This fixed-point value is shifted right by the
        /// number of bits given in <paramref name="dist"/>, and the integer part of this shifted value is returned in the
        /// <see cref="UInt64Extra.V"/> field of the result. The fractional part of the shifted value is modified as described above and
        /// returned in the <see cref="UInt64Extra.Extra"/> field of the result.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt64Extra ShiftRightJam64Extra(ulong a, ulong extra, int dist)
        {
            UInt64Extra z;
            if (dist < 64)
            {
                z.V = a >> dist;
                z.Extra = a << (-dist);
            }
            else
            {
                z.V = 0;
                z.Extra = (dist == 64) ? a : (a != 0 ? 1UL : 0UL);
            }

            z.Extra |= extra != 0 ? 1UL : 0UL;
            return z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SFUInt128 ShiftRightJam128(SFUInt128 a, int dist) => ShiftRightJam128(a.V64, a.V00, dist);

        // softfloat_shiftRightJam128
        /// <summary>
        /// Shifts the 128 bits formed by concatenating <paramref name="a64"/> and <paramref name="a0"/> right by the number of bits given in
        /// <paramref name="dist"/>, which must not be zero. If any nonzero bits are shifted off, they are "jammed" into the least-significant
        /// bit of the shifted value by setting the least-significant bit to 1. This shifted-and-jammed value is returned.
        /// </summary>
        /// <remarks>
        /// The value of <paramref name="dist"/> can be arbitrarily large. In particular, if <paramref name="dist"/> is greater than 128, the
        /// result will be either 0 or 1, depending on whether the original 128 bits are all zeros.
        /// </remarks>
        public static SFUInt128 ShiftRightJam128(ulong a64, ulong a0, int dist)
        {
            Debug.Assert(dist > 0, "Shift amount is out of range.");
            if (dist < 64)
            {
                var negDist = -dist;
                return new SFUInt128(
                    upper: a64 >> dist,
                    lower: (a64 << negDist) | (a0 >> dist) | ((a0 << negDist) != 0 ? 1UL : 0UL)
                );
            }
            else
            {
                return new SFUInt128(
                    upper: 0,
                    lower: (dist < 127)
                        ? (a64 >> dist) | (((a64 & ((1UL << dist) - 1)) | a0) != 0 ? 1UL : 0UL)
                        : ((a64 | a0) != 0 ? 1UL : 0UL)
                );
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt128Extra ShiftRightJam128Extra(SFUInt128 a, ulong extra, int dist) => ShiftRightJam128Extra(a.V64, a.V00, extra, dist);

        // softfloat_shiftRightJam128Extra
        /// <summary>
        /// Shifts the 192 bits formed by concatenating <paramref name="a64"/>, <paramref name="a0"/>, and <paramref name="extra"/> right by 64
        /// <i>plus</i> the number of bits given in <paramref name="dist"/>, which must not be zero. This shifted value is at most 128 nonzero
        /// bits and is returned in the <see cref="UInt64Extra.V"/> field of the <see cref="UInt64Extra"/> result. The 64-bit
        /// <see cref="UInt64Extra.Extra"/> field of the result contains a value formed as follows from the bits that were shifted off: The
        /// <i>last</i> bit shifted off is the most-significant bit of the <see cref="UInt64Extra.Extra"/> field, and the other 63 bits of the
        /// <see cref="UInt64Extra.Extra"/> field are all zero if and only if <i>all but the last</i> bits shifted off were all zero.
        /// </summary>
        /// <remarks>
        /// This function makes more sense if <paramref name="a64"/>, <paramref name="a0"/>, and <paramref name="extra"/> are considered to
        /// form an unsigned fixed-point number with binary point between <paramref name="a0"/> and <paramref name="extra"/>.  This fixed-point
        /// value is shifted right by the number of bits given in <paramref name="dist"/>, and the integer part of this shifted value is
        /// returned in the <see cref="UInt64Extra.V"/> field of the result. The fractional part of the shifted value is modified as described
        /// above and returned in the <paramref name="extra"/> field of the result.
        /// </remarks>
        public static UInt128Extra ShiftRightJam128Extra(ulong a64, ulong a0, ulong extra, int dist)
        {
            Debug.Assert(dist > 0, "Shift amount is out of range.");

            SFUInt128 zv;
            ulong zextra;

            var negDist = -dist;
            if (dist < 64)
            {
                zv.V64 = a64 >> dist;
                zv.V00 = (a64 << negDist) | (a0 >> dist);
                zextra = a0 << negDist;
            }
            else
            {
                zv.V64 = 0;
                if (dist == 64)
                {
                    zv.V00 = a64;
                    zextra = a0;
                }
                else
                {
                    extra |= a0;
                    if (dist < 128)
                    {
                        zv.V00 = a64 >> dist;
                        zextra = a64 << negDist;
                    }
                    else
                    {
                        zv.V00 = 0;
                        zextra = (dist == 128) ? a64 : (a64 != 0 ? 1UL : 0UL);
                    }
                }
            }

            return new UInt128Extra(
                extra: zextra | (extra != 0 ? 1UL : 0UL),
                v: zv
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SFUInt256 ShiftRightJam256M(SFUInt256 a, int dist)
        {
            Span<ulong> result = stackalloc ulong[4];
            ShiftRightJam256M(a.AsSpan(), dist, result);
            return new SFUInt256(result);
        }

        // softfloat_shiftRightJam256M
        /// <summary>
        /// Shifts the 256-bit unsigned integer pointed to by <paramref name="aPtr"/> right by the number of bits given in
        /// <paramref name="dist"/>, which must not be zero. If any nonzero bits are shifted off, they are "jammed" into the least-significant
        /// bit of the shifted value by setting the least-significant bit to 1. This shifted-and-jammed value is stored at the location pointed
        /// to by <paramref name="zPtr"/>. Each of <paramref name="aPtr"/> and <paramref name="zPtr"/> points to an array of four 64-bit
        /// elements that concatenate in the platform's normal endian order to form a 256-bit integer.
        /// </summary>
        /// <remarks>
        /// The value of <paramref name="dist"/> can be arbitrarily large.  In particular, if <paramref name="dist"/> is greater than 256, the
        /// stored result will be either 0 or 1, depending on whether the original 256 bits are all zeros.
        /// </remarks>
        public static void ShiftRightJam256M(ReadOnlySpan<ulong> aPtr, int dist, Span<ulong> zPtr)
        {
            Debug.Assert(aPtr.Length >= 4, "A is too small.");
            Debug.Assert(zPtr.Length >= 4, "Z is too small.");
            Debug.Assert(dist > 0, "Shift amount is out of range.");

            // softfloat_shortShiftRightJamM
            static void ShortShiftRightJamM(ReadOnlySpan<ulong> aPtr, int dist, Span<ulong> zPtr)
            {
                Debug.Assert(dist is > 0 and < 64, "Shift amount is out of range.");
                Debug.Assert(aPtr.Length <= zPtr.Length, "A length is less than Z length.");

                var sizeWords = aPtr.Length;
                var negDist = -dist;
                var index = IndexWordLo(sizeWords);
                var lastIndex = IndexWordHi(sizeWords);
                var wordA = aPtr[index];

                var partWordZ = wordA >> dist;
                if ((partWordZ << dist) != wordA)
                    partWordZ |= 1;

                while (index != lastIndex)
                {
                    wordA = aPtr[index + WordIncrement];
                    zPtr[index] = (wordA << negDist) | partWordZ;
                    index += WordIncrement;
                    partWordZ = wordA >> dist;
                }

                zPtr[index] = partWordZ;
            }

            int ptr = 0, i;

            var wordJam = 0UL;
            var wordDist = dist >> 6;
            if (wordDist != 0)
            {
                if (4 < wordDist)
                    wordDist = 4;

                ptr = IndexMultiwordLo(4, wordDist);
                i = wordDist;
                do
                {
                    wordJam = aPtr[ptr++];
                    if (wordJam != 0)
                        break;

                    --i;
                }
                while (i != 0);

                ptr = 0;
            }

            if (wordDist < 4)
            {
                var aPtrIndex = IndexMultiwordHiBut(4, wordDist);
                var innerDist = dist & 63;
                if (innerDist != 0)
                {
                    ShortShiftRightJamM(
                        aPtr.Slice(aPtrIndex, 4 - wordDist),
                        innerDist,
                        zPtr[IndexMultiwordLoBut(4, wordDist)..]
                    );

                    if (wordDist == 0)
                        goto wordJam;
                }
                else
                {
                    aPtrIndex += IndexWordLo(4 - wordDist);
                    ptr = IndexWordLo(4);
                    for (i = 4 - wordDist; i != 0; --i)
                    {
                        zPtr[ptr] = aPtr[aPtrIndex];
                        aPtrIndex += WordIncrement;
                        ptr += WordIncrement;
                    }
                }

                ptr = IndexMultiwordHi(4, wordDist);
            }

            Debug.Assert(wordDist != 0);
            do
            {
                zPtr[ptr++] = 0;
                --wordDist;
            }
            while (wordDist != 0);

        wordJam:
            if (wordJam != 0)
                zPtr[IndexWordLo(4)] |= 1;
        }

        // softfloat_add128
        /// <summary>
        /// Returns the sum of the 128-bit integer formed by concatenating <paramref name="a64"/> and <paramref name="a0"/> and the 128-bit
        /// integer formed by concatenating <paramref name="b64"/> and <paramref name="b0"/>. The addition is modulo 2^128, so any carry out is
        /// lost.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SFUInt128 Add128(ulong a64, ulong a0, ulong b64, ulong b0)
        {
#if NET7_0_OR_GREATER
        return new UInt128(upper: a64, lower: a0) + new UInt128(upper: b64, lower: b0);
#else
            SFUInt128 z;
            z.V00 = a0 + b0;
            z.V64 = a64 + b64 + (z.V00 < a0 ? 1UL : 0UL);
            return z;
#endif
        }

        // softfloat_add256M
        /// <summary>
        /// Adds the two 256-bit integers pointed to by <paramref name="aPtr"/> and <paramref name="bPtr"/>. The addition is modulo 2^256, so
        /// any carry out is lost. The sum is stored at the location pointed to by <paramref name="zPtr"/>. Each of <paramref name="aPtr"/>,
        /// <paramref name="bPtr"/>, and <paramref name="zPtr"/> points to an array of four 64-bit elements that concatenate in the platform's
        /// normal endian order to form a 256-bit integer.
        /// </summary>
        public static void Add256M(ReadOnlySpan<ulong> aPtr, ReadOnlySpan<ulong> bPtr, Span<ulong> zPtr)
        {
            Debug.Assert(aPtr.Length >= 4, "A is too small.");
            Debug.Assert(bPtr.Length >= 4, "B is too small.");
            Debug.Assert(zPtr.Length >= 4, "Z is too small.");

            var index = IndexWordLo(4);
            var carry = 0UL;
            while (true)
            {
                var wordA = aPtr[index];
                var wordZ = wordA + bPtr[index] + carry;
                zPtr[index] = wordZ;
                if (index == IndexWordHi(4))
                    break;

                if (wordZ != wordA)
                    carry = wordZ < wordA ? 1UL : 0UL;

                index += WordIncrement;
            }
        }

        // softfloat_sub128
        /// <summary>
        /// Returns the difference of the 128-bit integer formed by concatenating <paramref name="a64"/> and <paramref name="a0"/> and the
        /// 128-bit integer formed by concatenating <paramref name="b64"/> and <paramref name="b0"/>. The subtraction is modulo 2^128, so any
        /// borrow out (carry out) is lost.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SFUInt128 Sub128(ulong a64, ulong a0, ulong b64, ulong b0)
        {
#if NET7_0_OR_GREATER
        return new UInt128(upper: a64, lower: a0) - new UInt128(upper: b64, lower: b0);
#else
            return new(
                lower: a0 - b0,
                upper: a64 - b64 - (a0 < b0 ? 1UL : 0UL)
            );
#endif
        }

        // softfloat_sub256M
        /// <summary>
        /// Subtracts the 256-bit integer pointed to by <paramref name="bPtr"/> from the 256-bit integer pointed to by <paramref name="aPtr"/>.
        /// The addition is modulo 2^256, so any borrow out (carry out) is lost. The difference is stored at the location pointed to by
        /// <paramref name="zPtr"/>.  Each of <paramref name="aPtr"/>, <paramref name="bPtr"/>, and <paramref name="zPtr"/> points to an array
        /// of four 64-bit elements that concatenate in the platform's normal endian order to form a 256-bit integer.
        /// </summary>
        public static void Sub256M(ReadOnlySpan<ulong> aPtr, ReadOnlySpan<ulong> bPtr, Span<ulong> zPtr)
        {
            Debug.Assert(aPtr.Length >= 4, "A is too small.");
            Debug.Assert(bPtr.Length >= 4, "B is too small.");
            Debug.Assert(zPtr.Length >= 4, "Z is too small.");

            var index = IndexWordLo(4);
            var borrow = 0UL;
            while (true)
            {
                var wordA = aPtr[index];
                var wordB = bPtr[index];
                zPtr[index] = wordA - wordB - borrow;
                if (index == IndexWordHi(4))
                    break;

                borrow = ((borrow != 0) ? (wordA <= wordB) : (wordA < wordB)) ? 1UL : 0UL;
                index += WordIncrement;
            }
        }

        // softfloat_mul64ByShifted32To128
        /// <summary>
        /// Returns the 128-bit product of <paramref name="a"/>, <paramref name="b"/>, and 2^32.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SFUInt128 Mul64ByShifted32To128(ulong a, uint b)
        {
#if NET7_0_OR_GREATER
        return (UInt128)a * ((UInt128)b << 32);
#else
            var mid = (ulong)(uint)a * b;
            return new SFUInt128(
                upper: (ulong)(uint)(a >> 32) * b + (mid >> 32),
                lower: mid << 32
            );
#endif
        }

        // softfloat_mul64To128
        /// <summary>
        /// Returns the 128-bit product of <paramref name="a"/> and <paramref name="b"/>.
        /// </summary>
        public static SFUInt128 Mul64To128(ulong a, ulong b)
        {
#if NET7_0_OR_GREATER
        return (UInt128)a * b;
#else
            SFUInt128 z;

            var a32 = (uint)(a >> 32);
            var a0 = (uint)a;

            var b32 = (uint)(b >> 32);
            var b0 = (uint)b;

            z.V00 = (ulong)a0 * b0;
            var mid1 = (ulong)a32 * b0;
            var mid = mid1 + (ulong)a0 * b32;
            z.V64 = (ulong)a32 * b32;

            z.V64 += (mid < mid1 ? (1UL << 32) : 0UL) | (mid >> 32);
            mid <<= 32;
            z.V00 += mid;
            z.V64 += z.V00 < mid ? 1UL : 0UL;
            return z;
#endif
        }

        // softfloat_mul128By32
        /// <summary>
        /// Returns the product of the 128-bit integer formed by concatenating <paramref name="a64"/> and <paramref name="a0"/>, multiplied by
        /// <paramref name="b"/>. The multiplication is modulo 2^128; any overflow bits are discarded.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SFUInt128 Mul128By32(ulong a64, ulong a0, uint b)
        {
#if NET7_0_OR_GREATER
        return new UInt128(upper: a64, lower: a0) * b;
#else
            SFUInt128 z;
            z.V00 = a0 * b;
            var mid = (ulong)(uint)(a0 >> 32) * b;
            var carry = (uint)(z.V00 >> 32) - (uint)mid;
            z.V64 = a64 * b + (uint)((mid + carry) >> 32);
            return z;
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SFUInt256 Mul128To256M(SFUInt128 a, SFUInt128 b)
        {
            Span<ulong> result = stackalloc ulong[4];
            Mul128To256M(a.V64, a.V00, b.V64, b.V00, result);
            return new SFUInt256(result);
        }

        // softfloat_mul128To256M
        /// <summary>
        /// Multiplies the 128-bit unsigned integer formed by concatenating <paramref name="a64"/> and <paramref name="a0"/> by the 128-bit
        /// unsigned integer formed by concatenating <paramref name="b64"/> and <paramref name="b0"/>. The 256-bit product is stored at the
        /// location pointed to by <paramref name="zPtr"/>. Argument <paramref name="zPtr"/> points to an array of four 64-bit elements that
        /// concatenate in the platform's normal endian order to form a 256-bit integer.
        /// </summary>
        public static void Mul128To256M(ulong a64, ulong a0, ulong b64, ulong b0, Span<ulong> zPtr)
        {
            Debug.Assert(zPtr.Length >= 4, "Z is too small.");

#if NET7_0_OR_GREATER
        UInt128 z0, mid1, mid, z128;
        z0 = (UInt128)a0 * b0;
        mid1 = (UInt128)a64 * b0;
        mid = mid1 + (UInt128)a0 * b64;
        z128 = (UInt128)a64 * b64;
        z128 += new UInt128(upper: (mid < mid1) ? 1UL : 0, lower: (ulong)(mid >> 64));
        //z128 += ((mid < mid1 ? UInt128.One : UInt128.Zero) << 64) | (mid >> 64); // should be identical to above, but maybe slightly slower
        mid <<= 64;
        z0 += mid;
        z128 += (z0 < mid) ? UInt128.One : UInt128.Zero;

        zPtr[IndexWord(4, 0)] = (ulong)z0;
        zPtr[IndexWord(4, 1)] = (ulong)(z0 >> 64);
        zPtr[IndexWord(4, 2)] = (ulong)z128;
        zPtr[IndexWord(4, 3)] = (ulong)(z128 >> 64);
#else
            SFUInt128 p0, p64, p128;
            ulong z64, z128, z192;

            p0 = Mul64To128(a0, b0);
            zPtr[IndexWord(4, 0)] = p0.V00;
            p64 = Mul64To128(a64, b0);
            z64 = p64.V00 + p0.V64;
            z128 = p64.V64 + (z64 < p64.V00 ? 1UL : 0UL);
            p128 = Mul64To128(a64, b64);
            z128 += p128.V00;
            z192 = p128.V64 + (z128 < p128.V00 ? 1UL : 0UL);
            p64 = Mul64To128(a0, b64);
            z64 += p64.V00;
            zPtr[IndexWord(4, 1)] = z64;
            p64.V64 += z64 < p64.V00 ? 1UL : 0UL;
            z128 += p64.V64;
            zPtr[IndexWord(4, 2)] = z128;
            zPtr[IndexWord(4, 3)] = z192 + (z128 < p64.V64 ? 1UL : 0UL);
#endif
        }
    }

}