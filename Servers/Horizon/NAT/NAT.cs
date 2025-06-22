using DotNetty.Handlers.Logging;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Horizon.LIBRARY.Pipeline.Udp;
using System.Net;
using CustomLogger;
using DotNetty.Buffers;
using System.Text;
using NetworkLibrary.Extension;

namespace Horizon.NAT
{
    /// <summary>
    /// SCE-RT NAT.
    /// </summary>
    public class NAT
    {
        public int Port => NATClass.Settings.Port;

        protected IEventLoopGroup? _workerGroup = null;
        protected IChannel? _boundChannel = null;
        protected SimpleDatagramHandler? _scertHandler = null;

        public NAT()
        {

        }

        /// <summary>
        /// Start the SCE-RT Server.
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

                        // message has 4 bytes
                        if (MsgArray.Length == 4)
                        {
                            // get last byte in message
                            switch (MsgArray[3])
                            {
                                case 0xD4:
                                    // Not answear messages ending with 0xD4.
                                    break;
                                default:
                                    // get sender address and port
                                    byte[] senderAddress;
                                    ushort DestPort = (ushort)sender.Port;

                                    if (sender.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                                        senderAddress = sender.Address.MapToIPv4().GetAddressBytes();
                                    else
                                        senderAddress = sender.Address.GetAddressBytes();

                                    // log to console
                                    LoggerAccessor.LogInfo($"[NAT] - Received External IP {sender.Address} & Port {DestPort} request, sending their IP & Port as response!");

                                    // write response message
                                    IByteBuffer buffer = channel.Allocator.Buffer(6);
                                    buffer.WriteBytes(senderAddress);
                                    buffer.WriteUnsignedShort(DestPort);

                                    // send response message 3 times
                                    for (byte i = 0; i < 3; i++)
                                    {
                                        channel.WriteAsync(new DatagramPacket(buffer.Copy(), sender));
                                    }

                                    // flush channel
                                    channel.Flush();
                                    break;
                            }
                        }
                    }
                }
            };

            var bootstrap = new Bootstrap();
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