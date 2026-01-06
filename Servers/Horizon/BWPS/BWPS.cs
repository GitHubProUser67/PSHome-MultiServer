using CustomLogger;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using DotNetty.Handlers.Logging;
using Horizon.LIBRARY.Pipeline.Udp;
using System.Net;
using DotNetty.Buffers;
using CastleLibrary.Utils;

namespace Horizon.BWPS
{
    public class BWPS
    {
        public int Port => BWPSClass.Settings.BWPSPort;

        protected IEventLoopGroup? _workerGroup;
        protected IChannel? _boundChannel;
        protected SimpleDatagramHandler? _scertHandler;

        /// <summary>
        /// Start the BWPS UDP Server.
        /// </summary>
        public async Task Start()
        {
            _workerGroup = new MultithreadEventLoopGroup();

            _scertHandler = new SimpleDatagramHandler();

            // Queue all incoming messages
            _scertHandler.OnChannelMessage += (channel, message) =>
            {
                if (message.Sender is IPEndPoint sender && sender.Port != 0)
                {
                    IByteBuffer directBuf = message.Content;
                    if (directBuf.HasArray)
                    {
                        byte[] MsgArray = new byte[directBuf.ReadableBytes];
                        directBuf.GetBytes(directBuf.ReaderIndex, MsgArray);

                        if (MsgArray.Length == 18)
                        {
                            // log to console
                            LoggerAccessor.LogInfo($"[BWPS] - Received External IP {sender.Address} & Port {(ushort)sender.Port} identification request, sending server identification as a response!");

                            IByteBuffer buffer = channel.Allocator.Buffer(18);

                            buffer.WriteByte(MsgArray[0]); // MessageId
                            buffer.WriteByte(MsgArray[1]);
                            buffer.WriteByte(MsgArray[2]);
                            buffer.WriteByte(MsgArray[3]);
                            buffer.WriteIntLE(50982); // SequenceId
                            buffer.WriteBytes("03 03 02 00 00 00 00 00 02 03".HexStrToBytes());

                            // send response message 3 times
                            for (int i = 0; i < 3; i++)
                            {
                                channel.WriteAsync(new DatagramPacket(buffer.Copy(), sender));
                            }

                            // flush channel
                            channel.Flush();
                        }
                        else if (MsgArray.Length == 6)
                        {
                            // log to console
                            LoggerAccessor.LogInfo($"[BWPS] - Received External IP {sender.Address} & Port {(ushort)sender.Port} test request, sending server identification as a response!");

                            IByteBuffer buffer = channel.Allocator.Buffer(6);

                            buffer.WriteByte(MsgArray[0]); // MessageId
                            buffer.WriteByte(MsgArray[1]);
                            buffer.WriteByte(MsgArray[2]);
                            buffer.WriteByte(MsgArray[3]);
                            buffer.WriteIntLE(58595); // SequenceId

                            // send response message 3 times
                            for (int i = 0; i < 3; i++)
                            {
                                channel.WriteAsync(new DatagramPacket(buffer.Copy(), sender));
                            }

                            // flush channel
                            channel.Flush();
                        }
                    }
                }
            };

            Bootstrap bootstrap = new();
            bootstrap
                .Group(_workerGroup)
                .Channel<SocketDatagramChannel>()
                .Handler(new LoggingHandler(LogLevel.INFO))
                .Handler(new ActionChannelInitializer<IChannel>(channel =>
                {
                    IChannelPipeline pipeline = channel.Pipeline;

                    pipeline.AddLast(_scertHandler);
                }));

            _boundChannel = await bootstrap.BindAsync(Port);
        }

        /// <summary>
        /// Stop the server.
        /// </summary>
        public virtual async Task Stop()
        {
            try
            {
                if (_boundChannel != null)
                {
                    await _boundChannel.CloseAsync();
                    _boundChannel = null;
                }
            }
            finally
            {
                if (_workerGroup != null)
                    await Task.WhenAll(
                        _workerGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)));
            }
        }
    }
}
