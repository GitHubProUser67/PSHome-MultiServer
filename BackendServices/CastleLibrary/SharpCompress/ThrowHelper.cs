using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace CastleLibrary.SharpCompress
{
    internal static class ThrowHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfNull([NotNull] object? argument, string? paramName = null)
        {
            if (argument is null)
                throw new ArgumentNullException(paramName);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfNegative(int value, string? paramName = null)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(paramName);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfGreaterThan(uint value, uint other, string? paramName = null)
        {
            if (value > other)
                throw new ArgumentOutOfRangeException(paramName);
        }
    }
}
