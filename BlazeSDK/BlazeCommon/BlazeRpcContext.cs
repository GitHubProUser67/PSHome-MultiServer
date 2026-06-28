namespace BlazeCommon
{
    public class BlazeRpcContext
    {
        public BlazeServerConnection BlazeConnection { get; }
        public ProtoFireConnection Connection
        {
            get => BlazeConnection.ProtoFireConnection;
        }
        public object State
        {
            get => BlazeConnection.State;
            set => BlazeConnection.State = value;
        }
        public int ErrorCode { get; }
        public uint MsgNum { get; }
        public byte UserIndex { get; }
        public ulong Context { get; }

        internal BlazeRpcContext(
            BlazeServerConnection serverConnection,
            int errorCode,
            uint msgNum,
            byte userIndex,
            ulong context
        )
        {
            BlazeConnection = serverConnection;
            ErrorCode = errorCode;
            MsgNum = msgNum;
            UserIndex = userIndex;
            Context = context;
        }

        public static implicit operator BlazeServerConnection(BlazeRpcContext context) =>
            context.BlazeConnection;
    }
}
