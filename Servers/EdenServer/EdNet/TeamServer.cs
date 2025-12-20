namespace EdenServer.EdNet
{
    public class TeamServer : AbstractEdenServer
    {
        public override Dictionary<ushort, Type?> CrcToClass { get; } = new Dictionary<ushort, Type?>() {
        };

        public TeamServer() : base()
        {

        }
    }
}
