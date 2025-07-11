namespace EdenServer.EdNet
{
    public class TeamServer : AbstractEdenServer
    {
        public override Dictionary<ushort, Type?> CrcToClass { get; } = new Dictionary<ushort, Type?>() {
        };

        public TeamServer(ushort Port, int MaxConcurrentListeners = 10, int awaiterTimeoutInMS = 500) : base(Port, MaxConcurrentListeners, awaiterTimeoutInMS)
        {

        }
    }
}
