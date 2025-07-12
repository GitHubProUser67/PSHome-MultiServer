using EndianTools;
using Org.BouncyCastle.Utilities.Zlib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SonyEdge
{
    public class Zlib
    {
        public static byte[] EdgeZlibDecompress(byte[] inData)
        {
            int chunkIndex = 0;
            List<KeyValuePair<int, byte[]>> zlibResults = new List<KeyValuePair<int, byte[]>>();

            using (MemoryStream memoryStream = new MemoryStream(inData))
            {
                byte[] array = new byte[ChunkHeader.SizeOf];
                while (memoryStream.Position < memoryStream.Length)
                {
                    memoryStream.Read(array, 0, array.Length);
                    ChunkHeader header = ChunkHeader.FromBytes(EndianUtils.EndianSwap(array));
                    int compressedSize = header.CompressedSize;
                    byte[] array2 = new byte[compressedSize];
                    memoryStream.Read(array2, 0, compressedSize);
                    zlibResults.Add(new KeyValuePair<int, byte[]>(chunkIndex, DecompressEdgeZlibChunk(array2, header)));
                    chunkIndex++;
                }
            }

            using (MemoryStream memoryStream = new MemoryStream())
            {
                foreach (var result in zlibResults.OrderBy(kv => kv.Key))
                {
                    byte[] decompressedChunk = result.Value;
                    memoryStream.Write(decompressedChunk, 0, decompressedChunk.Length);
                }

                return memoryStream.ToArray();
            }
        }

        public static byte[] EdgeZlibCompress(byte[] inData)
        {
            int chunkIndex = 0;
            List<KeyValuePair<int, byte[]>> zlibResults = new List<KeyValuePair<int, byte[]>>();

            using (MemoryStream memoryStream = new MemoryStream(inData))
            {
                while (memoryStream.Position < memoryStream.Length)
                {
                    int currentBlockSize = Math.Min((int)(memoryStream.Length - memoryStream.Position), ushort.MaxValue);
                    byte[] array = new byte[currentBlockSize];
                    memoryStream.Read(array, 0, currentBlockSize);
                    zlibResults.Add(new KeyValuePair<int, byte[]>(chunkIndex, CompressEdgeZlibChunk(array)));
                    chunkIndex++;
                }
            }

            using (MemoryStream memoryStream = new MemoryStream(inData.Length))
            {
                foreach (var result in zlibResults.OrderBy(kv => kv.Key))
                {
                    byte[] compressedChunk = result.Value;
                    memoryStream.Write(compressedChunk, 0, compressedChunk.Length);
                }

                return memoryStream.ToArray();
            }
        }

        private static byte[] DecompressEdgeZlibChunk(byte[] InData, ChunkHeader header)
        {
            if (header.CompressedSize == header.SourceSize)
                return InData;
            using (MemoryStream memoryStream = new MemoryStream())
            using (ZOutputStream zoutputStream = new ZOutputStream(memoryStream, true))
            {
                byte[] array = new byte[InData.Length];
                Array.Copy(InData, 0, array, 0, InData.Length);
                zoutputStream.Write(array, 0, array.Length);
                zoutputStream.Close();
                memoryStream.Close();
                return memoryStream.ToArray();
            }
        }

        private static byte[] CompressEdgeZlibChunk(byte[] InData)
        {
            byte[] array, array2;

            using (MemoryStream memoryStream = new MemoryStream())
            using (ZOutputStream zoutputStream = new ZOutputStream(memoryStream, 9, true))
            {
                zoutputStream.Write(InData, 0, InData.Length);
                zoutputStream.Close();
                memoryStream.Close();
                array = memoryStream.ToArray();
            }
            if (array.Length >= InData.Length)
                array2 = InData;
            else
                array2 = array;
            byte[] array3 = new byte[array2.Length + 4];
            Array.Copy(array2, 0, array3, 4, array2.Length);
            ChunkHeader chunkHeader = default;
            chunkHeader.SourceSize = (ushort)InData.Length;
            chunkHeader.CompressedSize = (ushort)array2.Length;
            Array.Copy(EndianUtils.EndianSwap(chunkHeader.GetBytes()), 0, array3, 0, ChunkHeader.SizeOf);
            return array3;
        }

        internal struct ChunkHeader
        {
#if NETCOREAPP || NETSTANDARD2_1_OR_GREATER
            internal readonly byte[] GetBytes()
#else
            internal byte[] GetBytes()
#endif
            {
                byte[] array = new byte[4];
                Array.Copy(BitConverter.GetBytes(!BitConverter.IsLittleEndian ? EndianUtils.ReverseUshort(SourceSize) : SourceSize), 0, array, 2, 2);
                Array.Copy(BitConverter.GetBytes(!BitConverter.IsLittleEndian ? EndianUtils.ReverseUshort(CompressedSize) : CompressedSize), 0, array, 0, 2);
                return array;
            }

            internal static byte SizeOf
            {
                get
                {
                    return 4;
                }
            }

            internal static ChunkHeader FromBytes(byte[] inData)
            {
                ChunkHeader result = default;
                byte[] array = inData;

                if (inData.Length > SizeOf)
                {
                    array = new byte[4];
                    Array.Copy(inData, array, 4);
                }

                if (!BitConverter.IsLittleEndian)
                    Array.Reverse(array);

                result.SourceSize = BitConverter.ToUInt16(array, 2);
                result.CompressedSize = BitConverter.ToUInt16(array, 0);
                return result;
            }

            internal ushort SourceSize;

            internal ushort CompressedSize;
        }
    }
}
