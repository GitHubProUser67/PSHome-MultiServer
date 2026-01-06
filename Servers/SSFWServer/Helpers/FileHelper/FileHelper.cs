using CustomLogger;
using MultiServerLibrary.Extension;
using System.Text;
using CastleLibrary.S0ny.Edge;

namespace SSFWServer.Helpers.FileHelper
{
    public class FileHelper
    {
        private static readonly byte[] _cryptoBytes = new byte[] { 0x74, 0x72, 0x69, 0x70, 0x6c, 0x65, 0x64, 0x65, 0x73 };

        public static byte[]? ReadAllBytes(string filepath, string? key)
        {
            if (string.IsNullOrEmpty(filepath) || !File.Exists(filepath))
                return null;

            try
            {
                byte[] src = File.ReadAllBytes(filepath);

                if (src.Length > 4 && src[0] == 'T' && src[1] == 'L' && src[2] == 'Z' && src[3] == 'C')
                {
                    byte[]? DecompressedData = LZMA.Decompress(src);

                    if (!string.IsNullOrEmpty(key) && DecompressedData != null && DecompressedData.Length > 9 && ByteUtils.FindBytePattern(DecompressedData, _cryptoBytes) != -1)
                    {
                        byte[] dst = new byte[DecompressedData.Length - 9];
                        Array.Copy(DecompressedData, 9, dst, 0, dst.Length);
                        return FileHelperCryptoClass.DecryptData(dst, FileHelperCryptoClass.GetEncryptionKey(key));
                    }
                    
                    return DecompressedData;
                }

                if (!string.IsNullOrEmpty(key) && src.Length > 9 && ByteUtils.FindBytePattern(src, _cryptoBytes) != -1)
                {
                    byte[] dst = new byte[src.Length - 9];
                    Array.Copy(src, 9, dst, 0, dst.Length);
                    return FileHelperCryptoClass.DecryptData(dst, FileHelperCryptoClass.GetEncryptionKey(key));
                }

                return src;
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[FileHelper] - ReadAllBytes errored out with this exception : {ex}");
            }

            return null;
        }

        public static string? ReadAllText(string filepath, string? key)
        {
            byte[]? fileData = ReadAllBytes(filepath, key);

            if (fileData == null)
                return null;
            else
                return Encoding.UTF8.GetString(fileData);
        }
    }
}
