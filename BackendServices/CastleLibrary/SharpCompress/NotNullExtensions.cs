using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace CastleLibrary.SharpCompress
{
    internal static class NotNullExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T NotNull<T>(
            [NotNull] this T? obj,
            [CallerArgumentExpression(nameof(obj))] string? paramName = null
        )
            where T : class
        {
            ThrowHelper.ThrowIfNull(obj, paramName);
            return obj;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T NotNull<T>(
            [NotNull] this T? obj,
            [CallerArgumentExpression(nameof(obj))] string? paramName = null
        )
            where T : struct
        {
            if (!obj.HasValue)
            {
                throw new ArgumentNullException(paramName);
            }

            return obj.Value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string NotNullOrEmpty(this string obj, string name)
        {
            obj.NotNull(name);
            if (obj.Length == 0)
                throw new ArgumentException("String is empty.", name);
            return obj;
        }
    }
}
