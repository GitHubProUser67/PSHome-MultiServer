using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EndianTools;
using Org.BouncyCastle.Utilities.Zlib;

namespace CastleLibrary.Sony.Edge
{
    public class Zlib
    {
        private static readonly SemaphoreSlim zlibSema = new SemaphoreSlim(Environment.ProcessorCount);

        public static byte[] EdgeZlibDecompress(byte[] inData)
        {
            return Task.Run(async() => {
                int chunkIndex = 0;
                List<KeyValuePair<int, Task<byte[]>>> zlibResults = new List<KeyValuePair<int, Task<byte[]>>>();

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
                        zlibResults.Add(new KeyValuePair<int, Task<byte[]>>(chunkIndex, DecompressEdgeZlibChunk(array2, header)));
                        chunkIndex++;
                    }
                }

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    foreach (var result in zlibResults.OrderBy(kv => kv.Key))
                    {
                        byte[] decompressedChunk = await result.Value.ConfigureAwait(false);
                        memoryStream.Write(decompressedChunk, 0, decompressedChunk.Length);
                    }

                    return memoryStream.ToArray();
                }
            }).GetAwaiter().GetResult(); // Keep the exception handling intact for backward compatibility.
        }

        public static byte[] EdgeZlibCompress(byte[] inData)
        {
            return Task.Run(async() => {
                int chunkIndex = 0;
                List<KeyValuePair<int, Task<byte[]>>> zlibResults = new List<KeyValuePair<int, Task<byte[]>>>();

                using (MemoryStream memoryStream = new MemoryStream(inData))
                {
                    while (memoryStream.Position < memoryStream.Length)
                    {
                        int currentBlockSize = Math.Min((int)(memoryStream.Length - memoryStream.Position), ushort.MaxValue);
                        byte[] array = new byte[currentBlockSize];
                        memoryStream.Read(array, 0, currentBlockSize);
                        zlibResults.Add(new KeyValuePair<int, Task<byte[]>>(chunkIndex, CompressEdgeZlibChunk(array)));
                        chunkIndex++;
                    }
                }

                using (MemoryStream memoryStream = new MemoryStream(inData.Length))
                {
                    foreach (var result in zlibResults.OrderBy(kv => kv.Key))
                    {
                        byte[] compressedChunk = await result.Value.ConfigureAwait(false);
                        memoryStream.Write(compressedChunk, 0, compressedChunk.Length);
                    }

                    return memoryStream.ToArray();
                }
            }).GetAwaiter().GetResult(); // Keep the exception handling intact for backward compatibility.
        }

        private static async Task<byte[]> DecompressEdgeZlibChunk(byte[] InData, ChunkHeader header)
        {
            await zlibSema.WaitAsync().ConfigureAwait(false);

            try
            {
                if (header.CompressedSize == header.SourceSize)
                    return InData;
                using (MemoryStream memoryStream = new MemoryStream())
                using (ZOutputStream zoutputStream = new ZOutputStream(memoryStream, true))
                {
                    byte[] zlibPayload = new byte[InData.Length];
                    Array.Copy(InData, 0, zlibPayload, 0, InData.Length);
                    zoutputStream.Write(zlibPayload, 0, zlibPayload.Length);
                    zoutputStream.Close();
                    memoryStream.Close();
                    return memoryStream.ToArray();
                }
            }
            finally
            {
                zlibSema.Release();
            }
        }

        private static async Task<byte[]> CompressEdgeZlibChunk(byte[] InData)
        {
            await zlibSema.WaitAsync().ConfigureAwait(false);

            byte[] zlibPayload, compressedData;

            try
            {
                using (MemoryStream memoryStream = new MemoryStream())
                using (ZOutputStream zoutputStream = new ZOutputStream(memoryStream, 9, true))
                {
                    zoutputStream.Write(InData, 0, InData.Length);
                    zoutputStream.Close();
                    memoryStream.Close();
                    zlibPayload = memoryStream.ToArray();
                }
                if (zlibPayload.Length >= InData.Length)
                    compressedData = InData;
                else
                    compressedData = zlibPayload;
                byte[] finalOuput = new byte[compressedData.Length + 4];
                Array.Copy(compressedData, 0, finalOuput, 4, compressedData.Length);
                ChunkHeader chunkHeader = default;
                chunkHeader.SourceSize = (ushort)InData.Length;
                chunkHeader.CompressedSize = (ushort)compressedData.Length;
                Array.Copy(EndianUtils.EndianSwap(chunkHeader.GetBytes()), 0, finalOuput, 0, ChunkHeader.SizeOf);
                return finalOuput;
            }
            finally
            {
                zlibSema.Release();
            }
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
