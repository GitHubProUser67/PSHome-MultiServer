using System;
using System.Collections.Generic;
using XI5.Verification.Keys;
using XI5.Verification.Keys.Games;

namespace XI5.Verification
{
    public static class SigningKeyResolver
    {
        // Map Title IDs to PSN signing keys
        private static readonly Dictionary<string, PsnSigningKey> PsnKeys = new Dictionary<string, PsnSigningKey>(StringComparer.OrdinalIgnoreCase)
        {
            // game title ID, signing key, e.x
            // { "NPUAXXXXX", new ExempleSigningKey() },
            { "BLUS30536", new DriverSFNtscDiscSigningKey() },
        };

        /// <summary>
        /// Returns the appropriate signing key based on the issuer and title ID.
        /// </summary>
        public static ITicketSigningKey GetSigningKey(string issuer, string titleId)
        {
            // rpcn signing key
            if ("RPCN".Equals(issuer, StringComparison.OrdinalIgnoreCase))
                return RpcnSigningKey.Instance;

            // psn game signing key
            if (!string.IsNullOrWhiteSpace(titleId) && PsnKeys.TryGetValue(titleId, out PsnSigningKey psnKey))
                return psnKey;

            // default signing key
            return new DefaultSigningKey();
        }
    }
}
