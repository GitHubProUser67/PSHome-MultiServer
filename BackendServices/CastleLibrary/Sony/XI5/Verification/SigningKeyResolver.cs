using CastleLibrary.Sony.XI5.Verification.Keys;
using System.Collections.Generic;

namespace CastleLibrary.Sony.XI5.Verification
{
    public static class SigningKeyResolver
    {
#if !DISABLE_PSN_XI5_VERIFICATION
        // Map Title IDs to PSN signing keys
        private static readonly Dictionary<string, List<ITicketSigningKey>> PsnKeys = new Dictionary<string, List<ITicketSigningKey>>(System.StringComparer.OrdinalIgnoreCase)
        {
            // game title ID, signing key, e.x
            // { "NPUAXXXXX", new List<ITicketSigningKey> { new ExempleSigningKey() } },

            // PSHome
            { "NPEA00013", new List<ITicketSigningKey> { new Keys.Games.PSHome.HomeClosedBetaSigningKey(), new Keys.Games.DefaultSigningKey() } },

            // PSHome APIs
            { "NPUR00071", new List<ITicketSigningKey>
                {
                #region hellfire
                    new Keys.Games.PSHome.Hellfire.HellfirePALSigningKey(),
                    new Keys.Games.PSHome.Hellfire.HellfireNTSCSigningKey(),
                    new Keys.Games.PSHome.Hellfire.HellfireNTSC_JSigningKey(),
                #endregion
                } 
            },
            { "NPUR30111", new List<ITicketSigningKey>
                {
                #region UFC2010
                    new Keys.Games.PSHome.UFC2010.UFCPSHOME2010SigningKey(),

                #endregion
                }
            },

            // Driver SF
            { "BLUS30536", new List<ITicketSigningKey> { new Keys.Games.Ubisoft.DriverSFNtscDiscSigningKey() } },
        };
#endif
        /// <summary>
        /// Returns the appropriate signing key based on the issuer and title ID.
        /// </summary>
        public static List<ITicketSigningKey> GetSigningKeys(bool rpcn, string issuer, string titleId)
        {
            // rpcn signing key
            if (rpcn)
                return new List<ITicketSigningKey> { RpcnSigningKey.Instance };
#if !DISABLE_PSN_XI5_VERIFICATION
            // psn game signing key
            lock (PsnKeys)
            {
                if (!string.IsNullOrWhiteSpace(titleId) && PsnKeys.TryGetValue(titleId, out List<ITicketSigningKey> psnKeys))
                    return new List<ITicketSigningKey>(psnKeys);
            }

            // default signing key
            return new List<ITicketSigningKey> { new Keys.Games.DefaultSigningKey() };
#else
            return null;
#endif
        }
    }
}
