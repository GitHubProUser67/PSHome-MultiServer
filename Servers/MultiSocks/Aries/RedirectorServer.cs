using MultiSocks.Aries.Messages.AuthService;

namespace MultiSocks.Aries
{
    public class RedirectorServer : AbstractAriesServer
    {
        public override Dictionary<string, Type?> NameToClass { get; } =
            new Dictionary<string, Type?>()
            {
                { "@dir", typeof(Dir) },
                { "@tic", null } // We not respond so crypto is not applied.
            };

        public string RedirIP;
        public string RedirPort;

        public RedirectorServer(ushort port, string targetIP, ushort targetPort, string? Project = null, string? SKU = null, bool secure = false, string CN = "", bool WeakChainSignedRSAKey = false) : base(port, targetIP, Project, SKU, secure, CN, WeakChainSignedRSAKey)
        {
            RedirIP = targetIP;
            RedirPort = targetPort.ToString();
        }
    }
}
