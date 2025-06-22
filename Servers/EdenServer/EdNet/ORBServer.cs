namespace EdenServer.EdNet
{
    public class ORBServer : AbstractEdenServer
    {
        public override Dictionary<ushort, Type?> CrcToClass { get; } = new Dictionary<ushort, Type?>() {
        };

        public ORBServer(ushort Port, int MaxConcurrentListeners = 10, int awaiterTimeoutInMS = 500) : base(Port, MaxConcurrentListeners, awaiterTimeoutInMS)
        {

        }
    }
}
