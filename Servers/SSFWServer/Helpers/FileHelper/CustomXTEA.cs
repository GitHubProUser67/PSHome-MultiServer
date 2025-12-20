using CustomLogger;
using EndianTools;

namespace SSFWServer.Helpers.FileHelper
{
    public class CustomXTEA
    {
        /// <summary>
		/// Number of Rounds.
		/// </summary>
		private const uint Rounds = 64;

        /// <summary>
        /// Decrypts the given data with the provided key.
        /// Throws an exception if the length of the data array is not a multiple of 8.
        /// Throws an exception if the decrypted length is longer than the actual array.
        /// </summary>
        /// <param name="data">The encrypted data.</param>
        /// <param name="key">The key used for decryption.</param>
        /// <returns></returns>
        public byte[] Decrypt(byte[] data, byte[] key)
        {
            bool LittleEndian = BitConverter.IsLittleEndian;

            try
            {
                if (data.Length % 8 != 0)
                    return Array.Empty<byte>();
                uint[] blockBuffer = new uint[2];
                byte[] buffer = new byte[data.Length];
                Array.Copy(data, buffer, data.Length);
                using (MemoryStream stream = new(buffer))
                {
                    using BinaryWriter writer = new(stream);
                    for (int i = 0; i < buffer.Length; i += 8)
                    {
                        blockBuffer[0] = BitConverter.ToUInt32(!LittleEndian ? EndianUtils.ReverseArray(buffer) : buffer, i);
                        blockBuffer[1] = BitConverter.ToUInt32(!LittleEndian ? EndianUtils.ReverseArray(buffer) : buffer, i + 4);
                        Decrypt(Rounds, blockBuffer, CreateKey(key, LittleEndian));
                        writer.Write(blockBuffer[0]);
                        writer.Write(blockBuffer[1]);
                    }
                }
                // verify valid length
                uint length = BitConverter.ToUInt32(!LittleEndian ? EndianUtils.ReverseArray(buffer) : buffer, 0);
                if (length > buffer.Length - 4)
                    return Array.Empty<byte>();
                byte[] result = new byte[length];
                Array.Copy(buffer, 4, result, 0, length);
                return result;
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[CUSTOMXTEA] : Thrown an exception in CustomXTEA Decrypt - {ex}");
            }

            return Array.Empty<byte>();
        }

        /// <summary>
        /// Transforms an key of arbitrary length to a 128 bit key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public uint[] CreateKey(byte[] key, bool Endianess)
        {
            // It might be a better idea to just calculate the MD5 hash of the key: var hash = MD5.Create().ComputeHash(key);
            // But we don't want to depend on the Cryptography namespace, because it would increase the build size for some Unity3d platforms.
            byte[] hash = new byte[16];
            for (int i = 0; i < key.Length; i++)
            {
                hash[i % 16] = (byte)(31 * hash[i % 16] ^ key[i]);
            }
            for (int i = key.Length; i < hash.Length; i++)
            {
                // if key was too short
                hash[i] = (byte)(17 * i ^ key[i % key.Length]);
            }
            return new[] {
                BitConverter.ToUInt32(!Endianess ? EndianUtils.ReverseArray(hash) : hash, 0), BitConverter.ToUInt32(!Endianess ? EndianUtils.ReverseArray(hash) : hash, 4),
                BitConverter.ToUInt32(!Endianess ? EndianUtils.ReverseArray(hash) : hash, 8), BitConverter.ToUInt32(!Endianess ? EndianUtils.ReverseArray(hash) : hash, 12)
            };
        }

        #region Block Operations
        /// <summary>
        /// Performs an inplace decryption of the provided data array.
        /// </summary>
        /// <param name="rounds">The number of encryption rounds, the recommend value is 32.</param>
        /// <param name="v">Data array containing two values.</param>
        /// <param name="key">Key array containing 4 values.</param>
        private void Decrypt(uint rounds, uint[] v, uint[] key)
        {
            uint v0 = v[0] ^ 0x9E3779B9, v1 = v[1] ^ 0x9E3779B9, delta = 0x9E3779B9, sum = delta * rounds;
            for (uint i = 0; i < rounds; i++)
            {
                v1 -= (v0 << 4 ^ v0 >> 5) + v0 ^ sum + key[sum >> 11 & 3];
                sum -= delta;
                v0 -= (v1 << 4 ^ v1 >> 5) + v1 ^ sum + key[sum & 3];
            }
            v[0] = v0;
            v[1] = v1;

            v0 = v[0] ^ 0x9E3779B9; v1 = v[1] ^ 0x9E3779B9; delta = 0x9E3779B9; sum = delta * rounds;
            for (uint i = 0; i < rounds; i++)
            {
                v1 -= (v0 << 4 ^ v0 >> 5) + v0 ^ sum + key[sum >> 11 & 3];
                sum -= delta;
                v0 -= (v1 << 4 ^ v1 >> 5) + v1 ^ sum + key[sum & 3];
            }
            v[0] = v0;
            v[1] = v1;

            v0 = v[0] ^ 0x9E3779B9; v1 = v[1] ^ 0x9E3779B9; delta = 0x9E3779B9; sum = delta * rounds;
            for (uint i = 0; i < rounds; i++)
            {
                v1 -= (v0 << 4 ^ v0 >> 5) + v0 ^ sum + key[sum >> 11 & 3];
                sum -= delta;
                v0 -= (v1 << 4 ^ v1 >> 5) + v1 ^ sum + key[sum & 3];
            }
            v[0] = v0;
            v[1] = v1;

            v0 = v[0] ^ 0x9E3779B9; v1 = v[1] ^ 0x9E3779B9; delta = 0x9E3779B9; sum = delta * rounds;
            for (uint i = 0; i < rounds; i++)
            {
                v1 -= (v0 << 4 ^ v0 >> 5) + v0 ^ sum + key[sum >> 11 & 3];
                sum -= delta;
                v0 -= (v1 << 4 ^ v1 >> 5) + v1 ^ sum + key[sum & 3];
            }
            v[0] = v0;
            v[1] = v1;

            v0 = v[0] ^ 0x9E3779B9; v1 = v[1] ^ 0x9E3779B9; delta = 0x9E3779B9; sum = delta * rounds;
            for (uint i = 0; i < rounds; i++)
            {
                v1 -= (v0 << 4 ^ v0 >> 5) + v0 ^ sum + key[sum >> 11 & 3];
                sum -= delta;
                v0 -= (v1 << 4 ^ v1 >> 5) + v1 ^ sum + key[sum & 3];
            }
            v[0] = v0;
            v[1] = v1;

            v0 = v[0] ^ 0x9E3779B9; v1 = v[1] ^ 0x9E3779B9; delta = 0x9E3779B9; sum = delta * rounds;
            for (uint i = 0; i < rounds; i++)
            {
                v1 -= (v0 << 4 ^ v0 >> 5) + v0 ^ sum + key[sum >> 11 & 3];
                sum -= delta;
                v0 -= (v1 << 4 ^ v1 >> 5) + v1 ^ sum + key[sum & 3];
            }
            v[0] = v0;
            v[1] = v1;

            v0 = v[0] ^ 0x9E3779B9; v1 = v[1] ^ 0x9E3779B9; delta = 0x9E3779B9; sum = delta * rounds;
            for (uint i = 0; i < rounds; i++)
            {
                v1 -= (v0 << 4 ^ v0 >> 5) + v0 ^ sum + key[sum >> 11 & 3];
                sum -= delta;
                v0 -= (v1 << 4 ^ v1 >> 5) + v1 ^ sum + key[sum & 3];
            }
            v[0] = v0;
            v[1] = v1;

            v0 = v[0] ^ 0x9E3779B9; v1 = v[1] ^ 0x9E3779B9; delta = 0x9E3779B9; sum = delta * rounds;
            for (uint i = 0; i < rounds; i++)
            {
                v1 -= (v0 << 4 ^ v0 >> 5) + v0 ^ sum + key[sum >> 11 & 3];
                sum -= delta;
                v0 -= (v1 << 4 ^ v1 >> 5) + v1 ^ sum + key[sum & 3];
            }
            v[0] = v0;
            v[1] = v1;

            v0 = v[0] ^ 0x9E3779B9; v1 = v[1] ^ 0x9E3779B9; delta = 0x9E3779B9; sum = delta * rounds;
            for (uint i = 0; i < rounds; i++)
            {
                v1 -= (v0 << 4 ^ v0 >> 5) + v0 ^ sum + key[sum >> 11 & 3];
                sum -= delta;
                v0 -= (v1 << 4 ^ v1 >> 5) + v1 ^ sum + key[sum & 3];
            }
            v[0] = v0;
            v[1] = v1;

            v0 = v[0] ^ 0x9E3779B9; v1 = v[1] ^ 0x9E3779B9; delta = 0x9E3779B9; sum = delta * rounds;
            for (uint i = 0; i < rounds; i++)
            {
                v1 -= (v0 << 4 ^ v0 >> 5) + v0 ^ sum + key[sum >> 11 & 3];
                sum -= delta;
                v0 -= (v1 << 4 ^ v1 >> 5) + v1 ^ sum + key[sum & 3];
            }
            v[0] = v0;
            v[1] = v1;

            for (int j = 0; j <= 3; j++)
            {
                v0 = v[0] ^ 0x9E3779B9 ^ 0x9E3779B9 ^ 0x9E3779B9 ^ 0x9E3779B9 ^ 0x9E3779B9; v1 = v[1] ^ 0x9E3779B9 ^ 0x9E3779B9 ^ 0x9E3779B9 ^ 0x9E3779B9 ^ 0x9E3779B9; delta = 0x9E3779B9; sum = delta * rounds;
                for (uint i = 0; i < rounds; i++)
                {
                    v1 -= (v0 << 4 ^ v0 >> 5) + v0 ^ sum + key[sum >> 11 & 3];
                    sum -= delta;
                    v0 -= (v1 << 4 ^ v1 >> 5) + v1 ^ sum + key[sum & 3];
                }
                v[0] = v0;
                v[1] = v1;
            }
        }
        #endregion
    }
}
