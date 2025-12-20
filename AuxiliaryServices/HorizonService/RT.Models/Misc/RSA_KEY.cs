using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;
using System.Linq;

namespace Horizon.RT.Models
{
    public class RSA_KEY : IStreamSerializer
    {
        public readonly static RSA_KEY Empty = new RSA_KEY();

        public uint[] key = new uint[Constants.RSA_SIZE_DWORD];

        public RSA_KEY()
        { }

        public RSA_KEY(byte[] keyBytes)
        {
            for (int i = 0; i < key.Length; i++)
            {
                key[i] = (uint)((keyBytes[(i * 4) + 3] << 24) | (keyBytes[(i * 4) + 2] << 16) | (keyBytes[(i * 4) + 1] << 8) | (keyBytes[(i * 4) + 0]));
            }
        }

        public void Deserialize(BinaryReader reader)
        {
            key = new uint[Constants.RSA_SIZE_DWORD];
            for (int i = 0; i < Constants.RSA_SIZE_DWORD; ++i)
                key[i] = reader.ReadUInt32();
        }

        public void Serialize(BinaryWriter writer)
        {
            for (int i = 0; i < Constants.RSA_SIZE_DWORD; ++i)
                writer.Write(i >= key.Length ? 0 : key[i]);
        }

        public byte[] ToByteArray()
        {
            byte[] result = new byte[key.Length * 4]; // Each uint is 4 bytes.

            for (int i = 0; i < key.Length; i++)
            {
                // Convert each uint to 4 bytes in little-endian order.
                result[i * 4] = (byte)(key[i] & 0xFF);              // Byte 0 (least significant byte)
                result[(i * 4) + 1] = (byte)((key[i] >> 8) & 0xFF); // Byte 1
                result[(i * 4) + 2] = (byte)((key[i] >> 16) & 0xFF); // Byte 2
                result[(i * 4) + 3] = (byte)((key[i] >> 24) & 0xFF); // Byte 3 (most significant byte)
            }

            return result;
        }

        public override string ToString()
        {
            return string.Join(string.Empty, key?.Select(x => x.ToString("X8")));
        }
    }
}
