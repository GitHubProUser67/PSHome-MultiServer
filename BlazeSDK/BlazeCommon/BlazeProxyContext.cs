namespace BlazeCommon
{
    public class BlazeProxyContext(
        BlazeServerConnection serverConnection,
        BlazeClientConnection clientConnection,
        int errorCode,
        uint msgNum,
        byte userIndex,
        ulong context
    ) : BlazeRpcContext(serverConnection, errorCode, msgNum, userIndex, context)
    {
        public BlazeClientConnection ClientConnection { get; } = clientConnection;
    }
}
