using EndianTools;
using Horizon.LIBRARY.Common.Stream;
using Horizon.RT.Common;

namespace Horizon.RT.Models
{
    public class RSA_KEY : IStreamSerializer
    {
        public static readonly RSA_KEY Empty = new();

        public uint[] key = new uint[Constants.RSA_SIZE_DWORD];

        public RSA_KEY() { }

        public RSA_KEY(byte[] keyBytes)
        {
            for (var i = 0; i < key.Length; i++)
                key[i] = EndianAwareConverter.ToUInt32(
                    keyBytes,
                    Endianness.LittleEndian,
                    (uint)(i * 4)
                );
        }

        public void Deserialize(BinaryReader reader)
        {
            key = new uint[Constants.RSA_SIZE_DWORD];
            for (var i = 0; i < Constants.RSA_SIZE_DWORD; ++i)
                key[i] = reader.ReadUInt32();
        }

        public void Serialize(BinaryWriter writer)
        {
            for (var i = 0; i < Constants.RSA_SIZE_DWORD; ++i)
                writer.Write(i >= key.Length ? 0 : key[i]);
        }

        public byte[] ToByteArray()
        {
            var result = new byte[key.Length * 4]; // Each uint is 4 bytes.

            for (var i = 0; i < key.Length; i++)
                EndianAwareConverter.WriteUInt32(
                    result,
                    Endianness.LittleEndian,
                    (uint)(i * 4),
                    key[i]
                );

            return result;
        }

        public override string ToString()
        {
            return string.Join(string.Empty, key?.Select(x => x.ToString("X8")));
        }
    }
}
