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
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Tommunism.SoftFloat;

[StructLayout(LayoutKind.Sequential, Pack = sizeof(ulong), Size = sizeof(ulong) * 4)]
internal struct UInt256M : IEquatable<UInt256M>
{
    #region Fields

    public static readonly UInt256M Zero = new();
    public static readonly UInt256M One = new(
        0x0000000000000000,
        0x0000000000000000,
        0x0000000000000000,
        0x0000000000000001
    );
    public static readonly UInt256M MinValue = new(
        0x0000000000000000,
        0x0000000000000000,
        0x0000000000000000,
        0x0000000000000000
    );
    public static readonly UInt256M MaxValue = new(
        0xFFFFFFFFFFFFFFFF,
        0xFFFFFFFFFFFFFFFF,
        0xFFFFFFFFFFFFFFFF,
        0xFFFFFFFFFFFFFFFF
    );

    public ulong V000,
        V064,
        V128,
        V192;

    #endregion

    #region Constructors

    public UInt256M(ulong v192, ulong v128, ulong v64, ulong v0) =>
        (V000, V064, V128, V192) = (v0, v64, v128, v192);

    public UInt256M(UInt128M v128, UInt128M v0)
    {
        (V064, V000) = v0;
        (V192, V128) = v128;
    }

    #endregion

    #region Properties

    public bool IsZero => (V000 | V064 | V128 | V192) == 0;

    public UInt128M V000_UI128
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(V064, V000);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => (V064, V000) = value;
    }

    public UInt128M V128_UI128
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(V192, V128);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => (V192, V128) = value;
    }

    #endregion

    #region Methods

    public void Deconstruct(out ulong v192, out ulong v128, out ulong v64, out ulong v0) =>
        (v192, v128, v64, v0) = (V192, V128, V064, V000);

    public void Deconstruct(out UInt128M v128, out UInt128M v0) =>
        (v128, v0) = (new(V192, V128), new(V064, V000));

    public override bool Equals(object? obj) => obj is UInt256M int256 && Equals(int256);

    public bool Equals(UInt256M other) =>
        V000 == other.V000 && V064 == other.V064 && V128 == other.V128 && V192 == other.V192;

    public override int GetHashCode() => HashCode.Combine(V000, V064, V128, V192);

    // softfloat_mul128To256M
    /// <summary>
    /// Multiplies the 128-bit unsigned integer <paramref name="a"/> by the 128-bit unsigned integer <paramref name="b"/>. The 256-bit
    /// product is returned.
    /// </summary>
    public static UInt256M Multiply(UInt128M a, UInt128M b)
    {
        UInt256M z;

        UInt128 z0,
            mid1,
            mid,
            z128;
        z0 = (UInt128)a.V00 * b.V00;
        mid1 = (UInt128)a.V64 * b.V00;
        mid = mid1 + (UInt128)a.V00 * b.V64;
        z128 = (UInt128)a.V64 * b.V64;
        z128 += new UInt128(upper: (mid < mid1) ? 1UL : 0, lower: (ulong)(mid >> 64));
        mid <<= 64;
        z0 += mid;
        z128 += (z0 < mid) ? UInt128.One : UInt128.Zero;

        z.V000 = (ulong)z0;
        z.V064 = (ulong)(z0 >> 64);
        z.V128 = (ulong)z128;
        z.V192 = (ulong)(z128 >> 64);

        return z;
    }

    // softfloat_shiftRightJam256M
    /// <summary>
    /// Shifts this 256-bit unsigned integer right by the number of bits given in <paramref name="dist"/>, which must be greater than zero.
    /// If any nonzero bits are shifted off, they are "jammed" into the least-significant bit of the shifted value by setting the
    /// least-significant bit to 1. This shifted-and-jammed value is returned.
    /// </summary>
    /// <remarks>
    /// The value of <paramref name="dist"/> can be arbitrarily large. In particular, if <paramref name="dist"/> is greater than 256, the
    /// stored result will be either 0 or 1, depending on whether the original 256 bits are all zeros.
    /// </remarks>
    public UInt256M ShiftRightJam(int dist)
    {
        Debug.Assert(dist > 0, "Shift amount is out of range.");

        // Shortcut if shifting by 256 or more bits.
        if (dist >= 256)
            return !IsZero ? One : Zero;

        // Shift value right.
        var z = this >> dist;

        // Calculate jam value and set jam bit (if needed).
        var jamZ = this << -dist;
        if (!jamZ.IsZero)
            z.V000 |= 1;

        return z;
    }

    public override string ToString() => $"0x{V192:x16}{V128:x16}{V064:x16}{V000:x16}";

    public static explicit operator UInt256M(ulong value) =>
        new(v0: value, v64: 0, v128: 0, v192: 0);

    public static explicit operator UInt256M(UInt128M value) => new(v0: value, v128: UInt128M.Zero);

    public static explicit operator ulong(UInt256M value) => value.V000;

    public static explicit operator UInt128M(UInt256M value) => value.V000_UI128;

    public static bool operator ==(UInt256M left, UInt256M right) => left.Equals(right);

    public static bool operator !=(UInt256M left, UInt256M right) => !(left == right);

    public static UInt256M operator <<(UInt256M a, int dist)
    {
        dist &= 255;

        ulong z000,
            z064,
            z128,
            z192;
        int negDist;

        switch ((uint)dist >> 6)
        {
            case 0b00: // 0..63
            {
                Debug.Assert(dist is >= 0 and < 64);

                if (dist == 0)
                {
                    z192 = a.V192;
                    z128 = a.V128;
                    z064 = a.V064;
                    z000 = a.V000;
                }
                else
                {
                    negDist = -dist;
                    z192 = (a.V192 << dist) | (a.V128 >> negDist);
                    z128 = (a.V128 << dist) | (a.V064 >> negDist);
                    z064 = (a.V064 << dist) | (a.V000 >> negDist);
                    z000 = a.V000 << dist;
                }

                break;
            }
            case 0b01: // 64..127
            {
                dist -= 64;
                Debug.Assert(dist is >= 0 and < 64);

                if (dist == 0)
                {
                    z192 = a.V128;
                    z128 = a.V064;
                    z064 = a.V000;
                }
                else
                {
                    negDist = -dist;
                    z192 = (a.V128 << dist) | (a.V064 >> negDist);
                    z128 = (a.V064 << dist) | (a.V000 >> negDist);
                    z064 = a.V000 << dist;
                }

                z000 = 0;
                break;
            }
            case 0b10: // 128..191
            {
                dist -= 128;
                Debug.Assert(dist is >= 0 and < 64);

                if (dist == 0)
                {
                    z192 = a.V064;
                    z128 = a.V000;
                }
                else
                {
                    negDist = -dist;
                    z192 = (a.V064 << dist) | (a.V000 >> negDist);
                    z128 = a.V000 << dist;
                }

                z064 = 0;
                z000 = 0;
                break;
            }
            default: // 192..255
            {
                dist -= 192;
                Debug.Assert(dist is >= 0 and < 64);

                z192 = a.V000 << dist;
                z128 = 0;
                z064 = 0;
                z000 = 0;
                break;
            }
        }

        return new(z192, z128, z064, z000);
    }

    public static UInt256M operator >>>(UInt256M a, int dist) => a >> dist;

    public static UInt256M operator >>(UInt256M a, int dist)
    {
        dist &= 255;

        ulong z000,
            z064,
            z128,
            z192;
        int negDist;

        switch ((uint)dist >> 6)
        {
            case 0b00: // 0..63
            {
                Debug.Assert(dist is >= 0 and < 64);

                if (dist == 0)
                {
                    z192 = a.V192;
                    z128 = a.V128;
                    z064 = a.V064;
                    z000 = a.V000;
                }
                else
                {
                    negDist = -dist;
                    z192 = a.V192 >> dist;
                    z128 = (a.V128 >> dist) | (a.V192 << negDist);
                    z064 = (a.V064 >> dist) | (a.V128 << negDist);
                    z000 = (a.V000 >> dist) | (a.V064 << negDist);
                }

                break;
            }
            case 0b01: // 64..127
            {
                dist -= 64;
                Debug.Assert(dist is >= 0 and < 64);

                z192 = 0;
                if (dist == 0)
                {
                    z128 = a.V192;
                    z064 = a.V128;
                    z000 = a.V064;
                }
                else
                {
                    negDist = -dist;
                    z128 = a.V192 >> dist;
                    z064 = (a.V128 >> dist) | (a.V192 << negDist);
                    z000 = (a.V064 >> dist) | (a.V128 << negDist);
                }

                break;
            }
            case 0b10: // 128..191
            {
                dist -= 128;
                Debug.Assert(dist is >= 0 and < 64);

                z192 = 0;
                z128 = 0;
                if (dist == 0)
                {
                    z064 = a.V192;
                    z000 = a.V128;
                }
                else
                {
                    negDist = -dist;
                    z064 = a.V192 >> dist;
                    z000 = (a.V128 >> dist) | (a.V192 << negDist);
                }

                break;
            }
            default: // 192..255
            {
                dist -= 192;
                Debug.Assert(dist is >= 0 and < 64);

                z192 = 0;
                z128 = 0;
                z064 = 0;
                z000 = a.V192 >> dist;
                break;
            }
        }

        return new(z192, z128, z064, z000);
    }

    public static UInt256M operator +(UInt256M left, UInt256M right)
    {
        ulong a0 = left.V000,
            a64 = left.V064,
            a128 = left.V128,
            a192 = left.V192;
        ulong b0 = right.V000,
            b64 = right.V064,
            b128 = right.V128,
            b192 = right.V192;
        ulong z0,
            z1,
            z2,
            z3,
            carry;

        z0 = a0 + b0;
        carry = (z0 != a0 && z0 < a0) ? 1UL : 0UL;

        z1 = a64 + b64 + carry;
        if (z1 != a64)
            carry = (z1 < a64) ? 1UL : 0UL;

        z2 = a128 + b128 + carry;
        if (z2 != a128)
            carry = (z2 < a128) ? 1UL : 0UL;

        z3 = a192 + b192 + carry;
        return new(z3, z2, z1, z0);
    }

    public static UInt256M operator -(UInt256M left, UInt256M right)
    {
        ulong a0 = left.V000,
            a64 = left.V064,
            a128 = left.V128,
            a192 = left.V192;
        ulong b0 = right.V000,
            b64 = right.V064,
            b128 = right.V128,
            b192 = right.V192;
        ulong z0,
            z1,
            z2,
            z3,
            borrow;

        z0 = a0 - b0;
        borrow = (a0 < b0) ? 1UL : 0UL;

        z1 = a64 - b64 - borrow;
        borrow = ((borrow != 0) ? (a64 <= b64) : (a64 < b64)) ? 1UL : 0UL;

        z2 = a128 - b128 - borrow;
        borrow = ((borrow != 0) ? (a128 <= b128) : (a128 < b128)) ? 1UL : 0UL;

        z3 = a192 - b192 - borrow;
        return new(z3, z2, z1, z0);
    }

    public static UInt256M operator -(UInt256M value)
    {
        ulong b0 = value.V000,
            b64 = value.V064,
            b128 = value.V128,
            b192 = value.V192;
        ulong z0,
            z1,
            z2,
            z3,
            borrow;

        z0 = 0 - b0;
        borrow = (0 < b0) ? 1UL : 0UL;

        z1 = 0 - b64 - borrow;
        borrow = ((borrow != 0) ? (0 <= b64) : (0 < b64)) ? 1UL : 0UL;

        z2 = 0 - b128 - borrow;
        borrow = ((borrow != 0) ? (0 <= b128) : (0 < b128)) ? 1UL : 0UL;

        z3 = 0 - b192 - borrow;
        return new(z3, z2, z1, z0);
    }

    #endregion
}
