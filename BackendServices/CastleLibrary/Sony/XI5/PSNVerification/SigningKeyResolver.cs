using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.OpenSsl;
using System.Collections.Generic;
using System.IO;

namespace CastleLibrary.Sony.XI5.PSNVerification
{
    public static class SigningKeyResolver
    {
#if !DISABLE_PSN_XI5_VERIFICATION
        // Map Title IDs to PSN signing keys
        private static readonly Dictionary<string, List<ITicketPublicSigningKey>> PsnKeys = new Dictionary<string, List<ITicketPublicSigningKey>>(System.StringComparer.OrdinalIgnoreCase)
        {
            // game title ID, signing key, e.x
            // { "NPUAXXXXX", new List<ITicketSigningKey> { new ExempleSigningKey() } },

            // PSHome
            { "NPEA00013", new List<ITicketPublicSigningKey> { new Keys.Games.PSHome.HomeClosedBetaSigningKey(), new Keys.Games.DefaultSigningKey() } },

            // PSHome APIs
            { "NPUR00071", new List<ITicketPublicSigningKey>
                {
                #region hellfire
                    new Keys.Games.PSHome.Hellfire.HellfirePALSigningKey(),
                    new Keys.Games.PSHome.Hellfire.HellfireNTSCSigningKey(),
                    new Keys.Games.PSHome.Hellfire.HellfireNTSC_JSigningKey(),
                #endregion
                } 
            },
            { "NPUR30111", new List<ITicketPublicSigningKey>
                {
                #region UFC2010
                    new Keys.Games.PSHome.UFC2010.UFCPSHOME2010SigningKey(),

                #endregion
                }
            },

            // Driver SF
            { "BLUS30536", new List<ITicketPublicSigningKey> { new Keys.Games.Ubisoft.DriverSFNtscDiscSigningKey() } },
        };
#endif
        /// <summary>
        /// Returns the appropriate signing key based on the issuer and title ID.
        /// </summary>
        public static List<ITicketPublicSigningKey> GetSigningKeys(string issuer, string titleId)
        {
#if !DISABLE_PSN_XI5_VERIFICATION
            // psn game signing key
            lock (PsnKeys)
            {
                if (!string.IsNullOrWhiteSpace(titleId) && PsnKeys.TryGetValue(titleId, out List<ITicketPublicSigningKey> psnKeys))
                    return new List<ITicketPublicSigningKey>(psnKeys);
            }

            // default signing key
            return new List<ITicketPublicSigningKey> { new Keys.Games.DefaultSigningKey() };
#else
            return null;
#endif
        }

        public static bool VerifyTicketSignature(byte[] hashedMessage, string pemStr, DerSequence seq)
        {
            using (PemReader pr = new PemReader(new StringReader(pemStr)))
            {
                ECDsaSigner ECDsaPSN = new ECDsaSigner();
                ECDsaPSN.Init(false, (ECPublicKeyParameters)pr.ReadObject());
                return ECDsaPSN.VerifySignature(hashedMessage, ((DerInteger)seq[0]).Value, ((DerInteger)seq[1]).Value);
            }
        }
    }
}
