using EndianTools;
using System;
using System.IO;
using ComponentAce.Compression.Libs.zlib;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using FixedZlib;
using System.Threading;

namespace CompressionLibrary.Edge
{
    public class Zlib
    {
        // Process Environment.ProcessorCount process at a time, removing the limit is not tolerable as CPU usage goes way too high.
        private static readonly SemaphoreSlim zlibSema = new SemaphoreSlim(Environment.ProcessorCount);

        public static async Task<byte[]> EdgeZlibDecompress(byte[] inData, bool ICSharp = false)
        {
            int chunkIndex = 0;
            List<KeyValuePair<int, Task<byte[]>>> zlibTasks = new List<KeyValuePair<int, Task<byte[]>>>();

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
                    await zlibSema.WaitAsync().ConfigureAwait(false);
                    zlibTasks.Add(ICSharp
                        ? new KeyValuePair<int, Task<byte[]>>(chunkIndex, ICSharpDecompressEdgeZlibChunk(array2, header))
                        : new KeyValuePair<int, Task<byte[]>>(chunkIndex, ComponentAceDecompressEdgeZlibChunk(array2, header)));
                    chunkIndex++;
                }
            }

            using (MemoryStream memoryStream = new MemoryStream())
            {
                foreach (var result in zlibTasks.OrderBy(kv => kv.Key))
                {
                    try
                    {
                        // Await each decompression task
                        byte[] decompressedChunk = await result.Value.ConfigureAwait(false);
                        memoryStream.Write(decompressedChunk, 0, decompressedChunk.Length);
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException($"[Zlib] - EdgeZlibDecompress: Error during decompression at chunk {result.Key}", ex);
                    }
                }

                return memoryStream.ToArray();
            }
        }

        public static async Task<byte[]> EdgeZlibCompress(byte[] inData)
        {
            int chunkIndex = 0;
            List<KeyValuePair<int, Task<byte[]>>> zlibTasks = new List<KeyValuePair<int, Task<byte[]>>>();

            using (MemoryStream memoryStream = new MemoryStream(inData))
            {
                while (memoryStream.Position < memoryStream.Length)
                {
                    int currentBlockSize = Math.Min((int)(memoryStream.Length - memoryStream.Position), ushort.MaxValue);
                    byte[] array = new byte[currentBlockSize];
                    memoryStream.Read(array, 0, currentBlockSize);
                    await zlibSema.WaitAsync().ConfigureAwait(false);
                    zlibTasks.Add(new KeyValuePair<int, Task<byte[]>>(chunkIndex, ComponentAceCompressEdgeZlibChunk(array)));
                    chunkIndex++;
                }
            }

            using (MemoryStream memoryStream = new MemoryStream(inData.Length))
            {
                foreach (var result in zlibTasks.OrderBy(kv => kv.Key))
                {
                    try
                    {
                        // Await each compression task
                        byte[] compressedChunk = await result.Value.ConfigureAwait(false);
                        memoryStream.Write(compressedChunk, 0, compressedChunk.Length);
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException($"[Zlib] - EdgeZlibCompress: Error during compression at chunk {result.Key}", ex);
                    }
                }

                return memoryStream.ToArray();
            }
        }

        private static Task<byte[]> ICSharpDecompressEdgeZlibChunk(byte[] inData, ChunkHeader header)
        {
            try
            {
                if (header.CompressedSize == header.SourceSize)
                    return Task.FromResult(inData);
                InflaterInputStream inflaterInputStream = new InflaterInputStream(new MemoryStream(inData), new Inflater(true));
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    byte[] array = new byte[4096];
                    for (; ; )
                    {
                        int processedBytes = inflaterInputStream.Read(array, 0, array.Length);
                        if (processedBytes <= 0)
                            break;
                        memoryStream.Write(array, 0, processedBytes);
                    }
                    inflaterInputStream.Dispose();
                    return Task.FromResult(memoryStream.ToArray());
                }
            }
            finally
            {
                zlibSema.Release();
            }
        }

        private static Task<byte[]> ComponentAceDecompressEdgeZlibChunk(byte[] InData, ChunkHeader header)
        {
            try
            {
                if (header.CompressedSize == header.SourceSize)
                    return Task.FromResult(InData);
                byte[] array = null;
                if (NativeZlib.CanRun)
                    array = NativeZlib.InflateRaw(InData);
                if (array != null)
                    return Task.FromResult(array);
                using (MemoryStream memoryStream = new MemoryStream())
                using (ZOutputStream zoutputStream = new ZOutputStream(memoryStream, true))
                {
                    array = new byte[InData.Length];
                    Array.Copy(InData, 0, array, 0, InData.Length);
                    zoutputStream.Write(array, 0, array.Length);
                    zoutputStream.Close();
                    memoryStream.Close();
                    return Task.FromResult(memoryStream.ToArray());
                }
            }
            finally
            {
                zlibSema.Release();
            }
        }

        private static Task<byte[]> ComponentAceCompressEdgeZlibChunk(byte[] InData)
        {
            byte[] array2;
            byte[] array = null;

            try
            {
                if (NativeZlib.CanRun)
                    array = NativeZlib.DeflateRaw(InData);
                if (array == null)
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    using (ZOutputStream zoutputStream = new ZOutputStream(memoryStream, 9, true))
                    {
                        zoutputStream.Write(InData, 0, InData.Length);
                        zoutputStream.Close();
                        memoryStream.Close();
                        array = memoryStream.ToArray();
                    }
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
                return Task.FromResult(array3);
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
