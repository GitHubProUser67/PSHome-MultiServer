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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Tommunism.SoftFloat
{
    using static Primitives;

    // NOTE: Most spans, indexing, or enumeration in this implementation is highly dependent on the host endianness.

    [StructLayout(LayoutKind.Sequential, Pack = sizeof(ulong), Size = sizeof(ulong) * 4)]
    public struct SFUInt256 : IEquatable<SFUInt256>, IReadOnlyList<ulong>
    {
        #region Fields

        public static readonly SFUInt256 Zero = new();
        public static readonly SFUInt256 MinValue = new(0x0000000000000000, 0x0000000000000000, 0x0000000000000000, 0x0000000000000000);
        public static readonly SFUInt256 MaxValue = new(0xFFFFFFFFFFFFFFFF, 0xFFFFFFFFFFFFFFFF, 0xFFFFFFFFFFFFFFFF, 0xFFFFFFFFFFFFFFFF);

        // The meaning of these fields are dependent on the host's endianness.
        internal ulong _v0, _v1, _v2, _v3;

        #endregion

        #region Constructors

        public SFUInt256(ulong v192, ulong v128, ulong v64, ulong v0) =>
            (_v0, _v1, _v2, _v3) = BitConverter.IsLittleEndian ? (v0, v64, v128, v192) : (v192, v128, v64, v0);

        public SFUInt256(SFUInt128 v128, SFUInt128 v0)
        {
            if (BitConverter.IsLittleEndian)
            {
                (_v1, _v0) = v0;
                (_v3, _v2) = v128;
            }
            else
            {
                (_v3, _v2) = v0;
                (_v1, _v0) = v128;
            }
        }

        internal SFUInt256(ReadOnlySpan<ulong> span)
        {
            Debug.Assert(span.Length >= 4, "Span is too small.");
            (_v0, _v1, _v2, _v3) = (span[0], span[1], span[2], span[3]);
        }

        public SFUInt256(ReadOnlySpan<ulong> span, bool isLittleEndian)
        {
            Debug.Assert(span.Length >= 4, "Span is too small.");
            (_v0, _v1, _v2, _v3) = isLittleEndian
                ? (span[0], span[1], span[2], span[3])
                : (span[3], span[2], span[1], span[0]);
        }

        #endregion

        #region Properties

        // NOTE: All V{000,064,128,192}* properties are host endian-specific.

        public ulong V000
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]

            get => BitConverter.IsLittleEndian ? _v0 : _v3;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                if (BitConverter.IsLittleEndian)
                    _v0 = value;
                else
                    _v3 = value;
            }
        }

        public ulong V064
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => BitConverter.IsLittleEndian ? _v1 : _v2;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                if (BitConverter.IsLittleEndian)
                    _v1 = value;
                else
                    _v2 = value;
            }
        }

        public ulong V128
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]

            get => BitConverter.IsLittleEndian ? _v2 : _v1;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                if (BitConverter.IsLittleEndian)
                    _v2 = value;
                else
                    _v1 = value;
            }
        }

        public ulong V192
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]

            get => BitConverter.IsLittleEndian ? _v3 : _v0;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                if (BitConverter.IsLittleEndian)
                    _v3 = value;
                else
                    _v0 = value;
            }
        }

        public SFUInt128 V000_UI128
        {
            get => BitConverter.IsLittleEndian
                ? new SFUInt128(upper: _v1, lower: _v0)
                : new SFUInt128(upper: _v2, lower: _v3);

            set
            {
                if (BitConverter.IsLittleEndian)
                    (_v1, _v0) = value;
                else
                    (_v2, _v3) = value;
            }
        }

        public SFUInt128 V128_UI128
        {
            get => BitConverter.IsLittleEndian
                ? new SFUInt128(upper: _v3, lower: _v2)
                : new SFUInt128(upper: _v0, lower: _v1);

            set
            {
                if (BitConverter.IsLittleEndian)
                    (_v3, _v2) = value;
                else
                    (_v0, _v1) = value;
            }
        }

        public ulong this[int index]
        {
            get
            {
                Debug.Assert(index is >= 0 and < 4, "Index is out of range.");
                return index switch
                {
                    0 => _v0,
                    1 => _v1,
                    2 => _v2,
                    3 => _v3,
                    // easier to inline without exception
                    _ => 0 //throw new ArgumentOutOfRangeException(nameof(value))
                };
            }
            set
            {
                Debug.Assert(index is >= 0 and < 4, "Index is out of range.");
                switch (index)
                {
                    case 0: _v0 = value; return;
                    case 1: _v1 = value; return;
                    case 2: _v2 = value; return;
                    case 3: _v3 = value; return;
                }

                // easier to inline without exception
                //throw new ArgumentOutOfRangeException(nameof(value));
            }
        }

        int IReadOnlyCollection<ulong>.Count => 4;

        #endregion

        #region Methods

        // NOTE: This operation requires very careful layout of the fields and packing of this structure.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Span<ulong> AsSpan()
        {
            Debug.Assert(Unsafe.SizeOf<SFUInt256>() == sizeof(ulong) * 4);
            return MemoryMarshal.CreateSpan(ref _v0, 4);
        }

        public void Deconstruct(out ulong v192, out ulong v128, out ulong v64, out ulong v0) =>
            (v192, v128, v64, v0) = BitConverter.IsLittleEndian ? (_v3, _v2, _v1, _v0) : (_v0, _v1, _v2, _v3);

        public override bool Equals(object obj) => obj is SFUInt256 int256 && Equals(int256);

        public bool Equals(SFUInt256 other) =>
            _v0 == other._v0 && _v1 == other._v1 && _v2 == other._v2 && _v3 == other._v3;

        public IEnumerator<ulong> GetEnumerator()
        {
            yield return _v0;
            yield return _v1;
            yield return _v2;
            yield return _v3;
        }

        public override int GetHashCode() => HashCode.Combine(_v0, _v1, _v2, _v3);

        public void ToSpan(Span<ulong> span)
        {
            Debug.Assert(span.Length >= 4, "Span is too small.");
            (span[0], span[1], span[2], span[3]) = (_v0, _v1, _v2, _v3);
        }

        public override string ToString() => BitConverter.IsLittleEndian
            ? $"0x{_v3:x16}{_v2:x16}{_v1:x16}{_v0:x16}"
            : $"0x{_v0:x16}{_v1:x16}{_v2:x16}{_v3:x16}";

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public static explicit operator SFUInt256(ulong value) => new(v0: value, v64: 0, v128: 0, v192: 0);

        public static explicit operator SFUInt256(SFUInt128 value) => new(v0: value, v128: SFUInt128.Zero);

        public static explicit operator ulong(SFUInt256 value) => value.V000;

        public static explicit operator SFUInt128(SFUInt256 value) => value.V000_UI128;

        public static bool operator ==(SFUInt256 left, SFUInt256 right) => left.Equals(right);

        public static bool operator !=(SFUInt256 left, SFUInt256 right) => !(left == right);

        public static SFUInt256 operator +(SFUInt256 left, SFUInt256 right)
        {
            Span<ulong> result = stackalloc ulong[4];
            Add256M(left.AsSpan(), right.AsSpan(), result);
            return new SFUInt256(result);
        }

        public static SFUInt256 operator -(SFUInt256 left, SFUInt256 right)
        {
            Span<ulong> result = stackalloc ulong[4];
            Sub256M(left.AsSpan(), right.AsSpan(), result);
            return new SFUInt256(result);
        }

        public static SFUInt256 operator -(SFUInt256 value)
        {
            Span<ulong> result = stackalloc ulong[4];
            Sub256M(Zero.AsSpan(), value.AsSpan(), result);
            return new SFUInt256(result);
        }

        public static SFUInt256 operator >>(SFUInt256 value, int shift)
        {
            if (shift is < 0 or >= 256)
                return Zero;

            Span<ulong> src = value.AsSpan();
            Span<ulong> dst = stackalloc ulong[4];
            int wordShift = shift / 64;
            int bitShift = shift % 64;

            // shift whole words
            for (int i = 0; i < 4 - wordShift; i++)
                dst[i] = src[i + wordShift];

            // if shifting by bits too
            if (bitShift != 0)
            {
                ulong carry = 0;
                for (int i = 3; i >= 0; i--)
                {
                    ulong current = dst[i];
                    dst[i] = (current >> bitShift) | (carry << (64 - bitShift));
                    carry = current;
                }
            }

            return new SFUInt256(dst);
        }

        #endregion
    }

}