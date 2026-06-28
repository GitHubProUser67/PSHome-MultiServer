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

namespace Tommunism.SoftFloat;

internal struct UInt128Extra : IEquatable<UInt128Extra>
{
    #region Fields

    public ulong Extra;
    public ulong V00,
        V64;

    #endregion

    #region Constructors

    public UInt128Extra(UInt128M v)
        : this(v.V64, v.V00, 0) { }

    public UInt128Extra(UInt128M v, ulong extra)
        : this(v.V64, v.V00, extra) { }

    public UInt128Extra(ulong v64, ulong v0)
        : this(v64, v0, 0) { }

    public UInt128Extra(ulong v64, ulong v0, ulong extra)
    {
        Extra = extra;
        V00 = v0;
        V64 = v64;
    }

    #endregion

    #region Properties

    public UInt128M V
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(V64, V00);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => (V64, V00) = value;
    }

    #endregion

    #region Methods

    public void Deconstruct(out ulong extra, out UInt128M v)
    {
        extra = Extra;
        v = new UInt128M(V64, V00);
    }

    public void Deconstruct(out ulong extra, out ulong v64, out ulong v0)
    {
        extra = Extra;
        v64 = V64;
        v0 = V00;
    }

    public override bool Equals(object? obj) => obj is UInt128Extra extra && Equals(extra);

    public bool Equals(UInt128Extra other) =>
        Extra == other.Extra && V00 == other.V00 && V64 == other.V64;

    public override int GetHashCode() => HashCode.Combine(Extra, V00, V64);

    /// <summary>
    /// Shifts the 192 bits formed by concatenating <see cref="V"/> and <see cref="Extra"/> right by 64 <i>plus</i> the number of bits
    /// given in <paramref name="dist"/>, which must not be zero. This shifted value is at most 128 nonzero bits and is returned in the
    /// <see cref="V"/> field of the <see cref="UInt128Extra"/> result. The 64-bit <see cref="Extra"/> field of the result contains a value
    /// formed as follows from the bits that were shifted off: The <i>last</i> bit shifted off is the most-significant bit of the
    /// <see cref="Extra"/> field, and the other 63 bits of the <see cref="Extra"/> field are all zero if and only if <i>all but the
    /// last</i> bits shifted off were all zero.
    /// </summary>
    /// <remarks>
    /// This function makes more sense if <see cref="V"/> and <see cref="Extra"/> are considered to form an unsigned fixed-point number
    /// with binary point between <see cref="V"/> and <see cref="Extra"/>. This fixed-point value is shifted right by the number of bits
    /// given in <paramref name="dist"/>, and the integer part of this shifted value is returned in the <see cref="V"/> field of the
    /// result. The fractional part of the shifted value is modified as described above and returned in the <paramref name="extra"/> field
    /// of the result.
    /// </remarks>
    public UInt128Extra ShiftRightJam(int dist)
    {
        Debug.Assert(dist > 0, "Shift amount is out of range.");

        UInt128Extra z;
        var extra = Extra;

        var negDist = -dist;
        if (dist < 64)
        {
            z.V64 = V64 >> dist;
            z.V00 = (V64 << negDist) | (V00 >> dist);
            z.Extra = V00 << negDist;
        }
        else
        {
            z.V64 = 0;
            if (dist == 64)
            {
                z.V00 = V64;
                z.Extra = V00;
            }
            else
            {
                extra |= V00;
                if (dist < 128)
                {
                    z.V00 = V64 >> dist;
                    z.Extra = V64 << negDist;
                }
                else
                {
                    z.V00 = 0;
                    z.Extra = (dist == 128) ? V64 : (V64 != 0 ? 1UL : 0UL);
                }
            }
        }

        z.Extra |= (extra != 0) ? 1UL : 0UL;
        return z;
    }

    /// <summary>
    /// This function is the same as <see cref="ShiftRightJam(int)"/>, except that <paramref name="dist"/> must be in the range 1 to 63.
    /// </summary>
    public UInt128Extra ShortShiftRightJam(int dist)
    {
        Debug.Assert(dist is >= 1 and < 64, "Shift amount is out of range.");

        UInt128Extra z;
        (z.V64, z.V00) = V >> dist;
        z.Extra = (V00 << -dist) | (Extra != 0 ? 1UL : 0UL);
        return z;
    }

    public override string ToString() => $"0x{V64:x16}{V00:x16}:{Extra:x16}";

    public static bool operator ==(UInt128Extra left, UInt128Extra right) => left.Equals(right);

    public static bool operator !=(UInt128Extra left, UInt128Extra right) => !(left == right);

    #endregion
}
