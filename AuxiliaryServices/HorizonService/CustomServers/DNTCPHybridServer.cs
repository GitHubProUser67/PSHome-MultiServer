using System.Collections.Concurrent;
using System.Net.Sockets;
using CustomLogger;
using DotNetty.Handlers.Logging;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Horizon.LIBRARY.Pipeline.Tcp;
using MultiServerLibrary.Extension;

namespace Horizon.CustomServers
{
    public class DNTCPHybridServer
    {
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);

        public bool IsStarted { get; private set; } = false;

        private MultithreadEventLoopGroup? _bossGroup = null;
        private MultithreadEventLoopGroup? _workerGroup = null;
        private ConcurrentBag<IChannel?>? _boundChannel = null;

        public ScertServerHandler? ScertHandler { get; set; } = null;

        public void Start(
            IDictionary<ushort, bool> ports,
            int maxConcurrentListeners,
            ActionChannelInitializer<ISocketChannel> onSocketChannelInitialisation,
            ActionChannelInitializer<IChannel> onChannelInitialisation,
            Action<IChannel> onChannelActive,
            Action<IChannel> onChannelInactive,
            Action<IChannel, object> onChannelMessage
        )
        {
            _lock.Wait();

            try
            {
                if (IsStarted)
                {
                    LoggerAccessor.LogWarn("[DNTCP Hybrid Server] - Server already active.");
                    return;
                }

                bool isServerStarted = false;
                object bootstrap;

                _bossGroup = new MultithreadEventLoopGroup(1);
                _workerGroup = new MultithreadEventLoopGroup(maxConcurrentListeners);
                _boundChannel = new ConcurrentBag<IChannel?>();

                ScertHandler = new ScertServerHandler
                {
                    OnChannelActive = onChannelActive,
                    OnChannelInactive = onChannelInactive,
                    OnChannelMessage = onChannelMessage,
                };

                foreach (var portParam in ports)
                {
                    bool isTcpServer = portParam.Value;
                    ushort port = portParam.Key;

                    if (isTcpServer)
                    {
                        if (TcpUdpUtils.IsTCPPortAvailable(port))
                        {
                            bootstrap = new ServerBootstrap();

                            ((ServerBootstrap)bootstrap)
                                .Group(_bossGroup, _workerGroup)
                                .Channel<TcpServerSocketChannel>()
                                .Handler(new LoggingHandler(LogLevel.INFO));

                            if (onSocketChannelInitialisation != null)
                                ((ServerBootstrap)bootstrap).ChildHandler(
                                    onSocketChannelInitialisation
                                );
                            else if (onChannelInitialisation != null)
                                ((ServerBootstrap)bootstrap).ChildHandler(onChannelInitialisation);

                            ((ServerBootstrap)bootstrap)
                                .ChildOption(ChannelOption.TcpNodelay, true)
                                .ChildOption(ChannelOption.SoTimeout, 1000 * 60 * 15);

                            _ = ((ServerBootstrap)bootstrap)
                                .BindAsync(port)
                                .ContinueWith(t =>
                                {
                                    try
                                    {
                                        _boundChannel.Add(t.Result);

                                        isServerStarted = true;

                                        LoggerAccessor.LogInfo(
                                            $"[DNTCP Hybrid Server] - Listening on TCP/UDP port {port}..."
                                        );
                                    }
                                    catch (Exception ex)
                                    {
                                        LoggerAccessor.LogError(
                                            $"[DNTCP Hybrid Server] - Failed to bind TCP/UDP port {port}. (Exception:"
                                                + ex
                                                + ")"
                                        );
                                    }
                                });

                            continue;
                        }
                    }
                    else if (TcpUdpUtils.IsUDPPortAvailable(port))
                    {
                        bootstrap = new Bootstrap();

                        ((Bootstrap)bootstrap)
                            .Group(_workerGroup ?? _bossGroup)
                            .ChannelFactory(() =>
                            {
                                var socket = new Socket(
                                    AddressFamily.InterNetwork,
                                    SocketType.Dgram,
                                    ProtocolType.Udp
                                );
                                return new SocketDatagramChannel(socket);
                            })
                            .Handler(new LoggingHandler(LogLevel.INFO));

                        if (onSocketChannelInitialisation != null)
                            ((Bootstrap)bootstrap).Handler(onSocketChannelInitialisation);
                        else if (onChannelInitialisation != null)
                            ((Bootstrap)bootstrap).Handler(onChannelInitialisation);

                        _ = ((Bootstrap)bootstrap)
                            .BindAsync(port)
                            .ContinueWith(t =>
                            {
                                try
                                {
                                    _boundChannel.Add(t.Result);

                                    isServerStarted = true;

                                    LoggerAccessor.LogInfo(
                                        $"[DNTCP Hybrid Server] - Listening on UDP port {port}..."
                                    );
                                }
                                catch (Exception ex)
                                {
                                    LoggerAccessor.LogError(
                                        $"[DNTCP Hybrid Server] - Failed to bind UDP port {port}. (Exception:"
                                            + ex
                                            + ")"
                                    );
                                }
                            });

                        continue;
                    }

                    LoggerAccessor.LogError(
                        $"[DNTCP Hybrid Server] - Port:{port} is not available, skipping..."
                    );
                }

                IsStarted = true;
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task StopAsync()
        {
            await _lock.WaitAsync().ConfigureAwait(false);

            try
            {
                if (!IsStarted)
                    return;

                if (_boundChannel != null)
                {
                    foreach (var boundChannel in _boundChannel)
                    {
                        if (boundChannel != null)
                        {
                            if (
                                !await boundChannel
                                    .CloseAsync()
                                    .TryAwait(TimeSpan.FromMilliseconds(2000))
                                    .ConfigureAwait(false)
                            )
                                LoggerAccessor.LogWarn(
                                    $"[DNTCP Hybrid Server] - Timed out waiting for:{boundChannel.GetHashCode()} bound channel close."
                                );
                        }
                    }

                    _boundChannel = null;
                }
            }
            finally
            {
                if (_workerGroup != null)
                {
                    if (
                        !await _workerGroup
                            .ShutdownGracefullyAsync(
                                TimeSpan.FromMilliseconds(100),
                                TimeSpan.FromSeconds(1)
                            )
                            .TryAwait(TimeSpan.FromMilliseconds(2000))
                            .ConfigureAwait(false)
                    )
                        LoggerAccessor.LogWarn(
                            $"[DNTCP Hybrid Server] - Timed out waiting for:{_workerGroup.GetHashCode()} worker group shutdown."
                        );
                }

                if (_bossGroup != null)
                {
                    if (
                        !await _bossGroup
                            .ShutdownGracefullyAsync(
                                TimeSpan.FromMilliseconds(100),
                                TimeSpan.FromSeconds(1)
                            )
                            .TryAwait(TimeSpan.FromMilliseconds(2000))
                            .ConfigureAwait(false)
                    )
                        LoggerAccessor.LogWarn(
                            $"[DNTCP Hybrid Server] - Timed out waiting for:{_bossGroup.GetHashCode()} boss group shutdown."
                        );
                }

                IsStarted = false;

                _lock.Release();
            }

            LoggerAccessor.LogInfo("[DNTCP Hybrid Server] - All listeners stopped.");
        }
    }
}
