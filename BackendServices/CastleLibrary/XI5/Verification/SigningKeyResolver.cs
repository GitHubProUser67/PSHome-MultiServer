using System;
using System.Collections.Generic;
using XI5.Verification.Keys;
using XI5.Verification.Keys.Games;

namespace XI5.Verification
{
    public static class SigningKeyResolver
    {
        // Map Title IDs to PSN signing keys
        private static readonly Dictionary<string, List<ITicketSigningKey>> PsnKeys = new Dictionary<string, List<ITicketSigningKey>>(StringComparer.OrdinalIgnoreCase)
        {
            // game title ID, signing key, e.x
            // { "NPUAXXXXX", new List<ITicketSigningKey> { new ExempleSigningKey() } },
            { "BLUS30536", new List<ITicketSigningKey> { new DriverSFNtscDiscSigningKey() } },
            { "NPUR00071", new List<ITicketSigningKey> { new HellfirePALSigningKey(), new HellfireNTSCSigningKey(), new NovusPrimeNTSC_JSigningKey() } },
        };

        /// <summary>
        /// Returns the appropriate signing key based on the issuer and title ID.
        /// </summary>
        public static List<ITicketSigningKey> GetSigningKeys(string issuer, string titleId)
        {
            // rpcn signing key
            if ("RPCN".Equals(issuer, StringComparison.OrdinalIgnoreCase))
                return new List<ITicketSigningKey> { RpcnSigningKey.Instance };

            // psn game signing key
            if (!string.IsNullOrWhiteSpace(titleId) && PsnKeys.TryGetValue(titleId, out List<ITicketSigningKey> psnKeys))
                return psnKeys;

            // default signing key
            return new List<ITicketSigningKey> { new DefaultSigningKey() };
        }
    }
}
