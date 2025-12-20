using System;
using System.Runtime.CompilerServices;

namespace SpaceWizards.HttpListener
{
    internal static class StreamHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ValidateBufferArguments(byte[] buffer, int offset, int count)
        {
            if (buffer is null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), "Argument cannot be negative");
            }

            if ((uint)count > buffer.Length - offset)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), "Invalid offset length");
            }
        }
    }
}
