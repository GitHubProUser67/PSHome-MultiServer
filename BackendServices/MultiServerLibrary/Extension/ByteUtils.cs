using System;
using System.Linq;
using System.Runtime.InteropServices;
#if NETCOREAPP3_0_OR_GREATER
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;
#endif
using System.Security.Cryptography;
using System.Threading.Tasks;
using Tpm2Lib;

namespace MultiServerLibrary.Extension
{
    public static class ByteUtils
    {
        public enum CompareDirection { Forward, Backward }

        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern int memcmp(byte[] b1, byte[] b2, long count);

        // https://stackoverflow.com/questions/43289/comparing-two-byte-arrays-in-net
        /// <summary>
        /// Check if 2 byte arrays are strictly identical.
        /// <para>Savoir si 2 tableaux de bytes sont strictement identiques.</para>
        /// <param name="a">The left array.</param>
        /// <param name="b">The right array.</param>
        /// </summary>
        /// <returns>A boolean.</returns>
        public static bool EqualsTo(this byte[] a, byte[] b, CompareDirection direction = CompareDirection.Forward)
        {
            // returns when a and b are same array or both null
            if (a == b)
                return true;

            // if either is null, can't be equal
            else if (a == null || b == null)
                return false;

            int len = a.Length;

            // if different length, can't be equal
            if (len == b.Length)
            {
                if (direction == CompareDirection.Forward)
                {
                    if (Microsoft.Win32API.IsWindows)
                        // Validate buffers are the same.
                        return memcmp(a, b, len) == 0;

                    return a.SequenceEqual(b);
                }
                else
                {
                    int i = len - 1;

                    while (i >= 0 && (a[i] == b[i]))
                    {
                        i--;
                    }

                    if (i < 0)
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Finds a matching byte array within an other byte array.
        /// <para>Trouve un tableau de bytes correspondant dans un autre tableau de bytes.</para>
        /// </summary>
        /// <param name="data1">The data to search for.</param>
        /// <param name="data2">The data to search into for the data1.</param>
        /// <returns>A int (-1 if not found).</returns>
        private static int FindDataPositionInBinary(byte[] data1, byte[] data2)
        {
            if (data1 == null || data2 == null)
                return -1;

            for (int i = 0; i < data1.Length - data2.Length + 1; i++)
            {
                bool found = true;
                for (int j = 0; j < data2.Length; j++)
                {
                    if (data1[i + j] != data2[j])
                    {
                        found = false;
                        break;
                    }
                }

                if (found)
                    return i;
            }

            return -1; // data2 not found in data1
        }

        public static byte[] SubArray(this byte[] arr, int sizeToSubstract, bool atStart = false)
        {
            int newSize = arr.Length - sizeToSubstract;
            byte[] output = new byte[newSize];
            Array.Copy(arr, atStart ? sizeToSubstract : 0, output, 0, newSize);
            return output;
        }

        public static byte[] Trim(this byte[] arr)
        {
            int i = arr.Length - 1;
            while (arr[i] == 0) i--;
            byte[] data = new byte[i + 1];
            Array.Copy(arr, data, i + 1);
            return data;
        }

        public static byte[] ShadowCopy(this byte[] arr)
        {
            if (arr == null)
                return null;

            return (byte[])arr.Clone();
        }

        public static byte[] GenerateRandomBytes(ushort size)
        {
            try
            {
                using var cryptoDevice = new TbsDevice();
                cryptoDevice.Connect();
                using var tpm = new Tpm2(cryptoDevice);

                return tpm.GetRandom(size);
            }
            catch
            {
                byte[] result = new byte[size];
#if NETCOREAPP2_0_OR_GREATER
                RandomNumberGenerator.Fill(result);
#else
                using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
                    rng.GetBytes(result);
#endif
                return result;
            }
        }

        /// <summary>
        /// Combines 2 bytes array in one unique byte array.
        /// <para>Combiner 2 tableaux de bytes en un seul tableau de bytes.</para>
        /// </summary>
        /// <param name="first">The first byte array, which represents the left.</param>
        /// <param name="second">The second byte array, which represents the right.</param>
        /// <returns>A byte array.</returns>
        public static byte[] CombineByteArray(byte[] first, byte[] second)
        {
            bool isfirstNull = first == null;
            bool issecondNull = second == null;

            if (isfirstNull && issecondNull)
                return null;
            else if (issecondNull || second.Length == 0)
            {
                int sizeOfArray = first.Length;
                byte[] copy = new byte[sizeOfArray];
                Array.Copy(first, 0, copy, 0, sizeOfArray);
                return copy;
            }
            else if (isfirstNull || first.Length == 0)
            {
                int sizeOfArray = second.Length;
                byte[] copy = new byte[sizeOfArray];
                Array.Copy(second, 0, copy, 0, sizeOfArray);
                return copy;
            }

            int len1 = first.Length;
            int len2 = second.Length;

            int totalLength = len1 + len2;
#if NET6_0_OR_GREATER
            if (totalLength > Array.MaxLength || totalLength < 0)
#else
            if (totalLength > 0X7FFFFFC7 || totalLength < 0)
#endif
            {
                // Return the first array if total length exceeds limits
                int sizeOfArray = len1;
                byte[] copy = new byte[sizeOfArray];
                Array.Copy(first, 0, copy, 0, sizeOfArray);
                return copy;
            }

            int i = 0;
            int j = 0;

            byte[] resultBytes = new byte[totalLength];

            // Combine first, and second arrays
#if NETCOREAPP3_0_OR_GREATER
            unsafe
            {
                fixed (byte* src1Ptr = first, src2Ptr = second, dstPtr = resultBytes)
                {
#if NET8_0_OR_GREATER
                    if (Avx512F.IsSupported)
                    {
                        for (; i <= len1 - 64; i += 64)
                        {
                            Avx512F.Store(dstPtr + i, Avx512F.LoadVector512(src1Ptr + i));
                        }
                        for (; j <= len2 - 64; j += 64)
                        {
                            Avx512F.Store(dstPtr + len1 + j, Avx512F.LoadVector512(src2Ptr + j));
                        }
                    }
#endif
                    if (Avx.IsSupported)
                    {
                        for (; i <= len1 - 32; i += 32)
                        {
                            Avx.Store(dstPtr + i, Avx.LoadVector256(src1Ptr + i));
                        }
                        for (; j <= len2 - 32; j += 32)
                        {
                            Avx.Store(dstPtr + len1 + j, Avx.LoadVector256(src2Ptr + j));
                        }
                    }
                    if (Sse2.IsSupported)
                    {
                        for (; i <= len1 - 16; i += 16)
                        {
                            Sse2.Store(dstPtr + i, Sse2.LoadVector128(src1Ptr + i));
                        }
                        for (; j <= len2 - 16; j += 16)
                        {
                            Sse2.Store(dstPtr + len1 + j, Sse2.LoadVector128(src2Ptr + j));
                        }
                    }
                    if (AdvSimd.IsSupported)
                    {
                        for (; i <= len1 - 16; i += 16)
                        {
                            AdvSimd.Store(dstPtr + i, AdvSimd.LoadVector128(src1Ptr + i));
                        }
                        for (; j <= len2 - 16; j += 16)
                        {
                            AdvSimd.Store(dstPtr + len1 + j, AdvSimd.LoadVector128(src2Ptr + j));
                        }
                    }
                }
            }
#endif
            if (i < len1)
                Array.Copy(first, i, resultBytes, i, len1 - i);
            if (j < len2)
                Array.Copy(second, j, resultBytes, len1 + j, len2 - j);

            return resultBytes;
        }

        /// <summary>
        /// Combines 3 bytes arrays into one unique byte array.
        /// <para>Combine 3 tableaux de bytes en un seul tableau de bytes.</para>
        /// </summary>
        /// <param name="first">The first byte array, which represents the leftmost part.</param>
        /// <param name="second">The second byte array, which represents the middle part.</param>
        /// <param name="third">The third byte array, which represents the rightmost part.</param>
        /// <returns>A byte array.</returns>
        public static byte[] CombineByteArrays(byte[] first, byte[] second, byte[] third)
        {
            bool isfirstNull = first == null;
            bool issecondNull = second == null;
            bool isthirdNull = third == null;

            if (isfirstNull && issecondNull && isthirdNull)
                return null;
            else if (issecondNull && isthirdNull)
            {
                int sizeOfArray = first.Length;
                byte[] copy = new byte[sizeOfArray];
                Array.Copy(first, 0, copy, 0, sizeOfArray);
                return copy;
            }
            else if (isthirdNull && isfirstNull)
            {
                int sizeOfArray = second.Length;
                byte[] copy = new byte[sizeOfArray];
                Array.Copy(second, 0, copy, 0, sizeOfArray);
                return copy;
            }
            else if (isfirstNull && issecondNull)
            {
                int sizeOfArray = third.Length;
                byte[] copy = new byte[sizeOfArray];
                Array.Copy(third, 0, copy, 0, sizeOfArray);
                return copy;
            }
            else if (isfirstNull || first.Length == 0)
                return CombineByteArray(second, third);
            else if (issecondNull || second.Length == 0)
                return CombineByteArray(first, third);
            else if (isthirdNull || third.Length == 0)
                return CombineByteArray(first, second);

            int len1 = first.Length;
            int len2 = second.Length;
            int len3 = third.Length;

            int totalLength = len1 + len2 + len3;

#if NET6_0_OR_GREATER
            if (totalLength > Array.MaxLength || totalLength < 0)
#else
            if (totalLength > 0X7FFFFFC7 || totalLength < 0)
#endif
            {
                // Return the first array if total length exceeds limits
                int sizeOfArray = len1;
                byte[] copy = new byte[sizeOfArray];
                Array.Copy(first, 0, copy, 0, sizeOfArray);
                return copy;
            }

            int i = 0;
            int j = 0;
            int k = 0;

            byte[] resultBytes = new byte[totalLength];

            // Combine first, second, and third arrays
#if NETCOREAPP3_0_OR_GREATER
            unsafe
            {
                fixed (byte* src1Ptr = first, src2Ptr = second, src3Ptr = third, dstPtr = resultBytes)
                {
#if NET8_0_OR_GREATER
                    if (Avx512F.IsSupported)
                    {
                        for (; i <= len1 - 64; i += 64)
                        {
                            Avx512F.Store(dstPtr + i, Avx512F.LoadVector512(src1Ptr + i));
                        }
                        for (; j <= len2 - 64; j += 64)
                        {
                            Avx512F.Store(dstPtr + len1 + j, Avx512F.LoadVector512(src2Ptr + j));
                        }
                        for (; k <= len3 - 64; k += 64)
                        {
                            Avx512F.Store(dstPtr + len1 + len2 + k, Avx512F.LoadVector512(src3Ptr + k));
                        }
                    }
#endif
                    if (Avx.IsSupported)
                    {
                        for (; i <= len1 - 32; i += 32)
                        {
                            Avx.Store(dstPtr + i, Avx.LoadVector256(src1Ptr + i));
                        }
                        for (; j <= len2 - 32; j += 32)
                        {
                            Avx.Store(dstPtr + len1 + j, Avx.LoadVector256(src2Ptr + j));
                        }
                        for (; k <= len3 - 32; k += 32)
                        {
                            Avx.Store(dstPtr + len1 + len2 + k, Avx.LoadVector256(src3Ptr + k));
                        }
                    }
                    if (Sse2.IsSupported)
                    {
                        for (; i <= len1 - 16; i += 16)
                        {
                            Sse2.Store(dstPtr + i, Sse2.LoadVector128(src1Ptr + i));
                        }
                        for (; j <= len2 - 16; j += 16)
                        {
                            Sse2.Store(dstPtr + len1 + j, Sse2.LoadVector128(src2Ptr + j));
                        }
                        for (; k <= len3 - 16; k += 16)
                        {
                            Sse2.Store(dstPtr + len1 + len2 + k, Sse2.LoadVector128(src3Ptr + k));
                        }
                    }
                    if (AdvSimd.IsSupported)
                    {
                        for (; i <= len1 - 16; i += 16)
                        {
                            AdvSimd.Store(dstPtr + i, AdvSimd.LoadVector128(src1Ptr + i));
                        }
                        for (; j <= len2 - 16; j += 16)
                        {
                            AdvSimd.Store(dstPtr + len1 + j, AdvSimd.LoadVector128(src2Ptr + j));
                        }
                        for (; k <= len3 - 16; k += 16)
                        {
                            AdvSimd.Store(dstPtr + len1 + len2 + k, AdvSimd.LoadVector128(src3Ptr + k));
                        }
                    }
                }
            }
#endif
            if (i < len1)
                Array.Copy(first, i, resultBytes, i, len1 - i);
            if (j < len2)
                Array.Copy(second, j, resultBytes, len1 + j, len2 - j);
            if (k < len3)
                Array.Copy(third, k, resultBytes, len1 + len2 + k, len3 - k);

            return resultBytes;
        }

        /// <summary>
        /// Combines a byte array with an array of byte array to a unique byte array.
        /// <para>Combiner un tableau de bytes avec un tableau de tableaux de bytes en un seul tableau de bytes.</para>
        /// </summary>
        /// <param name="first">The first byte array, which represents the left.</param>
        /// <param name="second">The array of byte array, which represents the right.</param>
        /// <returns>A byte array.</returns>
        public static byte[] CombineByteArrays(byte[] first, byte[][] second)
        {
            if (second == null || second.Length == 0)
            {
                int sizeOfArray = first.Length;
                byte[] copy = new byte[sizeOfArray];
                Array.Copy(first, 0, copy, 0, sizeOfArray);
                return copy;
            }

            int firstLength = first?.Length ?? 0;
            int totalLength = firstLength + second.Sum(arr => arr.Length);
#if NET6_0_OR_GREATER
            if (totalLength > Array.MaxLength || totalLength < 0)
#else
            if (totalLength > 0X7FFFFFC7 || totalLength < 0)
#endif
            {
                // Return the first array if total length exceeds limits
                int sizeOfArray = first.Length;
                byte[] copy = new byte[sizeOfArray];
                Array.Copy(first, 0, copy, 0, sizeOfArray);
                return copy;
            }

            byte[] resultBytes = new byte[totalLength];

            if (first != null)
                Array.Copy(first, 0, resultBytes, 0, first.Length);

            // Calculate offsets for each array in `second` before the parallel operation.
            int[] offsets = new int[second.Length];
            int currentOffset = firstLength;

            for (int i = 0; i < second.Length; i++)
            {
                offsets[i] = currentOffset;
                currentOffset += second[i].Length;
            }

            Parallel.ForEach(Enumerable.Range(0, second.Length), new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, i =>
            {
                Array.Copy(second[i], 0, resultBytes, offsets[i], second[i].Length);
            });

            return resultBytes;
        }

        /// <summary>
        /// Finds a sequence of bytes within a byte array.
        /// <para>Trouve une séquence de bytes dans un tableau de bytes.</para>
        /// </summary>
        /// <param name="buffer">The array in which we search for the sequence.</param>
        /// <param name="searchPattern">The byte array sequence to find.</param>
        /// <param name="offset">The offset from where we start our research.</param>
        /// <param name="maxOffset">The maximum offset from where we do our research.</param>
        /// <returns>A int (-1 if not found).</returns>
        public static int FindBytePattern(byte[] buffer, byte[] searchPattern, int offset = 0, int maxOffset = -1)
        {
            int found = -1;

            if (buffer.Length == 0 || searchPattern.Length == 0 || offset < 0 || offset > buffer.Length - searchPattern.Length)
                return -1;

            int searchPatternSize = searchPattern.Length;

            // Calculate end boundary
            if (maxOffset == -1 || maxOffset > buffer.Length - searchPatternSize)
                maxOffset = buffer.Length - searchPatternSize;

            for (int i = offset; i <= maxOffset; i++)
            {
                if (buffer[i] == searchPattern[0])
                {
                    bool matched = true;

                    for (int y = 1; y < searchPatternSize; y++)
                    {
                        if (buffer[i + y] != searchPattern[y])
                        {
                            matched = false;
                            break;
                        }
                    }

                    if (matched)
                    {
                        found = i;
                        break;
                    }
                }
            }

            return found;
        }


        /// <summary>
        /// Finds a sequence of bytes within a byte array.
        /// <para>Trouve une séquence de bytes dans un tableau de bytes.</para>
        /// </summary>
        /// <param name="buffer">The Span byte in which we search for the sequence.</param>
        /// <param name="searchPattern">The Span byte sequence to find.</param>
        /// <param name="offset">The offset from where we start our research.</param>
        /// <param name="maxOffset">The maximum offset from where we do our research.</param>
        /// <returns>A int (-1 if not found).</returns>
        public static int FindBytePattern(ReadOnlySpan<byte> buffer, ReadOnlySpan<byte> searchPattern, int offset = 0, int maxOffset = -1)
        {
            if (searchPattern.IsEmpty || buffer.Length < searchPattern.Length || offset < 0 || offset > buffer.Length - searchPattern.Length)
                return -1;

            int searchPatternSize = searchPattern.Length;

            // Calculate end boundary
            if (maxOffset == -1 || maxOffset > buffer.Length - searchPatternSize)
                maxOffset = buffer.Length - searchPatternSize;

            for (int i = offset; i <= maxOffset; i++)
            {
                if (buffer[i] == searchPattern[0] && buffer.Slice(i, searchPatternSize).SequenceEqual(searchPattern))
                    return i;
            }

            return -1;
        }
    }
}
