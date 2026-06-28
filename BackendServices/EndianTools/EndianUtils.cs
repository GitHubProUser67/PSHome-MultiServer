using System.Buffers.Binary;

namespace EndianTools
{
    public static class EndianUtils
    {
        /// <summary>
        /// Reverse the endianess of a given byte array by 4 bytes chunck.
        /// <para>change l'endianess d'un tableau de bytes par blocs 4.</para>
        /// </summary>
        /// <param name="dataIn">The byte array to endian-swap.</param>
        /// <returns>A byte array.</returns>
        public static byte[] EndianSwap(byte[] dataIn)
        {
            if (dataIn == null)
                return null;

            const byte chunkSize = 4;

            var inputLength = dataIn.Length;

            if (inputLength <= chunkSize)
                return ReverseArray(dataIn);

            var reversedArray = new byte[inputLength];
            ReadOnlySpan<byte> inputSpan = dataIn;
            Span<byte> outputSpan = reversedArray;

            var i = 0;

            while (i + chunkSize <= inputLength)
            {
                BitConverter.TryWriteBytes(
                    outputSpan.Slice(i, chunkSize),
                    ReverseUint(
                        BitConverter.ToUInt32(inputSpan.Slice(i, chunkSize))
                    )
                );
                i += chunkSize;
            }

            // Handle remaining bytes
            var remaining = inputLength - i;
            if (remaining > 0)
            {
                for (var j = 0; j < remaining; j++)
                    reversedArray[i + j] = inputSpan[inputLength - j - 1];
            }
            return reversedArray;
        }

        /// <summary>
        /// Reverse the endianess of a given byte array.
        /// <para>change l'endianess d'un tableau de bytes.</para>
        /// </summary>
        /// <param name="dataIn">The byte array to endian-swap.</param>
        /// <returns>A byte array.</returns>
        public static byte[] ReverseArray(this byte[] dataIn)
        {
            if (dataIn == null)
                return null;

            // Clone the input array to avoid modifying the original array
            var reversedArray = (byte[])dataIn.Clone();
            Array.Reverse(reversedArray);
            return reversedArray;
        }

        /// <summary>
        /// Reverse the endianess of a given char.
        /// <para>change l'endianess d'un char.</para>
        /// </summary>
        /// <param name="dataIn">The char to endian-swap.</param>
        /// <returns>A char.</returns>
        public static char ReverseChar(char dataIn)
        {
            var bytes = BitConverter.GetBytes(dataIn);
            Array.Reverse(bytes);
            return BitConverter.ToChar(bytes, 0);
        }

        /// <summary>
        /// Reverse the endianess of a given int.
        /// <para>change l'endianess d'un int.</para>
        /// </summary>
        /// <param name="dataIn">The int to endian-swap.</param>
        /// <returns>A int.</returns>
        public static int ReverseInt(int dataIn)
        {
            return BinaryPrimitives.ReverseEndianness(dataIn);
        }

        /// <summary>
        /// Reverse the endianess of a given uint.
        /// <para>change l'endianess d'un uint.</para>
        /// </summary>
        /// <param name="dataIn">The uint to endian-swap.</param>
        /// <returns>A uint.</returns>
        public static uint ReverseUint(uint dataIn)
        {
            return BinaryPrimitives.ReverseEndianness(dataIn);
        }

        /// <summary>
        /// Reverse the endianess of a given long.
        /// <para>change l'endianess d'un long.</para>
        /// </summary>
        /// <param name="dataIn">The long to endian-swap.</param>
        /// <returns>A long.</returns>
        public static long ReverseLong(long dataIn)
        {
            return BinaryPrimitives.ReverseEndianness(dataIn);
        }

        /// <summary>
        /// Reverse the endianess of a given ulong.
        /// <para>change l'endianess d'un ulong.</para>
        /// </summary>
        /// <param name="dataIn">The ulong to endian-swap.</param>
        /// <returns>A ulong.</returns>
        public static ulong ReverseUlong(ulong dataIn)
        {
            return BinaryPrimitives.ReverseEndianness(dataIn);
        }

        /// <summary>
        /// Reverse the endianess of a given double.
        /// <para>change l'endianess d'un double.</para>
        /// </summary>
        /// <param name="dataIn">The double to endian-swap.</param>
        /// <returns>A double.</returns>
        public static double ReverseDouble(double dataIn)
        {
            var bytes = BitConverter.GetBytes(dataIn);
            Array.Reverse(bytes);
            return BitConverter.ToDouble(bytes, 0);
        }

        /// <summary>
        /// Reverse the endianess of a given float.
        /// <para>change l'endianess d'un float.</para>
        /// </summary>
        /// <param name="dataIn">The float to endian-swap.</param>
        /// <returns>A float.</returns>
        public static float ReverseFloat(float dataIn)
        {
            var bytes = BitConverter.GetBytes(dataIn);
            Array.Reverse(bytes);
            return BitConverter.ToSingle(bytes, 0);
        }

        /// <summary>
        /// Reverse the endianess of a given short.
        /// <para>change l'endianess d'un short.</para>
        /// </summary>
        /// <param name="dataIn">The short to endian-swap.</param>
        /// <returns>A short.</returns>
        public static short ReverseShort(short dataIn)
        {
            return BinaryPrimitives.ReverseEndianness(dataIn);
        }

        /// <summary>
        /// Reverse the endianess of a given ushort.
        /// <para>change l'endianess d'un ushort.</para>
        /// </summary>
        /// <param name="dataIn">The ushort to endian-swap.</param>
        /// <returns>A ushort.</returns>
        public static ushort ReverseUshort(ushort dataIn)
        {
            return BinaryPrimitives.ReverseEndianness(dataIn);
        }
    }
}
