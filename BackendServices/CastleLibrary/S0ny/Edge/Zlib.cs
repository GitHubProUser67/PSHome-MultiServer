using EndianTools;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using Org.BouncyCastle.Utilities.Zlib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CastleLibrary.S0ny.Edge
{
    public class Zlib
    {
        // The Zlib decompression logic is not fully understood (original tools used different implementations, the PS3 one wasn't reversed, as such the bouncy castle implementation can very rarely, fail.
        // In cases where it fails, we fallback to the ICSharp implementation (which also isn't perfect, but it works when BC doesn't, and vice-versa).
        public static byte[] EdgeZlibDecompress(byte[] inData)
        {
            try
            {
                return EdgeZlibDecompressInternalAsync(inData, true).Result;
            }
            catch
            {
                // Not Important.
            }

            return EdgeZlibDecompressInternalAsync(inData, false).GetAwaiter().GetResult(); // Keep the exception handling intact for backward compatibility.
        }

        private static async Task<byte[]> EdgeZlibDecompressInternalAsync(byte[] inData, bool icSharp)
        {
            int chunkIndex = 0;
            List<KeyValuePair<int, Task<byte[]>>> zlibResults = new List<KeyValuePair<int, Task<byte[]>>>();

            using (MemoryStream memoryStream = new MemoryStream(inData))
            {
                byte[] array = new byte[ZlibChunkHeader.sizeOf];
                while (memoryStream.Position < memoryStream.Length)
                {
                    await memoryStream.ReadAsync(array, 0, array.Length).ConfigureAwait(false);
                    ZlibChunkHeader header = ZlibChunkHeader.FromBytes(EndianUtils.EndianSwap(array));
                    int compressedSize = header.CompressedSize;
                    byte[] array2 = new byte[compressedSize];
                    await memoryStream.ReadAsync(array2, 0, compressedSize).ConfigureAwait(false);
                    zlibResults.Add(new KeyValuePair<int, Task<byte[]>>(chunkIndex, icSharp ? DecompressEdgeZlibChunkICSharpAsync(array2, header) : DecompressEdgeZlibChunkAsync(array2, header)));
                    chunkIndex++;
                }
            }

            using (MemoryStream memoryStream = new MemoryStream())
            {
                foreach (var result in zlibResults.OrderBy(kv => kv.Key))
                {
                    byte[] decompressedChunk = await result.Value.ConfigureAwait(false);
                    await memoryStream.WriteAsync(decompressedChunk, 0, decompressedChunk.Length).ConfigureAwait(false);
                }

                return memoryStream.ToArray();
            }
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
                        await memoryStream.ReadAsync(array, 0, currentBlockSize).ConfigureAwait(false);
                        zlibResults.Add(new KeyValuePair<int, Task<byte[]>>(chunkIndex, CompressEdgeZlibChunkAsync(array)));
                        chunkIndex++;
                    }
                }

                using (MemoryStream memoryStream = new MemoryStream(inData.Length))
                {
                    foreach (var result in zlibResults.OrderBy(kv => kv.Key))
                    {
                        byte[] compressedChunk = await result.Value.ConfigureAwait(false);
                        await memoryStream.WriteAsync(compressedChunk, 0, compressedChunk.Length).ConfigureAwait(false);
                    }

                    return memoryStream.ToArray();
                }
            }).GetAwaiter().GetResult(); // Keep the exception handling intact for backward compatibility.
        }

        private static async Task<byte[]> DecompressEdgeZlibChunkICSharpAsync(byte[] inData, ZlibChunkHeader header)
        {
            if (header.CompressedSize == header.SourceSize)
                return inData;
            const ushort blkSize = 4096;
            MemoryStream baseInputStream = new MemoryStream(inData);
            InflaterInputStream inflaterInputStream = new InflaterInputStream(baseInputStream, new Inflater(true));
            using (MemoryStream memoryStream = new MemoryStream())
            {
                byte[] array = new byte[blkSize];
                for (; ; )
                {
                    int num = inflaterInputStream.Read(array, 0, array.Length);
                    if (num <= 0)
                        break;
                    await memoryStream.WriteAsync(array, 0, num).ConfigureAwait(false);
                }
                inflaterInputStream.Close();
                return memoryStream.ToArray();
            }
        }

        private static Task<byte[]> DecompressEdgeZlibChunkAsync(byte[] InData, ZlibChunkHeader header)
        {
            if (header.CompressedSize == header.SourceSize)
                return Task.FromResult(InData);
            MemoryStream memoryStream = new MemoryStream();
            ZOutputStream zoutputStream = new ZOutputStream(memoryStream, true);
            byte[] zlibPayload = new byte[InData.Length];
            Array.Copy(InData, 0, zlibPayload, 0, InData.Length);
            zoutputStream.Write(zlibPayload, 0, zlibPayload.Length);
            zoutputStream.Close();
            memoryStream.Close();
            return Task.FromResult(memoryStream.ToArray());
        }

        private static Task<byte[]> CompressEdgeZlibChunkAsync(byte[] InData)
        {
            byte[] zlibPayload, compressedData;
            MemoryStream memoryStream = new MemoryStream();
            ZOutputStream zoutputStream = new ZOutputStream(memoryStream, 9, true);
            zoutputStream.Write(InData, 0, InData.Length);
            zoutputStream.Close();
            memoryStream.Close();
            zlibPayload = memoryStream.ToArray();
            if (zlibPayload.Length >= InData.Length)
                compressedData = InData;
            else
                compressedData = zlibPayload;
            byte[] finalOuput = new byte[compressedData.Length + 4];
            Array.Copy(compressedData, 0, finalOuput, 4, compressedData.Length);
            ZlibChunkHeader chunkHeader = default;
            chunkHeader.SourceSize = (ushort)InData.Length;
            chunkHeader.CompressedSize = (ushort)compressedData.Length;
            Array.Copy(EndianUtils.EndianSwap(chunkHeader.GetBytes()), 0, finalOuput, 0, ZlibChunkHeader.sizeOf);
            return Task.FromResult(finalOuput);
        }
    }
}
