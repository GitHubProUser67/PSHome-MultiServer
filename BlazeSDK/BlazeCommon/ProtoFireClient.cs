using System.Collections.Concurrent;

namespace BlazeCommon
{
    public abstract class ProtoFireClient
    {
        public ProtoFireConnection Connection { get; }
        public int RequestTimeout { get; set; }

        uint nextReqNum;

        ConcurrentDictionary<uint, TaskCompletionSource<ProtoFirePacket>> replyTasks;

        public ProtoFireClient(ProtoFireConnection connection)
        {
            if (!connection.Connected)
                throw new ArgumentException("The connection is disconnected.");

            replyTasks = new ConcurrentDictionary<uint, TaskCompletionSource<ProtoFirePacket>>();
            RequestTimeout = 15000;
            Connection = connection;
            nextReqNum = 0;
            _ = ReadPacket();
        }

        async Task ReadPacket()
        {
            while (Connection.Connected)
            {
                ProtoFirePacket? packet = await Connection.ReadPacketAsync().ConfigureAwait(false);

                if (packet == null)
                {
                    OnClientDisconnected();
                    break;
                }

                switch (packet.Frame.MsgType)
                {
                    case FireFrame.MessageType.ERROR_REPLY:
                    case FireFrame.MessageType.REPLY:
                        HandleReplyPacket(packet);
                        break;
                }

                OnPacketReceived(packet);
            }
        }

        void HandleReplyPacket(ProtoFirePacket reply)
        {
            if (replyTasks.TryRemove(reply.Frame.MsgNum, out TaskCompletionSource<ProtoFirePacket>? tcs))
                tcs.SetResult(reply);
        }

        public uint GetNextMsgNum()
        {
            return Interlocked.Increment(ref nextReqNum);
        }


        public ProtoFirePacket SendRequest(ProtoFirePacket packet)
        {
            TaskCompletionSource<ProtoFirePacket> tcs = new TaskCompletionSource<ProtoFirePacket>();
            replyTasks.TryAdd(packet.Frame.MsgNum, tcs);

            if (!Connection.Send(packet))
            {
                replyTasks.TryRemove(packet.Frame.MsgNum, out _);
                throw new Exception("Failed to send packet."); //TODO: better exception
            }

            CancellationTokenSource cts = new CancellationTokenSource(RequestTimeout);
            cts.Token.Register(() => tcs.TrySetCanceled(), useSynchronizationContext: false);
            try { return tcs.Task.GetAwaiter().GetResult(); }
            catch (TaskCanceledException)
            {
                throw new TimeoutException("The request timed out.");
            }

        }

        public async Task<ProtoFirePacket> SendRequestAsync(ProtoFirePacket packet)
        {
            TaskCompletionSource<ProtoFirePacket> tcs = new TaskCompletionSource<ProtoFirePacket>();
            replyTasks.TryAdd(packet.Frame.MsgNum, tcs);

            if (!await Connection.SendAsync(packet).ConfigureAwait(false))
            {
                replyTasks.TryRemove(packet.Frame.MsgNum, out _);
                throw new Exception("Failed to send packet."); //TODO: better exception
            }

            CancellationTokenSource cts = new CancellationTokenSource(RequestTimeout);
            cts.Token.Register(() => tcs.TrySetCanceled(), useSynchronizationContext: false);
            try { return await tcs.Task.ConfigureAwait(false); }
            catch (TaskCanceledException)
            {
                throw new TimeoutException("The request timed out.");
            }

        }

        public abstract void OnClientDisconnected();
        public abstract void OnPacketReceived(ProtoFirePacket request);
    }
}
