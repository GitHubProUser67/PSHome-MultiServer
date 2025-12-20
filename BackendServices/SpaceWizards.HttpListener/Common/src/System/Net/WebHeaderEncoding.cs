// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;
using System.Text;

namespace System.Net
{
    // we use this static class as a helper class to encode/decode HTTP headers.
    // what we need is a 1-1 correspondence between a char in the range U+0000-U+00FF
    // and a byte in the range 0x00-0xFF (which is the range that can hit the network).
    // The Latin-1 encoding (ISO-88591-1) (GetEncoding(28591)) works for byte[] to string, but is a little slow.
    // It doesn't work for string -> byte[] because of best-fit-mapping problems.
    internal static class WebHeaderEncoding
    {
        internal static unsafe string GetString(byte[] bytes, int byteIndex, int byteCount)
        {
            if (byteCount < 1)
            {
                return string.Empty;
            }

            Debug.Assert(bytes != null && (uint)byteIndex <= (uint)bytes.Length && (uint)(byteIndex + byteCount) <= (uint)bytes.Length);

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
            return string.Create(byteCount, (bytes, byteIndex), (buffer, state) =>
            {
                fixed (byte* pByt = &state.bytes[state.byteIndex])
                fixed (char* pStr = buffer)
                {
                    byte* pBytes = pByt;
                    char* pString = pStr;
                    int byteCount = buffer.Length;

                    while (byteCount >= 8)
                    {
                        pString[0] = (char)pBytes[0];
                        pString[1] = (char)pBytes[1];
                        pString[2] = (char)pBytes[2];
                        pString[3] = (char)pBytes[3];
                        pString[4] = (char)pBytes[4];
                        pString[5] = (char)pBytes[5];
                        pString[6] = (char)pBytes[6];
                        pString[7] = (char)pBytes[7];
                        pString += 8;
                        pBytes += 8;
                        byteCount -= 8;
                    }
                    for (int i = 0; i < byteCount; i++)
                    {
                        pString[i] = (char)pBytes[i];
                    }
                }
            });
#else
            // Allocate a string with the desired length
            string result = new string('\0', byteCount);

            fixed (byte* pBytes = &bytes[byteIndex])
            fixed (char* pStr = result)
            {
                byte* pSource = pBytes;
                char* pDest = pStr;
                int remaining = byteCount;

                // Copy 8 bytes at a time for efficiency
                while (remaining >= 8)
                {
                    pDest[0] = (char)pSource[0];
                    pDest[1] = (char)pSource[1];
                    pDest[2] = (char)pSource[2];
                    pDest[3] = (char)pSource[3];
                    pDest[4] = (char)pSource[4];
                    pDest[5] = (char)pSource[5];
                    pDest[6] = (char)pSource[6];
                    pDest[7] = (char)pSource[7];
                    pSource += 8;
                    pDest += 8;
                    remaining -= 8;
                }

                // Copy the remaining bytes one by one
                for (int i = 0; i < remaining; i++)
                {
                    pDest[i] = (char)pSource[i];
                }
            }

            return result;
#endif
        }

        internal static int GetByteCount(string myString) => myString.Length;

        internal static unsafe void GetBytes(string myString, int charIndex, int charCount, byte[] bytes, int byteIndex)
        {
            if (myString.Length == 0)
            {
                return;
            }

            fixed (byte* bufferPointer = bytes)
            {
                byte* newBufferPointer = bufferPointer + byteIndex;
                int finalIndex = charIndex + charCount;
                while (charIndex < finalIndex)
                {
                    *newBufferPointer++ = (byte)myString[charIndex++];
                }
            }
        }
        internal static byte[] GetBytes(string myString)
        {
            byte[] bytes = new byte[myString.Length];
            if (myString.Length != 0)
            {
                GetBytes(myString, 0, myString.Length, bytes, 0);
            }
            return bytes;
        }
    }
}
