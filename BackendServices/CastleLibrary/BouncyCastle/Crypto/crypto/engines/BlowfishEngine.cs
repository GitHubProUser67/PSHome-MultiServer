using System;
using CastleLibrary.Utils.Crypto;
using EndianTools;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Utilities;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Engines
{
    /**
    * A class that provides Blowfish key encryption operations,
    * such as encoding data and generating keys.
    * All the algorithms herein are from Applied Cryptography
    * and implement a simplified cryptography interface.
    */
    public sealed class BlowfishEngine : IBlockCipher
    {
        private bool encrypting;

        private byte[] workingKey;

        private readonly Blowfish _blowfish;

        public BlowfishEngine()
        {
            _blowfish = new Blowfish();
        }

        /**
        * initialise a Blowfish cipher.
        *
        * @param forEncryption whether or not we are for encryption.
        * @param parameters the parameters required to set up the cipher.
        * @exception ArgumentException if the parameters argument is
        * inappropriate.
        */
        public void Init(bool forEncryption, ICipherParameters parameters)
        {
            if (parameters is not KeyParameter)
                throw new ArgumentException(
                    "invalid parameter passed to Blowfish init - "
                        + Platform.GetTypeName(parameters)
                );

            encrypting = forEncryption;
            workingKey = ((KeyParameter)parameters).GetKey();
            _blowfish.SetKey(workingKey, Endianness.BigEndian);
        }

        public string AlgorithmName
        {
            get { return "Blowfish"; }
        }

        public int ProcessBlock(byte[] input, int inOff, byte[] output, int outOff)
        {
            if (workingKey == null)
                throw new InvalidOperationException("Blowfish not initialised");

            Check.DataLength(input, inOff, Blowfish.BlockSize, "input buffer too short");
            Check.OutputLength(output, outOff, Blowfish.BlockSize, "output buffer too short");

            if (encrypting)
            {
                EncryptBlock(input.AsSpan(inOff), output.AsSpan(outOff));
            }
            else
            {
                DecryptBlock(input.AsSpan(inOff), output.AsSpan(outOff));
            }

            return Blowfish.BlockSize;
        }

        public int ProcessBlock(ReadOnlySpan<byte> input, Span<byte> output)
        {
            if (workingKey == null)
                throw new InvalidOperationException("Blowfish not initialised");

            Check.DataLength(input, Blowfish.BlockSize, "input buffer too short");
            Check.OutputLength(output, Blowfish.BlockSize, "output buffer too short");

            if (encrypting)
            {
                EncryptBlock(input, output);
            }
            else
            {
                DecryptBlock(input, output);
            }

            return Blowfish.BlockSize;
        }

        public int GetBlockSize()
        {
            return Blowfish.BlockSize;
        }

        private void EncryptBlock(ReadOnlySpan<byte> input, Span<byte> output)
        {
            Span<uint> block = [Pack.BE_To_UInt32(input), Pack.BE_To_UInt32(input[4..])];

            _blowfish.Encipher(block, 0);

            Pack.UInt32_To_BE(block[1], output);
            Pack.UInt32_To_BE(block[0], output[4..]);
        }

        private void DecryptBlock(ReadOnlySpan<byte> input, Span<byte> output)
        {
            Span<uint> block = [Pack.BE_To_UInt32(input), Pack.BE_To_UInt32(input[4..])];

            _blowfish.Decipher(block, 0);

            Pack.UInt32_To_BE(block[1], output);
            Pack.UInt32_To_BE(block[0], output[4..]);
        }
    }
}
