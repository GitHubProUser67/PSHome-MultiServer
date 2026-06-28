using System;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;

namespace Org.BouncyCastle.Runtime.Intrinsics
{
    internal static class Vector
    {
        internal static bool IsPacked =>
            Unsafe.SizeOf<Vector64<byte>>() == 8
            && Unsafe.SizeOf<Vector128<byte>>() == 16
            && Unsafe.SizeOf<Vector256<byte>>() == 32;

        internal static bool IsPackedLittleEndian =>
            IsPacked && EndianTools.EndianAwareConverter.isLittleEndianSystem;
    }
}
