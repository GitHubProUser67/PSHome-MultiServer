using MultiServerLibrary.Extension;
using System;
using System.Linq;

namespace Horizon.RT.Cryptography.RC
{
    /// <summary>
    /// PS2's custom RC4 implementation,
    /// based off https://github.com/bcgit/bc-csharp/blob/f18a2dbbc2c1b4277e24a2e51f09cac02eedf1f5/crypto/src/crypto/engines/RC4Engine.cs
    /// </summary>
    public class PS2_RC4 : ICipher
    {
        private readonly static int STATE_LENGTH = 256;

        /// <summary>
        /// Cipher context.
        /// </summary>
        public CipherContext Context { get; protected set; } = CipherContext.ID_00;

        private class RC4State
        {
            /*
            * variables to hold the state of the RC4 engine
            * during encryption and decryption
            */
            public byte[] engineState;
            public int x;
            public int y;
        }

        private readonly byte[] workingKey;

        /// <summary>
        /// Initialize with key.
        /// UYA wants a 512 bit key.
        /// </summary>
        public PS2_RC4(byte[] key, CipherContext context)
        {
            Context = context;
            workingKey = key;
        }

        #region Initialization

        private static void SetKey(RC4State state, byte[] key, byte[] hash = null)
        {
            state.x = 0;
            state.y = 0;

            int keyIndex = 0;
            int li = 0;
            int cipherIndex = 0;
            int idIndex = 0;

            // Initialize engine state
            if (state.engineState == null)
                state.engineState = new byte[STATE_LENGTH];

            // reset the state of the engine
            // Normally this initializes values 0,1..254,255 but UYA does this in reverse.
            for (int i = 0; i < STATE_LENGTH; i++)
                state.engineState[i] = (byte)((STATE_LENGTH - 1) - i);

            if (hash != null && hash.Length == 4)
            {
                // Apply hash
                do
                {
                    int v1 = hash[idIndex];
                    idIndex = (idIndex + 1) & 3;

                    byte temp = state.engineState[cipherIndex];
                    v1 += li;
                    li = (temp + v1) & 0xFF;

                    state.engineState[cipherIndex] = state.engineState[li];
                    state.engineState[li] = temp;

                    cipherIndex = (cipherIndex + 5) & 0xFF;

                } while (cipherIndex != 0);

                // Reset
                keyIndex = 0;
                li = 0;
                cipherIndex = 0;
                idIndex = 0;
            }

            // Apply key
            do
            {
                int keyByte = key[keyIndex];
                keyByte += li;
                keyIndex += 1;
                keyIndex &= 0x3F;

                int cipherByte = state.engineState[cipherIndex];
                byte cipherValue = (byte)(cipherByte & 0xFF);

                cipherByte += keyByte;
                li = cipherByte & 0xFF;

                state.engineState[cipherIndex] = state.engineState[li];
                state.engineState[li] = cipherValue;

                cipherIndex += 3;
                cipherIndex &= 0xFF;
            } while (cipherIndex != 0);
        }

        #endregion

        #region Decrypt

        private static void Decrypt(
                RC4State state,
                byte[] input,
                int inOff,
                int length,
                byte[] output,
                int outOff)
        {
            for (int i = 0; i < length; ++i)
            {
                state.y = (state.y + 5) & 0xFF;

                int v0 = state.engineState[state.y];
                byte a2 = (byte)(v0 & 0xFF);
                v0 += state.x;
                state.x = (byte)(v0 & 0xFF);

                v0 = state.engineState[state.x];
                state.engineState[state.y] = (byte)(v0 & 0xFF);
                state.engineState[state.x] = a2;

                byte a0 = input[i + inOff];

                v0 += a2;
                v0 &= 0xFF;
                int v1 = state.engineState[v0];

                a0 ^= (byte)v1;
                output[i + outOff] = a0;


                v1 = state.engineState[a0] + state.x;
                state.x = v1 & 0xFF;
            }
        }

        public bool Decrypt(byte[] data, byte[] hash, out byte[] plain)
        {
            plain = new byte[data.Length];
            var state = new RC4State();

            // Check if empty hash
            // If hash is 0, the data is already in plaintext
            if (!IsHashValid(hash))
            {
                Array.Copy(data, 0, plain, 0, data.Length);
                return true;
            }

            // Set seed
            SetKey(state, workingKey, hash);

            Decrypt(state, data, 0, data.Length, plain, 0);
            Hash(plain, out var checkHash);
            return hash.EqualsTo(checkHash);
        }

        #endregion

        #region Encrypt

        private void Encrypt(
                RC4State state,
                byte[] input,
                int inOff,
                int length,
                byte[] output,
                int outOff)
        {

            for (int i = 0; i < length; ++i)
            {
                state.x = (state.x + 5) & 0xFF;
                state.y = (state.y + state.engineState[state.x]) & 0xFF;

                // Swap
                byte temp = state.engineState[state.x];
                state.engineState[state.x] = state.engineState[state.y];
                state.engineState[state.y] = temp;

                // Xor
                output[i + outOff] = (byte)(
                    input[i + inOff]
                    ^
                    state.engineState[(state.engineState[state.x] + state.engineState[state.y]) & 0xFF]
                    );

                state.y = (state.engineState[input[i + inOff]] + state.y) & 0xFF;
            }
        }

        public bool Encrypt(byte[] data, out byte[] cipher, out byte[] hash)
        {
            var state = new RC4State();

            // Set seed
            hash = Cryptography.Hash.SHA1.Hash(data, Context);
            SetKey(state, workingKey, hash);

            cipher = new byte[data.Length];
            Encrypt(state, data, 0, data.Length, cipher, 0);
            return true;
        }

        #endregion

        #region Hash

        public void Hash(byte[] input, out byte[] hash)
        {
            hash = Cryptography.Hash.SHA1.Hash(input, Context);
        }

        public virtual bool IsHashValid(byte[] hash)
        {
            if (hash == null || hash.Length != 4)
                return false;

            return !(hash[0] == 0 && hash[1] == 0 && hash[2] == 0 && (hash[3] & 0x1F) == 0);
        }

        #endregion

        #region Comparison

        public override bool Equals(object obj)
        {
            if (obj is PS2_RC4 rc)
                return rc.Equals(this);

            return base.Equals(obj);
        }

        public bool Equals(PS2_RC4 b)
        {
            return b.Context == Context && (b.workingKey?.EqualsTo(workingKey) ?? false);
        }

        #endregion

        public byte[] GetPublicKey()
        {
            return workingKey.ShadowCopy();
        }

        public override string ToString()
        {
            return $"PS2_RC4({Context}, {BitConverter.ToString(workingKey).Replace("-", string.Empty)})";
        }

    }
}