using CustomLogger;
using Org.BouncyCastle.Utilities.Zlib;
using System;
using System.IO;

namespace HomeTools.BARFramework
{
    internal static class ZLibCompressor
    {
        internal static byte[] Compress(byte[] inData, bool NoHeader)
        {
            byte[] result = null;
            using (MemoryStream memoryStream = new MemoryStream())
            using (ZOutputStream zoutputStream = new ZOutputStream(memoryStream, 9, NoHeader))
            {
                try
                {
                    zoutputStream.Write(inData, 0, inData.Length);
                }
                catch (Exception ex)
                {
                    LoggerAccessor.LogError($"[ZlibCompressor] - Compressed errored out with this exception - {ex}");
                    return Array.Empty<byte>();
                }
                finally
                {
                    zoutputStream.Close();
                    memoryStream.Close();
                    result = memoryStream.ToArray();
                }
            }
            return result;
        }

        internal static byte[] Decompress(byte[] inData, bool NoHeader)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            using (ZOutputStream zoutputStream = new ZOutputStream(memoryStream, NoHeader))
            {
                zoutputStream.Write(inData, 0, inData.Length);
                zoutputStream.Close();
                memoryStream.Close();
                return memoryStream.ToArray();
            }
        }
    }
}