using EndianTools;
using MultiServerLibrary.Extension;
using System;

namespace Horizon.RT.Cryptography.RC
{
    public class PS3_RCQ : ICipher
    {
        private byte[] _key = null;
        public CipherContext Context { get; protected set; } = CipherContext.ID_00;

        public PS3_RCQ(byte[] key, CipherContext context)
        {
            Context = context;
            SetKey(key);
        }

        public void SetKey(byte[] key)
        {
            _key = key;
        }

        public bool Encrypt(byte[] input, out byte[] cipher, out byte[] hash)
        {
            hash = null;
            cipher = null;
            if (input == null)
                return false;

            cipher = new byte[input.Length];
            Array.Copy(input, cipher, input.Length);
            if (_key == null)
                return false;

            hash = Hash(cipher, Context);

            // IV
            byte[] iv_buffer = new byte[0x10];
            uint[] iv = new uint[4];
            Array.Copy(_key, 0, iv_buffer, 0, 0x10);

            // Reload
            for (int i = 0; i < 4; ++i)
                iv[i] = BitConverter.ToUInt32(!BitConverter.IsLittleEndian ? EndianUtils.ReverseArray(iv_buffer) : iv_buffer, i * 4);
            RC_Pass(hash, ref iv);

            for (int i = 0; i < 4; ++i)
            {
                byte[] b = BitConverter.GetBytes(iv[i]);

                if (!BitConverter.IsLittleEndian)
                    Array.Reverse(b);

                Array.Copy(b, 0, iv_buffer, i * 4, 4);
            }

            RC_Pass(iv_buffer, ref iv, true);
            RC_Pass(cipher, ref iv, true);

            if (Decrypt(cipher, hash, out var plain))
            {

            }

            return true;
        }

        public bool Decrypt(byte[] input, byte[] hash, out byte[] plain)
        {
            plain = null;
            if (_key == null)
                return false;

            plain = new byte[input.Length];
            Array.Copy(input, 0, plain, 0, plain.Length);

            // Check if empty hash
            // If hash is 0, the data is already in plaintext
            if (hash[0] == 0 && hash[1] == 0 && hash[2] == 0 && (hash[3] & 0x1F) == 0)
                return true;

            // IV
            byte[] iv = new byte[0x10];
            uint[] seed = new uint[4];
            Array.Copy(_key, 0, iv, 0, 0x10);

            for (int i = 0; i < 4; ++i)
                seed[i] = BitConverter.ToUInt32(!BitConverter.IsLittleEndian ? EndianUtils.ReverseArray(iv) : iv, i * 4);
            RC_Pass(hash, ref seed);

            for (int i = 0; i < 4; ++i)
            {
                byte[] b = BitConverter.GetBytes(seed[i]);

                if (!BitConverter.IsLittleEndian)
                    Array.Reverse(b);

                Array.Copy(b, 0, iv, i * 4, 4);
            }

            RC_Pass(iv, ref seed, true);
            RC_Pass(plain, ref seed, true, true);

            Hash(plain, out var checkHash);
            return checkHash.EqualsTo(hash);
        }

        #region Hash

        public virtual bool IsHashValid(byte[] hash)
        {
            if (hash == null || hash.Length != 4)
                return false;

            return !(hash[0] == 0 && hash[1] == 0 && hash[2] == 0 && (hash[3] & 0x1F) == 0);
        }

        public virtual void Hash(byte[] input, out byte[] hash)
        {
            hash = Hash(input, Context);
        }

        public static byte[] Hash(byte[] input, CipherContext context)
        {
            uint r0 = 0x00000000;
            const uint r3 = 0x5B3AA654;
            uint r5 = 0x75970A4D;
            uint r6 = (uint)input.Length;

            int newLength = (input.Length % 4 != 0) ? (input.Length + (4 - (input.Length % 4))) : input.Length;
            byte[] buffer = new byte[newLength];
            Array.Copy(input, 0, buffer, 0, input.Length);
            FlipWords(buffer);

            // IV
            // Here the IV is determined by performing an RC pass on an empty 16 byte buffer.
            byte[] empty = new byte[0x10];
            uint[] iv = new uint[4];
            RC_Pass(empty, ref iv);

            // B5A0559C 88AA4C20 013D2CC7 CB2DE2B6
            uint r16 = iv[0];
            uint r17 = iv[1];
            uint r18 = iv[2];
            uint r19 = iv[3];

            for (int i = 0; i < input.Length; i += 4)
            {
                r19 ^= r3;
                r18 += r16;
                r18 += r19;
                r18 = (r18 << 7) | (r18 >> (32 - 7));
                r17 += r19;
                r17 += r18;
                r18 ^= r5;
                r17 = (r17 << 11) | (r17 >> (32 - 11));
                r16 += r18;
                r16 += r17;
                r16 = (r16 >> 15) | (r16 << (32 - 15));
                r0 = r16 & r17;
                r17 = ~r17;
                r6 = r18 & r17;
                r0 |= r6;
                r19 += r0;
                r16 = ~r16;

                r0 = (uint)((buffer[i + 0] << 24) | (buffer[i + 1] << 16) | (buffer[i + 2] << 8) | (buffer[i + 3] << 0));
                r19 ^= r0;
            }

            byte[] result = new byte[4];
            EndianAwareConverter.WriteUInt32(result, Endianness.LittleEndian, 0, (uint)(((ulong)((r16 + r17 + r18 + r19) & 0x1FFFFFFF) | (ulong)context << 29)));
            return result;
        }

        /// <summary>
        /// Iterates through buffer and flips endianness of each 4 byte word.
        /// </summary>
        /// <param name="input"></param>
        protected static void FlipWords(byte[] input)
        {
            for (int i = 0; i < input.Length; i += 4)
            {
                var temp = input[i + 0];
                input[i + 0] = input[i + 3];
                input[i + 3] = temp;
                temp = input[i + 1];
                input[i + 1] = input[i + 2];
                input[i + 2] = temp;
            }
        }

        protected static void RC_Pass(byte[] input, ref uint[] iv, bool sign = false, bool decrypt = false)
        {
            uint r0 = 0x00000000;
            const uint r3 = 0x5B3AA654;
            uint r5 = 0x75970A4D;
            uint r6 = 0x00000000;

            int newLength = (input.Length % 4 != 0) ? (input.Length + (4 - (input.Length % 4))) : input.Length;
            byte[] buffer = new byte[newLength];
            Array.Copy(input, 0, buffer, 0, input.Length);
            FlipWords(buffer);

            // B5A0559C 88AA4C20 013D2CC7 CB2DE2B6
            uint r16 = iv[0];
            uint r17 = iv[1];
            uint r18 = iv[2];
            uint r19 = iv[3];

            for (int i = 0; i < input.Length; i += 4)
            {
                r19 ^= r3;
                r18 += r16;
                r18 += r19;
                r18 = (r18 << 7) | (r18 >> (32 - 7));
                r17 += r19;
                r17 += r18;
                r18 ^= r5;
                r17 = (r17 << 11) | (r17 >> (32 - 11));
                r16 += r18;
                r16 += r17;
                r16 = (r16 >> 15) | (r16 << (32 - 15));
                r0 = r16 & r17;
                r17 = ~r17;
                r6 = r18 & r17;
                r0 |= r6;
                r19 += r0;
                r16 = ~r16;

                r0 = (uint)((buffer[i + 0] << 24) | (buffer[i + 1] << 16) | (buffer[i + 2] << 8) | (buffer[i + 3] << 0));
                if (decrypt)
                    r0 ^= r19;
                r19 ^= r0;

                if (sign)
                {
                    byte[] r19_b = BitConverter.GetBytes(decrypt ? r0 : r19);

                    if (!BitConverter.IsLittleEndian)
                        Array.Reverse(r19_b);

                    buffer[i + 0] = r19_b[0];
                    buffer[i + 1] = r19_b[1];
                    buffer[i + 2] = r19_b[2];
                    buffer[i + 3] = r19_b[3];
                }
            }

            iv[0] = r16;
            iv[1] = r17;
            iv[2] = r18;
            iv[3] = r19;

            // Copy signed buffer back into input
            // This can be moved into the loop at some point
            if (sign)
                for (int i = 0; i < input.Length; ++i)
                    input[i] = buffer[i];
        }

        #endregion

        #region Comparison

        public override bool Equals(object obj)
        {
            if (obj is PS3_RCQ rc)
                return rc.Equals(this);

            return base.Equals(obj);
        }

        public bool Equals(PS3_RCQ b)
        {
            return b.Context == Context && (b._key?.EqualsTo(_key) ?? false);
        }

        #endregion

        public byte[] GetPublicKey()
        {
            return _key.ShadowCopy();
        }

        public override string ToString()
        {
            return $"PS3_RCQ({Context}, {BitConverter.ToString(_key).Replace("-", string.Empty)})";
        }

    }
}