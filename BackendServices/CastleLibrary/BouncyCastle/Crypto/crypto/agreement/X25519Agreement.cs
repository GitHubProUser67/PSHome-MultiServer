using System;
using Org.BouncyCastle.Crypto.Parameters;

namespace Org.BouncyCastle.Crypto.Agreement
{
    public sealed class X25519Agreement : IRawAgreement
    {
        private X25519PrivateKeyParameters m_privateKey;

        public void Init(ICipherParameters parameters)
        {
            m_privateKey = (X25519PrivateKeyParameters)parameters;
        }

        public int AgreementSize
        {
            get { return X25519PrivateKeyParameters.SecretSize; }
        }

        public void CalculateAgreement(ICipherParameters publicKey, byte[] buf, int off)
        {
            CalculateAgreement(publicKey, buf.AsSpan(off));
        }

        public void CalculateAgreement(ICipherParameters publicKey, Span<byte> buf)
        {
            m_privateKey.GenerateSecret((X25519PublicKeyParameters)publicKey, buf);
        }
    }
}
