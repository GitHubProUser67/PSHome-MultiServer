namespace EdenServer.EdNet
{
    public class ORBServer : AbstractEdenServer
    {
        public override Dictionary<ushort, Type?> CrcToClass { get; } = new Dictionary<ushort, Type?>() {
        };

        public ORBServer() : base()
        {

        }
    }
}
