using System.Security.Authentication;

namespace CastleLibrary.FixedSsl
{
    public static class SslProtocolsUtils
    {
        private static readonly SslProtocols[] _allProtocols = Enum.GetValues<SslProtocols>()
#pragma warning disable
            .Where(p => p != SslProtocols.None && p != SslProtocols.Default)
            .ToArray();
#pragma warning restore

        extension(SslProtocols protocols)
        {
            /// <summary>
            /// Returns a list of enabled SslProtocols from a bitwise combination.
            /// </summary>
            public IEnumerable<SslProtocols> GetEnabledProtocols()
            {
                foreach (var p in _allProtocols)
                {
                    if ((protocols & p) != 0)
                        yield return p;
                }
            }
        }
    }
}
