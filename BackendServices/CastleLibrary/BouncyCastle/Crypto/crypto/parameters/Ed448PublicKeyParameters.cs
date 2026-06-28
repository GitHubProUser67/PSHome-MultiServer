using System;
using System.IO;
using Org.BouncyCastle.Math.EC.Rfc8032;
using Org.BouncyCastle.Utilities.IO;

namespace Org.BouncyCastle.Crypto.Parameters
{
    public sealed class Ed448PublicKeyParameters : AsymmetricKeyParameter
    {
        public static readonly int KeySize = Ed448.PublicKeySize;

        private readonly Ed448.PublicPoint m_publicPoint;

        public Ed448PublicKeyParameters(byte[] buf)
            : this(Validate(buf), 0) { }

        public Ed448PublicKeyParameters(byte[] buf, int off)
            : base(false)
        {
            m_publicPoint = Parse(buf, off);
        }

        public Ed448PublicKeyParameters(ReadOnlySpan<byte> buf)
            : base(false)
        {
            if (buf.Length != KeySize)
                throw new ArgumentException("must have length " + KeySize, nameof(buf));

            m_publicPoint = Parse(buf);
        }

        public Ed448PublicKeyParameters(Stream input)
            : base(false)
        {
            Span<byte> data = stackalloc byte[KeySize];

            if (KeySize != Streams.ReadFully(input, data))
                throw new EndOfStreamException("EOF encountered in middle of Ed448 public key");

            m_publicPoint = Parse(data);
        }

        public Ed448PublicKeyParameters(Ed448.PublicPoint publicPoint)
            : base(false)
        {
            m_publicPoint = publicPoint ?? throw new ArgumentNullException(nameof(publicPoint));
        }

        public void Encode(byte[] buf, int off)
        {
            Ed448.EncodePublicPoint(m_publicPoint, buf, off);
        }

        public void Encode(Span<byte> buf)
        {
            Ed448.EncodePublicPoint(m_publicPoint, buf);
        }

        public byte[] GetEncoded()
        {
            byte[] data = new byte[KeySize];
            Encode(data, 0);
            return data;
        }

        public bool Verify(
            Ed448.Algorithm algorithm,
            byte[] ctx,
            byte[] msg,
            int msgOff,
            int msgLen,
            byte[] sig,
            int sigOff
        )
        {
            switch (algorithm)
            {
                case Ed448.Algorithm.Ed448:
                {
                    if (null == ctx)
                        throw new ArgumentNullException(nameof(ctx));
                    if (ctx.Length > 255)
                        throw new ArgumentOutOfRangeException(nameof(ctx));

                    return Ed448.Verify(sig, sigOff, m_publicPoint, ctx, msg, msgOff, msgLen);
                }
                case Ed448.Algorithm.Ed448ph:
                {
                    if (null == ctx)
                        throw new ArgumentNullException(nameof(ctx));
                    if (ctx.Length > 255)
                        throw new ArgumentOutOfRangeException(nameof(ctx));
                    if (Ed448.PrehashSize != msgLen)
                        throw new ArgumentOutOfRangeException(nameof(msgLen));

                    return Ed448.VerifyPrehash(sig, sigOff, m_publicPoint, ctx, msg, msgOff);
                }
                default:
                {
                    throw new ArgumentOutOfRangeException(nameof(algorithm));
                }
            }
        }

        public bool Verify(
            Ed448.Algorithm algorithm,
            byte[] ctx,
            ReadOnlySpan<byte> msg,
            ReadOnlySpan<byte> sig
        )
        {
            switch (algorithm)
            {
                case Ed448.Algorithm.Ed448:
                {
                    if (null == ctx)
                        throw new ArgumentNullException(nameof(ctx));
                    if (ctx.Length > 255)
                        throw new ArgumentOutOfRangeException(nameof(ctx));

                    return Ed448.Verify(sig, m_publicPoint, ctx, msg);
                }
                case Ed448.Algorithm.Ed448ph:
                {
                    if (null == ctx)
                        throw new ArgumentNullException(nameof(ctx));
                    if (ctx.Length > 255)
                        throw new ArgumentOutOfRangeException(nameof(ctx));

                    return Ed448.VerifyPrehash(sig, m_publicPoint, ctx, ph: msg);
                }
                default:
                {
                    throw new ArgumentOutOfRangeException(nameof(algorithm));
                }
            }
        }

        private static Ed448.PublicPoint Parse(byte[] buf, int off)
        {
            return Ed448.ValidatePublicKeyPartialExport(buf, off)
                ?? throw new ArgumentException("invalid public key");
        }

        private static Ed448.PublicPoint Parse(ReadOnlySpan<byte> buf)
        {
            return Ed448.ValidatePublicKeyPartialExport(buf)
                ?? throw new ArgumentException("invalid public key");
        }

        private static byte[] Validate(byte[] buf)
        {
            if (buf.Length != KeySize)
                throw new ArgumentException("must have length " + KeySize, nameof(buf));

            return buf;
        }
    }
}
