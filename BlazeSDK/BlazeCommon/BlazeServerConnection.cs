namespace BlazeCommon
{
    public class BlazeServerConnection
    {
        /// <summary>
        ///    Lock to see if the blaze server is busy with answering the request, useful when you want to send a notification after the request is answered (not during it)
        /// </summary>
        public QueuedLock IsBusyLock { get; }

        public ProtoFireConnection ProtoFireConnection { get; }
        public BlazeServerConfiguration ServerConfiguration { get; }
        public object State { get; set; }

        public BlazeServerConnection(ProtoFireConnection connection, BlazeServerConfiguration serverConfiguration)
        {
            ProtoFireConnection = connection;
            State = new object();
            ServerConfiguration = serverConfiguration;
            IsBusyLock = new QueuedLock();
        }

        public async Task NotifyAsync(ushort componentId, ushort notificationId, object notification, bool waitUntilFree)
        {
            IBlazeComponent? component = ServerConfiguration.GetComponent(componentId);
            FireFrame frame = new FireFrame()
            {
                Component = componentId,
                Command = notificationId,
                ErrorCode = 0,
                MsgNum = 0,
                MsgType = FireFrame.MessageType.NOTIFICATION
            };

            Type fullType = typeof(BlazePacket<>).MakeGenericType(notification.GetType());
            IBlazePacket packet = (IBlazePacket)Activator.CreateInstance(fullType, frame, notification)!;
            ProtoFirePacket protoFirePacket = packet.ToProtoFirePacket(ServerConfiguration.Encoder);

            //if we have to wait until server finishes some previous request (it is forbidden to await notification task with waitUntilFree true in request handler, it may cause deadlock)
            if (waitUntilFree)
            {
                await IsBusyLock.EnterAsync().ConfigureAwait(false);
                IsBusyLock.Exit();
            }

            BlazeUtils.LogPacket(component, packet, false);
            await ProtoFireConnection.SendAsync(protoFirePacket).ConfigureAwait(false);
        }

        public async Task NotifyAsync(IBlazeComponent component, ushort notificationId, object notification, bool waitUntilFree)
        {
            FireFrame frame = new FireFrame()
            {
                Component = component.Id,
                Command = notificationId,
                ErrorCode = 0,
                MsgNum = 0,
                MsgType = FireFrame.MessageType.NOTIFICATION
            };

            Type fullType = typeof(BlazePacket<>).MakeGenericType(notification.GetType());
            IBlazePacket packet = (IBlazePacket)Activator.CreateInstance(fullType, frame, notification)!;
            ProtoFirePacket protoFirePacket = packet.ToProtoFirePacket(ServerConfiguration.Encoder);

            //if we have to wait until server finishes some previous request (it is forbidden to await notification task with waitUntilFree true in request handler, it may cause deadlock)
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
