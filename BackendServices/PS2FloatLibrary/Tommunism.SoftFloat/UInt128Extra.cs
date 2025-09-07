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

namespace Tommunism.SoftFloat
{
    internal struct UInt128Extra : IEquatable<UInt128Extra>
    {
        public ulong Extra;
        public SFUInt128 V;

        public UInt128Extra(ulong extra, SFUInt128 v)
        {
            Extra = extra;
            V = v;
        }

        public UInt128Extra(ulong extra, ulong v0, ulong v64)
        {
            Extra = extra;
            V = new SFUInt128(upper: v64, lower: v0);
        }

        public void Deconstruct(out ulong extra, out SFUInt128 v)
        {
            extra = Extra;
            v = V;
        }

        public void Deconstruct(out ulong extra, out ulong v64, out ulong v0)
        {
            extra = Extra;
            V.Deconstruct(v64: out v64, v0: out v0);
        }

        public override bool Equals(object obj) => obj is UInt128Extra extra && Equals(extra);

        public bool Equals(UInt128Extra other) => Extra == other.Extra && V.Equals(other.V);

        public override int GetHashCode() => HashCode.Combine(Extra, V.V00, V.V64);

        public override string ToString() => $"0x{V.V64:x16}{V.V00:x16}:{Extra:x16}";

        public static bool operator ==(UInt128Extra left, UInt128Extra right) => left.Equals(right);

        public static bool operator !=(UInt128Extra left, UInt128Extra right) => !(left == right);
    }

}