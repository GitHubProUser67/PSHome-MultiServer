using System;
using System.IO;
using Org.BouncyCastle.Math.EC.Rfc7748;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.IO;

namespace Org.BouncyCastle.Crypto.Parameters
{
    public sealed class X25519PrivateKeyParameters : AsymmetricKeyParameter
    {
        public static readonly int KeySize = X25519.ScalarSize;
        public static readonly int SecretSize = X25519.PointSize;

        private readonly byte[] data = new byte[KeySize];

        public X25519PrivateKeyParameters(SecureRandom random)
            : base(true)
        {
            X25519.GeneratePrivateKey(random, data);
        }

        public X25519PrivateKeyParameters(byte[] buf)
            : this(Validate(buf), 0) { }

        public X25519PrivateKeyParameters(byte[] buf, int off)
            : base(true)
        {
            Array.Copy(buf, off, data, 0, KeySize);
        }

        public X25519PrivateKeyParameters(ReadOnlySpan<byte> buf)
            : base(true)
        {
            if (buf.Length != KeySize)
                throw new ArgumentException("must have length " + KeySize, nameof(buf));

            buf.CopyTo(data);
        }

        public X25519PrivateKeyParameters(Stream input)
            : base(true)
        {
            if (KeySize != Streams.ReadFully(input, data))
                throw new EndOfStreamException("EOF encountered in middle of X25519 private key");
        }

        public void Encode(byte[] buf, int off)
        {
            Array.Copy(data, 0, buf, off, KeySize);
        }

        public void Encode(Span<byte> buf)
        {
            data.CopyTo(buf);
        }

        public byte[] GetEncoded()
        {
            return Arrays.Clone(data);
        }

        internal ReadOnlySpan<byte> DataSpan => data;

        internal ReadOnlyMemory<byte> DataMemory => data;

        public X25519PublicKeyParameters GeneratePublicKey()
        {
            Span<byte> publicKey = stackalloc byte[X25519.PointSize];
            X25519.GeneratePublicKey(data, publicKey);
            return new X25519PublicKeyParameters(publicKey);
        }

        public void GenerateSecret(X25519PublicKeyParameters publicKey, byte[] buf, int off)
        {
            GenerateSecret(publicKey, buf.AsSpan(off));
        }

        public void GenerateSecret(X25519PublicKeyParameters publicKey, Span<byte> buf)
        {
            Span<byte> encoded = stackalloc byte[X25519.PointSize];
            publicKey.Encode(encoded);
            if (!X25519.CalculateAgreement(data, encoded, buf))
                throw new InvalidOperationException("X25519 agreement failed");
        }

        private static byte[] Validate(byte[] buf)
        {
            if (buf.Length != KeySize)
                throw new ArgumentException("must have length " + KeySize, nameof(buf));

            return buf;
        }
    }
}
