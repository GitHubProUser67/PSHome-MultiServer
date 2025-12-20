namespace BlazeCommon
{
    public class BlazeProxyContext : BlazeRpcContext
    {
        public BlazeClientConnection ClientConnection { get; }
        public BlazeProxyContext(BlazeServerConnection serverConnection, BlazeClientConnection clientConnection, int errorCode, uint msgNum, byte userIndex, ulong context) : base(serverConnection, errorCode, msgNum, userIndex, context)
        {
            ClientConnection = clientConnection;
        }
    }
}
