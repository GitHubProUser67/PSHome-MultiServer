using CustomLogger;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Horizon.RT.Common;
using Horizon.RT.Cryptography;
using Horizon.RT.Models;
using Horizon.LIBRARY.Pipeline.Tcp;
using System.Collections.Concurrent;
using System.Net;
using Horizon.LIBRARY.Pipeline.Attribute;
using Horizon.SERVER;
using MultiServerLibrary.Extension;

namespace Horizon.DME
{
    public class MASClient
    {
        public bool IsConnected => _masChannel != null && _masChannel.Active && _masState > 0;
        public bool IsAuthenticated => masConnected;
        public DateTime? TimeLostConnection { get; set; } = null;
        public string? SessionKey = null;
        public string? AccessKey = null;
        public int ApplicationId { get; } = 0;

        private enum MASConnectionState
        {
            FAILED = -1,
            NO_CONNECTION,
            CONNECTED,
            HELLO,
            HANDSHAKE,
            CONNECT_TCP,
            PENDING_TCP_ACK,
            ACK_TCP,
            AUTHENTICATED
        }

        private bool masConnected;
        private DateTime _utcConnectionState;
        private MASConnectionState _masState = MASConnectionState.NO_CONNECTION;

        private IEventLoopGroup? _group = null;
        private IChannel? _masChannel = null;
        private Bootstrap? _bootstrap = null;
        private ScertServerHandler? _scertHandler = null;

        private CancellationTokenSource? ctsMPSQueue = null;

        private ConcurrentQueue<BaseScertMessage> _masRecvQueue { get; } = new();
        private ConcurrentQueue<BaseScertMessage> _masSendQueue { get; } = new();

        public MASClient(int appId)
        {
            ApplicationId = appId;
        }

        #region MAS Client

        public async Task Start()
        {
            masConnected = false;

            ctsMPSQueue = new();

            _group = new MultithreadEventLoopGroup();
            _scertHandler = new ScertServerHandler();

            TimeLostConnection = null;

            // Add client on connect
            _scertHandler.OnChannelActive += (channel) =>
            {

            };

            // Remove client on disconnect
            _scertHandler.OnChannelInactive += async (channel) =>
            {
                LoggerAccessor.LogWarn("MAS was disconnected or lost connection.");
                TimeLostConnection = DateTimeUtils.GetHighPrecisionUtcTime();
                await Stop();
            };

            // Queue all incoming messages
            _scertHandler.OnChannelMessage += (channel, message) =>
            {
                // Add to queue
                _masRecvQueue.Enqueue(message);

                // Log if id is set
                if (message.CanLog())
                    LoggerAccessor.LogDebug($"MAS {channel}: {message}");
            };

            _bootstrap = new Bootstrap();
            _bootstrap
                .Group(_group)
                .Channel<TcpSocketChannel>()
                .Option(ChannelOption.TcpNodelay, true)
                .Handler(new ActionChannelInitializer<ISocketChannel>(channel =>
                {
                    IChannelPipeline pipeline = channel.Pipeline;

                    pipeline.AddLast(new ScertEncoder());
                    pipeline.AddLast(new ScertIEnumerableEncoder());
                    pipeline.AddLast(new ScertTcpFrameDecoder(DotNetty.Buffers.ByteOrder.LittleEndian, Constants.MEDIUS_MESSAGE_MAXLEN, 1, 2, 0, 0, false));
                    pipeline.AddLast(new ScertDecoder());
                    pipeline.AddLast(new ScertMultiAppDecoder());
                    pipeline.AddLast(_scertHandler);
                }));

            await ConnectMAS();

            _ = Task.Run(async () =>
            {
                try
                {
                    const byte maxNumOfRetries = 6;
                    byte numOfRetries = 0;

                    while (!masConnected)
                    {
                        await Task.Delay(1000);

                        if (numOfRetries == maxNumOfRetries)
                        {
                            LoggerAccessor.LogError("[MASClient] - Start() - Failed to authenticate with the MAS server within 6 seconds, aborting client...");
                            await Stop();
                            return;
                        }

                        numOfRetries++;
                    }

                    MPSClient client = new MPSClient(ApplicationId, SessionKey, AccessKey);

                    lock (DmeClass.MPSManagersQueue)
                    {
                        if (!DmeClass.MPSManagersQueue.ContainsKey(ApplicationId))
                            DmeClass.MPSManagersQueue.Add(ApplicationId, client);
                        else
                            DmeClass.MPSManagersQueue[ApplicationId] = client; // Mostly placebo, unless you start 1000+ MAS for same appid at the same time...
                    }
                }
                catch (OperationCanceledException)
                {
                    LoggerAccessor.LogWarn("[MASClient] - Start() - MPS Queing Task was canceled.");
                }
                catch (Exception ex)
                {
                    LoggerAccessor.LogError($"[MASClient] - Start() - MPS Queing Task thrown an assertion: {ex}");
                }
            }, ctsMPSQueue.Token);
        }

        public async Task Stop()
        {
            if (_masChannel != null)
            {
                await _masChannel.CloseAsync();
                _masChannel = null;
            }
            if (_group != null)
                await _group.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1));

            _masRecvQueue.Clear();
            _masSendQueue.Clear();
            _masState = MASConnectionState.NO_CONNECTION;

            if (ctsMPSQueue != null)
            {
                ctsMPSQueue.Cancel();
                ctsMPSQueue.Dispose();
                ctsMPSQueue = null;
            }
        }

        public bool CheckMASConnectivity()
        {
            if (_masState == MASConnectionState.FAILED ||
                (_masState != MASConnectionState.AUTHENTICATED && (DateTimeUtils.GetHighPrecisionUtcTime() - _utcConnectionState).TotalSeconds > 30))
            {
                LoggerAccessor.LogError("[MASClient] - HandleIncomingMessages() - MAS server is not authenticated!");
                TimeLostConnection = DateTimeUtils.GetHighPrecisionUtcTime();
                Stop().Wait();
                return false;
            }
            else
                return true;
        }

        public async Task HandleIncomingMessages()
        {
            if (_masChannel == null)
                return;

            try
            {
                // Process all messages in queue
                while (_masRecvQueue.TryDequeue(out var message))
                {
                    try
                    {
                        await ProcessMessage(message, _masChannel);
                    }
                    catch (Exception e)
                    {
                        LoggerAccessor.LogError($"[MASClient] - HandleIncomingMessages() - Error while Processing incoming messages: {e}");
                    }
                }
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError($"[MASClient] - HandleIncomingMessages() - Error while Handling incoming messages: {e}");
            }
        }

        public async Task HandleOutgoingMessages()
        {
            if (_masChannel == null)
                return;

            List<BaseScertMessage> responses = new();

            try
            {
                // Send if writeable
                if (_masChannel.IsWritable)
                {
                    // Add send queue to responses
                    while (_masSendQueue.TryDequeue(out var message))
                        responses.Add(message);

                    if (responses.Count > 0)
                        await _masChannel.WriteAndFlushAsync(responses);
                }
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError($"[MASClient] - HandleOutgoingMessages() - Error while Handling outgoing messages: {e}");
            }
        }

        private async Task ConnectMAS()
        {
            _utcConnectionState = DateTimeUtils.GetHighPrecisionUtcTime();
            _masState = MASConnectionState.NO_CONNECTION;

            try
            {
                if (_bootstrap != null)
                    _masChannel = await _bootstrap.ConnectAsync(new IPEndPoint(IPAddress.Parse(DmeClass.Settings.MAS.Ip) ?? MediusClass.SERVER_IP, DmeClass.Settings.MAS.Port));
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError($"[MASClient] - Failed to connect to MAS. (Exception:{e})");
                TimeLostConnection = DateTimeUtils.GetHighPrecisionUtcTime();
                return;
            }

            if (_masChannel != null && !_masChannel.Active)
                return;

            _masState = MASConnectionState.CONNECTED;

            if (_masChannel != null && !_masChannel.HasAttribute(LIBRARY.Pipeline.Constants.SCERT_CLIENT))
                _masChannel.GetAttribute(LIBRARY.Pipeline.Constants.SCERT_CLIENT).Set(new ScertClientAttribute());
            var scertClient = _masChannel?.GetAttribute(LIBRARY.Pipeline.Constants.SCERT_CLIENT).Get();
            if (scertClient != null)
            {
                scertClient.RsaAuthKey = DmeClass.Settings.MAS.Key;
                scertClient.CipherService?.GenerateCipher(scertClient.RsaAuthKey);
            }

            // Send hello
            if (_masChannel != null)
                await _masChannel.WriteAndFlushAsync(new RT_MSG_CLIENT_HELLO()
                {
                    Parameters = new ushort[]
                    {
                        2,
                        0x6e,
                        0x6d,
                        1,
                        1
                    }
                });

            _masState = MASConnectionState.HELLO;
        }

        private async Task ProcessMessage(BaseScertMessage message, IChannel serverChannel)
        {
            // Get ScertClient data
            var scertClient = serverChannel.GetAttribute(LIBRARY.Pipeline.Constants.SCERT_CLIENT).Get();

            switch (message)
            {
                // Authentication
                case RT_MSG_SERVER_HELLO serverHello:
                    {
                        if (_masState != MASConnectionState.HELLO)
                            throw new Exception($"Unexpected RT_MSG_SERVER_HELLO from server. {serverHello}");

                        // Send public key
                        Enqueue(new RT_MSG_CLIENT_CRYPTKEY_PUBLIC()
                        {
                            PublicKey = DmeClass.Settings.MAS.Key.N.ToByteArrayUnsigned().Reverse().ToArray()
                        });

                        _masState = MASConnectionState.HANDSHAKE;
                        break;
                    }
                case RT_MSG_SERVER_CRYPTKEY_PEER serverCryptKeyPeer:
                    {
                        if (_masState != MASConnectionState.HANDSHAKE)
                            throw new Exception($"Unexpected RT_MSG_SERVER_CRYPTKEY_PEER from server. {serverCryptKeyPeer}");

                        // generate new client session key
                        scertClient.CipherService?.GenerateCipher(CipherContext.RC_CLIENT_SESSION, serverCryptKeyPeer.SessionKey ?? Array.Empty<byte>());

                        if (_masChannel != null)
                            await _masChannel.WriteAndFlushAsync(new RT_MSG_CLIENT_CONNECT_TCP()
                            {
                                TargetWorldId = 1,
                                AppId = ApplicationId,
                                Key = DmeClass.GlobalAuthPublic
                            });

                        _masState = MASConnectionState.CONNECT_TCP;
                        break;
                    }
                case RT_MSG_SERVER_CONNECT_ACCEPT_TCP serverConnectAcceptTcp:
                    {
                        if (_masState != MASConnectionState.CONNECT_TCP)
                            throw new Exception($"Unexpected RT_MSG_SERVER_CONNECT_ACCEPT_TCP from server. {serverConnectAcceptTcp}");

                        if (_masChannel != null)
                            await _masChannel.WriteAndFlushAsync(new RT_MSG_CLIENT_CONNECT_READY_TCP()
                            {

                            });

                        _masState = MASConnectionState.PENDING_TCP_ACK;
                        break;
                    }
                case RT_MSG_SERVER_CONNECT_COMPLETE serverComplete:
                    {
                        if (_masState != MASConnectionState.PENDING_TCP_ACK)
                            throw new Exception($"Unexpected RT_MSG_SERVER_CONNECT_COMPLETE from server. {serverComplete}");

                        if (_masChannel != null)
                            await _masChannel.WriteAndFlushAsync(new RT_MSG_CLIENT_APP_TOSERVER()
                            {
                                Message = new MediusServerSessionBeginRequest()
                                {
                                    MessageID = new MessageId(),
                                    LocationID = 0,
                                    Port = DmeClass.TcpServer.Port,
                                    ApplicationID = ApplicationId,
                                    ServerVersion = string.Empty,
                                    ServerType = MGCL_GAME_HOST_TYPE.MGCLGameHostIntegratedServer
                                }
                            });

                        _masState = MASConnectionState.ACK_TCP;
                        break;
                    }
                case RT_MSG_SERVER_CONNECT_REQUIRE serverRequire:
                    {
                        if (_masChannel != null)
                            await _masChannel.WriteAndFlushAsync(new RT_MSG_CLIENT_CONNECT_READY_REQUIRE()
                            {
                                ServReq = 0
                            });
                        break;
                    }
                case RT_MSG_SERVER_ECHO serverEcho:
                    {
                        Enqueue(serverEcho);
                        break;
                    }
                case RT_MSG_CLIENT_ECHO clientEcho:
                    {
                        Enqueue(new RT_MSG_CLIENT_ECHO() { Value = clientEcho.Value });
                        break;
                    }
                case RT_MSG_SERVER_CHEAT_QUERY cheatQuery:
                    {

                        break;
                    }
                case RT_MSG_SERVER_APP serverApp:
                    {
                        if (serverApp.Message != null)
                            await ProcessMediusMessage(serverApp.Message, serverChannel);
                        break;
                    }

                case RT_MSG_SERVER_FORCED_DISCONNECT serverForcedDisconnect:
                case RT_MSG_CLIENT_DISCONNECT_WITH_REASON clientDisconnectWithReason:
                    {
                        await serverChannel.CloseAsync();
                        _masState = MASConnectionState.NO_CONNECTION;
                        break;
                    }
                default:
                    {
                        LoggerAccessor.LogWarn($"UNHANDLED MAS MESSAGE: {message}");

                        break;
                    }
            }

            return;
        }

        private async Task ProcessMediusMessage(BaseMediusMessage message, IChannel clientChannel)
        {
            if (message == null)
                return;

            switch (message)
            {
                case MediusServerSessionBeginResponse setServerSessionResponse:
                    {
                        if (_masState != MASConnectionState.ACK_TCP)
                            throw new Exception($"Unexpected MediusServerSessionBeginResponse from server. {setServerSessionResponse}");

                        if (_masChannel != null)
                            await _masChannel.WriteAndFlushAsync(new RT_MSG_CLIENT_DISCONNECT_WITH_REASON()
                            {

                            });

                        SessionKey = setServerSessionResponse.ConnectInfo.SessionKey;
                        AccessKey = setServerSessionResponse.ConnectInfo.AccessKey;

                        /* Ideally, we should contact the NAT server with given infos to get our IP:PORT.
                         * For now we simply use a MPS constant in config */

                        _masState = MASConnectionState.AUTHENTICATED;

                        masConnected = true;

                        break;
                    }
                default:
                    {
                        LoggerAccessor.LogWarn($"UNHANDLED MAS MESSAGE: {message}");

                        break;
                    }
            }
        }

        #endregion

        #region Queue

        public void Enqueue(BaseScertMessage message)
        {
            _masSendQueue.Enqueue(message);
        }

        public void Enqueue(IEnumerable<BaseScertMessage> messages)
        {
            foreach (var message in messages)
                _masSendQueue.Enqueue(message);
        }

        public void Enqueue(BaseMediusMessage message)
        {
            _masSendQueue.Enqueue(new RT_MSG_CLIENT_APP_TOSERVER() { Message = message });
        }

        #endregion
    }
}
