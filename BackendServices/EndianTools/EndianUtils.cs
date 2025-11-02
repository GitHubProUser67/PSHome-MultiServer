using System;
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
using System.Buffers.Binary;
#endif

namespace EndianTools
{
    public class EndianUtils
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

            int inputLength = dataIn.Length;

            if (inputLength <= chunkSize)
                return ReverseArray(dataIn);

            byte[] reversedArray = new byte[inputLength];
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            byte chunkSize64 = chunkSize << 1;
            byte hilo64 = chunkSize << 3;

            ReadOnlySpan<byte> inputSpan = dataIn;
            Span<byte> outputSpan = reversedArray;

            int i = 0;

            while (i + chunkSize64 <= inputLength)
            {
                ulong word64 = BitConverter.ToUInt64(inputSpan.Slice(i, chunkSize64));
                BitConverter.TryWriteBytes(outputSpan.Slice(i, chunkSize64), ((ulong)BinaryPrimitives.ReverseEndianness((uint)(word64 & uint.MaxValue)) << hilo64) | (BinaryPrimitives.ReverseEndianness((uint)(word64 >> hilo64))));
                i += chunkSize64;
            }

            while (i + chunkSize <= inputLength)
            {
                uint val = BitConverter.ToUInt32(inputSpan.Slice(i, chunkSize));
                val = BinaryPrimitives.ReverseEndianness(val);
                BitConverter.TryWriteBytes(outputSpan.Slice(i, chunkSize), val);
                i += chunkSize;
            }

            // Handle remaining bytes
            int remaining = inputLength - i;
            if (remaining > 0)
            {
                for (int j = 0; j < remaining; j++)
                    reversedArray[i + j] = inputSpan[inputLength - j - 1];
            }
#else
            Array.Copy(dataIn, reversedArray, inputLength);

            int numofBytes;

            for (int i = 0; i < inputLength; i += numofBytes)
            {
                numofBytes = chunkSize;
                int remainingBytes = inputLength - i;
                if (remainingBytes < chunkSize)
                    numofBytes = remainingBytes;
                Array.Reverse(reversedArray, i, numofBytes);
            }
#endif
            return reversedArray;
        }

        [Obsolete("EndianSwap2 is a hack, never use it.")]
        public static byte[] EndianSwap2(byte[] dataIn)
        {
            if (dataIn == null)
                return null;
            else if (dataIn.Length % 4 != 0)
                throw new ArgumentException("[EndianUtils] - EndianSwap2: Array length must be a multiple of 4.");

            int inputLength = dataIn.Length;

            byte[] reversedArray = new byte[inputLength];
            Array.Copy(dataIn, reversedArray, inputLength);

            for (int i = 0; i < inputLength; i += 4)
            {
                // Swap bytes in positions [i] <-> [i+2], and [i+1] <-> [i+3]
                (reversedArray[i], reversedArray[i + 2]) = (reversedArray[i + 2], reversedArray[i]);
                (reversedArray[i + 1], reversedArray[i + 3]) = (reversedArray[i + 3], reversedArray[i + 1]);
            }

            return reversedArray;
        }

        /// <summary>
        /// Reverse the endianess of a given byte array.
        /// <para>change l'endianess d'un tableau de bytes.</para>
        /// </summary>
        /// <param name="dataIn">The byte array to endian-swap.</param>
        /// <returns>A byte array.</returns>
        public static byte[] ReverseArray(byte[] dataIn)
        {
            if (dataIn == null)
                return null;
			
            // Clone the input array to avoid modifying the original array
            byte[] reversedArray = (byte[])dataIn.Clone();
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
            byte[] bytes = BitConverter.GetBytes(dataIn);
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
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            return BinaryPrimitives.ReverseEndianness(dataIn);
#else
            byte[] bytes = BitConverter.GetBytes(dataIn);
            Array.Reverse(bytes);
            return BitConverter.ToInt32(bytes, 0);
#endif
        }

        /// <summary>
        /// Reverse the endianess of a given uint.
        /// <para>change l'endianess d'un uint.</para>
        /// </summary>
        /// <param name="dataIn">The uint to endian-swap.</param>
        /// <returns>A uint.</returns>
        public static uint ReverseUint(uint dataIn)
        {
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            return BinaryPrimitives.ReverseEndianness(dataIn);
#else
            return ((dataIn & 0x000000ff) << 24) +
                   ((dataIn & 0x0000ff00) << 8) +
                   ((dataIn & 0x00ff0000) >> 8) +
                   ((dataIn & 0xff000000) >> 24);
#endif
        }

        /// <summary>
        /// Reverse the endianess of a given long.
        /// <para>change l'endianess d'un long.</para>
        /// </summary>
        /// <param name="dataIn">The long to endian-swap.</param>
        /// <returns>A long.</returns>
        public static long ReverseLong(long dataIn)
        {
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            return BinaryPrimitives.ReverseEndianness(dataIn);
#else
            byte[] bytes = BitConverter.GetBytes(dataIn);
            Array.Reverse(bytes);
            return BitConverter.ToInt64(bytes, 0);
#endif
        }

        /// <summary>
        /// Reverse the endianess of a given ulong.
        /// <para>change l'endianess d'un ulong.</para>
        /// </summary>
        /// <param name="dataIn">The ulong to endian-swap.</param>
        /// <returns>A ulong.</returns>
        public static ulong ReverseUlong(ulong dataIn)
        {
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            return BinaryPrimitives.ReverseEndianness(dataIn);
#else
            return (0x00000000000000FF) & (dataIn >> 56)
                 | (0x000000000000FF00) & (dataIn >> 40)
                 | (0x0000000000FF0000) & (dataIn >> 24)
                 | (0x00000000FF000000) & (dataIn >> 8)
                 | (0x000000FF00000000) & (dataIn << 8)
                 | (0x0000FF0000000000) & (dataIn << 24)
                 | (0x00FF000000000000) & (dataIn << 40)
                 | (0xFF00000000000000) & (dataIn << 56);
#endif
        }

        /// <summary>
        /// Reverse the endianess of a given double.
        /// <para>change l'endianess d'un double.</para>
        /// </summary>
        /// <param name="dataIn">The double to endian-swap.</param>
        /// <returns>A double.</returns>
        public static double ReverseDouble(double dataIn)
        {
            byte[] bytes = BitConverter.GetBytes(dataIn);
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
            byte[] bytes = BitConverter.GetBytes(dataIn);
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
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            return BinaryPrimitives.ReverseEndianness(dataIn);
#else
            byte[] bytes = BitConverter.GetBytes(dataIn);
            Array.Reverse(bytes);
            return BitConverter.ToInt16(bytes, 0);
#endif
        }

        /// <summary>
        /// Reverse the endianess of a given ushort.
        /// <para>change l'endianess d'un ushort.</para>
        /// </summary>
        /// <param name="dataIn">The ushort to endian-swap.</param>
        /// <returns>A ushort.</returns>
        public static ushort ReverseUshort(ushort dataIn)
        {
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            return BinaryPrimitives.ReverseEndianness(dataIn);
#else
            // Use bitwise operations to swap the bytes
            return (ushort)((ushort)((dataIn & byte.MaxValue) << 8) | ((dataIn >> 8) & byte.MaxValue));
#endif
        }
    }
}
