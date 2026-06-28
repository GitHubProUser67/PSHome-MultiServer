using System.Net;
using CastleLibrary.Utils;
using CustomLogger;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Horizon.CustomServers;

namespace Horizon.BWPS
{
    public class BWPSProcessor
    {
        public readonly DNTCPHybridServer? _BWPSServer = null;

        public BWPSProcessor()
        {
            _BWPSServer ??= new DNTCPHybridServer();
        }

        public Task StartAsync(int maxConcurrentListeners = 10)
        {
            return Task.Run(() =>
            {
                _BWPSServer!.Start(
                    new Dictionary<ushort, bool> { { HorizonServerConfiguration.BWPSPort, false } },
                    maxConcurrentListeners,
                    null,
                    new ActionChannelInitializer<IChannel>(channel =>
                    {
                        channel.Pipeline.AddLast(_BWPSServer.ScertHandler);
                    }),
                    null,
                    null,
                    (channel, messageObject) =>
                    {
                        var message = (DatagramPacket)messageObject;

                        if (message.Sender is IPEndPoint sender && sender.Port != 0)
                        {
                            var directBuf = message.Content;
                            if (directBuf.HasArray)
                            {
                                var MsgArray = new byte[directBuf.ReadableBytes];
                                directBuf.GetBytes(directBuf.ReaderIndex, MsgArray);

                                if (MsgArray.Length == 18)
                                {
                                    LoggerAccessor.LogInfo(
                                        $"[BWPSProcessor] - Received External IP {sender.Address} & Port {(ushort)sender.Port} identification request, sending server identification as a response!"
                                    );

                                    var buffer = channel.Allocator.Buffer(18);

                                    buffer.WriteByte(MsgArray[0]); // MessageId
                                    buffer.WriteByte(MsgArray[1]);
                                    buffer.WriteByte(MsgArray[2]);
                                    buffer.WriteByte(MsgArray[3]);
                                    buffer.WriteIntLE(50982); // SequenceId
                                    buffer.WriteBytes(
                                        "03 03 02 00 00 00 00 00 02 03".HexStrToBytes()
                                    );

                                    // send response message 3 times
                                    for (var i = 0; i < 3; i++)
                                        channel.WriteAsync(
                                            new DatagramPacket(buffer.Copy(), sender)
                                        );

                                    channel.Flush();
                                }
                                else if (MsgArray.Length == 6)
                                {
                                    LoggerAccessor.LogInfo(
                                        $"[BWPSProcessor] - Received External IP {sender.Address} & Port {(ushort)sender.Port} test request, sending server identification as a response!"
                                    );

                                    var buffer = channel.Allocator.Buffer(6);

                                    buffer.WriteByte(MsgArray[0]); // MessageId
                                    buffer.WriteByte(MsgArray[1]);
                                    buffer.WriteByte(MsgArray[2]);
                                    buffer.WriteByte(MsgArray[3]);
                                    buffer.WriteIntLE(58595); // SequenceId

                                    // send response message 3 times
                                    for (var i = 0; i < 3; i++)
                                        channel.WriteAsync(
                                            new DatagramPacket(buffer.Copy(), sender)
                                        );

                                    channel.Flush();
                                }
                            }
                        }
                    }
                );
            });
        }

        public Task StopAsync()
        {
            return _BWPSServer!.StopAsync();
        }
    }
}
