using System;
using System.Security.Cryptography;

namespace NetworkLibrary.Extension
{
    public static class MathUtils
    {
        public static Guid ToGuid(this int number)
        {
            byte[] bytes = new byte[16]; // 16 bytes for a GUID

            if (!BitConverter.IsLittleEndian)
                number = EndianTools.EndianUtils.ReverseInt(number);

            BitConverter.GetBytes(number).CopyTo(bytes, 12); // Store the int in the last 4 bytes

            return new Guid(bytes);
        }

        public static string ToUuid(this int number)
        {
            return $"00000000-00000000-00000000-{number:D8}";
        }

        public static ulong GetRandomULong()
        {
            byte[] bytes = new byte[8];
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            RandomNumberGenerator.Fill(bytes);
#else
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
                rng.GetBytes(bytes);
#endif
            return BitConverter.ToUInt64(bytes, 0);
        }
    }
}
