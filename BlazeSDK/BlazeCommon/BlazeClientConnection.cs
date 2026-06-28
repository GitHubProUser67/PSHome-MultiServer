using CustomLogger;

namespace BlazeCommon
{
    public class BlazeClientConnection(
        ProtoFireConnection connection,
        BlazeClientConfiguration clientConfiguration
    ) : ProtoFireClient(connection)
    {
        public BlazeClientConfiguration Config { get; } = clientConfiguration;
        public object State { get; set; } = new object();

        public override void OnClientDisconnected()
        {
            LoggerAccessor.LogWarn("[BlazeClientConnection] - Client Disconnected.");
        }

        public override void OnPacketReceived(ProtoFirePacket packet)
        {
            if (packet.Frame.MsgType != FireFrame.MessageType.NOTIFICATION)
                return;

            var component = Config.GetComponent(packet.Frame.Component);
            if (component == null)
            {
                LoggerAccessor.LogWarn(
                    $"[BlazeClientConnection] - Unable to handle notification - component {packet.Frame.Component} handler not found"
                );
                return;
            }

            var notificationType = component.GetNotificationType(packet.Frame.Command);
            var blazePacket = packet.Decode(notificationType, Config.Decoder);
            BlazeUtils.LogPacket(component, blazePacket, true);

            var methodInfo = component.GetBlazeNotificationInfo(packet.Frame.Command);
            if (methodInfo == null)
            {
                LoggerAccessor.LogWarn(
                    $"[BlazeClientConnection] - Unable to handle notification for component {packet.Frame.Component} - notification {packet.Frame.Command} handler not found"
                );
                return;
            }

            try
            {
                methodInfo.InvokeAsync(blazePacket.DataObj).GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(
                    $"[BlazeClientConnection] - Error while handling notification for component {packet.Frame.Component} - notification {packet.Frame.Command} (Exception: {e})"
                );
            }
        }

        public TResponse SendRequest<TRequest, TResponse, TErrorResponse>(
            IBlazeComponent component,
            ushort commandId,
            TRequest request
        )
            where TRequest : notnull
            where TResponse : notnull
            where TErrorResponse : notnull
        {
            var frame = new FireFrame()
            {
                MsgNum = GetNextMsgNum(),
                Component = component.Id,
                Command = commandId,
                ErrorCode = 0,
                MsgType = FireFrame.MessageType.MESSAGE,
            };

            var blazeRequestPacketType = typeof(BlazePacket<>).MakeGenericType(typeof(TRequest));
            var blazeRequestPacket =
                (BlazePacket<TRequest>)
                    Activator.CreateInstance(blazeRequestPacketType, frame, request)!;
            var requestPacket = blazeRequestPacket.ToProtoFirePacket(Config.Encoder);

            BlazeUtils.LogPacket(component, blazeRequestPacket, false);
            var responsePacket = SendRequest(requestPacket);

            var responseType =
                responsePacket.Frame.MsgType == FireFrame.MessageType.REPLY
                    ? typeof(TResponse)
                    : typeof(TErrorResponse);
            var responseBlazePacket = responsePacket.Decode(responseType, Config.Decoder);
            BlazeUtils.LogPacket(component, responseBlazePacket, true);

            if (responsePacket.Frame.MsgType == FireFrame.MessageType.REPLY)
                return (TResponse)responseBlazePacket.DataObj;

            var errorResponse = (TErrorResponse)responseBlazePacket.DataObj;
            throw new BlazeRpcException(responsePacket.Frame.FullErrorCode, errorResponse);
        }

        public async Task<TResponse> SendRequestAsync<TRequest, TResponse, TErrorResponse>(
            IBlazeComponent component,
            ushort commandId,
            TRequest request
        )
            where TRequest : notnull
            where TResponse : notnull
            where TErrorResponse : notnull
        {
            var frame = new FireFrame()
            {
                MsgNum = GetNextMsgNum(),
                Component = component.Id,
                Command = commandId,
                ErrorCode = 0,
                MsgType = FireFrame.MessageType.MESSAGE,
            };

            var blazeRequestPacketType = typeof(BlazePacket<>).MakeGenericType(typeof(TRequest));
            var blazeRequestPacket =
                (BlazePacket<TRequest>)
                    Activator.CreateInstance(blazeRequestPacketType, frame, request)!;
            var requestPacket = blazeRequestPacket.ToProtoFirePacket(Config.Encoder);

            BlazeUtils.LogPacket(component, blazeRequestPacket, false);
            var responsePacket = await SendRequestAsync(requestPacket).ConfigureAwait(false);

            var responseType =
                responsePacket.Frame.MsgType == FireFrame.MessageType.REPLY
                    ? typeof(TResponse)
                    : typeof(TErrorResponse);
            var responseBlazePacket = responsePacket.Decode(responseType, Config.Decoder);
            BlazeUtils.LogPacket(component, responseBlazePacket, true);

            if (responsePacket.Frame.MsgType == FireFrame.MessageType.REPLY)
                return (TResponse)responseBlazePacket.DataObj;

            var errorResponse = (TErrorResponse)responseBlazePacket.DataObj;
            throw new BlazeRpcException(responsePacket.Frame.FullErrorCode, errorResponse);
        }
    }
}
