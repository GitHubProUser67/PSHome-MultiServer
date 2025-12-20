using CustomLogger;

namespace BlazeCommon
{
    public class BlazeClientConnection : ProtoFireClient
    {
        public BlazeClientConfiguration Config { get; }
        public object State { get; set; }

        public BlazeClientConnection(ProtoFireConnection connection, BlazeClientConfiguration clientConfiguration) : base(connection)
        {
            State = new object();
            Config = clientConfiguration;
        }

        public override void OnClientDisconnected()
        {
            LoggerAccessor.LogWarn("[BlazeClientConnection] - Client Disconnected.");
        }

        public override void OnPacketReceived(ProtoFirePacket packet)
        {
            if (packet.Frame.MsgType != FireFrame.MessageType.NOTIFICATION)
                return;

            IBlazeClientComponent? component = Config.GetComponent(packet.Frame.Component);
            if (component == null)
            {
                LoggerAccessor.LogWarn($"[BlazeClientConnection] - Unable to handle notification - component {packet.Frame.Component} handler not found");
                return;
            }

            Type notificationType = component.GetNotificationType(packet.Frame.Command);
            IBlazePacket blazePacket = packet.Decode(notificationType, Config.Decoder);
            BlazeUtils.LogPacket(component, blazePacket, true);

            BlazeClientNotificationMethodInfo? methodInfo = component.GetBlazeNotificationInfo(packet.Frame.Command);
            if (methodInfo == null)
            {
                LoggerAccessor.LogWarn($"[BlazeClientConnection] - Unable to handle notification for component {packet.Frame.Component} - notification {packet.Frame.Command} handler not found");
                return;
            }

            try
            {
                methodInfo.InvokeAsync(blazePacket.DataObj).GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError($"[BlazeClientConnection] - Error while handling notification for component {packet.Frame.Component} - notification {packet.Frame.Command} (Exception: {e})");
            }
        }

        public TResponse SendRequest<TRequest, TResponse, TErrorResponse>(IBlazeComponent component, ushort commandId, TRequest request) where TRequest : notnull where TResponse : notnull where TErrorResponse : notnull
        {
            FireFrame frame = new FireFrame()
            {
                MsgNum = GetNextMsgNum(),
                Component = component.Id,
                Command = commandId,
                ErrorCode = 0,
                MsgType = FireFrame.MessageType.MESSAGE,
            };

            Type blazeRequestPacketType = typeof(BlazePacket<>).MakeGenericType(typeof(TRequest));
            BlazePacket<TRequest> blazeRequestPacket = (BlazePacket<TRequest>)Activator.CreateInstance(blazeRequestPacketType, frame, request)!;
            ProtoFirePacket requestPacket = blazeRequestPacket.ToProtoFirePacket(Config.Encoder);

            BlazeUtils.LogPacket(component, blazeRequestPacket, false);
            ProtoFirePacket responsePacket = SendRequest(requestPacket);

            Type responseType = responsePacket.Frame.MsgType == FireFrame.MessageType.REPLY ? typeof(TResponse) : typeof(TErrorResponse);
            IBlazePacket responseBlazePacket = responsePacket.Decode(responseType, Config.Decoder);
            BlazeUtils.LogPacket(component, responseBlazePacket, true);

            if (responsePacket.Frame.MsgType == FireFrame.MessageType.REPLY)
                return (TResponse)responseBlazePacket.DataObj;

            TErrorResponse errorResponse = (TErrorResponse)responseBlazePacket.DataObj;
            throw new BlazeRpcException(responsePacket.Frame.FullErrorCode, errorResponse);
        }

        public async Task<TResponse> SendRequestAsync<TRequest, TResponse, TErrorResponse>(IBlazeComponent component, ushort commandId, TRequest request) where TRequest : notnull where TResponse : notnull where TErrorResponse : notnull
        {
            FireFrame frame = new FireFrame()
            {
                MsgNum = GetNextMsgNum(),
                Component = component.Id,
                Command = commandId,
                ErrorCode = 0,
                MsgType = FireFrame.MessageType.MESSAGE
            };

            Type blazeRequestPacketType = typeof(BlazePacket<>).MakeGenericType(typeof(TRequest));
            BlazePacket<TRequest> blazeRequestPacket = (BlazePacket<TRequest>)Activator.CreateInstance(blazeRequestPacketType, frame, request)!;
            ProtoFirePacket requestPacket = blazeRequestPacket.ToProtoFirePacket(Config.Encoder);

            BlazeUtils.LogPacket(component, blazeRequestPacket, false);
            ProtoFirePacket responsePacket = await SendRequestAsync(requestPacket).ConfigureAwait(false);

            Type responseType = responsePacket.Frame.MsgType == FireFrame.MessageType.REPLY ? typeof(TResponse) : typeof(TErrorResponse);
            IBlazePacket responseBlazePacket = responsePacket.Decode(responseType, Config.Decoder);
            BlazeUtils.LogPacket(component, responseBlazePacket, true);

            if (responsePacket.Frame.MsgType == FireFrame.MessageType.REPLY)
                return (TResponse)responseBlazePacket.DataObj;

            TErrorResponse errorResponse = (TErrorResponse)responseBlazePacket.DataObj;
            throw new BlazeRpcException(responsePacket.Frame.FullErrorCode, errorResponse);
        }
    }
}
