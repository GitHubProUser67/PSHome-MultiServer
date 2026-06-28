using System.Net;
using System.Net.Sockets;
using CustomLogger;
using EndianTools;
using MultiServerLibrary.CustomServers;

namespace Horizon.NAT
{
    public class NATProcessor
    {
        public readonly UDPServer? _NATServer = null;

        public NATProcessor()
        {
            _NATServer ??= new UDPServer();
        }

        public Task StartAsync(CancellationToken token = default)
        {
            return _NATServer!.StartAsync(
                new List<ushort> { HorizonServerConfiguration.NATPort },
                null,
                null,
                null,
                ProcessMessagesFromClient,
                token
            );
        }

        public void Stop()
        {
            _NATServer!.Stop();
        }

        #region Protected Functions
        protected virtual byte[]? ProcessMessagesFromClient(
            ushort serverPort,
            UdpClient listener,
            byte[] data,
            IPEndPoint remoteEP
        )
        {
            if (data != null)
            {
                // message has 4 bytes
                if (data.Length == 4)
                {
                    // get last byte in message
                    switch (data[3])
                    {
                        case 0xD4:
                            // Not answear messages ending with 0xD4.
                            break;
                        default:
                            // get sender address and port
                            byte[] senderAddress;
                            var DestPort = (ushort)remoteEP.Port;

                            senderAddress =
                                remoteEP.Address.AddressFamily == AddressFamily.InterNetworkV6
                                    ? remoteEP.Address.MapToIPv4().GetAddressBytes()
                                    : remoteEP.Address.GetAddressBytes();

                            // log to console
                            LoggerAccessor.LogInfo(
                                $"[NATProcessor] - Received External IP {remoteEP.Address} & Port {DestPort} request, sending their IP & Port as response!"
                            );

                            // write response message
                            var buffer = new byte[6];
                            Array.Copy(senderAddress, 0, buffer, 0, senderAddress.Length);
                            EndianAwareConverter.WriteUInt16(
                                buffer,
                                Endianness.BigEndian,
                                4,
                                DestPort
                            );

                            // send response message 3 times
                            for (byte i = 0; i < 3; i++)
                            {
                                try
                                {
                                    listener.Send(buffer, buffer.Length, remoteEP);
                                }
                                catch (SocketException socketException)
                                {
                                    if (
                                        socketException.ErrorCode != 995
                                        && socketException.SocketErrorCode
                                            != SocketError.ConnectionReset
                                        && socketException.SocketErrorCode
                                            != SocketError.ConnectionAborted
                                        && socketException.SocketErrorCode
                                            != SocketError.Interrupted
                                    )
                                        LoggerAccessor.LogError(
                                            $"[NATProcessor] - SocketException while sending response to client. (Exception:"
                                                + socketException
                                                + ")"
                                        );
                                }
                                catch (Exception e)
                                {
                                    LoggerAccessor.LogError(
                                        "[NATProcessor] - Assertion while sending response to client. (Exception:"
                                            + e
                                            + ")"
                                    );
                                }
                            }

                            break;
                    }
                }
            }

            return null;
        }
        #endregion
    }
}
