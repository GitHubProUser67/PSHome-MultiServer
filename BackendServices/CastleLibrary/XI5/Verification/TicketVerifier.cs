using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using System;
using System.Linq;

namespace XI5.Verification
{
    public class TicketVerifier
    {
        // credit: LittleBigRefresh team

        private readonly TicketSignatureMessageType _messageType;
        private readonly XI5Ticket _ticket;
        private readonly byte[] _ticketData;
        private readonly ISigner _signer;

        public TicketVerifier(byte[] ticketData, XI5Ticket ticket, ITicketSigningKey key)
        {
            this._messageType = key.MessageType;
            this._ticketData = ticketData;
            this._ticket = ticket;

            X9ECParameters xParams = ECNamedCurveTable.GetByName(key.CurveTable);
            ECDomainParameters domainParams = new ECDomainParameters(xParams.Curve, xParams.G, xParams.N, xParams.H, xParams.GetSeed());
            Org.BouncyCastle.Math.EC.ECPoint ecPoint = domainParams.Curve.CreatePoint(new Org.BouncyCastle.Math.BigInteger(key.PublicKeyX, 16), new Org.BouncyCastle.Math.BigInteger(key.PublicKeyY, 16));

            ECPublicKeyParameters publicKey = new ECPublicKeyParameters(ecPoint, domainParams);
            this._signer = SignerUtilities.GetSigner(key.HashAlgorithm + "withECDSA");
            this._signer.Init(false, publicKey);
        }

        // https://github.com/LBPUnion/ProjectLighthouse/blob/80cfb24d6f72ecdf45c8389b29a89fd1c13d0a96/ProjectLighthouse/Tickets/Signature/TicketSignatureVerifier.cs#L30
        // Sometimes psn signatures have one or two extra empty bytes
        // This is slow but it's better than carelessly chopping 0's
        private static byte[] TrimSignature(byte[] signature)
        {
            for (int i = 0; i <= 2; i++)
            {
                try
                {
                    Asn1Object.FromByteArray(signature);
                    break;
                }
                catch
                {
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                    signature = signature.SkipLast(1).ToArray();
#else
                    signature = signature.Take(signature.Length - 1).ToArray();
#endif
                }
            }

            return signature;
        }

        public bool IsTicketValid()
        {
            int inOff;
            int inLen;

            switch (this._messageType)
            {
                case TicketSignatureMessageType.Body:
                    inOff = this._ticket.BodySection.Position;
                    inLen = this._ticket.BodySection.Length + 4;
                    break;
                case TicketSignatureMessageType.Ticket:
                    inOff = 0;
                    inLen = this._ticketData.Length - this._ticket.SignatureData.Length;
                    break;
                default:
                    throw new NotImplementedException(this._messageType.ToString());
            }

            this._signer.BlockUpdate(this._ticketData, inOff, inLen);

            return this._signer.VerifySignature(TrimSignature(this._ticket.SignatureData));
        }
    }
}
