namespace BlazeCommon
{
    public class BlazeServerConnection(
        ProtoFireConnection connection,
        BlazeServerConfiguration serverConfiguration
    )
    {
        /// <summary>
        ///    Lock to see if the blaze server is busy with answering the request, useful when you want to send a notification after the request is answered (not during it)
        /// </summary>
        public QueuedLock IsBusyLock { get; } = new QueuedLock();

        public ProtoFireConnection ProtoFireConnection { get; } = connection;
        public BlazeServerConfiguration ServerConfiguration { get; } = serverConfiguration;
        public object State { get; set; } = new object();

        public async Task NotifyAsync(
            ushort componentId,
            ushort notificationId,
            object notification,
            bool waitUntilFree
        )
        {
            IBlazeComponent? component = ServerConfiguration.GetComponent(componentId);
            var frame = new FireFrame()
            {
                Component = componentId,
                Command = notificationId,
                ErrorCode = 0,
                MsgNum = 0,
                MsgType = FireFrame.MessageType.NOTIFICATION,
            };

            var fullType = typeof(BlazePacket<>).MakeGenericType(notification.GetType());
            var packet = (IBlazePacket)Activator.CreateInstance(fullType, frame, notification)!;
            var protoFirePacket = packet.ToProtoFirePacket(ServerConfiguration.Encoder);

            //if we have to wait until server finishes some previous request (it is forbidden to await notification task with waitUntilFree true in request handler, it may cause deadlock).ConfigureAwait(false)
            if (waitUntilFree)
            {
                await IsBusyLock.EnterAsync().ConfigureAwait(false);
                IsBusyLock.Exit();
            }

            BlazeUtils.LogPacket(component, packet, false);
            await ProtoFireConnection.SendAsync(protoFirePacket).ConfigureAwait(false);
        }

        public async Task NotifyAsync(
            IBlazeComponent component,
            ushort notificationId,
            object notification,
            bool waitUntilFree
        )
        {
            var frame = new FireFrame()
            {
                Component = component.Id,
                Command = notificationId,
                ErrorCode = 0,
                MsgNum = 0,
                MsgType = FireFrame.MessageType.NOTIFICATION,
            };

            var fullType = typeof(BlazePacket<>).MakeGenericType(notification.GetType());
            var packet = (IBlazePacket)Activator.CreateInstance(fullType, frame, notification)!;
            var protoFirePacket = packet.ToProtoFirePacket(ServerConfiguration.Encoder);

            //if we have to wait until server finishes some previous request (it is forbidden to await notification task with waitUntilFree true in request handler, it may cause deadlock).ConfigureAwait(false)
            if (waitUntilFree)
            {
                await IsBusyLock.EnterAsync().ConfigureAwait(false);
                IsBusyLock.Exit();
            }

            BlazeUtils.LogPacket(component, packet, false);
            await ProtoFireConnection.SendAsync(protoFirePacket).ConfigureAwait(false);
        }
    }
}
