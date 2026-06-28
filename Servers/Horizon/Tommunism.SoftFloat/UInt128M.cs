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
using System.Runtime.InteropServices;

namespace Tommunism.SoftFloat;

// NOTE: It looks like .NET 7 added support for 128-bit integers. But we have a few specialiazation functions that might not be so easy to
// do with the new integer type.

[StructLayout(LayoutKind.Sequential, Pack = sizeof(ulong), Size = sizeof(ulong) * 2)]
internal struct UInt128M : IEquatable<UInt128M>, IComparable<UInt128M>
{
    #region Fields

    public static readonly UInt128M Zero = new();
    public static readonly UInt128M One = new(0x0000000000000000, 0x0000000000000001);
    public static readonly UInt128M MinValue = new(0x0000000000000000, 0x0000000000000000);
    public static readonly UInt128M MaxValue = new(0xFFFFFFFFFFFFFFFF, 0xFFFFFFFFFFFFFFFF);

    public ulong V00;
    public ulong V64;

    #endregion

    #region Constructors

    public UInt128M(ulong v64, ulong v0) => (V00, V64) = (v0, v64);

    #endregion

    #region Properties

    public bool IsZero => (V00 | V64) == 0;

    #endregion

    #region Methods

    public int CompareTo(UInt128M other)
    {
        return ((UInt128)this).CompareTo(other);
    }

    public void Deconstruct(out ulong v64, out ulong v0) => (v64, v0) = (V64, V00);

    public bool Equals(UInt128M other) => this == other;

    public override bool Equals(object? obj) => obj is UInt128M int128 && Equals(int128);

    public override int GetHashCode() => HashCode.Combine(V00, V64);

    // softfloat_mul64To128
    /// <summary>
    /// Returns the 128-bit product of <paramref name="a"/> and <paramref name="b"/>.
    /// </summary>
    public static UInt128M Multiply(ulong a, ulong b)
    {
#if true
        var v64 = Math.BigMul(a, b, out var v0);
        return new UInt128M(v64, v0);
#else
        return (UInt128)a * b;
#endif
    }

    // softfloat_mul64ByShifted32To128
    /// <summary>
    /// Returns the 128-bit product of <paramref name="a"/>, <paramref name="b"/>, and 2^32.
    /// </summary>
    public static UInt128M Multiply64ByShifted32(ulong a, uint b)
    {
#if true
        var v64 = Math.BigMul(a, (ulong)b << 32, out var v0);
        return new UInt128M(v64, v0);
#else
        return (UInt128)a * ((ulong)b << 32);
#endif
    }

    // softfloat_shiftRightJam128
    /// <summary>
    /// Shifts the 128 bits formed by this instance right by the number of bits given in <paramref name="dist"/>, which must not be zero.
    /// If any nonzero bits are shifted off, they are "jammed" into the least-significant bit of the shifted value by setting the
    /// least-significant bit to 1. This shifted-and-jammed value is returned.
    /// </summary>
    /// <remarks>
    /// The value of <paramref name="dist"/> can be arbitrarily large. In particular, if <paramref name="dist"/> is greater than 128, the
    /// result will be either 0 or 1, depending on whether the original 128 bits are all zeros.
    /// </remarks>
    public UInt128M ShiftRightJam(int dist)
    {
        Debug.Assert(dist > 0, "Shift amount is out of range.");

        if (dist >= 128)
            return !IsZero ? One : Zero;

        var a = (UInt128)this;
        return (a >> dist) | ((a << -dist) != 0 ? UInt128.One : UInt128.Zero);
    }

    // softfloat_shortShiftRightJam128
    /// <summary>
    /// Shifts the 128 bits formed by this instance right by the number of bits given in <paramref name="dist"/>, which must be in the
    /// range 1 to 63. If any nonzero bits are shifted off, they are "jammed" into the least-significant bit of the shifted value by
    /// setting the least-significant bit to 1. This shifted-and-jammed value is returned.
    /// </summary>
    public UInt128M ShortShiftRightJam(int dist)
    {
        Debug.Assert(dist is > 0 and < 64, "Shift amount is out of range.");

        var a = (UInt128)this;
        return (a >> dist) | (((ulong)a << (-dist)) != 0 ? UInt128.One : UInt128.Zero);
    }

    public override string ToString() => $"0x{V64:x16}{V00:x16}";

    public static explicit operator UInt128M(ulong value) => new(0, value);

    public static explicit operator ulong(UInt128M value) => value.V00;

    public static implicit operator UInt128(UInt128M value) => new(value.V64, value.V00);

    public static implicit operator UInt128M(UInt128 value) =>
        new(value.GetUpperUI64(), value.GetLowerUI64());

    public static bool operator ==(UInt128M a, UInt128M b)
    {
        return (UInt128)a == (UInt128)b;
    }

    public static bool operator !=(UInt128M a, UInt128M b) => !(a == b);

    public static bool operator <(UInt128M a, UInt128M b)
    {
        return (UInt128)a < (UInt128)b;
    }

    public static bool operator >(UInt128M a, UInt128M b) => b < a;

    public static bool operator <=(UInt128M a, UInt128M b)
    {
        return (UInt128)a <= (UInt128)b;
    }

    public static bool operator >=(UInt128M a, UInt128M b) => b <= a;

    public static UInt128M operator <<(UInt128M a, int dist)
    {
        return (UInt128)a << dist;
    }

    public static UInt128M operator >>>(UInt128M a, int dist) => a >> dist;

    public static UInt128M operator >>(UInt128M a, int dist)
    {
        return (UInt128)a >> dist;
    }

    public static UInt128M operator +(UInt128M a, UInt128M b)
    {
        return (UInt128)a + (UInt128)b;
    }

    public static UInt128M operator +(UInt128M a, ulong b)
    {
        return (UInt128)a + b;
    }

    public static UInt128M operator -(UInt128M a, UInt128M b)
    {
        return (UInt128)a - (UInt128)b;
    }

    public static UInt128M operator -(UInt128M a, ulong b)
    {
        return (UInt128)a - b;
    }

    public static UInt128M operator -(UInt128M a)
    {
        return -(UInt128)a;
    }

    public static UInt128M operator *(UInt128M a, uint b)
    {
        return (UInt128)a * b;
    }

    public static UInt128M operator --(UInt128M a) => a - One;

    #endregion
}
