using System;
using System.Collections.Generic;
using XI5.Verification.Keys;
#if !DISABLE_PSN_XI5_VERIFICATION
using XI5.Verification.Keys.Games;
#endif
namespace XI5.Verification
{
    public static class SigningKeyResolver
    {
#if !DISABLE_PSN_XI5_VERIFICATION
        // Map Title IDs to PSN signing keys
        private static readonly Dictionary<string, List<ITicketSigningKey>> PsnKeys = new Dictionary<string, List<ITicketSigningKey>>(StringComparer.OrdinalIgnoreCase)
        {
            // game title ID, signing key, e.x
            // { "NPUAXXXXX", new List<ITicketSigningKey> { new ExempleSigningKey() } },

            // PSHome
            { "NPEA00013", new List<ITicketSigningKey> { new HomeClosedBetaSigningKey(), new DefaultSigningKey() } },

            // PSHome APIs
            { "NPUR00071", new List<ITicketSigningKey>
                {
                #region hellfire
                    new HellfirePALSigningKey(),
                    new HellfireNTSCSigningKey(),
                    new HellfireNTSC_JSigningKey(),
                #endregion
                } 
            },
            { "NPUR30111", new List<ITicketSigningKey>
                {
                #region UFC2010
                    new UFCPSHOME2010SigningKey(),

                #endregion
                }
            },

            // Driver SF
            { "BLUS30536", new List<ITicketSigningKey> { new DriverSFNtscDiscSigningKey() } },
        };
#endif
        /// <summary>
        /// Returns the appropriate signing key based on the issuer and title ID.
        /// </summary>
        public static List<ITicketSigningKey> GetSigningKeys(string issuer, string titleId)
        {
            // rpcn signing key
            if ("RPCN".Equals(issuer, StringComparison.OrdinalIgnoreCase))
                return new List<ITicketSigningKey> { RpcnSigningKey.Instance };
#if !DISABLE_PSN_XI5_VERIFICATION
            // psn game signing key
            lock (PsnKeys)
            {
                if (!string.IsNullOrWhiteSpace(titleId) && PsnKeys.TryGetValue(titleId, out List<ITicketSigningKey> psnKeys))
                    return new List<ITicketSigningKey>(psnKeys);
            }

            // default signing key
            return new List<ITicketSigningKey> { new DefaultSigningKey() };
#else
            return null;
#endif
        }
    }
}
