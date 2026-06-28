// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Text
{
    internal ref partial struct ValueStringBuilder
    {
        private char[]? _arrayToReturnToPool;
        private int _pos;
        private bool _canGrow;

        public ValueStringBuilder(Span<char> initialBuffer, bool canGrow = true)
        {
            _arrayToReturnToPool = null;
            RawChars = initialBuffer;
            _pos = 0;
            _canGrow = canGrow;
        }

        public ValueStringBuilder(int initialCapacity, bool canGrow = true)
        {
            _arrayToReturnToPool = ArrayPool<char>.Shared.Rent(initialCapacity);
            RawChars = _arrayToReturnToPool;
            _pos = 0;
            _canGrow = canGrow;
        }

        public int Length
        {
            get => _pos;
            set
            {
                Debug.Assert(value >= 0);
                Debug.Assert(value <= RawChars.Length);
                _pos = value;
            }
        }

        public int Capacity => RawChars.Length;

        public void EnsureCapacity(int capacity)
        {
            // This is not expected to be called this with negative capacity
            Debug.Assert(capacity >= 0);

            // If the caller has a bug and calls this with negative capacity, make sure to call Grow to throw an exception.
            if ((uint)capacity > (uint)RawChars.Length)
                Grow(capacity - _pos);
        }

        /// <summary>
        /// Get a pinnable reference to the builder.
        /// Does not ensure there is a null char after <see cref="Length"/>
        /// This overload is pattern matched in the C# 7.3+ compiler so you can omit
        /// the explicit method call, and write eg "fixed (char* c = builder)"
        /// </summary>
        public ref char GetPinnableReference()
        {
            return ref MemoryMarshal.GetReference(RawChars);
        }

        /// <summary>
        /// Get a pinnable reference to the builder.
        /// </summary>
        /// <param name="terminate">Ensures that the builder has a null char after <see cref="Length"/></param>
        public ref char GetPinnableReference(bool terminate)
        {
            if (terminate)
            {
                EnsureCapacity(Length + 1);
                RawChars[Length] = '\0';
            }
            return ref MemoryMarshal.GetReference(RawChars);
        }

        public ref char this[int index]
        {
            get
            {
                Debug.Assert(index < _pos);
                return ref RawChars[index];
            }
        }

        public override string ToString()
        {
            var s = RawChars.Slice(0, _pos).ToString();
            Dispose();
            return s;
        }

        /// <summary>Returns the underlying storage of the builder.</summary>
        public Span<char> RawChars { get; private set; }

        /// <summary>
        /// Returns a span around the contents of the builder.
        /// </summary>
        /// <param name="terminate">Ensures that the builder has a null char after <see cref="Length"/></param>
        public ReadOnlySpan<char> AsSpan(bool terminate)
        {
            if (terminate)
            {
                EnsureCapacity(Length + 1);
                RawChars[Length] = '\0';
            }
            return RawChars.Slice(0, _pos);
        }

        public ReadOnlySpan<char> AsSpan() => RawChars.Slice(0, _pos);

        public ReadOnlySpan<char> AsSpan(int start) => RawChars.Slice(start, _pos - start);

        public ReadOnlySpan<char> AsSpan(int start, int length) => RawChars.Slice(start, length);

        public bool TryCopyTo(Span<char> destination, out int charsWritten)
        {
            if (RawChars.Slice(0, _pos).TryCopyTo(destination))
            {
                charsWritten = _pos;
                Dispose();
                return true;
            }
            else
            {
                charsWritten = 0;
                Dispose();
                return false;
            }
        }

        public void Insert(int index, char value, int count)
        {
            if (_pos > RawChars.Length - count)
            {
                Grow(count);
            }

            var remaining = _pos - index;
            RawChars.Slice(index, remaining).CopyTo(RawChars.Slice(index + count));
            RawChars.Slice(index, count).Fill(value);
            _pos += count;
        }

        public void Insert(int index, string? s)
        {
            if (s == null)
            {
                return;
            }

            var count = s.Length;

            if (_pos > (RawChars.Length - count))
            {
                Grow(count);
            }

            var remaining = _pos - index;
            RawChars.Slice(index, remaining).CopyTo(RawChars.Slice(index + count));
            s.CopyTo(RawChars.Slice(index));
            _pos += count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Append(char c)
        {
            var pos = _pos;
            if ((uint)pos < (uint)RawChars.Length)
            {
                RawChars[pos] = c;
                _pos = pos + 1;
            }
            else
            {
                GrowAndAppend(c);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Append(string? s)
        {
            if (s == null)
            {
                return;
            }

            var pos = _pos;
            if (s.Length == 1 && (uint)pos < (uint)RawChars.Length) // very common case, e.g. appending strings from NumberFormatInfo like separators, percent symbols, etc.
            {
                RawChars[pos] = s[0];
                _pos = pos + 1;
            }
            else
            {
                AppendSlow(s);
            }
        }

        private void AppendSlow(string s)
        {
            var pos = _pos;
            if (pos > RawChars.Length - s.Length)
            {
                Grow(s.Length);
            }

            s.CopyTo(RawChars.Slice(pos));
            _pos += s.Length;
        }

        public void Append(char c, int count)
        {
            if (_pos > RawChars.Length - count)
            {
                Grow(count);
            }

            var dst = RawChars.Slice(_pos, count);
            for (var i = 0; i < dst.Length; i++)
            {
                dst[i] = c;
            }
            _pos += count;
        }

        public void Append(ReadOnlySpan<char> value)
        {
            var pos = _pos;
            if (pos > RawChars.Length - value.Length)
            {
                Grow(value.Length);
            }

            value.CopyTo(RawChars.Slice(_pos));
            _pos += value.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<char> AppendSpan(int length)
        {
            var origPos = _pos;
            if (origPos > RawChars.Length - length)
            {
                Grow(length);
            }

            _pos = origPos + length;
            return RawChars.Slice(origPos, length);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void GrowAndAppend(char c)
        {
            Grow(1);
            Append(c);
        }

        /// <summary>
        /// Resize the internal buffer either by doubling current buffer size or
        /// by adding <paramref name="additionalCapacityBeyondPos"/> to
        /// <see cref="_pos"/> whichever is greater.
        /// </summary>
        /// <param name="additionalCapacityBeyondPos">
        /// Number of chars requested beyond current position.
        /// </param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        private void Grow(int additionalCapacityBeyondPos)
        {
            Debug.Assert(additionalCapacityBeyondPos > 0);
            Debug.Assert(
                _pos > RawChars.Length - additionalCapacityBeyondPos,
                "Grow called incorrectly, no resize is needed."
            );

            // Is this instance allowed to grow into a new, larger, buffer?
            if (!_canGrow)
                throw new FormatException($"{nameof(ValueStringBuilder)} is not allowed to grow.");

            // Increase to at least the required size (_pos + additionalCapacityBeyondPos), but try
            // to double the size if possible, bounding the doubling to not go beyond the max array length.
            var newCapacity = (int)
                Math.Max(
                    (uint)(_pos + additionalCapacityBeyondPos),
                    Math.Min((uint)RawChars.Length * 2, Array.MaxLength)
                );

            // Make sure to let Rent throw an exception if the caller has a bug and the desired capacity is negative.
            // This could also go negative if the actual required length wraps around.
            var poolArray = ArrayPool<char>.Shared.Rent(newCapacity);

            RawChars.Slice(0, _pos).CopyTo(poolArray);

            var toReturn = _arrayToReturnToPool;
            RawChars = _arrayToReturnToPool = poolArray;
            if (toReturn != null)
            {
                ArrayPool<char>.Shared.Return(toReturn);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            var toReturn = _arrayToReturnToPool;

            // for safety, to avoid using pooled array if this instance is erroneously appended to again
            _arrayToReturnToPool = null;
            RawChars = default;
            _pos = 0;
            _canGrow = false;

            if (toReturn != null)
            {
                ArrayPool<char>.Shared.Return(toReturn);
            }
        }
    }
}
