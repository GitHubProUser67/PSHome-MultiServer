using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;

namespace FixedSsl
{
    public static class SslProtocolsUtils
    {
        private static readonly SslProtocols[] _allProtocols = Enum.GetValues(typeof(SslProtocols))
                               .Cast<SslProtocols>()
#pragma warning disable
                               .Where(p => p != SslProtocols.None && p != SslProtocols.Default).ToArray();
#pragma warning restore
        /// <summary>
        /// Returns a list of enabled SslProtocols from a bitwise combination.
        /// </summary>
        public static IEnumerable<SslProtocols> GetEnabledProtocols(this SslProtocols protocols)
        {
            foreach (var p in _allProtocols)
            {
                if ((protocols & p) != 0)
                    yield return p;
            }
        }
    }
}
