using CastleLibrary.ICSharpCode.SharpZipLib.Zip.Compression;
using CastleLibrary.ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using EndianTools;
using Org.BouncyCastle.Utilities.Zlib;

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

        private static async Task<byte[]> EdgeZlibDecompressInternalAsync(
            byte[] inData,
            bool icSharp
        )
        {
            var chunkIndex = 0;
            var zlibResults = new List<KeyValuePair<int, Task<byte[]>>>();

            using (var memoryStream = new MemoryStream(inData))
            {
                var array = new byte[ZlibChunkHeader.sizeOf];
                while (memoryStream.Position < memoryStream.Length)
                {
                    await memoryStream.ReadAsync(array).ConfigureAwait(false);
                    var header = ZlibChunkHeader.FromBytes(EndianUtils.EndianSwap(array));
                    int compressedSize = header.CompressedSize;
                    var array2 = new byte[compressedSize];
                    await memoryStream
                        .ReadAsync(array2.AsMemory(0, compressedSize))
                        .ConfigureAwait(false);
                    zlibResults.Add(
                        new KeyValuePair<int, Task<byte[]>>(
                            chunkIndex,
                            icSharp
                                ? DecompressEdgeZlibChunkICSharpAsync(array2, header)
                                : DecompressEdgeZlibChunkAsync(array2, header)
                        )
                    );
                    chunkIndex++;
                }
            }

            using (var memoryStream = new MemoryStream())
            {
                foreach (var result in zlibResults.OrderBy(kv => kv.Key))
                    await memoryStream
                        .WriteAsync(await result.Value.ConfigureAwait(false))
                        .ConfigureAwait(false);

                return memoryStream.ToArray();
            }
        }

        public static byte[] EdgeZlibCompress(byte[] inData)
        {
            return Task.Run(async () =>
                {
                    var chunkIndex = 0;
                    var zlibResults = new List<KeyValuePair<int, Task<byte[]>>>();

                    using (var memoryStream = new MemoryStream(inData))
                    {
                        while (memoryStream.Position < memoryStream.Length)
                        {
                            var currentBlockSize = Math.Min(
                                (int)(memoryStream.Length - memoryStream.Position),
                                ushort.MaxValue
                            );
                            var compressedBytes = new byte[currentBlockSize];
                            await memoryStream
                                .ReadAsync(compressedBytes.AsMemory(0, currentBlockSize))
                                .ConfigureAwait(false);
                            zlibResults.Add(
                                new KeyValuePair<int, Task<byte[]>>(
                                    chunkIndex,
                                    CompressEdgeZlibChunkAsync(compressedBytes)
                                )
                            );
                            chunkIndex++;
                        }
                    }

                    using (var memoryStream = new MemoryStream(inData.Length))
                    {
                        foreach (var result in zlibResults.OrderBy(kv => kv.Key))
                            await memoryStream
                                .WriteAsync(await result.Value.ConfigureAwait(false))
                                .ConfigureAwait(false);

                        return memoryStream.ToArray();
                    }
                })
                .GetAwaiter()
                .GetResult(); // Keep the exception handling intact for backward compatibility.
        }

        private static async Task<byte[]> DecompressEdgeZlibChunkICSharpAsync(
            byte[] inData,
            ZlibChunkHeader header
        )
        {
            if (header.CompressedSize == header.SourceSize)
                return inData;
            const ushort blkSize = 4096;
            var baseInputStream = new MemoryStream(inData);
            var inflaterInputStream = new InflaterInputStream(baseInputStream, new Inflater(true));
            using (var memoryStream = new MemoryStream())
            {
                var array = new byte[blkSize];
                for (; ; )
                {
                    var num = inflaterInputStream.Read(array, 0, array.Length);
                    if (num <= 0)
                        break;
                    await memoryStream.WriteAsync(array.AsMemory(0, num)).ConfigureAwait(false);
                }
                inflaterInputStream.Close();
                return memoryStream.ToArray();
            }
        }

        private static Task<byte[]> DecompressEdgeZlibChunkAsync(
            byte[] InData,
            ZlibChunkHeader header
        )
        {
            if (header.CompressedSize == header.SourceSize)
                return Task.FromResult(InData);
            var memoryStream = new MemoryStream();
            var zoutputStream = new ZOutputStream(memoryStream, true);
            var zlibPayload = new byte[InData.Length];
            Array.Copy(InData, 0, zlibPayload, 0, InData.Length);
            zoutputStream.Write(zlibPayload, 0, zlibPayload.Length);
            zoutputStream.Close();
            memoryStream.Close();
            return Task.FromResult(memoryStream.ToArray());
        }

        private static Task<byte[]> CompressEdgeZlibChunkAsync(byte[] InData)
        {
            byte[] zlibPayload,
                compressedData;
            var memoryStream = new MemoryStream();
            var zoutputStream = new ZOutputStream(memoryStream, 9, true);
            zoutputStream.Write(InData, 0, InData.Length);
            zoutputStream.Close();
            memoryStream.Close();
            zlibPayload = memoryStream.ToArray();
            compressedData = zlibPayload.Length >= InData.Length ? InData : zlibPayload;
            var finalOuput = new byte[compressedData.Length + ZlibChunkHeader.sizeOf];
            Array.Copy(
                compressedData,
                0,
                finalOuput,
                ZlibChunkHeader.sizeOf,
                compressedData.Length
            );
            ZlibChunkHeader chunkHeader = default;
            chunkHeader.SourceSize = (ushort)InData.Length;
            chunkHeader.CompressedSize = (ushort)compressedData.Length;
            Array.Copy(chunkHeader.GetBytes(), 0, finalOuput, 0, ZlibChunkHeader.sizeOf);
            return Task.FromResult(finalOuput);
        }
    }
}
